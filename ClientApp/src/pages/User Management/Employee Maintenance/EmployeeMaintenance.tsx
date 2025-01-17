import React, { useCallback, useEffect, useState } from 'react';
import { EmployeeDTO } from '../../../models/employeeMaintenanceDTOs';
import { PagedResult } from '../../../models/GenericResponseDTO';
import PaginationControls from '../../../components/Pagination/PaginationControls';
import Table from '../../../components/Table/Table';
import { Box, Typography, TextField } from '@mui/material';
import { globalStyle } from '../../../styles/theme';
import EmployeeService from '../../../services/employeeService';
import GlobalButton from '../../../components/Button/Button';
import { EmployeeStyle } from './EmployeeMaintenanceStyle';
import AddDataModal from '../../../components/Modal/FormModal';
import ToastService from '../../../utils/toast';
import { ERROR_MESSAGES } from '../../../utils/constants';
import { FormData } from '../../../models/formDTOs';

const EmployeeMaintenance: React.FC = () => {
  // Define state with proper initial structure
  const [pagedResult, setPagedResult] = useState<PagedResult<EmployeeDTO>>({
    items: [],
    totalCount: 0,
    pageNumber: 1,
    pageSize: 10,
    searchTerm: ''
  });

  const [searchTerm, setSearchTerm] = useState<string>(pagedResult.searchTerm);
  const [modalTitle, setModalTitle] = useState('');
  const [openAddModal, setOpenAddModal] = useState(false);

  const initialFormData = {
    employeeId: { value: '', error: false, helperText: '' },
    firstName: { value: '', error: false, helperText: '' },
    middleName: { value: '', error: false, helperText: '' },
    lastName: { value: '', error: false, helperText: '' },
  };

  const resetFormData = useCallback(() => {
    setFormData(initialFormData);
  }, []);

  const [formData, setFormData] = useState<FormData>(initialFormData);
  const REQUIRED_FIELDS = ['employeeId', 'firstName', 'lastName'];

  const closeAddModal = () => {
    resetFormData()
    setOpenAddModal(false)

  }

  const handleCreateEmployee = async () => {
    // Validate required fields
    let isValid = true;
    const updatedFormData = { ...formData };

    REQUIRED_FIELDS.forEach((field) => {
      if (typeof formData[field].value === 'string' && formData[field].value.trim() === '') {
        updatedFormData[field] = {
            ...formData[field],
            error: true,
            helperText: ERROR_MESSAGES.REQUIRED_FIELD,
        };
        isValid = false;
    } else {
        updatedFormData[field] = {
            ...formData[field],
            error: false,
            helperText: '',
        };
    }
    });

    setFormData(updatedFormData);

    if (!isValid) {
      return;
    }

    try {
      const result = await EmployeeService.createEmployee({
        employeeId: formData.employeeId.value,
        firstName: formData.firstName.value,
        middleName: formData.middleName.value,
        lastName: formData.lastName.value,
      });
      if (result.data.success) {
        closeAddModal();
        fetchEmployees()
        ToastService.success(result.data.message);
      } else {
        ToastService.error(result.data.message);
      }
    } catch (error) {
      console.error("Error creating employee", error);
      ToastService.error('An error occurred while creating the employee.');
    }
  };

  const handleInputChange = (field: string, value: string) => {
    const isRequired = REQUIRED_FIELDS.includes(field);

    setFormData({
      ...formData,
      [field]: {
        ...formData[field],
        value: value,
        error: isRequired && value === '', // Set error only if the field is required and value is empty
        helperText: isRequired && value === '' ? ERROR_MESSAGES.REQUIRED_FIELD : '', // Set helper text only if the field is required and value is empty
      },
    });
  };

  const handleOpenAddModal = () => {
    setOpenAddModal(true);
    setModalTitle('Add Employee');
  };

  const fetchEmployees = async () => {
    try {
      const result = await EmployeeService.getAllEmployees(
        pagedResult.pageNumber,
        pagedResult.pageSize,
        searchTerm
      );
      setPagedResult(result.data.data);
    } catch (error) {
      console.error("Error fetching employees", error);
    }
  };

  // UseEffect hook to refetch Employees based on page number, page size, or search term change
  useEffect(() => {
    fetchEmployees();
  }, [pagedResult.pageNumber, pagedResult.pageSize, searchTerm]);

  const columns = [
    { label: 'Employee ID', accessor: 'employeeId' },
    {
      label: 'Full Name',
      render: (data: any) => (
        ` ${data.lastName}, ${data.firstName} ${data.middleName} `
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
    pagedResult.pageNumber = 1;
  };

  return (
    <>
      <Typography variant="h6" component="h6" gutterBottom>
        Employee Maintenance
      </Typography>
      <Box sx={globalStyle.mainBox}>
        <Box sx={EmployeeStyle.btnBox}>
          <GlobalButton buttonAction="add" buttonName="Add Employee" onClick={handleOpenAddModal} />
        </Box>

        {/* Search input box with spacing */}
        <Box sx={globalStyle.searchBox}>
          {/* Search Input */}
          <TextField
            label="Search"
            variant="outlined"
            size="small"
            sx={globalStyle.searchInput} // Make the input box flexible
            value={searchTerm}  // Controlled input
            onChange={handleSearchChange}  // Update search term as user types
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

      <AddDataModal
        open={openAddModal}
        handleClose={closeAddModal}
        title={modalTitle}
        formData={formData}
        handleInputChange={handleInputChange}
        handleSave={handleCreateEmployee}
        requiredFields={REQUIRED_FIELDS}
      />
    </>
  );
};

export default EmployeeMaintenance;