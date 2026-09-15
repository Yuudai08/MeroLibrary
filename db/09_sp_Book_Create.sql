-- Phase 5 WRITE (EF Core calls this).
-- Problem: insert a book, return new IDENTITY value.
-- SQL concepts: INSERT, SCOPE_IDENTITY(), SET NOCOUNT ON.
-- Errors (UQ ISBN, FK, CHECK) bubble to EF Core for 409/400 mapping.
CREATE OR ALTER PROCEDURE dbo.sp_Book_Create
    @Title NVARCHAR(300),
    @ISBN NVARCHAR(20),
    @AuthorId INT,
    @CategoryId INT,
    @PublishedYear INT,
    @Status TINYINT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.Books (Title, ISBN, AuthorId, CategoryId, PublishedYear, Status)
        VALUES (@Title, @ISBN, @AuthorId, @CategoryId, @PublishedYear, @Status);
        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO
