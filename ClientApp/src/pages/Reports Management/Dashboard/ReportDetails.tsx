import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  Box,
  Typography,
  Grid,
  CircularProgress,
  Divider,
  FormControl,
  FormHelperText,
  Button,
  Autocomplete,
  TextField,
  Chip,
} from "@mui/material";
import ReportDetailsService from "../../../services/ReportDetailsService";
import {
  PaginatedList,
  RecipientDTO,
  RejectedDTO,
  ReportDetailsDTO,
  ReportStatus,
  SendRequestDTO,
} from "../../../models/reportDetailsDTOs";
import PaginationControls from "../../../components/Pagination/PaginationControls";
import Table from "../../../components/Table/Table";
import { FormattedDate } from "../../../utils/formatDate";
import ToastService from "../../../utils/toast";
import RejectModal from "../../../components/Modal/RejectModal";

import Cookies from 'js-cookie';
const Permission = Cookies.get("Permission") ?? "";
const userPermissions: string[] = JSON.parse(Permission || "[]");
const hasPermission = (permission: string) =>
  userPermissions.includes(permission);

const ReportDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [reportDetails, setReportDetails] = useState<ReportDetailsDTO | null>(
    null
  );
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const [pagedResult, setPagedResult] = useState<PaginatedList>({
    data: [],
    pageIndex: 1,
    totalPages: 0,
    countData: 0,
    hasPreviousPage: false,
    hasNextPage: false,
  });

  const [selectedRecipients, setSelectedRecipients] = useState<RecipientDTO[]>(
    []
  );

  const [recipientsOptions, setRecipientsOptions] = useState<RecipientDTO[]>(
    []
  );

  const [selectedCC, setSelectedCC] = useState<RecipientDTO[]>([]);

  const [CCOptions, setCCOptions] = useState<RecipientDTO[]>([]);

  const fetchRecipientOptions = async () => {
    try {
      const branchIds = String(reportDetails?.selectedBranches); // Ensure selectedBranches is converted to a string

      const recipients =
        await ReportDetailsService.populateBranchRecipients(branchIds);

      const updatedRecipients = recipients.map((recipient: RecipientDTO) => ({
        ...recipient,
        isSelected: reportDetails?.toList?.includes(recipient.text) || false, // Set isSelected based on toList
      }));

      setRecipientsOptions(updatedRecipients);
    } catch (error) {
      console.error("Error fetching recipient options:", error);
    }
  };

  const fetchCCOptions = async () => {
    try {
      const result = await ReportDetailsService.populateBranchRecipients(
        String(reportDetails?.selectedBranches)
      );
      //const filteredResult = result.filter(recipient => recipient.isSelected);
      result.forEach((x: any) => {
        if (reportDetails?.ccList?.includes(x.text)) {
          x.isSelected = true;
        }
      });
      setCCOptions(result);
    } catch (error) {
      console.error("Error fetching groups", error);
    }
  };

  useEffect(() => {
    fetchRecipientOptions();
    fetchCCOptions();
  }, [reportDetails]);

  const onRecipientChange = (value: string[]) => {
    const selected = recipientsOptions.filter((recipient) =>
      value.includes(recipient.value)
    );
    setSelectedRecipients(selected);
  };

  const onCCChange = (value: string[]) => {
    const selected = CCOptions.filter((recipient) =>
      value.includes(recipient.value)
    );
    setSelectedCC(selected); // Update the selected recipients

    // Additional logic if needed
  };
  const [branchName, setBranchName] = useState<string | null>(null);

  const fetchSelectedBranchName = async () => {
    try {
      const branchName = await ReportDetailsService.getSelectedBranchName(
        Number(reportDetails?.id)
      );
      setBranchName(branchName);
    } catch (error) {
      console.error("Error fetching selected branch name", error);
    }
  };

  useEffect(() => {
    if (reportDetails?.id) {
      fetchSelectedBranchName();
    }
  }, [reportDetails?.id]);

  const columns = [
    {
      label: "Actions", // New column for buttons
      render: (data: any) => (
        <>
          <a
            className="btn btn-sm btn-outline-secondary"
            href={`/ReportsManagement/BranchReply/${Number(id)}?refNo=${data.exceptionNo}`} // Dynamic link
          >
            Branch Reply
          </a>
          <button
            type="button"
            className="btn btn-sm btn-outline-secondary"
            onClick={() => PulloutRequest(data.exceptionNo)}
          >
            Pullout request
          </button>
        </>
      ),
    },
    {
      label: "Reference No",
      render: (data: any) => (
        <a
          href={`/SubExceptions/${data.exceptionNo}`} // Dynamic link based on `exceptionNo`
          target="_blank" // Opens the link in a new tab
          style={{ color: "#007bff" }} // Inline style for Bootstrap blue color
        >
          {data.exceptionNo}
        </a>
      ),
    },
    { label: "Branch Code", accessor: "branchCode" },
    { label: "Branch Name", accessor: "branchName" },
    { label: "Area", accessor: "area" },
    { label: "Division", accessor: "division" },
    { label: "Transaction Date", accessor: "transactionDate" },
    { label: "Aging", accessor: "aging" },
    { label: "Aging Category", accessor: "agingCategory" },
    { label: "Process", accessor: "process" },
    { label: "Account No", accessor: "accountNo" },
    { label: "Account Name", accessor: "accountName" },
    { label: "Deviation", accessor: "deviation" },
    { label: "Risk Classification", accessor: "riskClassification" },
    { label: "Deviation Category", accessor: "deviationCategory" },
    { label: "Amount", accessor: "amount" },
    { label: "Employee Responsible", accessor: "personResponsible" },
    { label: "Other Employee Responsible", accessor: "otherPersonResponsible" },
    { label: "Remarks", accessor: "remarks" },
    { label: "Action Plan", accessor: "actionPlan" },
    { label: "Encoded By", accessor: "encodedBy" },
    { label: "Root Cause", accessor: "rootCause" },
    { label: "Exception Approver", accessor: "deviationApprover" },
  ];

  const handlePageChange = (newPage: number) => {
    setPagedResult({
      ...pagedResult,
      pageIndex: newPage,
    });
  };

  const Getlist = async () => {
    try {
      const result = await ReportDetailsService.getList(
        Number(id),
        pagedResult.pageIndex
      );

      setPagedResult({
        pageIndex: result.pageIndex,
        totalPages: result.totalPages,
        countData: result.countData,
        hasPreviousPage: result.hasPreviousPage,
        hasNextPage: result.hasNextPage,
        data: result.data,
      });
    } catch (error) {
      console.error("Error Getlist", error);
    }
  };
  const PulloutRequest = async (refno: string) => {
    try {
      const result = await ReportDetailsService.pulloutRequest(
        Number(id),
        refno
      );
      if (result.success) {
        const fileData = result.data;

        // Check if fileData is valid
        if (!fileData || !fileData.fileByte || fileData.fileByte.length === 0) {
          console.error("File byte data is empty or invalid");
          return;
        }

        // Decode base64 string to byte array
        const byteCharacters = atob(fileData.fileByte);
        const byteArray = new Uint8Array(byteCharacters.length);

        for (let i = 0; i < byteCharacters.length; i++) {
          byteArray[i] = byteCharacters.charCodeAt(i);
        }

        // Create a Blob object from the byte array
        const blob = new Blob([byteArray], { type: fileData.contentType });

        // Create a download link
        const link = document.createElement("a");
        link.href = URL.createObjectURL(blob);
        link.download = fileData.fileName;
        // Programmatically click the link to trigger the download
        link.click();

        // Clean up
        URL.revokeObjectURL(link.href);
        ToastService.success(result.data.message);
      } else {
        ToastService.error(result.data.message);
      }
    } catch (error) {
      console.error("Error PulloutRequest", error);
    }
  };
  const ExportDataFromDetails = async () => {
    try {
      const result = await ReportDetailsService.exportDataFromDetails(
        Number(id)
      );
      if (result.success) {
        const fileData = result.data;

        // Check if fileData is valid
        if (!fileData || !fileData.fileByte || fileData.fileByte.length === 0) {
          console.error("File byte data is empty or invalid");
          return;
        }

        // Decode base64 string to byte array
        const byteCharacters = atob(fileData.fileByte);
        const byteArray = new Uint8Array(byteCharacters.length);

        for (let i = 0; i < byteCharacters.length; i++) {
          byteArray[i] = byteCharacters.charCodeAt(i);
        }

        // Create a Blob object from the byte array
        const blob = new Blob([byteArray], { type: fileData.contentType });

        // Create a download link
        const link = document.createElement("a");
        link.href = URL.createObjectURL(blob);
        link.download = fileData.fileName;
        // Programmatically click the link to trigger the download
        link.click();

        // Clean up
        URL.revokeObjectURL(link.href);
        ToastService.success(result.data.message);
      } else {
        ToastService.error(result.data.message);
      }
    } catch (error) {
      console.error("Error PulloutRequest", error);
    }
  };

  const handleApprove = async () => {
    try {
      const data = {
        id: reportDetails?.id,
        reportsGuid: reportDetails?.reportsGuid,
      };
      if (reportDetails?.id && reportDetails?.reportsGuid) {
        const result = await ReportDetailsService.approveReportDetails(data);

        if (result.success) {
          ToastService.success("Report approved successfully.");

          const updatedData = await ReportDetailsService.getReportDetails(
            Number(id)
          );
          setReportDetails(updatedData);
        } else {
          ToastService.error("Failed to approve the report.");
        }
      } else {
        ToastService.error("Report details are missing.");
      }
    } catch (error) {
      console.error("Error approving report:", error);
      ToastService.error("An error occurred while approving the report.");
    }
  };

  const [isModalOpen, setIsModalOpen] = useState(false);

  const rejectionData = {
    id: reportDetails?.id ?? 0, // Example ID
    reportsGuid: reportDetails?.reportsGuid ?? "", // Example reportsGuid
  };

  const handleRejectClick = () => {
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
  };

  const handleConfirmReject = async (data: RejectedDTO) => {
    console.log("Rejected Data:", data);

    setIsModalOpen(false);
    const updatedData = await ReportDetailsService.getReportDetails(Number(id));
    setReportDetails(updatedData);
  };

  useEffect(() => {
    Getlist();
  }, [pagedResult.pageIndex]);

  useEffect(() => {
    const fetchReportDetails = async () => {
      try {
        const data = await ReportDetailsService.getReportDetails(Number(id));
        setReportDetails(data);
        // console.log(data);
      } catch (error) {
        console.error("Error fetching report details:", error);
        setError("Failed to fetch report details.");
      } finally {
        setLoading(false);
      }
    };

    fetchReportDetails();
  }, [id]);

  

  const handleSendRequest = async () => {
    const payload: SendRequestDTO = {
      ToList: selectedRecipients.map((recipient) => recipient.text), // Convert array to string array
      CCList: selectedCC.map((recipient) => recipient.text), // Convert array to string array
      id: reportDetails?.id || 0, // Use the ID from your data
    };
    console.log(payload);
    try {
      const response = await ReportDetailsService.sendForApproval(payload);

      if (response.success) {
        ToastService.success(response.message);

        const updatedData = await ReportDetailsService.getReportDetails(
          Number(id)
        );
        setReportDetails(updatedData);
      } else {
        ToastService.error(response.message);
      }
    } catch (error) {
      console.error("Error sending request:", error);
    }
  };

  const handleGoBack = () => {
    window.location.href = `/ReportsManagement/Dashboard/`;
  };

  const handleCancel = () => {
    setIsEditMode(false); // Exit edit mode
    setSelectedRecipients(
      recipientsOptions.filter((recipient) =>
        reportDetails?.toList?.includes(recipient.text)
      )
    );
    setSelectedCC(
      CCOptions.filter((cc) => reportDetails?.ccList?.includes(cc.text))
    );
  };
  const [isEditMode, setIsEditMode] = useState(false); // New state for edit mode

  const handleReassign = () => {
    setIsEditMode(true); // Enable editing
    setSelectedRecipients(
      recipientsOptions.filter((recipient) =>
        reportDetails?.toList?.includes(recipient.text)
      )
    );
    setSelectedCC(
      CCOptions.filter((cc) => reportDetails?.ccList?.includes(cc.text))
    );
  };
  useEffect(() => {
    if (reportDetails) {
      setSelectedRecipients(
        recipientsOptions.filter((recipient) =>
          reportDetails.toList?.includes(recipient.text)
        )
      );
      setSelectedCC(
        CCOptions.filter((cc) => reportDetails.ccList?.includes(cc.text))
      );
    }
  }, [reportDetails, recipientsOptions, CCOptions]);

  if (loading) {
    return (
      <Box
        sx={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          height: "100vh",
        }}
      >
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box
        sx={{
          padding: 4,
          maxWidth: 800,
          margin: "auto",
          border: "1px solid #ddd",
          borderRadius: 2,
        }}
      >
        <Typography variant="h6" color="error">
          {error}
        </Typography>
      </Box>
    );
  }

  return (
    <>
      <Box
        sx={{
          padding: 4,
          width: "100%",
          margin: "auto",
          border: "1px solid #ddd",
          borderRadius: 2,
        }}
      >
        {/* Details Section */}
        <Typography variant="h5" gutterBottom>
          Report Details
        </Typography>
        {reportDetails?.status === ReportStatus.Standby && (
          <Box sx={{ mb: 2 }}>
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                backgroundColor: "#e3f2fd",
                padding: 2,
                borderRadius: 1,
                border: "1px solid #90caf9",
              }}
            >
              <Typography variant="body1" sx={{ flexGrow: 1 }}>
                <strong>Note:</strong> Sending a report will require an approval
                by approving officer.
              </Typography>
              <Button
                size="small"
                onClick={() => {
                  // Handle close action
                }}
              >
                &times;
              </Button>
            </Box>
          </Box>
        )}
        <Grid container spacing={2}>
          <Grid item xs={4}>
            <Typography variant="body2" fontWeight="bold">
              Category:
            </Typography>
          </Grid>
          <Grid item xs={8}>
            <Typography variant="body2">
              {reportDetails?.reportCategoryName}
            </Typography>
          </Grid>
          <Grid item xs={4}>
            <Typography variant="body2" fontWeight="bold">
              Coverage:
            </Typography>
          </Grid>
          <Grid item xs={8}>
            <Typography variant="body2">
              {reportDetails?.reportCoverageName}
            </Typography>
          </Grid>
          <Grid item xs={4}>
            <Typography variant="body2" fontWeight="bold">
              Sending Status:
            </Typography>
          </Grid>
          <Grid item xs={8}>
            <Typography
              variant="body2"
              sx={{
                color:
                  reportDetails?.statusName === "Standby"
                    ? "blue"
                    : reportDetails?.statusName === "PendingApproval"
                      ? "orange"
                      : reportDetails?.statusName === "Approved"
                        ? "green"
                        : reportDetails?.statusName === "Rejected"
                          ? "red"
                          : "black",
              }}
            >
              {reportDetails?.statusName}
            </Typography>
          </Grid>
          <Grid item xs={4}>
            <Typography variant="body2" fontWeight="bold">
              Generated By:
            </Typography>
          </Grid>
          <Grid item xs={8}>
            <Typography variant="body2">{reportDetails?.createdBy}</Typography>
          </Grid>
          <Grid item xs={4}>
            <Typography variant="body2" fontWeight="bold">
              Date Generated:
            </Typography>
          </Grid>
          <Grid item xs={8}>
            <Typography variant="body2">
              {reportDetails && reportDetails.dateGenerated
                ? FormattedDate(reportDetails.dateGenerated)
                : ""}
            </Typography>
          </Grid>
          <Grid item xs={4}>
            <Typography variant="body2" fontWeight="bold">
              Branch Code:
            </Typography>
          </Grid>
          <Grid item xs={8}>
            <Typography variant="body2">
              {reportDetails?.selectedBranches}
            </Typography>
          </Grid>
          <Grid item xs={4}>
            <Typography variant="body2" fontWeight="bold">
              Branch Covered:
            </Typography>
          </Grid>
          <Grid item xs={8}>
            <Typography variant="body2">{branchName}</Typography>
          </Grid>

          {reportDetails?.status === ReportStatus.Sent && (
            <>
              <Grid item xs={4}>
                <Typography variant="body2" fontWeight="bold">
                  Date Sent:
                </Typography>
              </Grid>
              <Grid item xs={8}>
                <Typography variant="body2">
                  {new Date(reportDetails?.dateSent ?? "").toLocaleDateString()}
                </Typography>
              </Grid>
            </>
          )}
          <Divider sx={{ border: "1px solid", borderColor: "black" }} />
          <Grid item xs={12}>
            <Typography variant="h5">Recipients</Typography>
          </Grid>
        </Grid>

        <Grid>
          {reportDetails?.statusName === "Standby" ? (
            <Grid item xs={12}>
              <FormControl fullWidth margin="normal" required error={false}>
                <Autocomplete
                  multiple
                  options={recipientsOptions}
                  getOptionLabel={(option) => option.value}
                  value={selectedRecipients}
                  onChange={(_event, newValue) =>
                    onRecipientChange(
                      newValue.map((recipient) => recipient.value)
                    )
                  }
                  renderInput={(params) => (
                    <TextField {...params} variant="outlined" label="Send to" />
                  )}
                  renderTags={(tagValue, getTagProps) =>
                    tagValue.map((option, index) => {
                      const props = getTagProps({ index });
                      return <Chip label={option.value} {...props} />;
                    })
                  }
                />
                {false && (
                  <FormHelperText className="text-danger">
                    At least one recipient is required.
                  </FormHelperText>
                )}
              </FormControl>
              <FormControl fullWidth margin="normal">
                <Autocomplete
                  multiple
                  options={CCOptions}
                  getOptionLabel={(option) => option.value}
                  value={selectedCC}
                  onChange={(_event, newValue) =>
                    onCCChange(newValue.map((recipient) => recipient.value))
                  }
                  renderInput={(params) => (
                    <TextField {...params} variant="outlined" label="CC:" />
                  )}
                  renderTags={(tagValue, getTagProps) =>
                    tagValue.map((option, index) => (
                      <Chip label={option.value} {...getTagProps({ index })} />
                    ))
                  }
                />
                <FormHelperText className="text-danger"></FormHelperText>
              </FormControl>
            </Grid>
          ) : (
            <Grid item xs={12}>
           
              <FormControl fullWidth margin="normal">
                <Autocomplete
                  multiple
                  options={recipientsOptions}
                  getOptionLabel={(option) => option.value}
                  value={selectedRecipients}
                  onChange={(_event, newValue) =>
                    onRecipientChange(
                      newValue.map((recipient) => recipient.value)
                    )
                  }
                  disabled={!isEditMode} // Use isEditMode to control the disabled state
                  renderInput={(params) => (
                    <TextField {...params} variant="outlined" label="Send to" />
                  )}
                  renderTags={(tagValue, getTagProps) =>
                    tagValue.map((option, index) => (
                      <Chip label={option.value} {...getTagProps({ index })} />
                    ))
                  }
                />
              </FormControl>
              <FormControl fullWidth margin="normal">
                <Autocomplete
                  multiple
                  options={CCOptions}
                  getOptionLabel={(option) => option.value}
                  value={selectedCC}
                  onChange={(_event, newValue) =>
                    onCCChange(newValue.map((recipient) => recipient.value))
                  }
                  disabled={!isEditMode} // Use isEditMode to control the disabled state
                  renderInput={(params) => (
                    <TextField {...params} variant="outlined" label="CC:" />
                  )}
                  renderTags={(tagValue, getTagProps) =>
                    tagValue.map((option, index) => (
                      <Chip label={option.value} {...getTagProps({ index })} />
                    ))
                  }
                />
              </FormControl>
            </Grid>
          )}
        </Grid>

        <Grid
          item
          xs={8}
          sx={{ display: "flex", justifyContent: "flex-end", gap: 2 }}
        >
          {isEditMode ? (
            <>
              <Button
                variant="outlined"
                color="inherit"
                onClick={handleGoBack}
              >
                Go back to previous page
              </Button>
              <Button
                variant="outlined"
                color="error"
                onClick={handleCancel}
              >
                Cancel
              </Button>
              <Button
                variant="contained"
                color="secondary"
                onClick={() => {
                  if (selectedRecipients.length === 0) {
                    ToastService.error("Please add at least one recipient.");
                    return;
                  } else {
                    if (confirm("Are you sure you want to proceed?")) {
                      handleSendRequest();
                      setIsEditMode(false); 
                    }
                  }
                }}
              >
                Send for Approval
              </Button>
            </>
          ) : (
            <>
              <Button
              variant="outlined"
              color="inherit"
              onClick={handleGoBack}
              >
              Go back to previous page
              </Button>
              {reportDetails?.statusName === "Standby" && (
              <Button
                variant="contained"
                color="secondary"
                onClick={() => {
                if (selectedRecipients.length === 0) {
                  ToastService.error("Please add at least one recipient.");
                  return;
                } else {
                  if (confirm("Are you sure you want to proceed?")) {
                  handleSendRequest();
                  setIsEditMode(false);
                  }
                }
                }}
              >
                Send for Approval
              </Button>
              )}
              {reportDetails?.statusName === "PendingApproval" &&  hasPermission("Permissions.Reports.Approval") && (
              <>
                <Button
                variant="outlined"
                color="info"
                onClick={handleReassign}
                >
                Re-assign
                </Button>
                <Button
                variant="outlined"
                color="warning"
                onClick={handleRejectClick}
                >
                Reject
                </Button>
                <RejectModal
                isOpen={isModalOpen}
                onClose={handleCloseModal}
                onConfirm={handleConfirmReject}
                rejectionData={rejectionData}
                />
                <Button
                variant="contained"
                color="primary"
                onClick={() => {
                  if (confirm("Are you sure you want to proceed?")) {
                  handleApprove();
                  }
                }}
                >
                Approve
                </Button>
              </>
              )}
            </>
          )}
        </Grid>
      </Box>

      <Box
        sx={{
          padding: 4,
          width: "100%",
          margin: "auto",
          border: "1px solid #ddd",
          borderRadius: 2,
          marginTop: 4, // Add some spacing between the boxes
        }}
      >
        <Typography variant="h5" gutterBottom>
          Report Data Content
        </Typography>
        <Grid container spacing={1}>
          <Grid item xs={3} sm={2}>
            <Button
              type="button"
              onClick={ExportDataFromDetails}
              variant="outlined"
              size="large"
              fullWidth
              sx={{
                color: "green",
                borderColor: "green",
                "&:hover": {
                  backgroundColor: "rgba(0, 128, 0, 0.1)", // Light green background on hover
                  borderColor: "darkgreen",
                },
              }}
            >
              Export Data
            </Button>
          </Grid>
        </Grid>
        <Grid container spacing={2}>
          <Grid item xs={12}>
            <Typography variant="body2"></Typography>
            <Box>
              <Table columns={columns} data={pagedResult.data} />
            </Box>
            <PaginationControls
              currentPage={pagedResult.pageIndex}
              totalPages={pagedResult.totalPages ?? 0}
              onPageChange={handlePageChange}
              totalItems={pagedResult.countData}
            />
          </Grid>
        </Grid>
      </Box>
    </>
  );
};

export default ReportDetails;
