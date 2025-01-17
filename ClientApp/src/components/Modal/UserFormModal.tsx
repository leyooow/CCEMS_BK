/* eslint-disable prefer-const */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect, useState } from "react";
import {
  TextField,
  Grid,
  MenuItem,
  Autocomplete,
  Button,
  DialogActions,
  DialogContent,
  DialogTitle,
  Divider,
  Box,
  IconButton,
} from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import { ERROR_MESSAGES } from "../../utils/constants";
import { GroupDTO } from "../../models/groupDTOs";
import { BranchAccessDTO } from "../../models/userManagementDTOs";
import { RoleDTO } from "../../models/RoleDTOs";
import userService from "../../services/userService";
import EmployeeService from "../../services/employeeService";

// Types and Interfaces
interface UserFormProps {
  formData: {
    loginName: string;
    employeeId: string;
    lastName: string;
    firstName: string;
    middleName: string;
    email: string;
    roleId: number;
    branchAccesses: BranchAccessDTO[];
  };
  onChange: (e: any) => void;
  onBranchChange: (e: any, value: GroupDTO[]) => void;
  handleSubmit: (e: any) => void;
  rolesOptions: RoleDTO[];
  branchOptions: GroupDTO[];
  selectedBranches: GroupDTO[];
  onCancel: () => void;
  isEditMode?: boolean;
  // userData?: any;
}

const UserFormModal: React.FC<UserFormProps> = ({
  formData,
  onChange,
  onBranchChange,
  handleSubmit,
  rolesOptions,
  branchOptions,
  selectedBranches,
  onCancel,
  isEditMode = false,
  // userData,
}) => {
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (isEditMode && formData) {
      // Map formData to formData fields
      onChange({ target: { name: "loginName", value: formData.loginName } });
      onChange({ target: { name: "employeeId", value: formData.employeeId } });
      onChange({ target: { name: "lastName", value: formData.lastName } });
      onChange({ target: { name: "firstName", value: formData.firstName } });
      onChange({ target: { name: "middleInitial", value: formData.middleName } });
      onChange({ target: { name: "email", value: formData.email } });
      onChange({ target: { name: "roleId", value: formData.roleId } });
      // onBranchChange(event, formData.branchAccesses || []);
    }
  }, [isEditMode, formData, onChange, onBranchChange]);

  const checkUserAD = async (loginName: string): Promise<boolean> => {
    try {
      const result = await userService.checkUserAD(loginName);
      return result.success ? true : false;

    } catch (error) {
      console.error("Error checking AD User:", error);
      return false; // Return false if an error occurs
    }
  };


  const checkEmployeeExist = async (employeeId: string): Promise<boolean> => {
    try {
      const result = await EmployeeService.getEmployeeById(employeeId);
      if (result.success) {
        formData.firstName = result.data.firstName;
        formData.middleName = result.data.middlename;
        formData.lastName = result.data.lastName;
      }
      return result.success ? true : false;

    } catch (error) {
      console.error("Error checking AD User:", error);
      return false; // Return false if an error occurs
    }
  };



  const validate = async () => {


    let tempErrors: Record<string, string> = {};

    // console.log(formData.branchAccess.length)
    // if (!checkUserAD) tempErrors.loginName = "Invalid AD User";

    if (!formData.loginName) {
      tempErrors.loginName = ERROR_MESSAGES.REQUIRED_FIELD;
    } else {
      try {
        const isValidUser = await checkUserAD(formData.loginName);
        if (!isValidUser) tempErrors.loginName = "Invalid AD User";
      } catch (error) {
        tempErrors.loginName = "Error validating AD User.";
      }
    }

    if (!formData.employeeId) {
      tempErrors.employeeId = ERROR_MESSAGES.REQUIRED_FIELD;
    } else {
      try {
        const isValidEmployee = await checkEmployeeExist(formData.employeeId);
        if (!isValidEmployee) tempErrors.employeeId = "Invalid employee";
      } catch (error) {
        tempErrors.employeeId = "Error validating employee.";
      }
    }



    // if (!formData.loginName) tempErrors.loginName = ERROR_MESSAGES.REQUIRED_FIELD;
    // if (!formData.employeeId) tempErrors.employeeId = ERROR_MESSAGES.REQUIRED_FIELD;
    if (!formData.lastName) tempErrors.lastName = ERROR_MESSAGES.REQUIRED_FIELD;
    if (!formData.firstName) tempErrors.firstName = ERROR_MESSAGES.REQUIRED_FIELD;
    if (!formData.email) tempErrors.email = ERROR_MESSAGES.REQUIRED_FIELD;
    else if (!/^[^@]+@[^@]+\.[^@]+$/.test(formData.email)) tempErrors.email = "Invalid email format.";
    if (!formData.roleId) tempErrors.roleId = "User Role is required.";
    if (selectedBranches.length == 0) tempErrors.branches = "Branches are required.";

    setErrors(tempErrors);
    return Object.keys(tempErrors).length === 0;
  };

  const handleFormSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const isValid = await validate(); // Await the asynchronous validate function
    if (isValid) {
      handleSubmit(e);
    }
  };

  return (
    <Box sx={{ pt: 2, pr: 4, pb: 2 }}>
      <DialogTitle
        sx={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}
      >
        {isEditMode ? "Edit User" : "Create New User"}
        <IconButton onClick={onCancel} size="small">
          <CloseIcon />
        </IconButton>
      </DialogTitle>
      <Divider />
      <DialogContent sx={{ overflowX: "hidden" }}>
        <Box sx={{ px: 3 }}>
          <Grid container spacing={2}>
            <Grid item xs={12} sm={6}>
              <TextField
                label="Username"
                variant="outlined"
                fullWidth
                name="loginName"
                value={formData.loginName}
                onChange={onChange}
                placeholder="BOC Active Directory"
                required
                error={!!errors.loginName}
                helperText={errors.loginName}
                disabled={isEditMode}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                label="Employee ID"
                variant="outlined"
                fullWidth
                name="employeeId"
                value={formData.employeeId}
                onChange={onChange}
                required
                error={!!errors.employeeId}
                helperText={errors.employeeId}
                disabled={isEditMode}
              />
            </Grid>
            <Grid item xs={12} sm={5}>
              <TextField
                label="Last Name"
                variant="outlined"
                fullWidth
                name="lastName"
                value={formData.lastName}
                onChange={onChange}
                required
                error={!!errors.lastName}
                helperText={errors.lastName}
              />
            </Grid>
            <Grid item xs={12} sm={5}>
              <TextField
                label="First Name"
                variant="outlined"
                fullWidth
                name="firstName"
                value={formData.firstName}
                onChange={onChange}
                required
                error={!!errors.firstName}
                helperText={errors.firstName}
              />
            </Grid>
            <Grid item xs={12} sm={2}>
              <TextField
                label="Middle (MI)"
                variant="outlined"
                fullWidth
                name="middleName"
                value={formData.middleName}
                onChange={onChange}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                label="E-mail"
                variant="outlined"
                fullWidth
                name="email"
                type="email"
                value={formData.email}
                onChange={onChange}
                required
                error={!!errors.email}
                helperText={errors.email}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                select
                label="User Role"
                variant="outlined"
                fullWidth
                name="roleId"
                value={formData.roleId}
                onChange={onChange}
                required
                error={!!errors.roleId}
                helperText={errors.roleId}
              >
                {rolesOptions.map((role, index) => (
                  <MenuItem key={index} value={role.id}>
                    {role.roleName}
                  </MenuItem>
                ))}
              </TextField>
            </Grid>
            <Grid item xs={12}>

              <Autocomplete
                multiple
                options={branchOptions}
                getOptionLabel={(option) => option.name}
                value={selectedBranches}
                onChange={(_event, value) => onBranchChange(_event, value)}
                renderInput={(params) => (
                  <TextField
                    {...params}
                    label="Branches"
                    placeholder="Select branches"
                    variant="outlined"
                    fullWidth
                    error={!!errors.branches}
                    helperText={errors.branches}
                  />
                )}
                renderOption={(props, option) => (
                  <li {...props} key={option.id || option.name}> {/* Ensure unique key */}
                    {option.name}
                  </li>
                )}
              />
            </Grid>
          </Grid>
        </Box>
      </DialogContent>
      <Divider />
      <DialogActions>
        <Button onClick={onCancel} variant="outlined" color="error">
          Cancel
        </Button>
        <Button type="submit" variant="contained" color="success" onClick={handleFormSubmit}>
          {isEditMode ? "Save" : "Add"}
        </Button>
      </DialogActions>
    </Box>
  );
};

export default UserFormModal;
