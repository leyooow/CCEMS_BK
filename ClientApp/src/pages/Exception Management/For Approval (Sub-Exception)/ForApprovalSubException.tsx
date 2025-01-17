/* eslint-disable react-hooks/exhaustive-deps */
/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect, useState } from 'react';
import { useNavigate } from "react-router-dom";

import Table from '../../../components/Table/Table';
import SubExceptionService from '../../../services/subExceptionService';
import { PagedResult } from '../../../models/GenericResponseDTO';
import { ExceptionCodeRevsDTO } from '../../../models/exceptionManagementDTOs';
import { globalStyle } from '../../../styles/theme';
import { Box, Button, Chip, TextField, Typography } from '@mui/material';

import PaginationControls from '../../../components/Pagination/PaginationControls';
import GlobalButton from '../../../components/Button/Button';

const ForApprovalSubException = () =>  {

  const navigate = useNavigate();
  const [pagedResult, setPagedResult] = useState<PagedResult<ExceptionCodeRevsDTO>>({
    items: [],
    totalCount: 0,
    pageNumber: 1,
    pageSize: 10,
    searchTerm: ''
  });
  const [searchTerm, setSearchTerm] = useState<string>(pagedResult.searchTerm);
  const columns = [
    {
      label: 'Action',
      render: (e: ExceptionCodeRevsDTO) => <GlobalButton buttonAction="view" onClick={() => onClickViewDetails(e.subReferenceNo)}></GlobalButton>
    },
    { label: 'Request', accessor: 'status',
      render: (rowData:any) => {
        switch (rowData.changes) {
          case "Delete":
            return <Chip label="For Deletion" color="error" />;
          case "Update":
            return <Chip label="For Status Update" color="info" />;
          default:
            return "";
        }
      }
    },
    { label: 'Sub Reference No.', accessor: 'subReferenceNo' },
    { label: 'Date Requested', accessor: 'modifiedDateTime' },
    { label: 'Requested By', accessor: 'modifiedBy' },
    { label: 'Exception Code', accessor: 'exCode' },
    { label: 'Description', accessor: 'exCodeDescription' }
  ];

  useEffect(() => {
    getExceptionsList();
  }, [pagedResult.pageNumber, pagedResult.pageSize,]);

  const getExceptionsList = async() => {
    try {
      const result = await SubExceptionService.getForApprovalList(
        pagedResult.pageNumber,
        pagedResult.pageSize,
        searchTerm,
        null
      );
      setPagedResult(result.data.data);
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
    navigate(`/SubExceptionsManagement/Details/${refNo}`)
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
      Sub-Exceptions Overview & Summary
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
        totalPages={Math.ceil(pagedResult.totalCount / pagedResult.pageSize)}
        onPageChange={handlePageChange}
        totalItems={pagedResult.totalCount}
      />
    </>
  )
}

export default ForApprovalSubException