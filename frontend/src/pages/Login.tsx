import { FormEvent, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function Login(){const [username,setUsername]=useState('admin');const [password,setPassword]=useState('Admin@123');const [error,setError]=useState('');const {login}=useAuth();const navigate=useNavigate();
const submit=async(e:FormEvent)=>{e.preventDefault();setError('');try{await login(username,password);navigate('/')}catch(err:any){setError(err.response?.data?.message??'Login failed')}};
return <div className="login-page"><form className="login-card" onSubmit={submit}><div className="brand big">EMS<span>.</span></div><h1>Sign in</h1><p>Employee Management System</p>{error&&<div className="alert error">{error}</div>}<label>Username<input value={username} onChange={e=>setUsername(e.target.value)} required/></label><label>Password<input type="password" value={password} onChange={e=>setPassword(e.target.value)} required/></label><button className="primary full">Sign in</button><small>Demo: admin / Admin@123</small></form></div>}
