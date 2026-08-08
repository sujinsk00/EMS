# Architecture

React UI -> Axios -> ASP.NET Core Web API -> EF Core -> MySQL

Authentication:
React login -> POST /api/auth/login -> JWT -> localStorage -> Axios Authorization header

Main backend layers:
- Controllers: HTTP/API boundary
- Dtos: request/response contracts
- Models: database entities
- Data: EF Core DbContext
- Services: JWT and report generation

Main frontend layers:
- pages: feature screens
- components: shared UI/layout
- context: authentication state
- services: API client/download helpers
- types: TypeScript contracts
