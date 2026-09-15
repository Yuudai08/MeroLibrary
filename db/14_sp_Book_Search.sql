-- Phase 5 READ (Dapper calls this).
-- Problem: search Title, ISBN, Author name.
-- SQL concepts: LIKE with parameters, JOINs.
-- Injection-safe: @Term never concatenated into SQL text,
-- pattern built as parameter value only.
-- Note: leading '%' prevents index seek; fine for this size (Phase 16 topic).
CREATE OR ALTER PROCEDURE dbo.sp_Book_Search
    @Term NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Pattern NVARCHAR(102) = N'%' + @Term + N'%';
    SELECT b.Id, b.Title, b.ISBN, b.AuthorId, a.Name AS AuthorName,
           b.CategoryId, c.Name AS CategoryName,
           b.PublishedYear, b.Status, b.CreatedAt, b.UpdatedAt
    FROM dbo.Books b
    INNER JOIN dbo.Authors a ON b.AuthorId = a.Id
    INNER JOIN dbo.Categories c ON b.CategoryId = c.Id
    WHERE b.Title LIKE @Pattern
       OR b.ISBN LIKE @Pattern
       OR a.Name LIKE @Pattern
    ORDER BY b.Id;
END;
GO
