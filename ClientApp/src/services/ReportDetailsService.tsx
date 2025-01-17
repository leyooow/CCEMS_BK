import apiClient from "./apiClient";

const ReportDetailsService = {
  async getReportDetails(id: number) {
    try {
      const response = await apiClient.get(`/ReportDetails/GetReport/${id}`);
      return response.data;
    } catch (error) {
      console.error("Error fetching report details:", error);
      throw new Error("Failed to fetch report details.");
    }
  },

  async getList(id: number, page: number) {
    try {
      const response = await apiClient.get(`/ReportDetails/getList/${id}`, {
        params: { page },
      });
      return response.data;
    } catch (error) {
      console.error("Error fetching report details:", error);
      throw new Error("Failed to fetch report details.");
    }
  },
  async pulloutRequest(id: number, refno: string) {
    try {
      const response = await apiClient.get(
        `/ReportDetails/PulloutRequest/${id}`,
        {
          params: { refno },
        }
      );
      return response.data;
    } catch (error) {
      console.error("Error fetching report details:", error);
      throw new Error("Failed to fetch pullout request.");
    }
  },
  async exportDataFromDetails(id: number) {
    try {
      const response = await apiClient.get(
        `/ReportDetails/ExportDataFromDetails/${id}`
      );
      return response.data;
    } catch (error) {
      console.error("Error fetching report details:", error);
      throw new Error("Failed to fetch pullout request.");
    }
  },

  async populateBranchRecipients(brCode: string): Promise<any> {
    try {
      const response = await apiClient.get(
        "/ReportDetails/PopulateBranchRecipients",
        {
          params: { selected: "selected", brCode },
        }
      );

      return response.data;
    } catch (error: any) {
      console.error(
        "Error fetching branch recipients:",
        error.message || error
      );
      throw new Error("Failed to fetch branch recipients.");
    }
  },

  async populateSelectedRecipients(brCode: string): Promise<any> {
    try {
      const response = await apiClient.get(
        "/ReportDetails/PopulateBranchRecipients",
        {
          params: { selected: "selected", brCode },
        }
      );

      return response.data;
    } catch (error: any) {
      console.error(
        "Error fetching branch recipients:",
        error.message || error
      );
      throw new Error("Failed to fetch branch recipients.");
    }
  },

  async getSelectedBranchName(id: Number) {
    try {
      const response = await apiClient.get("/ReportDetails/SelectedBranches", {
        params: { id },
      });
      return response.data;
    } catch (error: any) {
      console.error(
        "Error fetching branch recipients:",
        error.message || error
      );
      throw new Error("Failed to fetch branch recipients.");
    }
  },

  async approveReportDetails(data: any) {
    try {
    
      const response = await apiClient.post(`/ReportDetails/Approve?id=${data.id}&reportsGuid=${data.reportsGuid}`);
      return response.data;
    } catch (error) {
      console.error("Error approving report details:", error);
      throw new Error("Failed to approve report details.");
    }
  },

  async rejectReportDetails(data: { id: number; reportsGuid: string ; remarks:string;}) {
    try {
     
      const response = await apiClient.post(`/ReportDetails/Reject`, null, {
        params: { id: data.id, reportsGuid: data.reportsGuid , remarks: data.remarks },
      });
      return response.data;
    } catch (error) {
      console.error("Error rejecting report details:", error);
      throw new Error("Failed to approve report details.");
    }
  },

  async sendForApproval(payload: any) {
    try {
      // /ReportDetails/SendReport?ToList=20120021&CCList=&id=1098
      const formattedPayload = {
        ...payload,
        ToList: Array.isArray(payload.ToList) ? payload.ToList : [payload.ToList],
      };
      const response = await apiClient.post(`/ReportDetails/SendReport`, formattedPayload);
      return response.data;
    } catch (error) {
      console.error("Error sending for approval:", error);
      throw new Error("Failed to send for approval.");
    }
  },
  
};

export default ReportDetailsService;
