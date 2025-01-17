import React, { useEffect, useState } from 'react';
import { useParams, useLocation } from "react-router-dom";
import Box from '@mui/material/Box';
import PaginationControls from '../../../components/Pagination/PaginationControls';
import Table from '../../../components/Table/Table';
import {
  TextField,
  Button,
  Grid,
  DialogContent,
  Divider,
  InputLabel,
  Chip,
  Link,
} from "@mui/material";
import ReportBranchReplyService from '../../../services/ReportBranchReplyService';
import { BranchReplyViewModel, RouteParams, PaginatedList, BranchReplyTable } from '../../../models/reportBranchReplyDTO';
import Cookies from 'js-cookie';
import ToastService from '../../../utils/toast';

const BranchReply: React.FC = () => {
  const { id } = useParams<RouteParams>(); // Extract the ID from the URL
  const location = useLocation();
  const queryParams = new URLSearchParams(location.search); // Parse query parameters
  const refNo = queryParams.get("refNo"); // Extract 'refNo'
  const Permission = Cookies.get('Permission') ?? "";
  const PermissionReportsWriteActionPlan = Permission.split(',').includes('"Permissions.Reports.WriteActionPlan"');

  const [formData, setFormData] = useState<BranchReplyViewModel>({
    ActionPlan: "",
    ReportContentsId: id ?? "",
  });
  const [pagedResult, setPagedResult] = useState<PaginatedList>({
    data: [],
    pageIndex: 1,
    totalPages: 0,
    countData: 0,
    hasPreviousPage: false,
    hasNextPage: false,
  });
  const [replyPagedResult, setReplyPagedResult] = useState<BranchReplyTable[]>([]);

  const columns = [
    {
      label: "Reference No",
      render: (data: any) => (
        <a
          href={`/SubExceptionsManagement/Details/${data.exceptionNo}`} // Dynamic link based on `exceptionNo`
          target="_blank" // Opens the link in a new tab
          style={{ color: "#007bff" }} // Inline style for Bootstrap blue color
        >
          {data.exceptionNo}
        </a>
      ),
    },
    { label: 'Branch Code', accessor: 'branchCode' },
    { label: 'Branch Name', accessor: 'branchName' },
    { label: 'Area', accessor: 'area' },
    { label: 'Division', accessor: 'division' },
    { label: 'Transaction Date', accessor: 'transactionDate' },
    { label: 'Aging', accessor: 'aging' },
    { label: 'Aging Category', accessor: 'agingCategory' },
    { label: 'Process', accessor: 'process' },
    { label: 'Account No', accessor: 'accountNo' },
    { label: 'Account Name', accessor: 'accountName' },
    { label: 'Deviation', accessor: 'deviation' },
    { label: 'Risk Classification', accessor: 'riskClassification' },
    { label: 'Deviation Category', accessor: 'deviationCategory' },
    { label: 'Amount', accessor: 'amount' },
    { label: 'Employee Responsible', accessor: 'personResponsible' },
    { label: 'Other Employee Responsible', accessor: 'otherPersonResponsible' },
    { label: 'Remarks', accessor: 'remarks' },
    { label: 'Action Plan', accessor: 'actionPlan' },
    { label: 'Encoded By', accessor: 'encodedBy' },
    { label: 'Root Cause', accessor: 'rootCause' },
    { label: 'Exception Approver', accessor: 'deviationApprover' },
  ];

  const replyColumns = [
    { label: '', render: (data: any) => (
      data.isHighlight ? (
        <Chip label="Current" color="success" variant="filled" />
      ) : (
        ""
      )
    ),},
    { label: 'Exception No', accessor: 'exceptionNo' },
    { label: 'Date', accessor: 'dateCreated' },
    { label: 'Written By', accessor: 'createdBy' },
    { label: 'Branch Reply', accessor: 'actionPlan' }
  ];

  const GetReportContentList = async () => {
    try {
      const result = await ReportBranchReplyService.getReportContentList(
        id ?? "",
        refNo ?? "",
        pagedResult.pageIndex,
      );
      var x = result.data;
      setPagedResult({
        pageIndex: x.pageIndex,
        totalPages: x.totalPages,
        countData: x.countData,
        hasPreviousPage: x.hasPreviousPage,
        hasNextPage: x.hasNextPage,
        data: x.data,
      });
    } catch (error) {
      console.error("Error GetReportContentList", error);
    }
  };
  const GetBranchReplyList = async () => {
    try {
      const result = await ReportBranchReplyService.getBranchReplyList(
        id ?? "",
      );
      var x = result.data;
      setReplyPagedResult(x);
      console.log(replyPagedResult);
    } catch (error) {
      console.error("Error GetReportContentList", error);
    }
  };

  useEffect(() => {
    GetBranchReplyList();
  }, []);
  useEffect(() => {
    GetReportContentList();
  }, [pagedResult.pageIndex]);

  const handlePageChange = (newPage: number) => {
    setPagedResult({
      ...pagedResult,
      pageIndex: newPage,
    });
  };

  
  const FormDataOnChange = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = event.target;
    setFormData((prevData) => ({
      ...prevData,
      [name as string]: value,
    }));
  };

  const SubmitPostReply = async (e: any) => {
    e.preventDefault(); // Ensure the default form behavior is prevented
    try {
      const result = await ReportBranchReplyService.postReply(formData);
      if (result.data.success) {
        GetBranchReplyList();
        ToastService.success(result.data.message);
      } else {
        ToastService.error(result.data.message);
      }
    } catch (error) {
      console.error("Error in submitting reply:", error);
      ToastService.error('An error occurred while posting your reply.');
    }
  }
  const handleGoBack = () => {
    if(pagedResult.data.length > 0)
      window.location.href = `/ReportsManagement/Dashboard/ReportDetails/${pagedResult.data[0].reportId}`;
  };
  return <>
    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
      <h1 style={{ color: '#1976d2' }}>Branch Reply</h1>
      <Link onClick={handleGoBack} style={{ cursor: 'pointer', color: '#1976d2' }}>
        Go back to previous page
      </Link>
    </div>
    <Divider sx={{ bgcolor: "black" }} />

    { PermissionReportsWriteActionPlan && (
      <Box sx={{ width: '100%' }}>
      <h4 style={{ marginTop: 10 }}>Replies</h4>
        <form onSubmit={SubmitPostReply}>
          <DialogContent sx={{ overflowY: 'unset' }}>
            <Grid container spacing={2}>
            <Grid item xs={8} sm={8}>
              <InputLabel id="coverage-label">Action Plan</InputLabel>
              <TextField
                size="small" 
                autoComplete="off"
                variant="outlined"
                fullWidth
                name="ActionPlan"  // Fixed name to match formData property
                value={formData.ActionPlan}  // Ensure you bind to correct data
                onChange={FormDataOnChange}
                required
              />
            </Grid>
            <Grid item xs={2} sm={2} style={{ marginTop: 20 }}>
            <input
              type="hidden"
              name="reportContentsId"
              value={formData.ReportContentsId}
            />
            <Button type="submit" variant="contained" color="primary" fullWidth>
              Submit
            </Button>
            </Grid> 
            </Grid>
          </DialogContent>
        </form>
        <Table columns={replyColumns} data={replyPagedResult} />
      </Box>
    )}
    <Divider sx={{ bgcolor: "black" }} />
    <h4 style={{ marginTop: 10 }}>List</h4>
    <Box>
      <Table columns={columns} data={pagedResult.data} />
    </Box>
    <PaginationControls
      currentPage={pagedResult.pageIndex}
      totalPages={pagedResult.totalPages ?? 0}
      onPageChange={handlePageChange}
      totalItems={pagedResult.countData}
    /> 
    
    <Divider sx={{ bgcolor: "black" }} />
  </>
};

export default BranchReply;
