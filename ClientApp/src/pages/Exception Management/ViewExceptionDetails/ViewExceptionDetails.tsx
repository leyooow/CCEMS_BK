/* eslint-disable @typescript-eslint/no-unused-expressions */
/* eslint-disable @typescript-eslint/no-explicit-any */

import { useState, useEffect } from 'react';
import { useParams, useNavigate } from "react-router-dom";
import {
  Box,
  Button,
  MenuItem,
  Select,
  TextField,
  Typography,
  Checkbox,
  FormControlLabel,
  Grid,
  Chip,
  Tooltip,
  IconButton,
  InputLabel,
} from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import HelpIcon from '@mui/icons-material/Help';
import FlagIcon from '@mui/icons-material/Flag';
import Cookies from 'js-cookie';
import ExceptionManagement from '../../../services/exceptionManagement';
import { SubExceptionsListViewDTO, ExceptionDetailsDTO, ExceptionItemDTO } from '../../../models/exceptionManagementDTOs';
import Table from '../../../components/Table/Table';
import RemarksModal from '../../../components/Modal/RemarksModal';
import ToastService from "../../../utils/toast";
import GlobalButton from '../../../components/Button/Button';
import { FormattedDate } from '../../../utils/formatDate';

const ViewExceptionDetails = () => {
  const navigate = useNavigate();
  const { refNo } = useParams<{ refNo: string }>();
  const Permission = JSON.parse(Cookies.get('Permission') ?? "");
  const [formValues, setFormValues] = useState<ExceptionItemDTO>({} as ExceptionItemDTO);
  const [subExceptions, setSubExceptions] = useState<SubExceptionsListViewDTO[]>([]);
  const [exceptionDetails, setExceptionDetails] = useState<ExceptionDetailsDTO>();
  const [isDisabled, setIsDisabled] = useState<boolean>(false);
  const [isRemarksModalOpen, setIsRemarksModalOpen] = useState<boolean>(false);

  const columns = [
    { label: '',
      render: (x:any) => {
        if(x.approvalStatus == 0){
          return <GlobalButton buttonAction="edit" disabled>Update</GlobalButton>
        }else{
          if (formValues.status == 1 && Permission.includes("Permissions.SubExceptions.Edit"))
          {
            return <GlobalButton buttonAction="edit" onClick={() => navigate(`/SubExceptionsManagement/Details/${x.subReferenceNo}`)}>Update</GlobalButton>
          }
        }
      }
    },
    { label: 'Approval',
      render: (rowData:any) => {
        switch (rowData.approvalStatus) {
          case 0:
            return <Chip label="Pending" color="warning" />; 
          case 2:
            return <Chip label="Closed" color="primary" />;
          default:
            return <Chip label="Open" color="info" />;
        }
      }
    },
    { label: 'Deviation Status',
      render: (rowData:any) => {
        if(rowData.deviationStatus == 2){
          return <Chip label="Outstanding" color="error" />;
        }else{
          let x = "";
          if(rowData.deviationStatus == 4)
            x = 'Regularized';
          if(rowData.deviationStatus == 5)
            x = 'ForCompliance';
          if(rowData.deviationStatus == 7)
            x = 'Dispensed';
          return <Chip label={x} color="primary" />;
        }
      }
    },
    { label: 'Sub-ERN', accessor: 'subReferenceNo' },
    { label: 'Deviation', accessor: 'exCodeDescription' },
    { label: 'Deviation Category', accessor: 'deviationCategory' },
    { label: 'Risk Classification', accessor: 'riskClassification' },
    { label: 'Date', accessor: 'dateCreated', render: (data: any) => FormattedDate(data.dateCreated)}
  ];

  useEffect(() => {
    console.log(1)
    getExceptionDetails();
  }, [refNo]);

  const getExceptionDetails = async() => {
    try {
      const result = await ExceptionManagement.getExceptionDetails(refNo);
      const data = result.data.data.exceptionItem;
      data.transactionDate = FormattedDate(data.transactionDate);
      data.dateCreated =  FormattedDate(data.dateCreated);
      setIsDisabled(true)
      setExceptionDetails(result.data.data)
      setSubExceptions(result.data.data.subExceptionItems);
      setFormValues(data);
    } catch (error) {
      console.error("Error fetching groups", error);
    }
  }
  const handleInputChange = (field: any, value: any) => {
    setFormValues({
      ...formValues,
      [field]: value,
    });
  };
  
  const handleRemarksModalSubmit = async(remarks: string) => {
    try {
      const result = await ExceptionManagement.deleteException(refNo, remarks);
      result.data
      if(result.data.data.statusCode == 200){
        ToastService.success(result.data.data.message);
      }
      else{
        ToastService.error(result.data.data.message);
      }
      setIsRemarksModalOpen(false);
    } catch (error) {
      ToastService.error(`Error fetching groups ${error}`);
    }
  }

  const StatusUI = () => {
    if (exceptionDetails != null) {
      if (exceptionDetails.exceptionItem.status === 1) {
        return (
          <Typography variant="h5">
            Exception Details - <Chip label="Open" color="primary" />
          </Typography>
        );
      } else if (exceptionDetails.exceptionItem.status === 2) {
        return (
          <Typography variant="h5">
            Exception Details - <Chip label="Closed" color="success" />
          </Typography>
        );
      } else if (exceptionDetails.exceptionItem.status === 0) {
        return (
          <Typography variant="h5">
            <Chip label="Update Exception Request" color="primary" /> - <Chip label="Pending Approval" color="warning" />
          </Typography>
        );
      }
    }
    return null;
  };
  
  return (
    <div style={{padding: 20}}>
      {
      exceptionDetails &&
      <>
        <Grid container >
          <Grid item xs={6}>
            <StatusUI/>
          </Grid>
          <Grid item xs={6}>
            {Permission.includes("Permissions.Exceptions.Edit") && (
              <>
                {exceptionDetails?.hasPendingUpdate ? (
                  exceptionDetails.exceptionItem.status === 1 && (
                    <>
                      <Button variant="contained" color="primary" disabled startIcon={<EditIcon />}>
                        Edit Details
                      </Button>
                      <Tooltip title="Edit button disabled due to pending approvals" placement="bottom">
                        <IconButton color="error">
                          <HelpIcon />
                        </IconButton>
                      </Tooltip>
                    </>
                  )
                ) : (
                  exceptionDetails?.exceptionItem.status !== 0 &&
                  exceptionDetails?.exceptionItem.status !== 2 && (
                    <div style={{ float: 'right' }}>
                      <Button variant="contained" color="primary" startIcon={<EditIcon />} 
                      onClick={() => navigate(`/ExceptionsManagement/Edit/${exceptionDetails?.exceptionItem.refNo}`)}
                      >
                        Edit
                      </Button>
                      <Button variant="contained" color="error" startIcon={<DeleteIcon />} onClick={()=> setIsRemarksModalOpen(true)}>
                        Delete
                      </Button>
                    </div>
                  )
                )}
              </>
            )}
          </Grid>
        </Grid>
        <Grid container spacing={2}>
          {/* Information Section */}
          <Grid item xs={12} md={6}>
            <Typography variant="h5" mb={2}>
              Information
            </Typography>
            <Grid container columnSpacing={2} rowSpacing={2}>
              <Grid item xs={12}>
                <TextField
                  label="Exception Reference Number"
                  fullWidth
                  value={formValues.refNo}
                  onChange={(e) => handleInputChange('exceptionReference', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Employee ID"
                  fullWidth
                  value={formValues.employeeId}
                  onChange={(e) => handleInputChange('employeeId', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  label="Branch Name"
                  fullWidth
                  value={formValues.branchName}
                  onChange={(e) => handleInputChange('branchName', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  label="Branch Code"
                  fullWidth
                  value={formValues.branchCode}
                  onChange={(e) => handleInputChange('branchCode', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Division"
                  fullWidth
                  value={formValues.division}
                  onChange={(e) => handleInputChange('division', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Area"
                  fullWidth
                  value={formValues.area}
                  onChange={(e) => handleInputChange('area', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  type="date"
                  fullWidth
                  label="Transaction Date"
                  value={formValues.transactionDate}
                  //onChange={(newValue) => handleInputChange('transactionDate', newValue)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <InputLabel>Root Cause</InputLabel>
                <Select
                  fullWidth
                  value={formValues.rootCause}
                  onChange={(e) => handleInputChange('rootCause', e.target.value)}
                  disabled={isDisabled}
                >
                  <MenuItem value={1}>Employee Lapse</MenuItem>
                  <MenuItem value={2}>Business Decision</MenuItem>
                </Select>
              </Grid>
              <Grid item xs={12} sm={6} mt={'18px'}>
                <TextField
                  label="Exception Approver"
                  fullWidth
                  value={formValues.deviationApprover}
                  onChange={(e) => handleInputChange('deviationApprover', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <InputLabel>Aging Category</InputLabel>
                <Select
                    displayEmpty  
                    fullWidth
                    value={formValues.agingCategory}
                    onChange={(e) => handleInputChange('agingCategory', e.target.value)}
                    disabled={isDisabled}
                  >
                    <MenuItem value="">-- Select Root Cause --</MenuItem>
                    <MenuItem value="1">≤ 7D banking days</MenuItem>
                    <MenuItem value="2">≤ 15D banking days</MenuItem>
                    <MenuItem value="3">≤ 30D banking days</MenuItem>
                    <MenuItem value="4">≤ 45D banking days</MenuItem>
                    <MenuItem value="5">≤ 180D banking days</MenuItem>
                    <MenuItem value="6">≤ 1Y (251 banking days)</MenuItem>
                    <MenuItem value="7">≤ 2Y (2 x 251 banking days)</MenuItem>
                    <MenuItem value="8">≤ 3Y (3 x 251 banking days)</MenuItem>
                    <MenuItem value="9">≤ 4Y (4 x 251 banking days)</MenuItem>
                    <MenuItem value="10">≤ 5Y (5 x 251 banking days)</MenuItem>
                  </Select>
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Employee Responsible"
                  fullWidth
                  value={formValues.personResponsible}
                  onChange={(e) => handleInputChange('employee', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Other Employee/s Responsible"
                  fullWidth
                  value={formValues.otherPersonResponsible}
                  onChange={(e) => handleInputChange('responsible', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Remarks"
                  fullWidth
                  value={formValues.remarks}
                  onChange={(e) => handleInputChange('remarks', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={formValues.redFlag}
                      onChange={(e) => handleInputChange('redFlag', e.target.checked)}
                      disabled={isDisabled}
                    />
                  }
                  label={<><FlagIcon style={{ color: 'red'}}/> Red Flag</>}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Exception Creator"
                  fullWidth
                  value={formValues.createdBy}
                  onChange={(e) => handleInputChange('remarks', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Date Created"
                  fullWidth
                  value={formValues.dateCreated}
                  onChange={(e) => handleInputChange('remarks', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
            </Grid>
          </Grid>

          {/* Transaction Section */}
          <Grid item xs={12} md={6}>
            <Typography variant="h5" mb={2}>
              Transaction
            </Typography>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <Select
                  label="Transaction Type"
                  fullWidth
                  value={formValues.type}
                  onChange={(e) => handleInputChange('transactionType', e.target.value)}
                  disabled={isDisabled}
                >
                  <MenuItem value="">-- Select Transaction Type --</MenuItem>
                  <MenuItem value="1">Monetary</MenuItem>
                  <MenuItem value="2">Non Monetary</MenuItem>
                  <MenuItem value="3">Miscellaneous</MenuItem>
                </Select>
              </Grid>
              {/* TransactionTypeEnum.Monetary */}
              {formValues.type == 1 &&
              <>
              <Grid item xs={12} sm={6}>
                <TextField
                  label="BDS User ID"
                  fullWidth
                  value={formValues.monetaries[0].bdstellerId}
                  onChange={(e) => handleInputChange('cifNo', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  label="Sequence No"
                  fullWidth
                  value={formValues.monetaries[0].sequenceNo}
                  onChange={(e) => handleInputChange('cifNo', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Transaction Code"
                  fullWidth
                  value={formValues.monetaries[0].transCode}
                  onChange={(e) => handleInputChange('cifNo', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Transaction Description"
                  fullWidth
                  value={formValues.monetaries[0].transDescription}
                  onChange={(e) => handleInputChange('cifNo', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Credit Account Name"
                  fullWidth
                  value={formValues.monetaries[0].creditAccountName}
                  onChange={(e) => handleInputChange('cifNo', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Credit Account No"
                  fullWidth
                  value={formValues.monetaries[0].creditAccountNo}
                  onChange={(e) => handleInputChange('cifNo', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Debit Account Name"
                  fullWidth
                  value={formValues.monetaries[0].debitAccountName}
                  onChange={(e) => handleInputChange('cifNo', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Debit Account No"
                  fullWidth
                  value={formValues.monetaries[0].debitAccountNo}
                  onChange={(e) => handleInputChange('cifNo', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  label="Amount"
                  fullWidth
                  value={formValues.monetaries[0].amount}
                  onChange={(e) => handleInputChange('cifNo', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  label="Currency"
                  fullWidth
                  value={formValues.monetaries[0].currency}
                  onChange={(e) => handleInputChange('cifNo', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              </>}
              {/* TransactionTypeEnum.NonMonetary */}
              {formValues.type == 2 &&
              <>
              <Grid item xs={12}>
                <Select
                  label="Category"
                  fullWidth
                  value={formValues.nonMonetaries[0].category}
                  onChange={(e) => handleInputChange('category', e.target.value)}
                  disabled={isDisabled}
                >
                  <MenuItem value="">-- Select Category --</MenuItem>
                  <MenuItem value="1">CIF Creation</MenuItem>
                  <MenuItem value="2">CIF Maintenance</MenuItem>
                  <MenuItem value="3">Account Opening</MenuItem>
                  <MenuItem value="4">Reactivated Dormant</MenuItem>
                </Select>
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="CIF No"
                  fullWidth
                  value={formValues.nonMonetaries[0].cifnumber}
                  onChange={(e) => handleInputChange('cifNo', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Customer Account No"
                  fullWidth
                  value={formValues.nonMonetaries[0].customerAccountNo}
                  onChange={(e) => handleInputChange('customerAccountNo', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Customer Account Name"
                  fullWidth
                  value={formValues.nonMonetaries[0].customerName}
                  onChange={(e) => handleInputChange('customerAccountName', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              </>}
              {/* TransactionTypeEnum.Miscellaneous */}
              {formValues.type == 3 &&
              <>
              <Grid item xs={12}>
                <Select
                  label="Category"
                  fullWidth
                  value={formValues.miscs[0].category}
                  onChange={(e) => handleInputChange('category', e.target.value)}
                  disabled={isDisabled}
                >
                  <MenuItem value="">-- Select Category --</MenuItem>
                  <MenuItem value="1">Red EMV</MenuItem>
                  <MenuItem value="2">Bank Certification</MenuItem>
                  <MenuItem value="3">General Ledger</MenuItem>
                  <MenuItem value="4">Deposit Pick-up Authorization Form</MenuItem>
                  <MenuItem value="5">Due From Local Banks</MenuItem>
                  <MenuItem value="6">BDS Reports</MenuItem>
                  <MenuItem value="7">Other Clearing Deficiencies</MenuItem>
                  <MenuItem value="8">Checkbook</MenuItem>
                  <MenuItem value="9">New Account Tagging</MenuItem>
                  <MenuItem value="10">Surprise Count</MenuItem>
                  <MenuItem value="11">Official Receipt</MenuItem>
                </Select>
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Card Number"
                  fullWidth
                  value={formValues.miscs[0].cardNo}
                  onChange={(e) => handleInputChange('cifNo', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="Bank Cert Number"
                  fullWidth
                  value={formValues.miscs[0].bankCertNo}
                  onChange={(e) => handleInputChange('customerAccountNo', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  label="DPAF Number"
                  fullWidth
                  value={formValues.miscs[0].dpafno}
                  onChange={(e) => handleInputChange('customerAccountName', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  label="Check Number"
                  fullWidth
                  value={formValues.miscs[0].checkNo}
                  onChange={(e) => handleInputChange('customerAccountName', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  label="Amount"
                  fullWidth
                  value={formValues.miscs[0].amount}
                  onChange={(e) => handleInputChange('customerAccountName', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  label="Account Number"
                  fullWidth
                  value={formValues.miscs[0].glslaccountNo}
                  onChange={(e) => handleInputChange('customerAccountName', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  label="Account Name"
                  fullWidth
                  value={formValues.miscs[0].glslaccountName}
                  onChange={(e) => handleInputChange('customerAccountName', e.target.value)}
                  disabled={isDisabled}
                />
              </Grid>
              </>}
            </Grid>
          </Grid>
        </Grid>
        
        <Box mt={2}>
          <Typography variant='h5'>Deviations:</Typography>
          <Table columns={columns} data={subExceptions} />
        </Box>
      </>
      }
      <RemarksModal
        open={isRemarksModalOpen}
        title="Do you want to Delete this Exception?"
        buttonTitle="Submit"
        message="Please enter your reason for deleting this exception."
        onSubmit={handleRemarksModalSubmit}
        onClose={() => setIsRemarksModalOpen(false)}
      />
    </div>
  );
};

export default ViewExceptionDetails;
