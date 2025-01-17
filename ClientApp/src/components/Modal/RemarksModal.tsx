import React, { useState, useEffect } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Typography,
  TextField,
  Button,
} from "@mui/material";

interface RemarksModalProps {
  open: boolean;
  title: string;
  buttonTitle: string;
  message: string;
  onSubmit: (remarks: string) => void;
  onClose: () => void;
}

const RemarksModal: React.FC<RemarksModalProps> = ({
  open,
  title,
  buttonTitle,
  message,
  onSubmit,
  onClose,
}) => {
  const [remarks, setRemarks] = useState("");
  const [isOpen, setIsOpen] = useState<boolean>(false);
  useEffect(() => {
    setIsOpen(open)
  },[isOpen])
  const handleSubmit = () => {
    if (remarks.trim() === "") {
      alert("Please enter your remarks.");
      return;
    }
    onSubmit(remarks);
    setRemarks(""); 
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>{title}</DialogTitle>
      <DialogContent>
        <Typography variant="body1" sx={{ mb: 2, backgroundColor: "#eaf6fc", padding: "10px", borderRadius: "4px" }}>
          {message}
        </Typography>
        <TextField
          fullWidth
          multiline
          rows={4}
          variant="outlined"
          label="Enter Remarks"
          placeholder="Enter Remarks here.."
          value={remarks}
          onChange={(e) => setRemarks(e.target.value)}
        />
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} color="secondary">
          Cancel
        </Button>
        <Button onClick={handleSubmit} color="primary" variant="contained">
          {buttonTitle}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default RemarksModal;
