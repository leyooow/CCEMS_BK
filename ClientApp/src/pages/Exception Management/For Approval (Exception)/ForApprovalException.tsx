/* eslint-disable react-hooks/exhaustive-deps */
/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect, useState } from 'react';
import { useNavigate } from "react-router-dom";

import Table from '../../../components/Table/Table';


import ExceptionManagement from '../../../services/exceptionManagement';
import { PagedResult } from '../../../models/GenericResponseDTO';
import { ExceptionDTO } from '../../../models/exceptionManagementDTOs';
import { globalStyle } from '../../../styles/theme';
import { Box, Button, Chip, TextField, Typography } from '@mui/material';

import PaginationControls from '../../../components/Pagination/PaginationControls';
import GlobalButton from '../../../components/Button/Button';

const ForApprovalException = () =>  {

  const navigate = useNavigate();
  const [pagedResult, setPagedResult] = useState<PagedResult<ExceptionDTO>>({
    items: [],
    totalCount: 0,
    pageNumber: 1,
    pageSize: 10,
    searchTerm: ''
  });
  const [pageCount, setPageCount] = useState(0);
  const [searchTerm, setSearchTerm] = useState<string>(pagedResult.searchTerm);
  const columns = [
    {
      label: 'Action',
      render: (e: any) => <GlobalButton buttonAction="view" onClick={() => onClickViewDetails(e.refNo)}></GlobalButton>
    },
    { label: 'Request', accessor: 'status',
      render: (rowData:any) => {
        switch (rowData.changes) {
          case "Delete":
            return <Chip label="For Deletion" color="error" />;
          case "Update":
            return <Chip label="For Updating" color="info" />;
          default:
            return "";
        }
      }
    },
    { label: 'Reference No.', accessor: 'refNo' },
    { label: 'Employee ID', accessor: 'refNo' },
    { label: 'Branch Code/ Name', accessor: 'branchName' },
    { label: 'Transaction Type', accessor: 'type' },
    { label: 'Transaction Date', accessor: 'transactionDate' },
    { label: 'Created By', accessor: 'createdBy' },
    { label: 'Severity	', accessor: 'employeeId' },
    { label: 'Status', accessor: 'employeeID' },
    { label: 'Date Created', accessor: 'dateCreated' }
  ];

  useEffect(() => {
    getExceptionsList();
  }, [pagedResult.pageNumber, pagedResult.pageSize]);

  const getExceptionsList = async() => {
    try {
      const result = await ExceptionManagement.getForApprovalList(
        pagedResult.pageNumber,
        pagedResult.pageSize,
        searchTerm,
        null
      );
      setPagedResult(result.data.data);
      setPageCount(Math.ceil(pagedResult.totalCount / pagedResult.pageSize));
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
  };

  const onClickViewDetails = (refNo: string) => {
    navigate(`/ExceptionsManagement/Approval/${refNo}`)
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
      <Typography variant="h4" component="h6" gutterBottom>
      Approval Dashboard
      </Typography>
      <Typography variant="h6" component="h6" gutterBottom>
      Exceptions Overview & Summary
      </Typography>

      <Box sx={globalStyle.mainBox}>

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
     
     <Table columns={columns} data={pagedResult.items} />

     <PaginationControls
        currentPage={pagedResult.pageNumber}
        totalPages={pageCount}
        onPageChange={handlePageChange}
        totalItems={pagedResult.totalCount}
      />
    </>
  )
}

export default ForApprovalException