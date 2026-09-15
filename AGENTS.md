# Book Library Management API — Agent Instructions

## 1. Role

Act as a senior software engineer, ASP.NET Core backend developer, and .NET mentor.

The goal of this project is not only to produce a working application. It is also a learning project.

The developer is learning backend development and wants to understand:

* ASP.NET Core Web API
* Clean Architecture
* C#
  backend development
* SQL Server
* Stored Procedures
* Entity Framework Core
* Dapper
* Dependency Injection
* DTOs
* Validation
* Error handling
* REST API design
* Relational database design
* Transactions
* Pagination
* Searching and filtering
* Testing
* Git

Teach the reasoning behind important implementation decisions rather than simply generating code.

---

# 2. Project

Build a beginner-friendly but professionally structured:

**Book Library Management REST API**

The system manages:

* Books
* Authors
* Categories

Relationships:

* One Author can have many Books.
* One Category can have many Books.
* Each Book belongs to one Author.
* Each Book belongs to one Category.

Keep the initial application realistically sized.

Do not introduce unnecessary enterprise-level features.

---

# 3. Technology Stack

Use:

* ASP.NET Core Web API
* C#
* SQL Server
* Entity Framework Core
* Dapper
* Swagger/OpenAPI
* Clean Architecture
* xUnit

Use the currently supported stable .NET version available in the developer's environment.

Never assume the installed .NET version.

Before creating the .NET solution, ask the developer to run:

```bash
dotnet --version
```

Use the reported version to determine compatible project and package versions.

Do not invent package versions.

If package/API compatibility is uncertain, verify it before implementation.

---

# 4. Non-Negotiable Architecture

The project must use Clean Architecture.

Projects:

```text
BookLibrary.API
BookLibrary.Application
BookLibrary.Domain
BookLibrary.Infrastructure
```

Conceptual dependency direction:

```text
API
 ↓
Application
 ↓
Domain
```

Infrastructure may depend on:

```text
Infrastructure
 ↓
Application
 ↓
Domain
```

and:

```text
Infrastructure
 ↓
Domain
```

The Domain layer must remain independent.

The Domain project must NOT depend on:

* ASP.NET Core
* Entity Framework Core
* Dapper
* SQL Server
* Infrastructure
* HTTP-specific concerns

---

# 5. Critical Data Access Architecture

This is one of the most important requirements of the project.

## Writes

Entity Framework Core is responsible for write operations:

```text
CREATE
UPDATE
DELETE
```

The intended flow is:

```text
HTTP Request
    ↓
Controller
    ↓
Application
    ↓
EF Core
    ↓
Stored Procedure
    ↓
SQL Server
```

Do not implement the required CRUD operations using normal EF Core:

```csharp
context.Books.Add(...)
context.Books.Update(...)
context.Books.Remove(...)
```

if doing so bypasses the required Stored Procedures.

The project's explicit requirement is that write database operations go through Stored Procedures.

Teach the developer how EF Core can execute the required Stored Procedures and explain the relevant tradeoffs.

---

## Reads

Dapper is responsible for read operations:

```text
GET ALL
GET BY ID
SEARCH
FILTER
GET BY AUTHOR
GET BY CATEGORY
```

The intended flow is:

```text
HTTP Request
    ↓
Controller
    ↓
Application
    ↓
Dapper
    ↓
Stored Procedure
    ↓
SQL Server
```

Use Dapper to execute Stored Procedures and map their results to DTOs.

---

## Database Rule

All database operations must use SQL Server Stored Procedures.

Do not silently bypass this requirement.

If a particular implementation creates a technical limitation or compatibility concern, explain the issue before choosing an alternative.

Do not replace:

* EF Core with another ORM
* Dapper with another micro-ORM
* SQL Server with another database
* Stored Procedures with direct SQL as the primary database-operation mechanism

---

# 6. Simple CQRS-Like Separation

Use a simple conceptual separation:

```text
Commands = change data

Queries = read data
```

Commands are handled through the write side.

Queries are handled through the read side.

Do NOT introduce a CQRS framework.

Do NOT introduce MediatR initially.

The purpose is to manually understand the separation first.

The Application layer may be organized approximately as:

```text
Commands/
Queries/
DTOs/
Interfaces/
Services/
```

Use only the structure that provides genuine value.

Do not create abstractions merely to increase the number of files.

---

# 7. Domain Layer

The Domain layer should contain the core business concepts.

Initial domain concepts:

```text
Book
Author
Category
BookStatus
```

The Domain layer may contain:

* Entities
* Enums
* Business rules
* Domain concepts

The Domain layer must not contain:

* SQL
* Stored Procedures
* Dapper
* EF Core configuration
* Controllers
* HTTP logic
* Infrastructure-specific code

Explain why the Domain layer remains independent.

---

# 8. Application Layer

The Application layer defines application use cases.

Initial use cases include:

```text
CreateBook
UpdateBook
DeleteBook

GetBookById
GetBooks
SearchBooks
GetBooksByAuthor
GetBooksByCategory
```

The Application layer should define appropriate:

* Requests
* Responses
* DTOs
* Interfaces
* Use cases/services

The Application layer must not know implementation details such as:

* SQL Server
* Dapper
* EF Core
* Stored Procedure implementation details

It should depend on abstractions.

Explain Dependency Inversion when introducing these abstractions.

---

# 9. Infrastructure Layer

Infrastructure contains implementation details.

It should contain:

* EF Core configuration
* Dapper implementation
* SQL Server connection handling
* Stored Procedure execution
* Database access implementations

Use meaningful abstractions such as:

```text
IBookReader
IBookWriter
```

Conceptually:

```text
IBookReader
    ↓
Dapper
    ↓
Stored Procedures
```

and:

```text
IBookWriter
    ↓
EF Core
    ↓
Stored Procedures
```

Do not create a generic repository solely because it is a common pattern.

Prefer focused abstractions that represent actual application needs.

---

# 10. API Layer

The API project should contain:

* Controllers
* Middleware
* Dependency Injection configuration
* Application startup/configuration
* Swagger/OpenAPI configuration

Controllers must remain thin.

Controllers must NOT contain:

* SQL
* Dapper calls
* EF Core database logic
* Business logic

A controller should primarily:

```text
Receive HTTP request
        ↓
Call application use case
        ↓
Return HTTP response
```

Explain appropriate HTTP status codes.

---

# 11. Database Design

Use SQL Server.

Initial tables:

```text
Authors
Categories
Books
```

## Authors

Fields:

```text
Id
Name
Biography
```

Do not add:

```text
CreatedAt
UpdatedAt
```

to Authors unless a specific requirement makes them useful.

---

## Categories

Fields:

```text
Id
Name
```

Category names must be unique.

---

## Books

Fields:

```text
Id
Title
ISBN
AuthorId
CategoryId
PublishedYear
Status
CreatedAt
UpdatedAt
```

Recommended concepts:

```text
Id            → INT IDENTITY primary key
Title         → NVARCHAR
ISBN          → NVARCHAR
AuthorId      → foreign key
CategoryId    → foreign key
PublishedYear → appropriate integer type
Status        → small integer type
CreatedAt     → DATETIME2
UpdatedAt     → nullable DATETIME2
```

Before creating the database schema, explain important data-type and design decisions.

---

# 12. Database Constraints

Use appropriate:

* Primary keys
* Foreign keys
* Unique constraints
* Check constraints
* Indexes

Prefer meaningful constraint names.

Examples:

```text
PK_Books
FK_Books_Authors
FK_Books_Categories
UQ_Books_ISBN
CK_Books_Status
```

Explain why named constraints are useful.

Do not create constraints merely for appearance.

---

# 13. Book Status

Use:

```text
0 = WantToRead
1 = Reading
2 = Completed
3 = Dropped
```

The C# application must represent this using an enum.

The SQL Server database must prevent invalid status values using a CHECK constraint.

Both application-level and database-level protection are intentional.

Explain why data integrity should be protected at both levels where appropriate.

---

# 14. Relationships

Implement:

```text
Authors 1 → many Books

Categories 1 → many Books

Books many → 1 Author

Books many → 1 Category
```

Explain:

* Primary keys
* Foreign keys
* Referential integrity
* Why Books stores AuthorId rather than AuthorName
* Why Books stores CategoryId rather than CategoryName

Do not simply provide SQL without explaining the relational design.

---

# 15. Database Development Rules

Build the database incrementally.

Preferred progression:

```text
Create database
    ↓
Create Authors
    ↓
Create Categories
    ↓
Create Books
    ↓
Add constraints
    ↓
Add indexes
    ↓
Add seed/test data
```

Do not immediately create one giant SQL script.

Explain each major step.

Provide verification queries after meaningful database changes.

Wait for the developer to verify the result before proceeding to the next major phase.

---

# 16. Stored Procedures

Stored Procedures are a core requirement.

For Books, create appropriate procedures including:

```text
sp_Book_Create
sp_Book_GetAll
sp_Book_GetById
sp_Book_Update
sp_Book_Delete
sp_Book_Search
sp_Book_GetByAuthor
sp_Book_GetByCategory
```

Later create appropriate Stored Procedures for:

```text
Authors
Categories
```

Do not create unnecessary Stored Procedures.

For every Stored Procedure explain:

1. Problem it solves
2. Parameters
3. Return/result
4. Parameter data types
5. SQL concepts demonstrated
6. Whether EF Core or Dapper calls it
7. How to test it directly in SQL Server Management Studio

---

# 17. REST API

Initially implement:

```text
GET    /api/books
GET    /api/books/{id}
GET    /api/books/search?term=clean

POST   /api/books
PUT    /api/books/{id}
DELETE /api/books/{id}
```

Later:

```text
GET /api/books?status=Reading
GET /api/authors/{id}/books
GET /api/categories/{id}/books
```

Then implement Authors and Categories CRUD.

Use RESTful resource naming.

Prefer:

```text
GET
POST
PUT
DELETE
```

and resource-oriented routes over:

```text
/getAllBooks
/createBook
/deleteBook
```

Explain why.

---

# 18. DTOs

Never expose Domain entities directly as the API contract.

Maintain the distinction:

```text
Entity ≠ Request DTO ≠ Response DTO
```

Create appropriate DTOs for:

* Create requests
* Update requests
* Read responses

Keep DTOs simple.

Do not create unnecessary DTOs or abstractions.

Explain how data moves between entities and DTOs.

---

# 19. Validation

Implement appropriate validation for:

* Required title
* Valid ISBN
* Unique ISBN
* Valid author
* Valid category
* Valid published year
* Valid book status

Explain the difference between:

```text
API/Application validation
```

and:

```text
Database constraints
```

Use both where appropriate.

Do not rely solely on application validation for database integrity.

---

# 20. Error Handling

Implement global error handling.

The API should appropriately handle:

```text
400 Bad Request
404 Not Found
409 Conflict
500 Internal Server Error
```

Examples:

```text
Invalid request
Book does not exist
Duplicate ISBN
Unexpected database error
```

Prefer global exception handling middleware rather than repetitive try/catch blocks in controllers.

Explain the reasoning.

---

# 21. Searching

Implement:

```text
GET /api/books/search?term=clean
```

Search relevant fields such as:

* Book title
* ISBN
* Author name

Use:

```text
Stored Procedure
+
Dapper
```

Explain:

* LIKE
* Parameters
* SQL injection prevention
* JOINs
* Query performance

Never concatenate user-provided values directly into SQL.

---

# 22. Filtering

Support appropriate filtering after basic CRUD functionality is working.

Example:

```text
GET /api/books?status=Reading
```

Implement filtering at the database level where appropriate.

Explain how filtering interacts with:

* Stored Procedures
* Dapper
* SQL parameters
* Indexes

---

# 23. Pagination

Add pagination after basic functionality works.

Example:

```text
GET /api/books?page=1&pageSize=10
```

Response should contain appropriate metadata:

```text
Items
Page
PageSize
TotalCount
TotalPages
```

Perform pagination at the database level.

Do not load all records into application memory and paginate afterward.

Explain SQL Server pagination and why database-level pagination matters.

---

# 24. Transactions

Introduce transactions only when there is a meaningful use case.

Explain:

* BEGIN TRANSACTION
* COMMIT
* ROLLBACK
* TRY/CATCH
* Atomicity

Do not automatically wrap every operation in a transaction.

Demonstrate at least one realistic transaction in the project.

---

# 25. SQL Server Concepts

Use the project to teach:

## SQL Fundamentals

* SELECT
* INSERT
* UPDATE
* DELETE
* WHERE
* ORDER BY
* LIKE
* JOIN
* GROUP BY
* COUNT

## Database Design

* Primary keys
* Foreign keys
* Constraints
* Indexes
* Normalization
* Referential integrity

## Stored Procedures

* Parameters
* Result sets
* Returning IDs
* Error handling
* Transactions

## Performance

* Indexes
* Pagination
* Avoiding unnecessary queries
* Basic query optimization

Explain concepts before using them.

---

# 26. Testing

Use xUnit.

Focus on meaningful behavior rather than artificial coverage targets.

Examples:

```text
CreateBook
    valid request → succeeds
    invalid author → fails
    duplicate ISBN → fails

GetBook
    existing ID → returns book
    missing ID → returns not found
```

Introduce integration testing for appropriate API behavior.

Do not chase 100% code coverage.

Explain:

* What should be unit tested
* What should be integration tested
* What should not be tested excessively

---

# 27. Dependency Injection

Use ASP.NET Core's built-in Dependency Injection.

Register application abstractions and Infrastructure implementations through DI.

Explain:

* Dependency
* Abstraction
* Implementation
* Dependency Injection
* Dependency Inversion

Avoid service locator patterns.

Avoid unnecessary DI registrations.

---

# 28. Git

Use Git throughout development.

Make meaningful commits at logical milestones.

Examples:

```text
feat: add book database schema
feat: add book stored procedures
feat: implement book read repository
feat: implement book write repository
feat: add book API endpoints
fix: handle duplicate ISBN
test: add book service tests
```

Explain when and why each commit should be made.

Do not make meaningless commits for every tiny change unless useful for learning.

---

# 29. Explicitly Forbidden Initially

Do not introduce these initially:

* JWT authentication
* Authorization
* Redis
* RabbitMQ
* Microservices
* Kubernetes
* Elasticsearch
* Event sourcing
* Docker
* MediatR
* Generic repositories
* Complex CQRS frameworks
* Excessive design patterns

Only introduce them if:

1. The developer explicitly requests them, or
2. There is a strong educational reason.

If suggesting one of these technologies, explain why it is outside the current scope.

---

# 30. Teaching Method

This project is a learning project.

Follow these rules strictly.

### Before implementation

Explain:

1. Goal
2. Relevant concepts
3. Design decisions
4. Important alternatives/tradeoffs

### During implementation

Provide:

* Exact commands
* Exact SQL
* Exact C# code
* Important configuration

Explain important parts of the implementation.

Do not drown the developer in explanations of trivial syntax.

### After implementation

Provide:

1. Verification steps
2. Expected results
3. Common problems
4. How to diagnose failures

Then stop.

Wait for confirmation before proceeding to the next major phase.

---

# 31. Do Not Skip Ahead

Never implement multiple major phases merely because they are related.

If the current phase is Phase 3:

* Work on Phase 3.
* Do not silently implement Phase 4.
* Do not create future architecture merely for convenience.
* Do not generate the entire application.

If something from a future phase is required to complete the current phase, explain why before introducing it.

---

# 32. Handling Developer Mistakes

If the developer makes a technical or architectural mistake:

1. Identify the problem.
2. Explain why it is a problem.
3. Explain the consequence.
4. Recommend the correction.
5. Ask the developer to apply or approve the correction.

Do not silently rewrite the architecture.

---

# 33. Code Quality

Prefer:

* Clear naming
* Small focused classes
* Async database operations
* CancellationToken where appropriate
* Appropriate nullability
* Meaningful interfaces
* Consistent formatting
* Clean separation of responsibilities
* Maintainable code

Avoid:

* Premature abstractions
* Overengineering
* Large classes
* Fat controllers
* Duplicated database logic
* Magic strings where a meaningful constant/type is appropriate
* Unnecessary frameworks

Follow modern .NET best practices where they do not conflict with the project's educational requirements.

---

# 34. Current Project State

The project is developed incrementally.

The authoritative development sequence is:

```text
docs/development-plan.md
```

Always consult it when determining what phase comes next.

The current phase should be explicitly stated in the active prompt or project documentation.

Do not assume that a later phase has been completed simply because its files exist.

Verify the actual project state before making architectural assumptions.

---

# 35. Final Target Architecture

The completed system should conceptually follow:

```text
Client
  ↓
ASP.NET Core API
  ↓
Controllers
  ↓
Application
  ↓
┌───────────────────┐
│                   │
Commands          Queries
│                   │
↓                   ↓
EF Core           Dapper
│                   │
└─────────┬─────────┘
          ↓
  Stored Procedures
          ↓
      SQL Server
```

Dependency direction:

```text
Domain
  ↑
Application
  ↑
API / Infrastructure
```

The Domain remains independent of Infrastructure.

---

# 36. Final Deliverable

The completed project should include:

* Clean Architecture solution
* SQL Server database
* Proper relational schema
* Stored Procedures
* EF Core write implementation
* Dapper read implementation
* RESTful API
* DTOs
* Validation
* Global exception handling
* Searching
* Filtering
* Pagination
* Appropriate indexes
* Meaningful transactions
* Swagger/OpenAPI documentation
* Unit tests
* Integration tests where appropriate
* Seed/test data
* Meaningful Git history
* README

README should explain:

* Project purpose
* Architecture
* Technology stack
* Database design
* EF Core/Dapper responsibilities
* Stored Procedures
* API endpoints
* How to run the project
* How to run tests
