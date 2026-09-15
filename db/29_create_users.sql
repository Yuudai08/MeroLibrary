-- Phase: MeroLibrary Users table
-- Purpose: Store registered user accounts for multi-user library isolation.
CREATE TABLE dbo.Users (
    Id INT IDENTITY(1,1) NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Users_Username UNIQUE NONCLUSTERED (Username),
    CONSTRAINT UQ_Users_Email UNIQUE NONCLUSTERED (Email)
);
GO
