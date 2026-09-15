# Book Library Management API — Development Plan

## Purpose

This document defines the authoritative development sequence for the Book Library Management API.

The project must be implemented incrementally.

Do not skip phases.

Do not implement future phases prematurely.

Each phase follows:

```text
Understand
    ↓
Design
    ↓
Implement
    ↓
Verify
    ↓
Developer confirmation
    ↓
Next phase
```

---

# Phase 1 — Requirements and Architecture

## Goal

Understand the system before writing application code.

## Topics

* Project requirements
* Functional requirements
* Technology stack
* Database requirements
* Clean Architecture
* Project responsibilities
* Dependency direction
* EF Core vs Dapper
* Commands vs Queries
* Overall request flow

## Deliverable

A documented high-level architecture and confirmed project requirements.

## Do not

* Create database tables
* Create Stored Procedures
* Implement controllers
* Implement repositories
* Implement application code

---

# Phase 2 — SQL Server Database Design

## Goal

Design the relational database before implementing it.

## Topics

* Entities
* Tables
* Columns
* Data types
* Primary keys
* Foreign keys
* Relationships
* Normalization
* Constraints
* Index strategy

## Tables

```text
Authors
Categories
Books
```

## Deliverable

Approved database design.

---

# Phase 3 — Database Schema

## Goal

Create the SQL Server database schema.

## Tasks

* Create database
* Create Authors
* Create Categories
* Create Books
* Add primary keys
* Add foreign keys
* Add unique constraints
* Add CHECK constraints
* Add indexes
* Verify relationships

## Deliverable

Working database schema.

---

# Phase 4 — Seed/Test Data

## Goal

Insert representative development data.

## Tasks

* Insert authors
* Insert categories
* Insert books
* Verify foreign keys
* Verify status constraints
* Verify ISBN uniqueness

## Deliverable

Useful test dataset.

---

# Phase 5 — Book Stored Procedures

## Goal

Create and test Book Stored Procedures.

## Procedures

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

## Topics

* Parameters
* Result sets
* Returning generated IDs
* Error handling
* Transactions
* SQL Server procedure conventions

## Deliverable

Individually tested Book Stored Procedures.

---

# Phase 6 — Create Clean Architecture Solution

## Goal

Create the .NET solution structure.

## Projects

```text
BookLibrary.API
BookLibrary.Application
BookLibrary.Domain
BookLibrary.Infrastructure
```

## Tasks

* Create solution
* Create projects
* Configure project references
* Verify dependency direction
* Build solution

## Deliverable

Compiling Clean Architecture solution.

---

# Phase 7 — Domain Layer

## Goal

Implement core domain concepts.

## Concepts

```text
Book
Author
Category
BookStatus
```

## Tasks

* Create entities
* Create enum
* Establish appropriate domain relationships
* Add only meaningful domain rules

## Deliverable

Independent Domain project.

---

# Phase 8 — Application Layer

## Goal

Define application use cases and abstractions.

## Commands

```text
CreateBook
UpdateBook
DeleteBook
```

## Queries

```text
GetBooks
GetBookById
SearchBooks
GetBooksByAuthor
GetBooksByCategory
```

## Tasks

* Create DTOs
* Create requests
* Create responses
* Create interfaces
* Define use cases
* Explain Dependency Inversion

## Deliverable

Application layer independent from database implementation.

---

# Phase 9 — EF Core Configuration

## Goal

Configure EF Core for the write side.

## Tasks

* Install compatible EF Core packages
* Configure DbContext
* Configure SQL Server connection
* Configure domain entities where required
* Configure DI
* Verify database connection

## Important

Do not use normal EF Core CRUD as a replacement for the required Stored Procedures.

---

# Phase 10 — EF Core Write Operations

## Goal

Implement:

```text
CREATE
UPDATE
DELETE
```

using EF Core to execute the required Stored Procedures.

## Tasks

* Implement writer abstraction
* Implement Infrastructure writer
* Execute Stored Procedures through EF Core
* Handle generated IDs
* Handle failures
* Test duplicate ISBN behavior
* Test missing related entities

## Deliverable

Working EF Core write side.

---

# Phase 11 — Dapper Configuration

## Goal

Configure Dapper for read operations.

## Topics

* IDbConnection
* SQL Server connection
* Dependency Injection
* Dapper package
* CommandType.StoredProcedure
* Parameters
* Async operations

## Deliverable

Working Dapper infrastructure.

---

# Phase 12 — Dapper Read Operations

## Goal

Implement:

```text
GET ALL
GET BY ID
SEARCH
GET BY AUTHOR
GET BY CATEGORY
```

## Tasks

* Implement reader abstraction
* Implement Dapper reader
* Execute Stored Procedures
* Map results to DTOs
* Handle missing records
* Use async operations

## Deliverable

Working Dapper read side.

---

# Phase 13 — API Integration

## Goal

Connect Application, Infrastructure and API layers.

## Endpoints

```text
GET    /api/books
GET    /api/books/{id}
GET    /api/books/search?term=clean
POST   /api/books
PUT    /api/books/{id}
DELETE /api/books/{id}
```

## Tasks

* Create thin controllers
* Configure DI
* Connect use cases
* Return appropriate HTTP status codes
* Configure Swagger

## Deliverable

Working Book REST API.

---

# Phase 14 — Validation and Error Handling

## Goal

Make the API robust.

## Validation

* Required fields
* ISBN
* Author
* Category
* Published year
* Status
* Duplicate ISBN

## Error handling

```text
400 Bad Request
404 Not Found
409 Conflict
500 Internal Server Error
```

## Tasks

* Global exception handling
* Consistent error responses
* Application validation
* Database constraint handling

## Deliverable

Predictable API error behavior.

---

# Phase 15 — Authors and Categories

## Goal

Implement Authors and Categories functionality.

## Tasks

* Stored Procedures
* Application use cases
* DTOs
* Infrastructure implementations
* Controllers
* CRUD operations

## Deliverable

Complete basic Author and Category management.

---

# Phase 16 — Searching and Filtering

## Goal

Provide useful book discovery functionality.

## Search

```text
GET /api/books/search?term=clean
```

Search:

* Title
* ISBN
* Author name

## Filtering

Example:

```text
GET /api/books?status=Reading
```

## Topics

* LIKE
* Parameters
* JOIN
* SQL injection prevention
* Query performance
* Stored Procedure design

---

# Phase 17 — Pagination

## Goal

Add database-level pagination.

## Example

```text
GET /api/books?page=1&pageSize=10
```

## Response

```text
Items
Page
PageSize
TotalCount
TotalPages
```

## Topics

* SQL Server pagination
* OFFSET
* FETCH
* COUNT
* Performance

Do not load the entire dataset into memory before paginating.

---

# Phase 18 — Transactions

## Goal

Introduce transactions only where they provide meaningful value.

## Topics

* BEGIN TRANSACTION
* COMMIT
* ROLLBACK
* TRY/CATCH
* Atomicity

## Requirement

Demonstrate at least one realistic transaction.

Do not wrap every operation in a transaction unnecessarily.

---

# Phase 19 — Testing

## Goal

Test important behavior.

## Unit Tests

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

## Integration Tests

Test appropriate API behavior end-to-end.

## Philosophy

Do not chase 100% coverage.

Test behavior that matters.

---

# Phase 20 — Architecture Review and Refactoring

## Goal

Review the completed project as a professional codebase.

## Review

### Architecture

* Dependency direction
* Domain independence
* Application abstractions
* Infrastructure responsibilities
* Thin controllers

### Data access

* EF Core writes
* Dapper reads
* Stored Procedure usage
* Parameterization

### Database

* Constraints
* Relationships
* Indexes
* Stored Procedures
* Pagination

### API

* REST conventions
* DTO usage
* Validation
* HTTP status codes
* Error handling

### Code quality

* Naming
* Duplication
* Unnecessary abstractions
* Async operations
* Maintainability

### Testing

* Important behaviors covered
* Unit/integration test boundaries

### Documentation

* README
* Setup instructions
* Architecture explanation
* Database explanation
* API documentation

## Final Deliverable

A complete, understandable and maintainable Book Library Management REST API.
