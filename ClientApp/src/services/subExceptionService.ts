import apiClient from './apiClient';

const SubExceptionService = {
    async getForApprovalList(pageNumber: number = 1, pageSize: number = 10, searchString: string = '', status: number|null= null) {
      try {
        const response = await apiClient.get('/SubExceptions/GetSubExceptionsForApprovalList', {
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
    async getSubExceptionDetails(subERN: string = '') {
      try {
        const response = await apiClient.get('/SubExceptions/GetSubExceptionDetails', {
          params: {
            subERN
          },
        });
        return response;
      } catch (error) {
        throw error;
      }
    },
}

export default SubExceptionService;