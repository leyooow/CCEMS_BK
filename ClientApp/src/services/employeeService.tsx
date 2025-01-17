// src/services/EmployeeService.ts

import apiClient from './apiClient'; // Assuming apiClient.ts handles base configurations like axios instance
import { EmployeeCreateDTO, EmployeeUpdateDTO } from '../models/employeeMaintenanceDTOs'; // Update with actual DTO paths

const EmployeeService = {
  async getAllEmployees(pageNumber: number = 1, pageSize: number = 10, searchTerm: string = '') {
    try {
      const response = await apiClient.get('/Employee/GetPaginatedEmployees', {
        params: {
          pageNumber,
          pageSize, 
          searchTerm,
        },
      });
      return response;
    } catch (error) {
      throw error;
    }
  },

  async getEmployeeById(employeeId: string) {
    try {
      const response = await apiClient.get(`/Employee/GetEmployeeById/${employeeId}`);
      return response.data;
    } catch (error) {
      throw error;
    }
  },

  async createEmployee(EmployeeCreateDto: EmployeeCreateDTO) {
    try {
      const response = await apiClient.post('/Employee/CreateEmployee', EmployeeCreateDto);
      return response
    } catch (error) {
      throw error;
    }
  },

  async updateEmployee(id: number, EmployeeUpdateDto: EmployeeUpdateDTO): Promise<void> {
    try {
      await apiClient.put(`/Employee/UpdateEmployee/${id}`, EmployeeUpdateDto);
    } catch (error) {
      throw error;
    }
  },

  async deleteEmployee(id: number): Promise<void> {
    try {
      await apiClient.delete(`/Employees/DeleteEmployee/${id}`);
    } catch (error) {
      throw error;
    }
  },
};

export default EmployeeService;
