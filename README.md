# MeroLibrary Management System

A production-grade, educational **ASP.NET Core Solution** featuring a **RESTful Web API** (Clean Architecture, SQL Server, EF Core Writes, Dapper Reads via Stored Procedures), **JWT & Cookie Authentication** for multi-user library isolation, and a modern **Razor Pages Frontend Web Application** (`BookLibrary.Web`).

---

## 1. System Architecture Overview

```text
                        Browser (Client)
                               │
                               ▼
                     MeroLibrary.Web (Razor Pages)
           [ASP.NET Core 10, Bootstrap 5.3, Cookie Auth, IHttpClientFactory]
                               │
                               │ HTTP / REST + Bearer JWT (JSON)
                               ▼
                     MeroLibrary.Api (Web API)
                    [JWT Bearer Authentication]
                               │
             ┌─────────────────┴─────────────────┐
             ▼                                   ▼
  Application Layer (Writes)           Application Layer (Reads)
             │                                   │
        EF Core 10                             Dapper
             │                                   │
             └─────────────────┬─────────────────┘
                               ▼
                        Stored Procedures
                               │
                               ▼
                          SQL Server
               [Users, Books, Authors, Categories]
```

### Strict Architectural Boundaries
- **Backend (`BookLibrary.Api`)**: Independent REST API. Accesses SQL Server exclusively through Stored Procedures using EF Core for Writes and Dapper for Reads. All book operations are scoped by authenticated `UserId`.
- **Frontend (`BookLibrary.Web`)**: Standalone ASP.NET Core Razor Pages web application with Cookie Authentication. Communicates with `BookLibrary.Api` purely through HTTP REST calls using typed `HttpClient` services forwarding JWT Bearer tokens via `AuthHeaderHandler`.

---

## 2. Projects in the Solution

| Project | Type | Description |
|---|---|---|
| `BookLibrary.Api` | Web API (.NET 10) | REST API controllers, JWT authentication, OpenAPI/Swagger docs, global error middleware. |
| `BookLibrary.Application` | Class Library | Use cases, DTOs, request/response models, reader/writer abstractions. |
| `BookLibrary.Domain` | Class Library | Core POCO entities (`User`, `Book`, `Author`, `Category`, `BookStatus`) with zero external dependencies. |
| `BookLibrary.Infrastructure` | Class Library | EF Core DbContext for SP writes, Dapper for SP reads, PBKDF2 password hasher, JWT token service, SQL Server connection management. |
| `BookLibrary.Web` | Web App (.NET 10) | Server-rendered Razor Pages frontend UI with typed API clients and Cookie Auth. |
| `BookLibrary.Tests` | xUnit Test Project | Unit and integration test suite covering API controllers (Auth, Books) and frontend API client services. |

---

## 3. Technology Stack

- **Framework**: .NET 10 / ASP.NET Core
- **Frontend**: Razor Pages, Bootstrap 5.3, Cookie Authentication, `IHttpClientFactory` typed clients, `DataAnnotations` validation
- **Backend API**: ASP.NET Core Web API, JWT Bearer Authentication, Clean Architecture, Dependency Inversion
- **Database**: SQL Server (LocalDB / SQL Server)
- **Security**: PBKDF2 with SHA-256 password hashing, JWT Bearer tokens
- **Write ORM**: Entity Framework Core 10 (executing Stored Procedures via `ExecuteSqlRawAsync` / `SqlQueryRaw`)
- **Read Micro-ORM**: Dapper 2.1 (executing Stored Procedures via `QueryAsync` / `QueryMultipleAsync`)
- **Documentation**: OpenAPI / Swagger UI with Bearer auth support
- **Testing**: xUnit, Moq

---

## 4. Key Features

- **Multi-User Library Isolation**:
  - Secure registration and login (`/Account/Register`, `/Account/Login`, `/Account/Logout`).
  - Each user has their own private library: books are scoped to `UserId`.
  - Authors and Categories are shared catalog data across all users.
- **Optional Book Number Tracking**:
  - When adding a new book, users have the optional choice to specify a `Book Number` (e.g. for books currently being read, reading list sequence, or series volume).
  - Displays as a prominent `#BookNumber` badge across catalog, details, and edit views.
- **Dashboard**:
  - High-level statistics cards (Total Books, Authors, Categories, Currently Reading count), reading progress breakdown, and recent books catalog preview.
- **Books Management (`/Books`)**:
  - Full CRUD capabilities (List, Details, Create, Edit, Delete).
  - Server-side searching by Title, ISBN, or Author Name (scoped to user).
  - Server-side filtering by Status, Author, and Category.
  - Database-level pagination (`page` and configurable `pageSize`).
  - Color-coded BookStatus badges (*Want to Read*, *Reading*, *Completed*, *Dropped*).
- **Authors & Categories Management (`/Authors`, `/Categories`)**:
  - Full CRUD capabilities, author cascade deletion protection, unique category name enforcement.

---

## 5. How to Run the Application

### Step 1: Database Setup
1. Ensure **SQL Server** (or LocalDB) is running.
2. Execute the SQL schema scripts in `db/` (numbered `01_` through `35_`) to create tables, constraints, indexes, seed data, and stored procedures.
3. Verify connection string in `BookLibrary.Api/appsettings.json`.

### Step 2: Start the Web API
In your terminal, navigate to the solution root and start the API:
```bash
dotnet run --project BookLibrary.Api/BookLibrary.Api.csproj
```
- API starts at: `http://localhost:5288`
- Swagger Documentation: `http://localhost:5288/swagger`

### Step 3: Start the Razor Pages Frontend
In a **second terminal window**, start the web frontend:
```bash
dotnet run --project BookLibrary.Web/BookLibrary.Web.csproj
```
- Web Application starts at: `http://localhost:5277`
- Open `http://localhost:5277` in your browser.

---

## 6. How to Run Tests

Execute the complete xUnit test suite (covering Auth, Books controllers, and API clients):
```bash
dotnet test
```

---

## 7. Solution Structure

```text
d:\Dryice\
├── BookLibrary.Api/             ← REST Web API (JWT Bearer Auth, Controllers)
├── BookLibrary.Application/     ← Use cases, DTOs, abstractions
├── BookLibrary.Domain/          ← Core entities (User, Book, Author, Category)
├── BookLibrary.Infrastructure/  ← EF Core & Dapper data access, Hasher, TokenService
├── BookLibrary.Tests/           ← xUnit test suite (13 unit & integration tests)
├── BookLibrary.Web/             ← Razor Pages frontend application (Cookie Auth)
│   ├── Configuration/           ← ApiSettings options model
│   ├── Middleware/              ← FrontendExceptionHandlingMiddleware
│   ├── Models/                  ← Frontend DTOs & request models
│   ├── Pages/
│   │   ├── Account/             ← Login, Register, Logout pages
│   │   ├── Authors/             ← Authors CRUD pages
│   │   ├── Books/               ← Books CRUD pages with BookNumber support
│   │   ├── Categories/          ← Categories CRUD pages
│   │   ├── Shared/              ← Layout, navigation, error pages
│   │   └── Index.cshtml         ← Dashboard
│   └── Services/                ← Typed API Client services & AuthHeaderHandler
├── db/                          ← 35 SQL Server setup scripts
├── BookLibrary.slnx
└── README.md
```
