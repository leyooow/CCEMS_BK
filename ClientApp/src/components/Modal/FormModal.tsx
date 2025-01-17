import React from 'react';
import { Box, Modal, Typography, Button, IconButton, TextField, Grid, Divider } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import { formatLabel } from '../../utils/formatLabel';

// Define the props for the modal
interface FormDataModalProps {
    open: boolean;
    handleClose: () => void;
    title: string;
    formData: { [key: string]: any };
    handleInputChange: (field: string, value: string) => void;
    handleSave: () => void;
    requiredFields: string[];
}

// Style for the modal content
const modalStyle = {
    position: 'absolute' as const,
    top: '50%',
    left: '50%',
    transform: 'translate(-50%, -50%)',
    width: 1200,
    bgcolor: 'background.paper',
    border: '1px solid #000',
    boxShadow: 24,
    borderRadius: 2,
    pl: 4,
    pt: 2,
    pr: 4,
    pb: 2,
};

const headerStyle = {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
};

const FormDataModal: React.FC<FormDataModalProps> = ({
    open,
    handleClose,
    title,
    formData,
    handleInputChange,
    handleSave,
    requiredFields,
}) => {
    return (
        <Modal
            open={open}
            onClose={() => { }}
            disableEscapeKeyDown
            BackdropProps={{ onClick: () => { } }}
            aria-labelledby="modal-title"
            aria-describedby="modal-description"
        >
            <Box sx={modalStyle}>
                {/* Modal Header */}
                <Box sx={headerStyle}>
                    <Typography id="modal-title" variant="h6" component="h2" sx={{ fontWeight: 'bold' }}>
                        {title}
                    </Typography>

                    <IconButton
                        aria-label="close"
                        onClick={handleClose}
                        sx={{
                            color: (theme) => theme.palette.grey[500],
                        }}
                    >
                        <CloseIcon />
                    </IconButton>
                </Box>
                <Divider />

                {/* Form Fields */}
                <Box component="form" sx={{ m: 2 }}>
                    <Grid container spacing={2}>
                        {Object.keys(formData).map((field) => (
                            <Grid item xs={12} sm={6} key={field}>
                                <TextField
                                    label={formatLabel(field)} // Use the utility function here
                                    value={formData[field].value}
                                    onChange={(e) => handleInputChange(field, e.target.value)}
                                    fullWidth
                                    margin="normal"
                                    variant="outlined"
                                    required={requiredFields.includes(field)} // Conditionally make the field required
                                    error={formData[field].error} // Dynamically set error
                                    helperText={formData[field].helperText} // Dynamically set helper text
                                />
                            </Grid>
                        ))}
                    </Grid>
                </Box>

                <Divider />

                {/* Action Buttons */}
                <Box sx={{ textAlign: 'right', mt: 3 }}>
                  
                    <Button variant="outlined" color='error' onClick={handleClose} sx={{ mr: 1 }}>
                        Cancel
                    </Button>

                    <Button variant="contained" color="success" onClick={handleSave}>
                        Save
                    </Button>
                </Box>
            </Box>
        </Modal>
    );
};

export default FormDataModal;