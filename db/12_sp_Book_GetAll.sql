-- Phase 16 READ (Dapper calls this).
-- Problem: list all books with optional filtering by Status, AuthorId, and CategoryId.
-- SQL concepts: Optional parameters, IS NULL logic, JOINs, performance indexing.
CREATE OR ALTER PROCEDURE dbo.sp_Book_GetAll
    @Status TINYINT = NULL,
    @AuthorId INT = NULL,
    @CategoryId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT b.Id, b.Title, b.ISBN, b.AuthorId, a.Name AS AuthorName,
           b.CategoryId, c.Name AS CategoryName,
           b.PublishedYear, b.Status, b.CreatedAt, b.UpdatedAt
    FROM dbo.Books b
    INNER JOIN dbo.Authors a ON b.AuthorId = a.Id
    INNER JOIN dbo.Categories c ON b.CategoryId = c.Id
    WHERE (@Status IS NULL OR b.Status = @Status)
      AND (@AuthorId IS NULL OR b.AuthorId = @AuthorId)
      AND (@CategoryId IS NULL OR b.CategoryId = @CategoryId)
    ORDER BY b.Id;
END;
GO
