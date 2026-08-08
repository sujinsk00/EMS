# Employee Management System

A full-stack HR/employee management assignment implementation using:

- Frontend: React + TypeScript + Vite
- Backend: ASP.NET Core 8 Web API
- Database: MySQL 8
- Authentication: JWT + ASP.NET PasswordHasher
- Reports: PDF + Excel
- Bonus analytics: hiring trend, department growth, attendance pattern

## Features

- Secure login
- Employee add/edit/delete/view
- Bulk employee API
- Department management
- Daily attendance management
- Dashboard metrics and charts
- Employee directory PDF/Excel export
- Attendance Excel export
- Responsive UI
- Swagger API documentation

## 1. Prerequisites

Install:

- .NET 8 SDK
- Node.js 20+ (Node 22 recommended)
- Git
- Docker Desktop (recommended for MySQL)

## 2. Start MySQL

From the repository root:

```bash
docker compose up -d mysql
```

## 3. Start backend

```bash
cd backend/EmployeeManagement.Api
dotnet restore
dotnet run --urls http://localhost:5000
```

The API will be available at:

- http://localhost:5000/api/health
- http://localhost:5000/swagger

The app automatically creates the database schema on first start and seeds:

- Username: `admin`
- Password: `Admin@123`

For production, immediately change the seeded password and JWT secret.

## 4. Start frontend

Open another terminal:

```bash
cd frontend
npm install
npm run dev
```

Open http://localhost:5173

## 5. Configuration

Backend connection string and JWT settings are in:

`backend/EmployeeManagement.Api/appsettings.json`

Frontend API URL is configured by `frontend/.env`:

```env
VITE_API_URL=http://localhost:5000/api
```

## 6. GitHub repository

Create an empty GitHub repository named `employee-management-system`, then:

```bash
git init
git add .
git commit -m "Initial employee management system"
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/employee-management-system.git
git push -u origin main
```

Then share:

`https://github.com/YOUR_USERNAME/employee-management-system`

## Demo flow for Loom

1. Login with admin credentials.
2. Show dashboard metrics/charts.
3. Create a department.
4. Add an employee.
5. Edit the employee.
6. Mark attendance.
7. Show attendance history.
8. Generate PDF employee report.
9. Generate Excel employee report.
10. Generate attendance Excel report.
11. Show responsive layout by resizing the browser.
12. Open Swagger and demonstrate protected API endpoints.

## Important production improvements

Before deploying publicly:

- Move secrets to environment variables / secret manager.
- Replace `EnsureCreated` with EF Core migrations.
- Add refresh tokens and account lockout/rate limiting.
- Use HTTPS.
- Add role-based permissions for HR/Admin users.
- Add automated tests and CI/CD.
- Use a managed MySQL instance instead of local Docker.
