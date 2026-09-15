-- Phase 3: Books per Phase 2 approved design.
-- Named constraints: readable errors, e.g. violation of UQ_Books_ISBN.
-- NO ACTION on deletes (default): cannot delete Author/Category with Books.
CREATE TABLE dbo.Books (
    Id INT IDENTITY(1,1) NOT NULL,
    Title NVARCHAR(300) NOT NULL,
    ISBN NVARCHAR(20) NOT NULL,
    AuthorId INT NOT NULL,
    CategoryId INT NOT NULL,
    PublishedYear INT NOT NULL,
    Status TINYINT NOT NULL CONSTRAINT DF_Books_Status DEFAULT (0),
    CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Books_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt DATETIME2(0) NULL,
    CONSTRAINT PK_Books PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Books_ISBN UNIQUE NONCLUSTERED (ISBN),
    CONSTRAINT FK_Books_Authors FOREIGN KEY (AuthorId) REFERENCES dbo.Authors (Id),
    CONSTRAINT FK_Books_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories (Id),
    CONSTRAINT CK_Books_Status CHECK (Status IN (0, 1, 2, 3)),
    CONSTRAINT CK_Books_PublishedYear CHECK (PublishedYear BETWEEN 1400 AND 2100)
);
GO
