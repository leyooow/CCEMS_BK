/* eslint-disable no-useless-catch */
/* eslint-disable @typescript-eslint/no-explicit-any */
import apiClient from './apiClient';

const userService = {

  async getPaginatedUsers(pageNumber: number = 1, pageSize: number = 10, searchTerm: string = '') {
    try {
      const response = await apiClient.get('/User/GetPaginatedUsers', {
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

  async getAllRoles() {
    const response = await apiClient.get('/User/GetAllRoles', {
    });
    return response.data;
  },

  async getAllPermissionLookups() {
    const response = await apiClient.get('/User/GetAllPermissionLookups', {
    });
    return response.data;
  },

  async getAllPermissionByRoleId(roleId: number) {
    const response = await apiClient.get(`/User/GetPermissionsByRoleId/${roleId}`, {
    });
    return response.data;
  },

  async addPermission(permissionsData: any) {
    const response = await apiClient.post(`/User/AddPermissions/`, permissionsData, {
    });
    return response.data;
  },

  async getUserById(userId: any) {
    const response = await apiClient.get(`/User/GetUserById/${userId}`, {
    });
    return response.data;
  },

  async addUser(UserData: any) {
    const response = await apiClient.post(`/User/CreateUser/`, UserData, {
    });
    return response.data;
  },

  async updateUser(UserData: any) {
    const response = await apiClient.put(`/User/UpdateUser/`, UserData, {
    });
    return response.data;
  },

  async checkUserAD(loginName: any) {
    const response = await apiClient.get(`/User/CheckAdUsername/${loginName}`, {
    });
    return response.data;
  },

  async deleteUser(employeeId: string) {
    const response = await apiClient.delete(`/User/DeleteUser/${employeeId}`, {
    });
    return response.data;
  },

  


};

export default userService;
