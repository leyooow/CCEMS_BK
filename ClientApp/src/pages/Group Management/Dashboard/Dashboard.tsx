import React, { useEffect, useState } from "react";
import GroupService from "../../../services/groupService";
import { GroupCreateDTO, GroupDTO, GroupUpdateDTO } from "../../../models/groupDTOs";
import PaginationControls from "../../../components/Pagination/PaginationControls";
import Table from "../../../components/Table/Table";
import { Box, Typography, TextField, IconButton, Tooltip } from "@mui/material";
import EditNoteTwoTone from "@mui/icons-material/EditNoteTwoTone";
import DeleteTwoTone from "@mui/icons-material/DeleteTwoTone";
import { FormattedDate } from "../../../utils/formatDate";
import { globalStyle } from "../../../styles/theme";
import GlobalButton from "../../../components/Button/Button";
import GroupFormModal from "../../../components/Modal/GroupFormModal";
import { PagedResult } from "../../../models/GenericResponseDTO";
import ToastService from "../../../utils/toast";
import ConfirmationModal from "../../../components/Modal/ConfirmationModal";

const GroupList: React.FC = () => {
  // Define state with proper initial structure
  const [pagedResult, setPagedResult] = useState<PagedResult<GroupDTO>>({
    items: [],
    totalCount: 0,
    pageNumber: 1,
    pageSize: 10,
    searchTerm: "",
  });

  const [modalTitle, setModalTitle] = useState("");
  const [openAddModal, setOpenAddModal] = useState(false);
  const [openConfirmModal, setOpenConfirmModal] = useState(false);
  const [selectedGroupId, setSelectedGroupId] = useState<number | null>(null);
  const [formData, setFormData] = useState<GroupCreateDTO>({
    code: "",
    name: "",
    area: "",
    division: "",
  });

  const [searchTerm, setSearchTerm] = useState<string>(pagedResult.searchTerm);

  const fetchGroups = async () => {
    try {
      const result = await GroupService.getPaginatedGroups(
        pagedResult.pageNumber,
        pagedResult.pageSize,
        searchTerm
      );
      setPagedResult(result.data);
    } catch (error) {
      console.error("Error fetching groups", error);
    }
  };

  // UseEffect hook to refetch groups based on page number, page size, or search term change
  useEffect(() => {
    fetchGroups();
  }, [pagedResult.pageNumber, pagedResult.pageSize, searchTerm]);

  const columns = [
    { label: "Branch Code", accessor: "code" },
    { label: "Branch Name", accessor: "name" },
    { label: "Description", accessor: "description" },
    {
      label: "Date Created",
      render: (data: any) => FormattedDate(data.dateCreated),
    },
    {
      label: "Date Modified",
      render: (data: any) => FormattedDate(data.dateModified),
    },
    { label: "Area", accessor: "area" },
    { label: "Division", accessor: "division" },
    {
      label: "Action",
      render: (data: GroupUpdateDTO) => (
        <Box sx={globalStyle.buttonBox}>
         <Tooltip title="Edit">
        <IconButton color="primary" onClick={() => handleEditGroup(data)}>
          <EditNoteTwoTone />
        </IconButton>
      </Tooltip>
          <Tooltip title="Delete">
            <IconButton
              sx={globalStyle.buttonRed}
              onClick={() => handleOpenConfirmModal(data.id)}
            >
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
      searchTerm: e.target.value,
    });
  };

  const handleOpenAddModal = () => {
    setOpenAddModal(true);
    setModalTitle("Add Group");
  };

  const handleCloseAddModal = () => {
    setOpenAddModal(false);
    setFormData({ code: "", name: "", area: "", division: "" });
  };

  const handleEditGroup = (group: GroupUpdateDTO) => {
  setFormData({
    code: group.code,
    name: group.name,
    area: group.area || '',
    division: group.division || '',
  });
  setSelectedGroupId(group.id);
  setModalTitle('Edit Group');
  setOpenAddModal(true);
};

  const handleSave = () => {
    console.log("Data saved:", formData);
    handleCloseAddModal();
    fetchGroups();
  };

  const handleOpenConfirmModal = (groupId: number) => {
    setSelectedGroupId(groupId);
    setOpenConfirmModal(true);
  };

  const handleCloseConfirmModal = () => {
    setOpenConfirmModal(false);
    setSelectedGroupId(null);
  };

  const handleDeleteGroup = async () => {
    if (selectedGroupId !== null) {
      try {
        const result = await GroupService.deleteGroup(selectedGroupId);
        if (result.success) {
          fetchGroups();
          ToastService.success(result.message);
        } else {
          ToastService.error(result.message);
        }
      } catch (error) {
        console.error("Error deleting group:", error);
        ToastService.error("An error occurred while deleting the group.");
      }
      handleCloseConfirmModal();
    }
  };

  return (
    <>
      <Typography variant="h6" component="h6" gutterBottom>
        Groups
      </Typography>

      <Box sx={globalStyle.mainBox}>
        <Box sx={{ m: 1 }}>
          <GlobalButton
            buttonAction="add"
            buttonName="Add Group"
            onClick={handleOpenAddModal}
          />
        </Box>

        {/* Search input box with spacing */}
        <Box sx={globalStyle.searchBox}>
          {/* Search Input */}
          <TextField
            label="Search"
            variant="outlined"
            size="small"
            sx={globalStyle.searchInput} // Make the input box flexible
            value={searchTerm} // Controlled input
            onChange={handleSearchChange} // Update search term as user types
          />

        </Box>
      </Box>

      {/* Table wrapped inside a responsive container */}
      <Table columns={columns} data={pagedResult.items} />

      {/* Pagination Controls Component */}
      <PaginationControls
        currentPage={pagedResult.pageNumber}
        totalPages={pageCount}
        onPageChange={handlePageChange}
        totalItems={pagedResult.totalCount}
      />

      <GroupFormModal
        open={openAddModal}
        handleClose={handleCloseAddModal}
        title={modalTitle}
        formData={formData}
        handleSave={handleSave}
        setFormData={setFormData}
        selectedGroupId={selectedGroupId}
      />

      <ConfirmationModal
        open={openConfirmModal}
        handleClose={handleCloseConfirmModal}
        handleConfirm={handleDeleteGroup}
        title="Delete Confirmation" 
        content="Are you sure you want to delete this group?"
        buttonName="Delete"
        id={selectedGroupId}
      />
    </>
  );
};

export default GroupList;