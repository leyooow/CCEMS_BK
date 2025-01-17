import {
  Box,
  IconButton,
  Tooltip,
  Typography,
  Modal,
  FormGroup,
  FormControlLabel,
  Checkbox,
  Backdrop,
  Button,
} from "@mui/material";
import userService from "../../../services/userService";
import { useEffect, useState } from "react";
import { GenericResponse, RoleDTO } from "../../../models/RoleDTOs";
import { globalStyle } from "../../../styles/theme";
import { modalStyles } from "../Manage Roles/ManageRoleStyle"
import EditNoteTwoTone from "@mui/icons-material/EditNoteTwoTone";
import Table from "../../../components/Table/Table";
import ToastService from "../../../utils/toast";


const RoleList: React.FC = () => {
  const [roleList, setRoleList] = useState<RoleDTO[]>([]);
  const [toastMessage, setToastMessage] = useState("");
  const [roleId, setRoleId] = useState();
  const [open, setOpen] = useState(false);
  const [selectedPermissions, setSelectedPermissions] = useState<(number | string)[]>([]);
  const [permissions, setPermissions] = useState([]);

  const handleOpen = async (roleId: any) => {
    try {
      // Fetch all permissions
      await fetchPermissions();

      // Fetch permissions by role ID
      await fetchPermissionsByRoleId(roleId);

      setRoleId(roleId);
      // Set modal to open
      setOpen(true);
    } catch (error) {
      console.error('Error fetching permissions:', error);
    }
  };

  const handleSave = async () => {

    const permissionData = {
      roleId: roleId,
      permissionList: selectedPermissions,
    }

    // console.log(permissionData)
    try {
      const result = await userService.addPermission(JSON.stringify(permissionData),);
      setToastMessage(result.message);
      // console.log(result)

      ToastService.success("Permission/s added successfully")


    } catch (error) {
      ToastService.error("Error in adding permission/s")
      console.error("Error saving permissions", error);
    }

    handleClose();
  }

  const handleClose = () => setOpen(false);

  const handleCheckboxChange = (permissionId: any) => {
    setSelectedPermissions(prev => {
      if (prev.includes(permissionId)) {
        return prev.filter(permission => permission !== permissionId); // Unselect if already selected
      } else {
        return [...prev, permissionId]; // Select if not already selected
      }
    });
  };

  const fetchPermissions = async () => {
    try {
      const result = await userService.getAllPermissionLookups();
      setPermissions(result.data);
      // console.log(permissions)
    } catch (error) {
      console.error("Error fetching roles", error);
    }
  };


  const fetchPermissionsByRoleId = async (roleId: any) => {
    try {
      const result = await userService.getAllPermissionByRoleId(roleId);
      setSelectedPermissions(result.data.map((p: any) => p.permission)); // Extract only the ID for comparison
      // console.log(selectedPermissions)
    } catch (error) {
      console.error("Error fetching permissions", error);
    }
  };

  const fetchRoles = async () => {
    try {
      const result: GenericResponse = await userService.getAllRoles();
      setRoleList(result.data);
    } catch (error) {
      console.error("Error fetching roles", error);
    }
  };

  useEffect(() => {
    fetchRoles();
    // fetchPermissions();
  }, [toastMessage]);

  const columns = [
    { label: 'Role ID', accessor: 'id' },
    { label: 'Role Name', accessor: 'roleName' },
    { label: 'Description', accessor: 'description' },
    {
      label: 'Action',
      render: (data: any) => (
        <Box sx={globalStyle.buttonBox}>
          <Tooltip title="Manage Roles">
            <IconButton color='primary' onClick={() => handleOpen(data.id)}>
              <EditNoteTwoTone />
            </IconButton>
          </Tooltip>
        </Box>
      ),
    },
  ];


  return (
    <>
      <Box sx={globalStyle.mainBox}>
        <Typography variant="h6" component="h6" gutterBottom>
          Roles
        </Typography>

      </Box>

      {/* Table wrapped inside a responsive container */}
      <Table columns={columns} data={roleList} />


      <Modal
        open={open}
        onClose={handleClose}
        closeAfterTransition
        BackdropComponent={Backdrop}
        BackdropProps={{
          timeout: 500,
          sx: { backgroundColor: "rgba(0, 0, 0, 0.8)" }, // Static backdrop styling
        }}
      >
        <Box
          sx={modalStyles.permissionsModal}
        >
          {/* Title Section */}
          <Box sx={{ borderBottom: '1px solid #ddd', paddingBottom: '8px', marginBottom: '16px' }}>
            <Typography variant="h6" component="h2">
              Select Permissions
            </Typography>
          </Box>

          {/* Body Section */}
          <Box sx={{ overflowY: 'auto', maxHeight: '60vh', marginBottom: '16px' }}>
            <FormGroup>
              {permissions.map((permission: any) => (
                <FormControlLabel
                  key={permission.id}
                  control={
                    <Checkbox
                      checked={selectedPermissions.includes(permission.function)} // Check if permission ID is in selectedPermissions
                      onChange={() => handleCheckboxChange(permission.function)}
                    />
                  }
                  label={permission.function} // Use the function field from the DB
                />
              ))}
            </FormGroup>
          </Box>

          {/* Footer Section */}
          <Box sx={{ display: 'flex', justifyContent: 'flex-end', borderTop: '1px solid #ddd', paddingTop: '8px' }}>
            <Button variant="outlined" onClick={handleClose} sx={{ mr: 2 }}>
              Cancel
            </Button>
            <Button variant="contained" onClick={() => {handleSave(); }}>
              Save
            </Button>
          </Box>
        </Box>
      </Modal>



    </>

  );
};

export default RoleList;