/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable no-useless-catch */
import { ExceptionDTO } from '../models/exceptionManagementDTOs';
import apiClient from './apiClient';

const ExceptionManagement = {

  async getExceptionsList(pageNumber: number = 1, pageSize: number = 10, searchTerm: string = '', status: number = 1) {
    try {
      const response = await apiClient.get('/ExceptionsMgmt/GetExceptionsList', {
        params: {
          pageNumber,
          pageSize,
          searchTerm,
          status
        },
      });
      return response;
    } catch (error) {
      throw error;
    }
  },
  async getExceptionDetails(id: string = '') {
    try {
      const response = await apiClient.get('/ExceptionsMgmt/GetExceptionDetails', {
        params: {
          id
        },
      });
      return response;
    } catch (error) {
      throw error;
    }
  },

  async getExceptionUpdateDetails(id: string = ''): Promise<any> {
    try {
      const response = await apiClient.get('/ExceptionsMgmt/GetExceptionUpdateDetails', {
        params: {
          id
        },
      });
      return response;
    } catch (error) {
      throw error;
    }
  },
  async getDeviationByClasification(classification: any) {
    try {
      const response = await apiClient.get(`/ExceptionsMgmt/GetDeviationByClasification/${classification}`);
      return response.data;

    } catch (error) {
      throw error;
    }

  },
  async getDeviationByIds(ids: number[]) {
    try {
      const query = ids.map(id => `ids=${id}`).join('&');
      const response = await apiClient.get(`/ExceptionsMgmt/GetDeviationByIds?${query}`);
      return response.data;
    } catch (error) {
      throw error;
    }
  },

  async deleteException(id: string = '', remarks: string = '') {
    try {
      const response = await apiClient.delete(`/ExceptionsMgmt/DeleteException/${id}`, {
        params: {
          remarks
        },
      });
      return response.data;
    } catch (error) {
      throw error;
    }
  },
  async getForApprovalList(pageNumber: number = 1, pageSize: number = 10, searchString: string = '', status: number | null = null) {
    try {
      const response = await apiClient.get('/ExceptionsMgmt/GetExceptionsForApprovalList', {
        params: {
          pageNumber,
          pageSize,
          searchString,
          status
        },
      });
      return response;
    } catch (error) {
      throw error;
    }
  },
  async approveException(refNo: string = '', remarks: string = '') {
    try {
      const response = await apiClient.put(`/ExceptionsMgmt/ApproveException/${refNo}?remarks=${remarks}`);
      return response.data;
    } catch (error) {
      throw error;
    }
  },
  async rejectException(refNo: string = '', remarks: string = '') {
    try {
      const response = await apiClient.put(`/ExceptionsMgmt/RejectException/${refNo}?remarks=${remarks}`);
      return response.data;
    } catch (error) {
      throw error;
    }
  },
  async getExceptionForApprovalDetails(id: string = '') {
    try {
      const response = await apiClient.get('/ExceptionsMgmt/GetExceptionsForApprovalDetails', {
        params: {
          id
        },
      });
      return response;
    } catch (error) {
      throw error;
    }
  },

  async saveException(ExceptionDTO: ExceptionDTO) {
    try {
      const response = await apiClient.post('/ExceptionsMgmt/SaveException', ExceptionDTO);
      return response.data;
    } catch (error) {
      throw error;
    }
  },

  async updateException(ExceptionDTO: ExceptionDTO) {
    try {
      const response = await apiClient.put('/ExceptionsMgmt/UpdateException', ExceptionDTO);
      return response.data;
    } catch (error) {
      throw error;
    }
  },
}

export default ExceptionManagement;