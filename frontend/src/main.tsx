import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import Layout from './components/Layout';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import Employees from './pages/Employees';
import EmployeeForm from './pages/EmployeeForm';
import Departments from './pages/Departments';
import Attendance from './pages/Attendance';
import Reports from './pages/Reports';
import './styles.css';

createRoot(document.getElementById('root')!).render(<StrictMode><AuthProvider><BrowserRouter><Routes><Route path="/login" element={<Login/>}/><Route element={<ProtectedRoute/>}><Route element={<Layout/>}><Route index element={<Dashboard/>}/><Route path="employees" element={<Employees/>}/><Route path="employees/new" element={<EmployeeForm/>}/><Route path="employees/:id/edit" element={<EmployeeForm/>}/><Route path="departments" element={<Departments/>}/><Route path="attendance" element={<Attendance/>}/><Route path="reports" element={<Reports/>}/></Route></Route><Route path="*" element={<Navigate to="/" replace/>}/></Routes></BrowserRouter></AuthProvider></StrictMode>);
