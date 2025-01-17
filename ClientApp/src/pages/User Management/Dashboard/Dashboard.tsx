/* eslint-disable react-hooks/exhaustive-deps */
/* eslint-disable @typescript-eslint/no-empty-object-type */
/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect, useState } from "react";
import { Typography, Box, Tooltip, IconButton, TextField, Dialog } from "@mui/material";
import { PagedResult } from "../../../models/GenericResponseDTO";
import userService from "../../../services/userService";
import { FormattedDate } from "../../../utils/formatDate";
import EditNoteTwoTone from "@mui/icons-material/EditNoteTwoTone";
import { globalStyle } from "../../../styles/theme";
import DeleteTwoTone from "@mui/icons-material/DeleteTwoTone";
import Table from "../../../components/Table/Table";
import PaginationControls from "../../../components/Pagination/PaginationControls";
import UserFormModal from "../../../components/Modal/UserFormModal";
import GlobalButton from "../../../components/Button/Button";
import { EmployeeDTO } from "../../../models/employeeMaintenanceDTOs";
import groupService from "../../../services/groupService";
import { BranchAccessDTO, UserCreateDto, UserUpdateDto } from "../../../models/userManagementDTOs";
import { GroupDTO } from "../../../models/groupDTOs";
import { RoleDTO } from "../../../models/RoleDTOs";
import ToastService from "../../../utils/toast";
import ConfirmationModal from "../../../components/Modal/ConfirmationModal";


const Dashboard: React.FC = () => {
  // Define state with proper initial structure
  const [openCreateUserModal, setOpenCreateUserModal] = useState(false);
  const [openEditUserModal, setOpenEditUserModal] = useState(false);
  const [openDeleteModal, setOpenDeleteModal] = useState(false);
  // const [employeeId, setEmployeeId] = useState();
  const [selectedBranches, setSelectedBranches] = useState<GroupDTO[]>([]);
  const [roleOptions, setRoleOptions] = useState<RoleDTO[]>([]);
  const [branchOptions, setBranchOptions] = useState<GroupDTO[]>([]);
  const [employeeId, setEmployeeId] = useState('');
  const [pagedResult, setPagedResult] = useState<PagedResult<EmployeeDTO>>({
    items: [],
    totalCount: 0,
    pageNumber: 1,
    pageSize: 10,
    searchTerm: ''
  });

  const initialFormData: UserCreateDto = {
    loginName: "",
    employeeId: "",
    lastName: "",
    firstName: "",
    middleName: "",
    email: "",
    roleId: 0,
    branchAccesses: [],
  };

  // Define form fields, including value, error, and helper text
  const [userFormData, setUserFormData] = useState(initialFormData);
  const [editFormData, setEditFormData] = useState<UserUpdateDto>({
    loginName: "",
    employeeId: "",
    firstName: "",
    middleName: "",
    lastName: "",
    email: "",
    roleId: 0,
    branchAccessIds: [],
    branchAccesses: [],
  });
  // const [branchIds, setBranchIds] = useState([]);
  const [searchTerm, setSearchTerm] = useState<string>(pagedResult.searchTerm);

  const fetchRolesOptions = async () => {
    try {
      const result = await userService.getAllRoles();
      setRoleOptions(result.data);
    } catch (error) {
      console.error("Error fetching roles", error);
    }
  };

  const fetchBranchOptions = async () => {
    try {
      const result = await groupService.getAllGroups();
      setBranchOptions(result.data);
    } catch (error) {
      console.error("Error fetching groups", error);
    }
  };

  // Handle input changes
  const handleChange = (e: any) => {
    const { name, value } = e.target;
    if (openEditUserModal) {
      setEditFormData({ ...editFormData, [name]: value });
    } else {
      setUserFormData({ ...userFormData, [name]: value });
    }
  };

  // Handle branch changes
  const handleSaveBranchBranchChange = (
    _event: React.ChangeEvent<{}>,
    value: GroupDTO[]
  ) => {
    setSelectedBranches(value);

    const branchAccesses: BranchAccessDTO[] = value.map((branch) => ({
      employeeId: userFormData.employeeId,
      branchId: Number(branch.code),
      usersLoginName: userFormData.loginName,
    }));


    setUserFormData({ ...userFormData, branchAccesses });
  };

  const handleEditBranchBranchChange = (
    _event: React.ChangeEvent<{}>,
    value: GroupDTO[]
  ) => {
    setSelectedBranches(value);


    const branchAccesses: BranchAccessDTO[] = Array.from(
      new Map(
        value.map((branch) => [branch.code, {
          employeeId: userFormData.employeeId,
          branchId: Number(branch.code),
          usersLoginName: userFormData.loginName,
        }])
      ).values()
    );


    // Extract branch IDs from the user data
    const branchAccessIds = Array.from(
      new Set(branchAccesses.map((branch: any) => branch.branchId))
    );

    // Update editFormData with both branchAccesses and branchAccessIds
    setEditFormData({
      ...editFormData,
      branchAccesses,
      branchAccessIds,
    });

  };

  const resetUserFormData = () => {
    setUserFormData(initialFormData);
    setSelectedBranches([]);
  };

  const handleCloseModal = () => {
    resetUserFormData();
    setOpenCreateUserModal(false);
    setOpenEditUserModal(false);
    setOpenDeleteModal(false);
  };

  const handleAddOpenModal = async () => {
    fetchBranchOptions();
    fetchRolesOptions();
    setOpenCreateUserModal(true);
  };

  const handleEditOpenModal = async (employeeId: any) => {
    try {
      fetchBranchOptions();
      fetchRolesOptions();
      // Fetch user data by ID
      const result = await userService.getUserById(employeeId);

      if (result.success) {
        const userData = result.data;
        setUserFormData(userData);

        const branchIds = userData.branchAccesses.map((branch: any) => branch.branchId);

        const branchDetails = await groupService.getBranchDetails(branchIds);
        setSelectedBranches(branchDetails);

        // Set the edit form data based on userFormData
        setEditFormData({
          loginName: userData.loginName,
          employeeId: userData.employeeId,
          firstName: userData.firstName,
          middleName: userData.middleName,
          lastName: userData.lastName,
          email: userData.email,
          roleId: userData.roleId,
          branchAccessIds: branchIds,
          branchAccesses: userData.branchAccesses,
        });

        setOpenEditUserModal(true);
      } else {
        ToastService.error("No data found.");
      }
    } catch (error) {
      console.error("An error occurred while fetching user details:", error);
      ToastService.error("An error occurred while loading data.");
    }
  };

  const handleDeleteOpenModal = async (employeeId: any) => {
    setOpenDeleteModal(true);
    setEmployeeId(employeeId);

  };

  const fetchUsers = async () => {
    try {
      const result = await userService.getPaginatedUsers(
        pagedResult.pageNumber,
        pagedResult.pageSize,
        searchTerm
      );
      setPagedResult(result.data.data);
    } catch (error) {
      console.error("Error fetching users", error);
    }
  };

  useEffect(() => {
    fetchUsers();

  }, [pagedResult.pageNumber, pagedResult.pageSize, searchTerm]);

  const columns = [
    { label: 'Username', accessor: 'loginName' },
    { label: 'Employee ID', accessor: 'employeeId' },
    {
      label: 'Date Created',
      render: (data: any) => (
        FormattedDate(data.createdDate)
      ),
    },
    {
      label: 'Last Log-in',
      render: (data: any) => (
        FormattedDate(data.lastLogIn)
      ),
    },
    {
      label: 'Branches',
      render: (data: any) => (
        data.branchAccesses?.length || 0
      ),
    },
    { label: 'Role', accessor: 'roleName' },
    {
      label: 'Action',
      render: (data: any) => (
        <Box sx={globalStyle.buttonBox}>
          <Tooltip title="Edit">
            <IconButton color='primary' onClick={() => handleEditOpenModal(data.employeeId)}>
              <EditNoteTwoTone />
            </IconButton>
          </Tooltip>
          <Tooltip title="Delete" onClick={() => handleDeleteOpenModal(data.employeeId)}>
            <IconButton sx={globalStyle.buttonRed}>
              <DeleteTwoTone />
            </IconButton>
          </Tooltip>
        </Box>
      ),
    },
  ];

  const pageCount = Math.ceil(pagedResult.totalCount / pagedResult.pageSize);


  const handlePageChange = (newPage: number) => {
    setPagedResult({
      ...pagedResult,
      pageNumber: newPage,
    });
  };

  // Handle search input change
  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value);
    setPagedResult({
      ...pagedResult,
      pageNumber: 1,
    });
  };

  const handleSubmitCreateUser = async (e: any) => {
    e.preventDefault();

    try {
      const result = await userService.addUser(userFormData);

      if (result.success) {
        handleCloseModal();
        fetchUsers()
        ToastService.success(result.message);
      } else {
        ToastService.error(result.message);
      }
    } catch (error) {
      console.error("Error creating user", error);
      ToastService.error('An error occurred while creating the user.');
    }

  };

  const handleSubmitEditUser = async (e: any) => {
    e.preventDefault();
    try {
      const result = await userService.updateUser(editFormData);

      if (result.success) {
        handleCloseModal();
        fetchUsers()
        ToastService.success(result.message);
      } else {
        ToastService.error(result.message);
      }
    } catch (error) {
      console.error("Error creating user", error);
      ToastService.error('An error occurred while creating the user.');
    }
  };

  const handleConfirmDelete = async (employeeId: string) => {
    try {
      const result = await userService.deleteUser(employeeId);

      if (result.success) {
        handleCloseModal();
        fetchUsers()
        ToastService.success(result.message);
      } else {
        ToastService.error(result.message);
      }
    } catch (error) {
      console.error("Error creating user", error);
      ToastService.error('An error occurred while creating the user.');
    }
  };

  return (
    <>
      <Typography variant="h6" component="h6" gutterBottom>
        User Dashboard
      </Typography>

      <Box sx={globalStyle.mainBox}>
        <Box sx={{ m: 1 }}>
          <GlobalButton buttonAction="add" buttonName="Add User" onClick={handleAddOpenModal} />
        </Box>

        <Box sx={globalStyle.searchBox}>
          <TextField
            label="Search"
            variant="outlined"
            size="small"
            sx={globalStyle.searchInput}
            value={searchTerm}
            onChange={handleSearchChange}
          />
        </Box>
      </Box>

      <Table columns={columns} data={pagedResult.items} />

      <PaginationControls
        currentPage={pagedResult.pageNumber}
        totalPages={pageCount}
        onPageChange={handlePageChange}
        totalItems={pagedResult.totalCount}
      />

      <Dialog open={openCreateUserModal} onClose={handleCloseModal} maxWidth="lg" fullWidth>
        <UserFormModal
          formData={userFormData}
          onChange={handleChange}
          onBranchChange={handleSaveBranchBranchChange}
          handleSubmit={handleSubmitCreateUser}
          rolesOptions={roleOptions}
          branchOptions={branchOptions}
          selectedBranches={selectedBranches}
          onCancel={handleCloseModal}
          isEditMode={false}
        />
      </Dialog>


      <Dialog open={openEditUserModal} onClose={handleCloseModal} maxWidth="lg" fullWidth>
        <UserFormModal
          formData={editFormData} // Use editFormData here
          onChange={handleChange}
          onBranchChange={handleEditBranchBranchChange}
          handleSubmit={handleSubmitEditUser}
          rolesOptions={roleOptions}
          branchOptions={branchOptions}
          selectedBranches={selectedBranches}
          onCancel={handleCloseModal}
          isEditMode={true}
        />
      </Dialog>


      <ConfirmationModal
        open={openDeleteModal}
        handleClose={handleCloseModal}
        title="Delete Confirmation"
        content="Are you sure you want to delete this user?"
        handleConfirm={() => handleConfirmDelete(employeeId)}
        buttonName="Delete" id={null}      >

      </ConfirmationModal >
    </>
  );
};

export default Dashboard;