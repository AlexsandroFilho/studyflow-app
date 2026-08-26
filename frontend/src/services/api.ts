import axios from 'axios';

export const api = axios.create({
  baseURL: 'http://localhost:5233/api/v1',
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response) {
      console.error(
        `[API Error ${error.response.status}] ${error.config?.method?.toUpperCase()} ${error.config?.url}:`,
        error.response.data
      );
    } else {
      console.error('[API Error Network/Unknown]:', error.message);
    }
    return Promise.reject(error);
  }
);