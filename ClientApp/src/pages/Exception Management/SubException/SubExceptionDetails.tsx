/* eslint-disable react-hooks/exhaustive-deps */
/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect, useState } from 'react';
import { useParams,useNavigate } from "react-router-dom";
import { Alert, Box, Button, Card, CardContent, Divider, Grid, IconButton, InputLabel, Link, MenuItem, Select, SelectChangeEvent, TextField, Typography } from '@mui/material';
import { DeviationStatusDTO, DeviationStatusDisplayNames, DeviationStatusUpdate, SubExceptionsDetailsDTO } from '../../../models/subExceptionDetailsDTO';
import SubExceptionDetailsService from '../../../services/SubExceptionDetailsService';
import ToastService from '../../../utils/toast';
import { RouteParams } from '../../../models/reportBranchReplyDTO';
import Cookies from 'js-cookie';
import DeleteTwoTone from '@mui/icons-material/DeleteTwoTone';
import { globalStyle } from '../../../styles/theme';
import ConfirmationModal from '../../../components/Modal/ConfirmationModal';
import { ArrowForward } from '@mui/icons-material';

const SubExceptionDetails = () => {
  const navigate = useNavigate();
  const { subERN } = useParams<RouteParams>(); // Extract the ID from the URL
  const Permission = Cookies.get('Permission') ?? "";
  const PermissionsSubExceptionsApproval = Permission.split(',').includes('"Permissions.SubExceptions.Approval"')
  const PermissionsSubExceptionsView = Permission.split(',').includes('"Permissions.SubExceptions.View"')
  
  const [formData, setFormData] = useState<SubExceptionsDetailsDTO>({
    exceptionCode: {
        id: 0,
        subReferenceNo: '',
        exCode: 0,
        exItemRefNo: '',
        exCodeDescription: '',
        deviationStatus: 0, // Default deviation status (e.g., Outstanding)
        dateCreated: new Date().toISOString(), // Default to current date/time
        approvalStatus: 0,
        entryDate: "",
        approvalRemarks: '',
        taggingDate: "",
        taggingDateDisplay: "",
    },
    newStatus: DeviationStatusDTO.Outstanding, // Default to Outstanding
    remarks: '',
    riskClassification: '',
    deviationCategory: '',
    taggingDate: '',
    taggingDateDisplay: "",
    branchReplyDetails: [],
    forDeletion: 0,
  });
  const [formUpdate, setFormUpdate] = useState<DeviationStatusUpdate>({
    subRefNo: "",
    newStatus: 2,
    taggingDate: new Date().toISOString(),
    exItemRefNo: "",
  });
  const [DeleteRemarks, setDeleteRemarks] = useState('');
  const [RejectRemarks, setRejectRemarks] = useState('');
  const [taggingDate, setTaggingDate] = useState(new Date().toISOString().split('T')[0]);

  
  const [deleteOpenConfirmModal, setDeleteOpenConfirmModal] = useState(false);
  const [rejectOpenConfirmModal, setRejectOpenConfirmModal] = useState(false);
  const isDateValid = formData?.taggingDate && formData?.taggingDate !== "" && formData?.taggingDate !== null;

  const GetSubExceptionDetails = async () => {
    try {
      const result = await SubExceptionDetailsService.getSubExceptionDetails(subERN ?? "");
      if (result.success) {
        result.data.taggingDateDisplay = formatDate(result.data.taggingDate);
        result.data.exceptionCode.taggingDateDisplay = formatDate(result.data.exceptionCode.taggingDate);
        
        setFormUpdate({
          ...formUpdate,
          newStatus: result.data.exceptionCode.deviationStatus,
        });
        setFormData(result.data);
        setFormUpdate((prevData) => ({
          ...prevData,
          subRefNo: result.data.exceptionCode.subReferenceNo,
          exItemRefNo: result.data.exceptionCode.exItemRefNo,
        }));
        console.log(formData)
      } else {
        handleNavigate();
      }
    } catch (error) {
      console.error("Error GetSubExceptionDetails", error);
    }
  };

  useEffect(() => {
    GetSubExceptionDetails();
  }, [subERN]);

  useEffect(() => {
    setFormUpdate({
      ...formUpdate,
      subRefNo: formData?.exceptionCode.subReferenceNo ?? "",
      exItemRefNo: formData?.exceptionCode.exItemRefNo ?? "",
    });
  }, [formData?.exceptionCode.exItemRefNo,formData?.exceptionCode.subReferenceNo]);

  const FormUpdateOnChange = (
    event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement> | SelectChangeEvent<DeviationStatusDTO>
  ) => {
    const { name, value } = event.target;

    setFormUpdate((prevData) => ({
      ...prevData,
      [name]: value, // Handle TextField
    }));
  };
  const DateOnChange = (
    event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement> | SelectChangeEvent<DeviationStatusDTO>
  ) => {
    const { name, value } = event.target;
    if (value) {
      const date = new Date(value);
      const isoString = date.toISOString();
      setTaggingDate(isoString.split('T')[0])
    } else {
      // Handle the case where taggingDate is null or undefined
    }

    setFormUpdate((prevData) => ({
      ...prevData,
      [name]: value,
    }));
    console.log(formUpdate);
  };
  const DeleteOpenConfirmModal = () => {
    setDeleteRemarks("");
    setDeleteOpenConfirmModal(true);
  };  
  const DeleteCloseConfirmModal = () => {
    setDeleteOpenConfirmModal(false);
  };
  const RejectOpenConfirmModal = () => {
    setRejectRemarks("");
    setRejectOpenConfirmModal(true);
  };  
  const RejectCloseConfirmModal = () => {
    setRejectOpenConfirmModal(false);
  };
  const DeleteOnClick = async () => {
    try {
      const result = await SubExceptionDetailsService.deleteSubException(subERN ?? "", DeleteRemarks);
      if (result.success) {
        ToastService.success(result.message);
      } else {
        ToastService.error(result.message);
      }
    } catch (error) {
      console.error("Error DeleteOnClick", error);
    } finally{
      setDeleteOpenConfirmModal(false);
    }
  };
  const UpdateOnClick= async () => {
    try {
      const result = await SubExceptionDetailsService.updateSubException(subERN ?? "",formUpdate);
      if (result.success) {
        ToastService.success(result.message);
        setFormUpdate((prevState) => ({
          ...prevState,
          newStatus: 2,
          taggingDate: new Date().toISOString(),
        }));
        GetSubExceptionDetails();
      } else {
        ToastService.error(result.message);
      }
    } catch (error) {
      console.error("Error UpdateOnClick", error);
    } finally{
      setDeleteOpenConfirmModal(false);
    }
  };
  const ApprovalOnClick= async () => {
    try {
      const result = await SubExceptionDetailsService.approveSubException(formData);
      if (result.success) {
        ToastService.success(result.message);
        GetSubExceptionDetails();
      } else {
        ToastService.error(result.message);
      }
    } catch (error) {
      console.error("Error ApprovalOnClick", error);
    } finally{
      setDeleteOpenConfirmModal(false);
    }
  };
  const RejectOnClick= async () => {
    try {
      const result = await SubExceptionDetailsService.rejectSubException(formData,RejectRemarks);
      if (result.success) {
        ToastService.success(result.message);
        GetSubExceptionDetails();
      } else {
        ToastService.error(result.message);
      }
    } catch (error) {
      console.error("Error RejectOnClick", error);
    } finally{
      setRejectOpenConfirmModal(false);
    }
  };  
  const DeleteRemarksOnChange = (event:any) => {
    setDeleteRemarks(event.target.value); // Update the state with the new value
  };
  const RejectRemarksOnChange = (event:any) => {
    setRejectRemarks(event.target.value); // Update the state with the new value
  };
  const handleNavigate = () => {
    // Navigate to the desired page
    navigate('/ExceptionsManagement/ForApprovalSubExceptions');
  };
  const formatDate = (inputDate:any) => {
    const date = new Date(inputDate);
    return date.toLocaleString('en-US', {
      month: '2-digit',
      day: '2-digit',
      year: 'numeric',
    });
  };
  const [isCollapsed, setIsCollapsed] = useState(false);

  const toggleCollapse = () => {
    setIsCollapsed(!isCollapsed);
  };

  return (
    <>
      {PermissionsSubExceptionsView && (
        <>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <h4>Edit Sub Exception {subERN}</h4>
            <Link onClick={handleNavigate} style={{ cursor: 'pointer', color: '#1976d2' }}>
              Go back to previous page
            </Link>
          </div>
          
          <div>
              {formData.branchReplyDetails && formData.branchReplyDetails.length > 0 ? (
                <>
                  <p>
                    <button
                      className="btn btn-success text-white"
                      type="button"
                      onClick={toggleCollapse}
                      aria-expanded={isCollapsed}
                      aria-controls="collapseExample"
                    >
                      <span className="fa fa-paper-plane">
                        <i></i>
                      </span>
                      View Branch Reply
                    </button>
                  </p>

                  {isCollapsed && (
                    <div className="collapse show" id="collapseExample">
                      {formData.branchReplyDetails.map((reply, index) => {
                        if (index % 3 === 0) {
                          return (
                            <div key={index} className="card card-body" style={{ margin: '10px' }}>
                              <p>{reply}</p>
                              <p className="card-text">
                                <small className="text-muted">
                                  [Replied by test: {formData.branchReplyDetails[index + 1]}] [Date Replied: {formData.branchReplyDetails[index + 2]}]
                                </small>
                              </p>
                            </div>
                          );
                        }
                        return null;
                      })}
                    </div>
                  )}
                </>
              ) : (
                <p>No branch replies available.</p>
              )}
            </div>
          <Box
            sx={{
              padding: 4,
              width: "100%",
              //margin: "auto",
              border: "1px solid #ddd",
              borderRadius: 2,
              //marginTop: 4, // Add some spacing between the boxes
            }}
          >
            <Box>
              <Box display="flex" justifyContent="space-between" alignItems="center">
                <h3 style={{ color: "#1976d2" }}>Sub Exception Details</h3>
                <IconButton sx={globalStyle.buttonRed} onClick={DeleteOpenConfirmModal}>
                  <DeleteTwoTone />
                </IconButton>
              </Box>
              <form>
                {formData?.exceptionCode.approvalStatus == 0 ? (
                  <Card>
                    <CardContent>
                      <Typography variant="h5">Status</Typography>
                      <Divider />
                      <Grid container spacing={3}>
                        {formData?.forDeletion === 1 && (
                          <Grid item xs={12}>
                            <Typography color="error">This child exception is for deletion.</Typography>
                          </Grid>
                        )}
                      </Grid>
                      <Grid container spacing={3}>
                        <Grid item xs={3}>
                          <InputLabel>Status</InputLabel>
                          <TextField
                            value={DeviationStatusDisplayNames[formData.exceptionCode.deviationStatus as DeviationStatusDTO] || "Unknown Status"}
                            fullWidth
                            variant="filled"
                          />
                        </Grid>
                        <Grid item xs={1} style={{ marginTop: 30 }}>
                          <div style={{ marginLeft: '25%' }}>
                            <ArrowForward />
                          </div>
                        </Grid>
                        <Grid item xs={3}>
                          <InputLabel>New Status</InputLabel>
                          <TextField
                            value={DeviationStatusDisplayNames[formData.newStatus as DeviationStatusDTO] || "Unknown Status"}
                            fullWidth
                            variant="filled"
                          />
                        </Grid>
                        
                        {PermissionsSubExceptionsApproval && (
                          <>
                            <Grid item xs={2} style={{ marginTop: 15 }}>
                              <Button variant="outlined" color="primary" size='large' fullWidth onClick={ApprovalOnClick}>
                                <i className="fa fa-check" /> Approve
                              </Button>
                            </Grid>
  
                            <Grid item xs={2} style={{ marginTop: 15 }}>
                              <Button variant="outlined" color="warning" size='large' fullWidth onClick={() => RejectOpenConfirmModal()}>
                                <i className="fa fa-backspace" /> Reject
                              </Button>
                            </Grid>
                          </>
                        )}
                      </Grid>
                      <Grid container spacing={3}>
                        <Grid item xs={3}>
                          <InputLabel>Tagged</InputLabel>
                          <TextField
                            value={formData.exceptionCode.taggingDateDisplay === '01/01/1' ? 'N/A' : formData.exceptionCode.taggingDateDisplay}
                            fullWidth
                            variant="filled"
                          />
                        </Grid>
                        <Grid item xs={1} style={{ marginTop: 30 }}>
                          <div style={{ marginLeft: '25%' }}>
                            <ArrowForward />
                          </div>
                        </Grid>
                        <Grid item xs={3}>
                          <InputLabel>New Tagging Date</InputLabel>
                          <TextField
                            value={formData.taggingDateDisplay}
                            fullWidth
                            variant="filled"
                          />
                        </Grid>
                      </Grid>
                    </CardContent>
                  </Card>
                ) : (
                  <>
                    <Alert severity="info" sx={{ width: '100%' }}>
                      <Typography variant="h6" color="text.primary">
                        Note:
                      </Typography>
                      <Typography>
                        - Updating of Sub-Exception Status will require approval from <strong>Approving Bank Officers</strong>.
                      </Typography>
                      <Typography>
                        - Backdated Tagging Date will require approval from <strong>Approving Bank Officers</strong>.
                      </Typography>
                    </Alert>
  
                    <Grid container spacing={2}>
                      <Grid item xs={4} md={3}>
                        <Typography variant="body1" gutterBottom>
                          Status
                        </Typography>
                        
                        <Select
                          size="small"
                          variant="outlined"
                          fullWidth
                          name="newStatus"
                          value={formUpdate.newStatus}
                          onChange={FormUpdateOnChange}
                        >
                          {Object.values(DeviationStatusDTO)
                            .filter((value) => typeof value === "number")
                            .map((x) => (
                              <MenuItem key={x} value={x}>
                                {DeviationStatusDisplayNames[x as DeviationStatusDTO]}
                              </MenuItem>
                            ))}
                        </Select>
                      </Grid>
  
                      <Grid item xs={4} md={3}>
                        <Typography variant="body1" gutterBottom>
                          Tagging Date
                        </Typography>
                        <TextField
                          value={taggingDate}
                          name="taggingDate"
                          type="date"
                          variant="outlined"
                          fullWidth
                          size="small"
                          onChange={DateOnChange}
                        />
                      </Grid>
  
                      <Grid item xs={4} md={2} style={{ marginTop: '18px' }}>
                        <Button
                          size="large"
                          variant="contained"
                          color="primary"
                          type="button"
                          fullWidth
                          onClick={UpdateOnClick}
                        >
                          Update Status
                        </Button>
                      </Grid>
                    </Grid>
                  </>
                )}
              </form>
  
              <Divider sx={{ my: 2 }} />
              <Typography variant="h5">Details</Typography>
              <Divider sx={{ my: 2 }} />
  
              <Box sx={{ mb: 2 }}>
                <Typography variant="body1" gutterBottom>
                  Sub Reference No
                </Typography>
                <TextField
                  value={formData?.exceptionCode.subReferenceNo ?? ''} // Fallback to empty string if undefined
                  variant="filled"
                  fullWidth
                  size="small"
                  disabled
                />
              </Box>
  
              <Grid container spacing={0}>
                <Grid item xs={12} md={3} style={{ paddingRight: '10px' }}>
                  <Typography variant="body1" gutterBottom>
                    Deviation Code
                  </Typography>
                  <TextField
                    value={formData?.exceptionCode.exCode ?? ''} // Fallback to empty string if undefined
                    variant="filled"
                    fullWidth
                    size="small"
                    disabled
                  />
                </Grid>
  
                <Grid item xs={12} md={9}>
                  <Typography variant="body1" gutterBottom>
                    Description
                  </Typography>
                  <TextField
                    value={formData?.exceptionCode.exCodeDescription ?? ''} // Fallback to empty string if undefined
                    variant="filled"
                    fullWidth
                    size="small"
                    disabled
                  />
                </Grid>
              </Grid>
  
              <Box sx={{ mb: 2 }}>
                <Typography variant="body1" gutterBottom>
                  Deviation Category
                </Typography>
                <TextField
                  value={formData?.deviationCategory ?? ''} // Fallback to empty string if undefined
                  variant="filled"
                  fullWidth
                  size="small"
                  disabled
                />
              </Box>
  
              <Box sx={{ mb: 2 }}>
                <Typography variant="body1" gutterBottom>
                  Risk Classification
                </Typography>
                <TextField
                  value={formData?.riskClassification ?? ''} // Fallback to empty string if undefined
                  variant="filled"
                  fullWidth
                  size="small"
                  disabled
                />
              </Box>
  
              <Box sx={{ mb: 2 }}>
                <Typography variant="body1" gutterBottom>
                  Date Created
                </Typography>
                <TextField
                  value={formData?.exceptionCode.dateCreated ?? 'N/A'} // Fallback to 'N/A' if undefined
                  variant="filled"
                  fullWidth
                  size="small"
                  disabled
                />
              </Box>
  
              <Box sx={{ mb: 2 }}>
                <Typography variant="body1" gutterBottom>
                  Tagging Date
                </Typography>
                <TextField
                  value={isDateValid && formData?.taggingDate ? formData?.taggingDate : 'N/A'} // Fallback to 'N/A' if undefined
                  variant="filled"
                  fullWidth
                  disabled
                  size="small"
                  placeholder={isDateValid ? undefined : 'N/A'}
                />
              </Box>
            </Box>
          </Box>
        </>
      )}
      <ConfirmationModal
        open={deleteOpenConfirmModal}
        handleClose={DeleteCloseConfirmModal}
        handleConfirm={DeleteOnClick}
        title="Are you sure?"
        content=""
        buttonName="Delete"
        id={0}
      >
      <>  
          <Alert severity="error" sx={{ width: '100%' }}>
            <Typography>
            This sub exception will be deleted if you clicked the submit button.
            </Typography>
          </Alert>
          <Box sx={{ mb: 2 }}>
            <Typography variant="body1" gutterBottom>
            Please type in the reason for deleting this sub exception:
            </Typography>
            <TextField
              value={DeleteRemarks} // Fallback to empty string if undefined
              onChange={DeleteRemarksOnChange}
              variant="outlined"
              fullWidth
              size="small"
            />
          </Box>
      </>
    </ConfirmationModal>
    
    <ConfirmationModal
        open={rejectOpenConfirmModal}
        handleClose={RejectCloseConfirmModal}
        handleConfirm={RejectOnClick}
        title="Do you want to Reject this request?"
        content=""
        buttonName="Reject"
        id={0}
      >
      <>  
          <Box sx={{ mb: 2 }}>
            <Typography variant="body1" gutterBottom>
            Please let your Team know the reason you rejected this..
            </Typography>
            <TextField
              value={RejectRemarks} // Fallback to empty string if undefined
              onChange={RejectRemarksOnChange}
              variant="outlined"
              fullWidth
              size="small"
            />
          </Box>
      </>
    </ConfirmationModal>
    </>
  );
};

export default SubExceptionDetails;
