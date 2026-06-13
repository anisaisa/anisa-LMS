# LearnHub LMS API (anisa-lms)

REST API for the **LearnHub** Learning Management System. Built with ASP.NET Core 8 and PostgreSQL using a layered Service-Oriented Architecture.

The Angular frontend is a separate application and communicates with this API over HTTP/JSON.

## Tech Stack

- ASP.NET Core 8 Web API
- Entity Framework Core 8
- PostgreSQL (Npgsql)
- ASP.NET Core Identity
- JWT Bearer authentication
- AutoMapper
- Swagger / OpenAPI

## Features

- User authentication and role-based authorization (Admin, Instructor, Student)
- Course and module management
- Student enrollment with capacity limits
- Sequential module unlocking based on student progress
- Assessments and assessment scores
- Module progress tracking with automatic course completion
- Role-based dashboard statistics
- RESTful services exposed under `/api`

## Architecture

```
Angular Client
      ↓ HTTP/JSON
Controllers  →  Services  →  Repositories  →  EF Core  →  PostgreSQL
```

| Layer | Responsibility |
|-------|----------------|
| **Controllers** | HTTP endpoints, status codes, request/response handling |
| **Services** | Business rules, validation, caching, access control |
| **Repositories** | Database queries via Entity Framework Core |
| **DTOs** | API request and response models |
| **Models** | Database entities |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/) (local or hosted)
- Optional: [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/anisaisa/anisa-LMS.git
cd anisa-lms
```

### 2. Configure the database

For local development, update `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=LMS;Username=postgres;Password=YOUR_PASSWORD"
}
```

Use your own PostgreSQL credentials. Do not commit real passwords to Git.

### 3. Configure JWT (local)

In `appsettings.json`, set a secret key of at least 32 characters:

```json
"Jwt": {
  "Key": "YOUR_SECRET_KEY_AT_LEAST_32_CHARACTERS",
  "Issuer": "http://localhost:5000",
  "Audience": "http://localhost:5000"
}
```

### 4. Run the API

```bash
dotnet restore
dotnet run
```

| Environment | URL |
|-------------|-----|
| API (HTTPS) | `https://localhost:7253` |
| API (HTTP) | `http://localhost:5000` |
| Swagger UI | `https://localhost:7253/swagger` |

Database migrations are applied automatically on startup.

To apply migrations manually:

```bash
dotnet ef database update
```

## Configuration

| Setting | Description |
|---------|-------------|
| `ConnectionStrings:DefaultConnection` | Local PostgreSQL connection string |
| `PGHOST`, `PGPORT`, `PGDATABASE`, `PGUSER`, `PGPASSWORD` | Railway PostgreSQL variables (production) |
| `Jwt:Key` | Secret used to sign JWT tokens |
| `Jwt:Issuer` / `Jwt:Audience` | JWT issuer and audience values |

On Railway, the app builds the connection string from `PGHOST`, `PGPORT`, `PGDATABASE`, `PGUSER`, and `PGPASSWORD` when those variables are present.

## API Endpoints

| Controller | Base Route | Purpose |
|------------|------------|---------|
| User | `/api/user` | Login, register, logout, role assignment, user lists |
| Course | `/api/course` | Create, read, update, delete courses |
| Module | `/api/module` | Module CRUD and student module list with lock logic |
| Enrollment | `/api/enrollment` | Enroll students and manage enrollment status |
| Assessment | `/api/assessment` | Assessment CRUD, upcoming deadlines, pass/fail results |
| AssessmentScore | `/api/assessment-score` | Record and update student scores |
| Progress | `/api/progress` | Track and update module completion |
| Dashboard | `/api/dashboard` | Role-based KPIs for Admin, Instructor, and Student |

Use Swagger for the full list of endpoints, request bodies, and authorization requirements.

## Authentication

1. Register with `POST /api/user/register` (default role: **Student**).
2. Log in with `POST /api/user/login` to receive a JWT token.
3. Send the token on protected requests:

   ```
   Authorization: Bearer {your-token}
   ```

4. Endpoints are protected with `[Authorize(Roles = "...")]`.
5. Admin can assign roles via `POST /api/user/assign-role`.

Roles seeded at startup: **Admin**, **Instructor**, **Student**.

## Testing

Unit tests use **xUnit** and **NSubstitute** to mock dependencies. Controllers, services, and repositories are covered.

```bash
dotnet test
```

## Deployment (Railway)

| Service | URL |
|---------|-----|
| Production API | `https://anisa-lms-production.up.railway.app/api` |
| Production frontend | `https://anisa-lms-frontend-production.up.railway.app` |

Railway provides PostgreSQL through environment variables. EF Core migrations run when the API starts. CORS is configured for the production Angular frontend.

## Project Structure

```
anisa-lms/
├── Controllers/       # REST API endpoints
├── Services/          # Business logic
├── Repositories/      # Data access layer
├── Interfaces/
│   ├── IService/      # Service contracts
│   └── IRepository/   # Repository contracts
├── Models/            # EF Core entities
├── DTOs/              # Request and response models
├── Mappings/          # AutoMapper profiles
├── Data/              # AppDbContext and role seeding
├── Migrations/        # EF Core migrations
├── Middleware/        # Custom exception handling
├── Exceptions/
└── Program.cs         # Application startup and DI
```

## Frontend Repository

The Angular client is maintained separately:

- Repository: `https://github.com/anisaisa/anisa-lms-frontend`
- Local dev: `http://localhost:4200` (proxies API requests to `https://localhost:7253`)

## Author

**Anisa**  
South East European University  
Service Oriented Architecture — 2025/2026
