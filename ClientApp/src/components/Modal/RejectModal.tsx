import React, { useState } from "react";
import {
  Box,
  Modal,
  Typography,
  Button,
  Stack,
  TextField,
} from "@mui/material";
import ToastService from "../../utils/toast";
import ReportDetailsService from "../../services/ReportDetailsService";

export interface RejectedDTO {
  id: number;
  reportsGuid: string;
  remarks: string;
}

interface RejectModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: (data: RejectedDTO) => void;
  rejectionData: { id: number; reportsGuid: string }; // Pre-populated data
}

const RejectModal: React.FC<RejectModalProps> = ({
  isOpen,
  onClose,
  onConfirm,
  rejectionData,
}) => {
  const [remarks, setRemarks] = useState("");

const handleConfirm = async () => {
    const rejectedDTO: RejectedDTO = {
        id: rejectionData.id,
        reportsGuid: rejectionData.reportsGuid,
        remarks,
    };

    try {
        
        const result = await ReportDetailsService.rejectReportDetails(rejectedDTO);

        if (result.success) {
          ToastService.success("Report rejected successfully.");
        } else {
          ToastService.error(result.message);
        }
        onConfirm(rejectedDTO);
        setRemarks(""); 
    } catch (error) {
        console.error('Error submitting rejection:', error);
    }
};


  return (
    <Modal
      open={isOpen}
      onClose={onClose}
      aria-labelledby="reject-modal-title"
      aria-describedby="reject-modal-description"
    >
      <Box
        sx={{
          position: "absolute",
          top: "50%",
          left: "50%",
          transform: "translate(-50%, -50%)",
          width: 400,
          bgcolor: "background.paper",
          boxShadow: 24,
          p: 4,
          borderRadius: 2,
        }}
      >
        <Typography id="reject-modal-title" variant="h6" component="h2">
        Are you sure you want to Reject this request?
        </Typography>
        <Typography id="reject-modal-description" sx={{ mt: 2 }}>
         Please let your Team know the reason why you rejected this..
        </Typography>
        <TextField
          label="Remarks"
          fullWidth
          multiline
          rows={4}
          value={remarks}
          onChange={(e) => setRemarks(e.target.value)}
          sx={{ mt: 2 }}
        />
        <Stack direction="row" spacing={2} sx={{ mt: 3, justifyContent: "flex-end" }}>
          <Button variant="outlined" color="primary" onClick={onClose}>
            Cancel
          </Button>
          <Button
            variant="contained"
            color="error"
            onClick={handleConfirm}
            disabled={!remarks.trim()} // Disable if no remarks
          >
            Reject
          </Button>
        </Stack>
      </Box>
    </Modal>
  );
};

export default RejectModal;
