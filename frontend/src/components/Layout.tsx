import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { BarChart3, Building2, CalendarCheck, FileText, LayoutDashboard, LogOut, Users } from 'lucide-react';
import { useAuth } from '../context/AuthContext';

export default function Layout(){const {logout,username}=useAuth();const navigate=useNavigate(); const nav=[['/','Dashboard',LayoutDashboard],['/employees','Employees',Users],['/departments','Departments',Building2],['/attendance','Attendance',CalendarCheck],['/reports','Reports',FileText]] as const;
return <div className="app-shell"><aside className="sidebar"><div className="brand">EMS<span>.</span></div><nav>{nav.map(([to,label,Icon])=><NavLink key={to} to={to} end={to==='/' }><Icon size={18}/>{label}</NavLink>)}</nav><button className="logout" onClick={()=>{logout();navigate('/login')}}><LogOut size={18}/>Logout</button></aside><main className="main"><header><div><h1>Employee Management</h1><p>Welcome back, {username}</p></div></header><section className="content"><Outlet/></section></main></div>}
