import { createContext, useContext, useMemo, useState, type ReactNode } from 'react';
import { api } from '../services/api';

interface AuthContextValue { token:string|null; username:string|null; login:(username:string,password:string)=>Promise<void>; logout:()=>void; }
const AuthContext = createContext<AuthContextValue | null>(null);
export function AuthProvider({children}:{children:ReactNode}) { const [token,setToken]=useState(localStorage.getItem('ems_token')); const [username,setUsername]=useState(localStorage.getItem('ems_user'));
  const login=async(username:string,password:string)=>{ const {data}=await api.post('/auth/login',{username,password}); localStorage.setItem('ems_token',data.token); localStorage.setItem('ems_user',data.username); setToken(data.token); setUsername(data.username); };
  const logout=()=>{localStorage.removeItem('ems_token');localStorage.removeItem('ems_user');setToken(null);setUsername(null);};
  return <AuthContext.Provider value={useMemo(()=>({token,username,login,logout}),[token,username])}>{children}</AuthContext.Provider>;
}
export function useAuth(){const ctx=useContext(AuthContext);if(!ctx)throw new Error('useAuth must be inside AuthProvider');return ctx;}
