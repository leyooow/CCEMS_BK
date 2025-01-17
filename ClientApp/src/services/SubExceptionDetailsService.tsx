// src/services/EmployeeService.ts

import { DeviationStatusUpdate, SubExceptionsDetailsDTO } from '../models/subExceptionDetailsDTO';
import apiClient from './apiClient'; // Assuming apiClient.ts handles base configurations like axios instance

const SubExceptionDetailsService = {
  
  async getSubExceptionDetails(subERN: string) {
    try {
      const response = await apiClient.get(`/SubExceptions/GetSubExceptionDetails/${subERN}`);
      return response.data;
    } catch (error) {
      console.error("Error fetching report details:", error);
      throw new Error("Failed to fetch exportDataFromDetails.");
    }
  },
  async deleteSubException(subERN: string,deleteSubExceptionRemarks : string) {
    try {
      const response = await apiClient.delete(`/SubExceptions/DeleteSubException/${subERN}?deleteSubExceptionRemarks=${deleteSubExceptionRemarks}`);
      return response.data;
    } catch (error) {
      console.error("Error fetching report details:", error);
      throw new Error("Failed to fetch exportDataFromDetails.");
    }
  },
  async updateSubException(subERN: string, formUpdate: DeviationStatusUpdate) {
    try {
      const { newStatus, taggingDate, exItemRefNo } = formUpdate;
  
      // Send data in the request body
      const response = await apiClient.put(`/SubExceptions/UpdateSubException/${subERN}?NewStatus=${newStatus}&TaggingDate=${taggingDate}&ExItemRefNo=${exItemRefNo}`);
      return response.data;
    } catch (error) {
      console.error("Error updating sub-exception:", error);
      throw new Error("Failed to update sub-exception.");
    }
  },
  async approveSubException(value : SubExceptionsDetailsDTO) {
    try {
      const response = await apiClient.post(`/SubExceptions/ApproveSubException`, {
        ...value
      });
      return response.data;
    } catch (error) {
      console.error("Error fetching report details:", error);
      throw new Error("Failed to fetch exportDataFromDetails.");
    }
  },
  async rejectSubException(value : SubExceptionsDetailsDTO, remarks : string) {
    try {
      const response = await apiClient.post(`/SubExceptions/RejectSubException?remarks=${remarks}`, {
        ...value
      });
      return response.data;
    } catch (error) {
      console.error("Error fetching report details:", error);
      throw new Error("Failed to fetch exportDataFromDetails.");
    }
  },
};

export default SubExceptionDetailsService;
