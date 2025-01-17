// src/services/EmployeeService.ts

import { BranchReplyViewModel } from '../models/reportBranchReplyDTO';
import apiClient from './apiClient'; 

const ReportBranchReplyService = {
  async getReportContentList(Id: string,refNo : string,page: number = 1) {
    try {
      const response = await apiClient.get('/ReportBranchReply/GetReportContentList', {
        params: {
          Id,
          refNo,
          page
        }
      });
      return response;
    } catch (error) {
      throw error;
    }
  },
  async getBranchReplyList(Id: string) {
    try {
      const response = await apiClient.get('/ReportBranchReply/GetBranchReplyList', {
        params: {
          Id
        }
      });
      return response;
    } catch (error) {
      throw error;
    }
  },
  async postReply(data: BranchReplyViewModel) {
    try {
      const response = await apiClient.post('/ReportBranchReply/PostReply', {
        ...data
      });
      return response;
    } catch (error) {
      throw error;
    }
  },
};

export default ReportBranchReplyService;
