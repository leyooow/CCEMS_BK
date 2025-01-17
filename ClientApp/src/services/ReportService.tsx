// src/services/EmployeeService.ts

import apiClient from './apiClient'; // Assuming apiClient.ts handles base configurations like axios instance
import { DownloadAdhocViewModel } from '../models/reportDTO'; // Update with actual DTO paths

const ReportService = {
  async getlist(searchString: string = '',Page: number = 1) {
    try {
      const response = await apiClient.get('/Report/GetList', {
        params: {
          searchString,
          Page
        },
      });
      return response;
    } catch (error) {
      throw error;
    }
  },
  async downloadAdhoc(vm: DownloadAdhocViewModel) {
    try {
      const response = await apiClient.post('/Report/DownloadAdhoc', {
        ...vm
      });
      return response;
    } catch (error) {
      throw error;
    }
  },
};

export default ReportService;
