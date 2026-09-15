-- Phase 5 READ (Dapper calls this).
-- Problem: books by one category. Supports GET /api/categories/{id}/books later.
CREATE OR ALTER PROCEDURE dbo.sp_Book_GetByCategory
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT b.Id, b.Title, b.ISBN, b.AuthorId, a.Name AS AuthorName,
           b.CategoryId, c.Name AS CategoryName,
           b.PublishedYear, b.Status, b.CreatedAt, b.UpdatedAt
    FROM dbo.Books b
    INNER JOIN dbo.Authors a ON b.AuthorId = a.Id
    INNER JOIN dbo.Categories c ON b.CategoryId = c.Id
    WHERE b.CategoryId = @CategoryId
    ORDER BY b.Id;
END;
GO
