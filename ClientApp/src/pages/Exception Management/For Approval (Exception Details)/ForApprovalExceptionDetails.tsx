/* eslint-disable react-hooks/exhaustive-deps */
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from "react-router-dom";
import {
  Box,
  Button,
  Typography,
  Grid,
  Chip,
  Divider,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableRow,
} from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import CloseIcon from '@mui/icons-material/Close';
import FlagIcon from '@mui/icons-material/Flag';
import ArrowForwardIcon from '@mui/icons-material/ArrowForward';
import Cookies from 'js-cookie';
import ExceptionManagement from '../../../services/exceptionManagement';
import {
  ExceptionDetailsDTO, getRootCauseDisplayName, getAgingCategoryDisplayName,
  getTransactionTypeDisplayName, getNonMonetaryTypesDisplayName
} from '../../../models/exceptionManagementDTOs';
import RemarksModal from '../../../components/Modal/RemarksModal';
import ToastService from "../../../utils/toast";
import Loader from '../../../components/Loader/Loader';

interface TableHeadUIProps {
  type: string;
}

interface TableRowUI {
  label: string;
  value: React.ReactNode | null;
  revisionValue?: React.ReactNode | null | undefined;
  arrowIcon?: React.ReactNode | '';
}
const ForApprovalExceptionDetails = () => {
  const navigate = useNavigate();
  const [isloading, setIsloading] = useState(false);
  const { refNo } = useParams<{ refNo: string }>();
  const Permission = JSON.parse(Cookies.get('Permission') ?? "");
  const [exceptionDetails, setExceptionDetails] = useState<ExceptionDetailsDTO>({} as ExceptionDetailsDTO);
  const [isRemarksModalOpen, setIsRemarksModalOpen] = useState<boolean>(false);
  const [action, setAction] = useState<string>('');
  const [remarksLabel, setRemarksLabel] = useState<string>('');
  const [remarksHeader, setRemarksHeader] = useState<string>('');
  useEffect(() => {
    getExceptionForApprovalDetails();
  }, [refNo]);

  const getExceptionForApprovalDetails = async () => {
    try {
      const result = await ExceptionManagement.getExceptionForApprovalDetails(refNo);
      const data = result.data.data;
      data.exceptionItem.rootCause = getRootCauseDisplayName(data.exceptionItem.rootCause);
      data.exceptionItemRevs.rootCause = getRootCauseDisplayName(data.exceptionItemRevs.rootCause);
      data.exceptionItem.agingCategory = getAgingCategoryDisplayName(data.exceptionItem.agingCategory);
      data.exceptionItemRevs.agingCategory = getAgingCategoryDisplayName(data.exceptionItemRevs.agingCategory);
      data.exceptionItem.type = getTransactionTypeDisplayName(data.exceptionItem.type);
      if (data.exceptionItem.nonMonetaries.length > 0)
        data.exceptionItem.nonMonetaries[0].category = getNonMonetaryTypesDisplayName(data.exceptionItem.nonMonetaries[0].category);
      if (data.exceptionItemRevs.nonMonetaryRevs != null)
        data.exceptionItemRevs.nonMonetaryRevs.type = getNonMonetaryTypesDisplayName(data.exceptionItemRevs.nonMonetaryRevs?.type);

      setExceptionDetails(data)
    } catch (error) {
      console.error("Error fetching groups", error);
    }
  }
  const handleRemarksModalSubmit = async (remarks: string) => {
    setIsloading(true)
    try {
      if (action == "approve") {
        const result = await ExceptionManagement.approveException(refNo, remarks);
        if (result.statusCode == 200) {
          ToastService.success(result.message);
          // setTimeout(() => navigate(`/ExceptionsManagement/ForApprovalExceptions`), 1500)
          navigate(`/ExceptionsManagement/ForApprovalExceptions`)
        }
        else {
          ToastService.error(result.message);
        }
      } else {
        const result = await ExceptionManagement.rejectException(refNo, remarks);
        if (result.statusCode == 200) {
          ToastService.success(result.message);
          // setTimeout(() => navigate(`/ExceptionsManagement/ForApprovalExceptions`), 1500)
          navigate(`/ExceptionsManagement/ForApprovalExceptions`)
        }
        else {
          ToastService.error(result.message);
        }
      }

      setIsRemarksModalOpen(false);
    } catch (error) {
      ToastService.error(`Error fetching groups ${error}`);
    } finally {
      setIsloading(false)
    }

  }
  const onClickAction = (action: string) => {
    setRemarksLabel(action == "approve" ? "Please enter your approval Remarks before submission." : "Please let your Team know the reason you rejected this.")
    setRemarksHeader(action == "approve" ? "Do you want to Approve this request?" : "Do you want to Reject this request?")
    setAction(action);
    setIsRemarksModalOpen(true)
  }

  const TableHeadUI: React.FC<TableHeadUIProps> = ({ type }) => {
    return <>
      <TableRow>
        <TableCell><Chip label={type} color="info" /></TableCell>
        <TableCell><Chip label="Original Data" color="primary" /></TableCell>
        <TableCell>
        </TableCell>
        <TableCell><Chip label="Requested Data" color="primary" /></TableCell>
      </TableRow>
    </>;
  }

  const TableRowUI: React.FC<TableRowUI> = ({
    label,
    value,
    revisionValue,
    arrowIcon = ''
  }) => {
    return <TableRow>
      <TableCell><strong>{label}</strong></TableCell>
      <TableCell>{value}</TableCell>
      <TableCell>{arrowIcon}</TableCell>
      <TableCell>{revisionValue}</TableCell>
    </TableRow>
  }

  return (
    <>
      {isloading ? <Loader message='submitting please wait...'/> :
        <div>


          <Grid container>
            <Grid item xs={6}>
              <Typography variant="h5">
                <Chip label="Update Exception Request" color="primary" /> - <Chip label="Pending Approval" color="warning" />
              </Typography>
            </Grid>
            <Grid item xs={6}>
              {Permission.includes("Permissions.Exceptions.Approval") && (
                <>
                  <div style={{ float: 'right' }}>
                    <Button variant="contained" color="primary" startIcon={<EditIcon />} onClick={() => onClickAction("approve")}>
                      Approve
                    </Button>
                    <Button variant="contained" color="warning" startIcon={<CloseIcon />} onClick={() => onClickAction("reject")}>
                      Reject
                    </Button>
                  </div>
                </>
              )}
            </Grid>
          </Grid>
          <Box p={2}>

            {/* Update Info Section */}
            <Typography variant="h6">Update Info</Typography>
            <Divider sx={{ my: 2 }} />
            {exceptionDetails.exceptionItem && <TableContainer sx={{ mb: 4 }}>
              <Table>
                <TableBody>
                  <TableHeadUI type={'Primary Info'} />
                  <TableRowUI
                    label={'Reference Number'}
                    value={exceptionDetails.exceptionItem.refNo}
                  />
                  <TableRowUI
                    label={'Branch'}
                    value={`${exceptionDetails.exceptionItem.branchCode} - ${exceptionDetails.exceptionItem.branchName}`}
                  />
                  <TableRowUI
                    label={'Division'}
                    value={exceptionDetails.exceptionItem.division}
                  />
                  <TableRowUI
                    label={'Area'}
                    value={exceptionDetails.exceptionItem.area}
                  />
                  <TableRowUI
                    label={'Remarks'}
                    value={exceptionDetails.exceptionItem.remarks}
                    arrowIcon={<ArrowForwardIcon color="action" />}
                    revisionValue={exceptionDetails.exceptionItemRevs?.remarks}
                  />
                  <TableRowUI
                    label={'Tran Date'}
                    value={exceptionDetails.exceptionItem.transactionDate}
                    arrowIcon={<ArrowForwardIcon color="action" />}
                    revisionValue={exceptionDetails.exceptionItemRevs?.transactionDate}
                  />
                  <TableRowUI
                    label="Root Cause"
                    value={exceptionDetails.exceptionItem.rootCause}
                    arrowIcon={<ArrowForwardIcon color="action" />}
                    revisionValue={exceptionDetails.exceptionItemRevs?.rootCause}
                  />
                  <TableRowUI
                    label="Exception Approver"
                    value={exceptionDetails.exceptionItem.deviationApprover}
                    revisionValue={exceptionDetails.exceptionItemRevs.deviationApprover}
                    arrowIcon={<ArrowForwardIcon color="action" />}
                  />
                  <TableRowUI
                    label="Exception Responsible"
                    value={exceptionDetails.exceptionItem.personResponsible}
                    revisionValue={exceptionDetails.exceptionItemRevs.personResponsible}
                    arrowIcon={<ArrowForwardIcon color="action" />}
                  />
                  <TableRowUI
                    label="Other Responsible"
                    value={exceptionDetails.exceptionItem.otherPersonResponsible}
                    revisionValue={exceptionDetails.exceptionItemRevs.otherPersonResponsible}
                    arrowIcon={<ArrowForwardIcon color="action" />}
                  />
                  <TableRowUI
                    label="Aging Category"
                    value={exceptionDetails.exceptionItem.agingCategory}
                    revisionValue={exceptionDetails.exceptionItemRevs.agingCategory}
                    arrowIcon={<ArrowForwardIcon color="action" />}
                  />
                  <TableRowUI
                    label="Red Flag"
                    value={
                      exceptionDetails.exceptionItem.redFlag ? (
                        <FlagIcon style={{ color: 'red' }} />
                      ) : (
                        <FlagIcon style={{ color: 'gray' }} />
                      )
                    }
                    revisionValue={
                      exceptionDetails.exceptionItemRevs.redFlag ? (
                        <FlagIcon style={{ color: 'red' }} />
                      ) : (
                        <FlagIcon style={{ color: 'gray' }} />
                      )
                    }
                    arrowIcon={<ArrowForwardIcon color="action" />}
                  />
                </TableBody>
              </Table>
            </TableContainer>}

            {/* Transaction Info Section */}
            {exceptionDetails.exceptionItem &&
              <TableContainer>
                <Table>
                  <TableBody>
                    <TableHeadUI type={'Transaction Info'} />
                    <TableRowUI
                      label={'Transaction Type'}
                      value={exceptionDetails.exceptionItem.type}
                    />
                    {/* TransactionTypeEnum.Monetary */}
                    {exceptionDetails.exceptionItemRevs.type == 1 &&
                      <>
                        <TableRowUI
                          label="BDS BDS User ID"
                          value={exceptionDetails.exceptionItem.monetaries[0].bdstellerId}
                          revisionValue={exceptionDetails.exceptionItemRevs.monetaryRevs?.bdstellerId}
                          arrowIcon={<ArrowForwardIcon color="action" />}
                        />
                        <TableRowUI
                          label="Sequence Number"
                          value={exceptionDetails.exceptionItem.monetaries[0].sequenceNo}
                          revisionValue={exceptionDetails.exceptionItemRevs.monetaryRevs?.sequenceNo}
                          arrowIcon={<ArrowForwardIcon color="action" />}
                        />
                        <TableRowUI
                          label="Transaction Code"
                          value={exceptionDetails.exceptionItem.monetaries[0].transCode}
                          revisionValue={exceptionDetails.exceptionItemRevs.monetaryRevs?.transCode}
                          arrowIcon={<ArrowForwardIcon color="action" />}
                        />
                        <TableRowUI
                          label="Transaction Description"
                          value={exceptionDetails.exceptionItem.monetaries[0].transDescription}
                          revisionValue={exceptionDetails.exceptionItemRevs.monetaryRevs?.transDescription}
                          arrowIcon={<ArrowForwardIcon color="action" />}
                        />
                        <TableRowUI
                          label="Credit Account Name"
                          value={exceptionDetails.exceptionItem.monetaries[0].creditAccountName}
                          revisionValue={exceptionDetails.exceptionItemRevs.monetaryRevs?.creditAccountName}
                          arrowIcon={<ArrowForwardIcon color="action" />}
                        />
                        <TableRowUI
                          label="Credit Account Number"
                          value={exceptionDetails.exceptionItem.monetaries[0].creditAccountNo}
                          revisionValue={exceptionDetails.exceptionItemRevs.monetaryRevs?.creditAccountNo}
                          arrowIcon={<ArrowForwardIcon color="action" />}
                        />
                        <TableRowUI
                          label="Debit Account Name"
                          value={exceptionDetails.exceptionItem.monetaries[0].debitAccountName}
                          revisionValue={exceptionDetails.exceptionItemRevs.monetaryRevs?.debitAccountName}
                          arrowIcon={<ArrowForwardIcon color="action" />}
                        />
                        <TableRowUI
                          label="Debit Account Number"
                          value={exceptionDetails.exceptionItem.monetaries[0].debitAccountNo}
                          revisionValue={exceptionDetails.exceptionItemRevs.monetaryRevs?.debitAccountNo}
                          arrowIcon={<ArrowForwardIcon color="action" />}
                        />
                        <TableRowUI
                          label="Amount"
                          value={exceptionDetails.exceptionItem.monetaries[0].amount}
                          revisionValue={exceptionDetails.exceptionItemRevs.monetaryRevs?.amount}
                          arrowIcon={<ArrowForwardIcon color="action" />}
                        />
                        <TableRowUI
                          label="Currency"
                          value={exceptionDetails.exceptionItem.monetaries[0].currency}
                          revisionValue={exceptionDetails.exceptionItemRevs.monetaryRevs?.currency}
                          arrowIcon={<ArrowForwardIcon color="action" />}
                        />
                      </>}
                    {/* TransactionTypeEnum.NonMonetary */}
                    {exceptionDetails.exceptionItemRevs.type == 2 &&
                      <>
                        <TableRowUI
                          label="Non-Monetary Category"
                          value={exceptionDetails.exceptionItem.nonMonetaries[0].category}
                          revisionValue={exceptionDetails.exceptionItemRevs.nonMonetaryRevs?.type}
                          arrowIcon={<ArrowForwardIcon color="action" />}
                        />
                        <TableRowUI
                          label="CIF Number"
                          value={exceptionDetails.exceptionItem.nonMonetaries[0].cifnumber}
                          revisionValue={exceptionDetails.exceptionItemRevs.nonMonetaryRevs?.cifnumber}
                          arrowIcon={<ArrowForwardIcon color="action" />}
                        />
                        <TableRowUI
                          label="Customer Account Number"
                          value={exceptionDetails.exceptionItem.nonMonetaries[0].customerAccountNo}
                          revisionValue={exceptionDetails.exceptionItemRevs.nonMonetaryRevs?.customerAccountNo}
                          arrowIcon={<ArrowForwardIcon color="action" />}
                        />
                        <TableRowUI
                          label="Customer Name"
                          value={exceptionDetails.exceptionItem.nonMonetaries[0].customerName}
                          revisionValue={exceptionDetails.exceptionItemRevs.nonMonetaryRevs?.customerName}
                          arrowIcon={<ArrowForwardIcon color="action" />}
                        />
                      </>}
                    {/* TransactionTypeEnum.Miscellaneous */}
                    {exceptionDetails.exceptionItemRevs.type == 3 &&
                      <>
                        <TableRow>
                          <TableCell>Miscellaneous Category</TableCell>
                          <TableCell>{exceptionDetails.exceptionItem.miscs[0].category}</TableCell>
                          <TableCell><ArrowForwardIcon color="action" /></TableCell>
                          <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.type}</TableCell>
                        </TableRow>
                        {/* MiscTypes.EMV */}
                        {exceptionDetails.exceptionItemRevs.miscRevs?.type == 1 &&
                          <>
                            <TableRow>
                              <TableCell>Card Number</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].cardNo}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.cardNo}</TableCell>
                            </TableRow>
                            <TableRow>
                              <TableCell>Account No</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].glslaccountNo}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.glslaccountNo}</TableCell>
                            </TableRow>
                            <TableRow>
                              <TableCell>Account Name</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].glslaccountName}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.glslaccountName}</TableCell>
                            </TableRow>
                          </>}
                        {/* MiscTypes.BankCert */}
                        {exceptionDetails.exceptionItemRevs.miscRevs?.type == 2 &&
                          <>
                            <TableRow>
                              <TableCell>Bank Certificate No</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].bankCertNo}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.bankCertNo}</TableCell>
                            </TableRow>
                            <TableRow>
                              <TableCell>Account No</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].glslaccountNo}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.glslaccountNo}</TableCell>
                            </TableRow>
                            <TableRow>
                              <TableCell>Account Name</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].glslaccountName}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.glslaccountName}</TableCell>
                            </TableRow>
                          </>}

                        {/* MiscTypes.GeneralLedger */}
                        {exceptionDetails.exceptionItemRevs.miscRevs?.type == 3 &&
                          <>
                            <TableRow>
                              <TableCell>Account No</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].glslaccountNo}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.glslaccountNo}</TableCell>
                            </TableRow>
                            <TableRow>
                              <TableCell>Account Name</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].glslaccountName}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.glslaccountName}</TableCell>
                            </TableRow>
                          </>}

                        {/* MiscTypes.DepositPickupAuth */}
                        {exceptionDetails.exceptionItemRevs.miscRevs?.type == 4 &&
                          <>
                            <TableRow>
                              <TableCell>DPAF No</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].dpafno}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.dpafno}</TableCell>
                            </TableRow>
                            <TableRow>
                              <TableCell>Account No</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].glslaccountNo}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.glslaccountNo}</TableCell>
                            </TableRow>
                            <TableRow>
                              <TableCell>Account Name</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].glslaccountName}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.glslaccountName}</TableCell>
                            </TableRow>
                          </>}

                        {/* MiscTypes.DueFromLocalBanks */}
                        {exceptionDetails.exceptionItemRevs.miscRevs?.type == 5 &&
                          <>
                            <TableRow>
                              <TableCell>Account No</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].glslaccountNo}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.glslaccountNo}</TableCell>
                            </TableRow>
                            <TableRow>
                              <TableCell>Account Name</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].glslaccountName}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.glslaccountName}</TableCell>
                            </TableRow>
                          </>}

                        {/* MiscTypes.BDSReports */}
                        {exceptionDetails.exceptionItemRevs.miscRevs?.type == 6 &&
                          <>
                            <TableRow>
                              <TableCell>Account No</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].glslaccountNo}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.glslaccountNo}</TableCell>
                            </TableRow>
                            <TableRow>
                              <TableCell>Account Name</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].glslaccountName}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.glslaccountName}</TableCell>
                            </TableRow>
                          </>}

                        {/* MiscTypes.ClearingDeficiencies */}
                        {exceptionDetails.exceptionItemRevs.miscRevs?.type == 7 &&
                          <>
                            <TableRow>
                              <TableCell>Check No</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].checkNo}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.checkNo}</TableCell>
                            </TableRow>
                            <TableRow>
                              <TableCell>Amount</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].amount}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.amount}</TableCell>
                            </TableRow>
                            <TableRow>
                              <TableCell>Account No</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].glslaccountNo}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.glslaccountNo}</TableCell>
                            </TableRow>
                            <TableRow>
                              <TableCell>Account Name</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].glslaccountName}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.glslaccountName}</TableCell>
                            </TableRow>
                          </>}

                        {/* MiscTypes.Checkbook */}
                        {exceptionDetails.exceptionItemRevs.miscRevs?.type == 8 &&
                          <>
                            <TableRow>
                              <TableCell>Account No</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].glslaccountNo}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.glslaccountNo}</TableCell>
                            </TableRow>
                            <TableRow>
                              <TableCell>Account Name</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].glslaccountName}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.glslaccountName}</TableCell>
                            </TableRow>
                          </>}

                        {/* MiscTypes.NewAccountTagging */}
                        {exceptionDetails.exceptionItemRevs.miscRevs?.type == 9 &&
                          <>
                            <TableRow>
                              <TableCell>Account No</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].glslaccountNo}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.glslaccountNo}</TableCell>
                            </TableRow>
                            <TableRow>
                              <TableCell>Account Name</TableCell>
                              <TableCell>{exceptionDetails.exceptionItem.miscs[0].glslaccountName}</TableCell>
                              <TableCell><ArrowForwardIcon color="action" /></TableCell>
                              <TableCell>{exceptionDetails.exceptionItemRevs.miscRevs?.glslaccountName}</TableCell>
                            </TableRow>
                          </>}
                      </>}
                  </TableBody>
                </Table>
              </TableContainer>}
          </Box>
          <RemarksModal
            open={isRemarksModalOpen}
            title={remarksHeader}
            buttonTitle="Submit"
            message={remarksLabel}
            onSubmit={handleRemarksModalSubmit}
            onClose={() => setIsRemarksModalOpen(false)}
          />
        </div>
      }
    </>
  );
};

export default ForApprovalExceptionDetails;
