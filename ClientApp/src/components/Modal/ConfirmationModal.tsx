import React from 'react';
import { Box, Modal, Typography, Button, IconButton, Divider } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';

// Define the props for the modal
interface CustomModalProps {
  open: boolean;
  handleClose: () => void;
  title: string;
  buttonName: string; // Dynamic content prop
  content: string;
  handleConfirm: (id: number | null) => void; // Callback for confirm action
  id: number | null;
  children?: React.ReactNode; // Add children prop
}

// Style for the modal content
const modalStyle = {
  position: 'absolute' as const,
  top: '25%',
  left: '50%',
  transform: 'translate(-50%, -50%)',
  width: 400,
  bgcolor: 'background.paper',
  border: '2px solid #000',
  boxShadow: 24,
  borderRadius: 2,
  p: 2,
};

const headerStyle = {
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
};

const ConfirmationModal: React.FC<CustomModalProps> = ({
  open,
  handleClose,
  title,
  buttonName,
  content,
  handleConfirm,
  id,
  children, // Receive children
}) => {
  return (
    <Modal open={open} onClose={handleClose}>
      <Box sx={modalStyle}>
        <Box sx={headerStyle}>
          <Typography variant="h6"><strong>{title}</strong></Typography>
          <IconButton onClick={handleClose} sx={{ color: (theme) => theme.palette.grey[500] }}>
            <CloseIcon />
          </IconButton>
        </Box>

        <Divider /> {/* Divider between header and content */}

        {/* Modal Content */}
        <Typography id="modal-description" variant="body1" gutterBottom sx={{ p: 2 }}>
          {content || children} {/* Render content or children */}

        </Typography>

        {/* Action Buttons */}
        <Box sx={{ textAlign: 'right', mt: 2 }}>
          <Button variant="contained" color="error" onClick={() => handleConfirm(id)} sx={{ mr: 1 }}>
            {buttonName}
          </Button>
          <Button variant="outlined" color="error" onClick={handleClose}>
            Close
          </Button>
        </Box>
      </Box>
    </Modal>
  );
};

export default ConfirmationModal;
