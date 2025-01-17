import React, { useEffect, useState } from 'react';
import { useNavigate } from "react-router-dom";

import Table from '../../../components/Table/Table';


import ExceptionManagement from '../../../services/exceptionManagement';
import { PagedResult } from '../../../models/GenericResponseDTO';
import { ExceptionDTO, getAgingCategoryDisplayName, getTransactionTypeDisplayName } from '../../../models/exceptionManagementDTOs';
import { globalStyle } from '../../../styles/theme';

import { Box, Button, Chip, MenuItem, TextField, Typography } from '@mui/material';

import PaginationControls from '../../../components/Pagination/PaginationControls';
import GlobalButton from '../../../components/Button/Button';
import { FormattedDate } from '../../../utils/formatDate';

import Cookies from 'js-cookie';
const Permission = Cookies.get("Permission") ?? "";
const userPermissions: string[] = JSON.parse(Permission || "[]");
const hasPermission = (permission: string) =>
  userPermissions.includes(permission);


const Dashboard: React.FC = () => {

  const navigate = useNavigate();
  const [pagedResult, setPagedResult] = useState<PagedResult<ExceptionDTO>>({
    items: [],
    totalCount: 0,
    pageNumber: 1,
    pageSize: 10,
    searchTerm: ''
  });
  const [searchTerm, setSearchTerm] = useState<string>(pagedResult.searchTerm);
  const [status, setStatus] = useState<number|null>(null);
  const statusOptions = [
    { label: "All", value: 99},
    { label: "For Approval", value: 0 },
    { label: "Open", value: 1 },
    { label: "Closed", value: 2 },
  ];
  const columns = [
    {
      label: 'Action',
      render: (e: any) => <GlobalButton buttonAction="view" onClick={() => onClickViewDetails(e.refNo)}></GlobalButton>
    },
    {
      label: 'Status', accessor: 'status',
      render: (rowData: any) => {
        switch (rowData.status) {
          case 0:
            return <Chip label="For Approval" color="info" />;
          case 1:
            return <Chip label="Open" color="success" />;
          case 2:
            return <Chip label="Closed" color="error" />;
          default:
            return "";
        }
      }
    },
    { label: 'Reference No.', accessor: 'refNo' },
    { label: 'Branch Code/ Name', accessor: 'branchName' },
    { label: 'Transaction Type', accessor: 'type',
      render: (e: any) => getTransactionTypeDisplayName(e.type)
    },
    { label: 'Transaction Date', accessor: 'transactionDate', 
      render: (e: any) => FormattedDate(e.transactionDate)
    },
    { label: 'Aging Category', accessor: 'agingCategory', 
      render: (e: any) => getAgingCategoryDisplayName(e.agingCategory)
    },
    { label: 'Created By', accessor: 'createdBy' },
    { label: 'Deviation Count	', accessor: 'exceptionCodes',
      render: (e: any) => e.exceptionCodes.length
    },
    { label: 'Date Created', accessor: 'dateCreated',
      render: (e: any) => FormattedDate(e.transactionDate)
    },
    { label: 'Employee ID', accessor: 'employeeID' },
    { label: 'Employee Responsible', accessor: 'personResponsible' },
  ];

  useEffect(() => {
    getExceptionsList();
  }, [pagedResult.pageNumber, pagedResult.pageSize, status]);

  const getExceptionsList = async () => {
    try {
      const result = await ExceptionManagement.getExceptionsList(
        pagedResult.pageNumber,
        pagedResult.pageSize,
        searchTerm,
        (status === 99 || status === null) ? undefined : status
      );
      setPagedResult(result.data.data);
      console.log(result)
    } catch (error) {
      console.error("Error fetching groups", error);
    }
  }

  const handlePageChange = (newPage: number) => {
    setPagedResult({
      ...pagedResult,
      pageNumber: newPage,
    });
  };

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value);
    //pagedResult.pageNumber = 1;
  };

  const onChangeStatus = (e: React.ChangeEvent<HTMLInputElement>) => {
    setStatus(parseInt(e.target.value));
  };

  const onClickViewDetails = (refNo: string) => {
    navigate(`/ExceptionsManagement/Details/${refNo}`)
  }

  const onClickAddException = () => {
    navigate("/ExceptionsManagement/AddException")
  }

  const onClickSearch = ()=>{
    setPagedResult({
      ...pagedResult,
      pageNumber: 1,
    });
    getExceptionsList();
  }

  return (
    <>
      <Typography variant="h4" component="h4" gutterBottom>
        Dashboard
      </Typography>
      <Typography variant="h6" component="h6" gutterBottom>
        Exceptions Overview & Summary
      </Typography>
      <Box sx={globalStyle.mainBox}>
        {hasPermission("Permissions.Exceptions.Create") && (
          <Box sx={{ m: 1 }}>
            <GlobalButton buttonAction="add" buttonName="Add Exception" onClick={() => onClickAddException()} />
          </Box>
        )}

        {/* Search input box with spacing */}
        <Box sx={globalStyle.searchBox}>
          {/* Search Input */}
          <TextField
            label="Search"
            variant="outlined"
            size="small"
            sx={globalStyle.searchInput} 
            value={searchTerm}
            onChange={handleSearchChange} 
          />
          <Button variant="contained" color="primary" onClick={onClickSearch}>
            Search
          </Button>
        </Box>
      </Box>
      <Box style={{ width: 150, marginBottom: 5 }}>
        <TextField
          select
          label="Status"
          variant="outlined"
          fullWidth
          name="userRole"
          //value={formData.userRole}
          onChange={onChangeStatus}
          required
        >
          {statusOptions.map((x, index) => (
            <MenuItem key={index} value={x.value}>
              {x.label}
            </MenuItem>
          ))}
        </TextField>
      </Box>

      <Table columns={columns} data={pagedResult.items} />

      <PaginationControls
        currentPage={pagedResult.pageNumber}
        totalPages={Math.ceil(pagedResult.totalCount / pagedResult.pageSize)}
        onPageChange={handlePageChange}
        totalItems={pagedResult.totalCount}
      />
    </>
  );
}

export default Dashboard