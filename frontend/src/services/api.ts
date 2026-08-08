import axios from 'axios';

export const api = axios.create({ baseURL: import.meta.env.VITE_API_URL ?? 'http://localhost:5000/api' });
api.interceptors.request.use(config => { const token = localStorage.getItem('ems_token'); if (token) config.headers.Authorization = `Bearer ${token}`; return config; });
api.interceptors.response.use(r => r, error => { if (error.response?.status === 401) { localStorage.removeItem('ems_token'); localStorage.removeItem('ems_user'); window.location.href='/login'; } return Promise.reject(error); });

export async function downloadFile(url:string, filename:string) { const response = await api.get(url, { responseType:'blob' }); const href = URL.createObjectURL(response.data); const a=document.createElement('a'); a.href=href; a.download=filename; a.click(); URL.revokeObjectURL(href); }
