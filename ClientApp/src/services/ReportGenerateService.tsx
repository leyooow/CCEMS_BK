// src/services/EmployeeService.ts

import apiClient from './apiClient'; // Assuming apiClient.ts handles base configurations like axios instance
import { GenerateMainReportsViewModel } from '../models/reportGenerateDTO';

const ReportGenerateService = {
  async populateGroupsDropDownList() {
    try {
      const response = await apiClient.get('/ReportGenerate/PopulateGroupsDropDownList');
      return response;
    } catch (error) {
      throw error;
    }
  },
  async generateReport(data: GenerateMainReportsViewModel) {
    try {
      const response = await apiClient.post('/ReportGenerate/GenerateReport', {
        ...data
      });
      return response;
    } catch (error) {
      throw error;
    }
  },
};

export default ReportGenerateService;
