# Task Management API

A robust, scalable, and secure RESTful API for Project and Task Management, built with **.NET 9** and designed using **Clean Architecture**, **CQRS**, and **MediatR**. 

This project goes beyond the standard requirements by implementing all 8 optional bonus points, showcasing modern C# backend best practices, data isolation, performance optimization, and containerization.

---

## 🏗 Architecture

The solution strictly adheres to **Clean Architecture** principles, ensuring separation of concerns, testability, and independence from frameworks and external services.

- **Domain:** Contains enterprise logic, POCO entities (`User`, `Project`, `TaskItem`), and Enums. Has **zero** external dependencies.
- **Application:** Contains business rules, MediatR commands/queries, DTOs, and Interfaces (e.g., `IApplicationDbContext`). Operates completely decoupled from the database or web layers.
- **Infrastructure:** Implements interfaces defined in Application. Contains EF Core configurations, Fluent API entity mappings, Redis caching logic, and the JWT Token Service.
- **WebAPI (Presentation):** The entry point. It contains lightweight, "skinny" controllers that simply dispatch requests to MediatR and return standard HTTP responses. Contains Middleware for global exception handling.

### CQRS & MediatR
The **Command Query Responsibility Segregation (CQRS)** pattern is heavily utilized via the `MediatR` library. Every use case is divided into discrete Commands (Writes) and Queries (Reads), each with its own single-responsibility Handler.

---

## 🛠 Tech Stack

- **Framework:** .NET 9.0 Web API
- **Database:** SQL Server 2022
- **ORM:** Entity Framework Core 9.0 (Code-First)
- **Caching:** Redis (StackExchange.Redis)
- **Authentication:** JWT Bearer Tokens (Microsoft.AspNetCore.Authentication.JwtBearer)
- **Architecture Patterns:** Clean Architecture, CQRS, Repository Pattern (via DbSet)
- **Testing:** xUnit, Moq (Unit Testing)
- **Containerization:** Docker, Docker Compose
- **Versioning:** Asp.Versioning.Http (URL-based API Versioning)

---

## ✨ Bonus Features Achieved

This project implements **100% of the mandatory requirements** and all requested **Bonus Features**:

1. **CQRS Implementation:** Full segregation of Reads and Writes using MediatR.
2. **MediatR Pattern:** Decoupled business logic from controllers for ultra-thin endpoints.
3. **Dockerization:** Multi-stage `Dockerfile` for the API and a `docker-compose.yml` to orchestrate the API, SQL Server, and Redis simultaneously.
4. **Unit Tests:** `Application.UnitTests` project utilizing `xUnit` and `Moq` (e.g., `CreateProjectCommandHandlerTests` covering success and failure paths).
5. **Redis Caching (with Graceful Degradation):** Read-through cache implementation for fetching projects. If Redis goes down, the application catches the timeout silently and safely falls back to SQL Server without failing the request.
6. **Generic Response Wrapper:** Every endpoint returns a consistent `ApiResponse<T>` object containing `Success`, `Message`, `Errors`, and `Data` properties.
7. **Role-based Authorization:** Users have roles (`Admin` or `User`). An Admin can view *all* projects across the system via the `GET /api/v1/projects/all` endpoint.
8. **API Versioning:** Integrated URL segment versioning (e.g., `/api/v1/projects`).

---

## 🚀 Quick Start / Setup Instructions

The easiest way to run the entire stack (API, Database, Cache) is using Docker Compose.

### Prerequisites
- Docker Desktop installed and running.

### Run with Docker Compose
1. Open your terminal at the root of the repository (where `docker-compose.yml` is located).
2. Execute the following command:
   ```bash
   docker-compose up -d --build
   ```
3. The following services will spin up:
   - **SQL Server 2022** on port `1433`
   - **Redis 7** on port `6379`
   - **Task Management API** on port `5125`

*(Note: The `docker-compose.yml` uses Health Checks to ensure the API only starts after the database and cache are fully ready).*

### Automatic Database Setup
On startup, the API will automatically apply any pending **EF Core Migrations** and **Seed the database** with sample data and test accounts. You do not need to run `dotnet ef database update` manually.

### Accessing the API
Once the containers are running, navigate to the Swagger UI to test the endpoints:
👉 **[http://localhost:5125/index.html](http://localhost:5125/index.html)**

---

## 🔐 Authentication & Testing Flow

To ensure a realistic production-like environment, the database does not rely on dummy seeded credentials. You will need to register your own accounts to test Role-based Authorization and Data Isolation.

**1. Regular User (Role: User)**
- Navigate to the `POST /api/v1/auth/register` endpoint in Swagger.
- Create a new account by providing a Name, Email, and a strong Password.
- Login using `POST /api/v1/auth/login` with the newly created credentials to receive your JWT token.
- *Permissions: This user can only perform CRUD operations on their own Projects and Tasks (Strict Data Isolation).*

**2. Admin User (Role: Admin)**
- First, register a new account via the `POST /api/v1/auth/register` endpoint.
- Open your SQL Server database (using SSMS or Azure Data Studio).
- Locate the newly registered user in the `Users` table and manually update the `Role` column value from `User` to `Admin`.
- Login via `POST /api/v1/auth/login` to receive your JWT token containing the Admin role claim.
- *Permissions: This user can access the restricted `GET /api/v1/projects/all` endpoint to view all projects across the system.*

---

## 📂 Project Structure

```text
Backend .NET Developer - Technical Assessment Task/
│
├── Domain/                               # Core Entities, Enums, and Rules
│   ├── Entities/ (User, Project, TaskItem)
│   └── Enums/ (TaskStatus, TaskPriority)
│
├── Application/                          # CQRS, MediatR, DTOs, Interfaces
│   ├── Common/ (ApiResponse<T>)
│   ├── Features/ (Projects, Tasks, Auth Commands/Queries)
│   └── Interfaces/ (IApplicationDbContext, ITokenService, etc.)
│
├── Infrastructure/                       # EF Core, Redis, JWT implementation
│   ├── Data/ (ApplicationDbContext, Configurations, Seeder)
│   ├── Migrations/
│   └── Services/ (TokenService, CurrentUserService)
│
├── Application.UnitTests/                # xUnit + Moq tests
│
├── Backend .NET Developer - Technical Assessment Task/   # Presentation Layer
│   ├── Controllers/ (v1 versioned controllers)
│   ├── Middlewares/ (GlobalExceptionHandler)
│   └── Program.cs
│
├── docker-compose.yml
└── Dockerfile
```
