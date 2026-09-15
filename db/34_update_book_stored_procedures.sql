-- Phase: Update all Book Stored Procedures for Multi-User Isolation and BookNumber
-- Problem: Scope all book queries/commands to a specific UserId and support optional BookNumber.

-- 1. sp_Book_Create
CREATE OR ALTER PROCEDURE dbo.sp_Book_Create
    @Title NVARCHAR(300),
    @ISBN NVARCHAR(20),
    @AuthorId INT,
    @CategoryId INT,
    @PublishedYear INT,
    @Status TINYINT,
    @UserId INT,
    @BookNumber INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.Books (Title, ISBN, AuthorId, CategoryId, PublishedYear, Status, UserId, BookNumber)
        VALUES (@Title, @ISBN, @AuthorId, @CategoryId, @PublishedYear, @Status, @UserId, @BookNumber);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- 2. sp_Book_Update
CREATE OR ALTER PROCEDURE dbo.sp_Book_Update
    @Id INT,
    @Title NVARCHAR(300),
    @ISBN NVARCHAR(20),
    @AuthorId INT,
    @CategoryId INT,
    @PublishedYear INT,
    @Status TINYINT,
    @UserId INT,
    @BookNumber INT = NULL
AS
BEGIN
    SET NOCOUNT OFF;
    UPDATE dbo.Books
    SET Title = @Title,
        ISBN = @ISBN,
        AuthorId = @AuthorId,
        CategoryId = @CategoryId,
        PublishedYear = @PublishedYear,
        Status = @Status,
        BookNumber = @BookNumber,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id AND UserId = @UserId;
END;
GO

-- 3. sp_Book_Delete
CREATE OR ALTER PROCEDURE dbo.sp_Book_Delete
    @Id INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT OFF;
    DELETE FROM dbo.Books
    WHERE Id = @Id AND UserId = @UserId;
END;
GO

-- 4. sp_Book_GetById
CREATE OR ALTER PROCEDURE dbo.sp_Book_GetById
    @Id INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT b.Id, b.Title, b.ISBN, b.AuthorId, a.Name AS AuthorName,
           b.CategoryId, c.Name AS CategoryName,
           b.PublishedYear, b.Status, b.UserId, b.BookNumber,
           b.CreatedAt, b.UpdatedAt
    FROM dbo.Books b
    INNER JOIN dbo.Authors a ON b.AuthorId = a.Id
    INNER JOIN dbo.Categories c ON b.CategoryId = c.Id
    WHERE b.Id = @Id AND b.UserId = @UserId;
END;
GO

-- 5. sp_Book_GetAll
CREATE OR ALTER PROCEDURE dbo.sp_Book_GetAll
    @UserId INT,
    @Status TINYINT = NULL,
    @AuthorId INT = NULL,
    @CategoryId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT b.Id, b.Title, b.ISBN, b.AuthorId, a.Name AS AuthorName,
           b.CategoryId, c.Name AS CategoryName,
           b.PublishedYear, b.Status, b.UserId, b.BookNumber,
           b.CreatedAt, b.UpdatedAt
    FROM dbo.Books b
    INNER JOIN dbo.Authors a ON b.AuthorId = a.Id
    INNER JOIN dbo.Categories c ON b.CategoryId = c.Id
    WHERE b.UserId = @UserId
      AND (@Status IS NULL OR b.Status = @Status)
      AND (@AuthorId IS NULL OR b.AuthorId = @AuthorId)
      AND (@CategoryId IS NULL OR b.CategoryId = @CategoryId)
    ORDER BY b.Id DESC;
END;
GO

-- 6. sp_Book_GetPaged
CREATE OR ALTER PROCEDURE dbo.sp_Book_GetPaged
    @UserId INT,
    @Status TINYINT = NULL,
    @AuthorId INT = NULL,
    @CategoryId INT = NULL,
    @Page INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@Page - 1) * @PageSize;

    -- Result Set 1: Total count matching filter criteria for this user
    SELECT COUNT(*) AS TotalCount
    FROM dbo.Books b
    WHERE b.UserId = @UserId
      AND (@Status IS NULL OR b.Status = @Status)
      AND (@AuthorId IS NULL OR b.AuthorId = @AuthorId)
      AND (@CategoryId IS NULL OR b.CategoryId = @CategoryId);

    -- Result Set 2: Paged items
    SELECT b.Id, b.Title, b.ISBN, b.AuthorId, a.Name AS AuthorName,
           b.CategoryId, c.Name AS CategoryName,
           b.PublishedYear, b.Status, b.UserId, b.BookNumber,
           b.CreatedAt, b.UpdatedAt
    FROM dbo.Books b
    INNER JOIN dbo.Authors a ON b.AuthorId = a.Id
    INNER JOIN dbo.Categories c ON b.CategoryId = c.Id
    WHERE b.UserId = @UserId
      AND (@Status IS NULL OR b.Status = @Status)
      AND (@AuthorId IS NULL OR b.AuthorId = @AuthorId)
      AND (@CategoryId IS NULL OR b.CategoryId = @CategoryId)
    ORDER BY b.Id DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- 7. sp_Book_Search
CREATE OR ALTER PROCEDURE dbo.sp_Book_Search
    @UserId INT,
    @Term NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Pattern NVARCHAR(102) = '%' + @Term + '%';

    SELECT b.Id, b.Title, b.ISBN, b.AuthorId, a.Name AS AuthorName,
           b.CategoryId, c.Name AS CategoryName,
           b.PublishedYear, b.Status, b.UserId, b.BookNumber,
           b.CreatedAt, b.UpdatedAt
    FROM dbo.Books b
    INNER JOIN dbo.Authors a ON b.AuthorId = a.Id
    INNER JOIN dbo.Categories c ON b.CategoryId = c.Id
    WHERE b.UserId = @UserId
      AND (
          b.Title LIKE @Pattern
          OR b.ISBN LIKE @Pattern
          OR a.Name LIKE @Pattern
      )
    ORDER BY b.Id DESC;
END;
GO

-- 8. sp_Book_GetByAuthor
CREATE OR ALTER PROCEDURE dbo.sp_Book_GetByAuthor
    @UserId INT,
    @AuthorId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT b.Id, b.Title, b.ISBN, b.AuthorId, a.Name AS AuthorName,
           b.CategoryId, c.Name AS CategoryName,
           b.PublishedYear, b.Status, b.UserId, b.BookNumber,
           b.CreatedAt, b.UpdatedAt
    FROM dbo.Books b
    INNER JOIN dbo.Authors a ON b.AuthorId = a.Id
    INNER JOIN dbo.Categories c ON b.CategoryId = c.Id
    WHERE b.UserId = @UserId AND b.AuthorId = @AuthorId
    ORDER BY b.Id DESC;
END;
GO

-- 9. sp_Book_GetByCategory
CREATE OR ALTER PROCEDURE dbo.sp_Book_GetByCategory
    @UserId INT,
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT b.Id, b.Title, b.ISBN, b.AuthorId, a.Name AS AuthorName,
           b.CategoryId, c.Name AS CategoryName,
           b.PublishedYear, b.Status, b.UserId, b.BookNumber,
           b.CreatedAt, b.UpdatedAt
    FROM dbo.Books b
    INNER JOIN dbo.Authors a ON b.AuthorId = a.Id
    INNER JOIN dbo.Categories c ON b.CategoryId = c.Id
    WHERE b.UserId = @UserId AND b.CategoryId = @CategoryId
    ORDER BY b.Id DESC;
END;
GO
