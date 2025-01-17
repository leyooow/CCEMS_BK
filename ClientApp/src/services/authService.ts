import apiClient from './apiClient';
import Cookies from 'js-cookie';

interface LoginRequest {
  username: string;
  password: string;
}

interface LoginResponse {
  isAuthenticated: string;
  message : string;
  token: string;
}

const authService = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const response = await apiClient.post<LoginResponse>('/auth/login', credentials);
    return response.data;
  },

  logout: () => {
    localStorage.removeItem('token');

    Cookies.remove('LoginName');
    Cookies.remove('Role');
    Cookies.remove('Permission');
    Cookies.remove('RoleID');
  },
  
};

export default authService;
