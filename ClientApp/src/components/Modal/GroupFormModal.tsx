import React, { useState } from 'react';
import { Modal, Box, TextField, Button, Typography } from '@mui/material';
import { ERROR_MESSAGES } from '../../utils/constants';
import { BranchOption, GroupUpdateDTO } from '../../models/groupDTOs';
import GroupService from '../../services/groupService';
import ToastService from '../../utils/toast';

interface FormData {
  code: string;
  name: string;
  area: string;
  division: string;
}

interface ModalProps {
  open: boolean;
  handleClose: () => void;
  title: string;
  formData: FormData;
  setFormData: React.Dispatch<React.SetStateAction<FormData>>;
  handleSave: () => void;
  selectedGroupId: number | null;
}

const GroupFormModal: React.FC<ModalProps> = ({ open, handleClose, title, formData, setFormData, handleSave, selectedGroupId }) => {
  const [errors, setErrors] = useState<Record<string, string>>({});

  const handleInputChange = (field: keyof FormData, value: string) => {
    setFormData((prevData) => ({
      ...prevData,
      [field]: value,
    }));
  };

  const validate = () => {
    let tempErrors: Record<string, string> = {};

    if (!formData.code) tempErrors.code = ERROR_MESSAGES.REQUIRED_FIELD;
    if (!formData.area) tempErrors.area = ERROR_MESSAGES.REQUIRED_FIELD;
    if (!formData.name) tempErrors.name = ERROR_MESSAGES.REQUIRED_FIELD;
    if (!formData.division) tempErrors.division = ERROR_MESSAGES.REQUIRED_FIELD;

    setErrors(tempErrors);
    return Object.keys(tempErrors).length === 0;
  };

  const fetchBranchDetails = async (branchCode: string) => {
    try {
      const branchDetails: BranchOption = await GroupService.getBranchCodeByID(branchCode);
      setFormData((prevData: FormData) => ({
        ...prevData,
        //code: branchDetails.brCode,
        name: branchDetails.brName,
      }));
    } catch (error) {
      console.error('Error fetching branch details:', error);
      ToastService.error('Branch Code is not valid.');
      setFormData((prevData: FormData) => ({
        ...prevData,
        //code: branchDetails.brCode,
        name: "",
      }));
    }
  };

  const handleSaveWithValidation = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) {
      return;
    }
  
    try {
      let result;
      if (selectedGroupId) {
        console.log('Updating group with ID:', selectedGroupId); // Debug log
        const groupUpdateData: GroupUpdateDTO = {
          id: selectedGroupId,
          ...formData,
        };
        result = await GroupService.updateGroup(selectedGroupId, groupUpdateData);
      } else {
        console.log('Creating new group'); // Debug log
        result = await GroupService.createGroup(formData);
      }
  
      if (result && result.success) {
        handleSave();
        if (selectedGroupId) {
          ToastService.success('Group updated successfully.');
        } else {
          ToastService.success('Group added successfully.');
        }
      } else if (result) {
        ToastService.error(result.message);
      } else {
        ToastService.error('An unexpected error occurred.');
      }
    } catch (error) {
      console.error('Error saving data:', error);
      ToastService.error('An error occurred while saving the group.');
    }
  };

  return (
    <Modal open={open} onClose={handleClose}>
      <Box
        sx={{
          position: 'absolute',
          top: '50%',
          left: '50%',
          transform: 'translate(-50%, -50%)',
          width: 800,
          bgcolor: 'background.paper',
          boxShadow: 24,
          p: 4,
          borderRadius: 2,
        }}
      >
        <Typography variant="h6" gutterBottom>
          {title}
        </Typography>

        {/* Branch Code Field */}
        <TextField
          label="Branch Code"
          variant="outlined"
          size="small"
          value={formData.code}
          fullWidth
          margin="normal"
          required
          onChange={(e) => {
            handleInputChange('code', e.target.value);
            fetchBranchDetails(e.target.value);
          }}
        />

        {/* Branch Name Field */}
        <TextField
          label="Branch Name"
          variant="outlined"
          size="small"
          value={formData.name || ""}
          fullWidth
          margin="normal"
          disabled // Make it read-only since it is auto-filled
        />

        {/* Area Field */}
        <TextField
          label="Area"
          variant="outlined"
          size="small"
          value={formData.area}
          fullWidth
          margin="normal"
          error={!!errors.area}
          helperText={errors.area}
          onChange={(e) => handleInputChange('area', e.target.value)}
        />

        {/* Division Field */}
        <TextField
          label="Division"
          variant="outlined"
          size="small"
          value={formData.division}
          fullWidth
          margin="normal"
          error={!!errors.division}
          helperText={errors.division}
          onChange={(e) => handleInputChange('division', e.target.value)}
        />

        {/* Action Buttons */}
        <Box mt={2} display="flex" justifyContent="flex-end" gap={1}>
          <Button variant="outlined" onClick={handleClose}>
            Cancel
          </Button>
          <Button variant="contained" color="primary" onClick={handleSaveWithValidation}>
            Save
          </Button>
        </Box>
      </Box>
    </Modal>
  );
};

export default GroupFormModal;