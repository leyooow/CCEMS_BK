import axios from 'axios';

// Access environment variables using import.meta.env
const token = localStorage.getItem('token');
const API_BASE_URL = import.meta.env.REACT_APP_API_URL_DEV || 'https://localhost:7192/api';

// Dynamically get the correct API base URL
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`,
  },
});

export default apiClient;
