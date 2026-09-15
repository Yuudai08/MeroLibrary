-- Phase 5 READ (Dapper calls this).
-- Problem: single book detail. Empty result = 404.
CREATE OR ALTER PROCEDURE dbo.sp_Book_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT b.Id, b.Title, b.ISBN, b.AuthorId, a.Name AS AuthorName,
           b.CategoryId, c.Name AS CategoryName,
           b.PublishedYear, b.Status, b.CreatedAt, b.UpdatedAt
    FROM dbo.Books b
    INNER JOIN dbo.Authors a ON b.AuthorId = a.Id
    INNER JOIN dbo.Categories c ON b.CategoryId = c.Id
    WHERE b.Id = @Id;
END;
GO
