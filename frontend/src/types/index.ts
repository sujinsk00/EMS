export interface Employee { id:number; employeeCode:string; firstName:string; lastName:string; email:string; phone:string; jobTitle:string; salary:number; joiningDate:string; isActive:boolean; departmentId:number; departmentName:string; }
export interface Department { id:number; name:string; description?:string; employeeCount:number; }
export type AttendanceStatus = 'Present'|'Absent'|'Leave'|'HalfDay';
export interface Attendance { id:number; employeeId:number; employeeName:string; date:string; status:AttendanceStatus; notes?:string; }
export interface Dashboard { total:number; active:number; salary:number; hiredThisMonth:number; presentToday:number; absentToday:number; departments:{name:string;employeeCount:number}[]; hiringTrend:{month:string;count:number}[]; attendance:{status:string;count:number}[]; }
