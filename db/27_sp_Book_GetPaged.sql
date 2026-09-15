-- Phase 17 READ (Dapper calls this).
-- Problem: database-level pagination with total count and filtering.
-- SQL concepts: OFFSET, FETCH NEXT, COUNT(*), multi-result set reading.
CREATE OR ALTER PROCEDURE dbo.sp_Book_GetPaged
    @Status TINYINT = NULL,
    @AuthorId INT = NULL,
    @CategoryId INT = NULL,
    @Page INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@Page - 1) * @PageSize;

    -- Result Set 1: Total count matching filter criteria
    SELECT COUNT(*) AS TotalCount
    FROM dbo.Books b
    WHERE (@Status IS NULL OR b.Status = @Status)
      AND (@AuthorId IS NULL OR b.AuthorId = @AuthorId)
      AND (@CategoryId IS NULL OR b.CategoryId = @CategoryId);

    -- Result Set 2: Paged items
    SELECT b.Id, b.Title, b.ISBN, b.AuthorId, a.Name AS AuthorName,
           b.CategoryId, c.Name AS CategoryName,
           b.PublishedYear, b.Status, b.CreatedAt, b.UpdatedAt
    FROM dbo.Books b
    INNER JOIN dbo.Authors a ON b.AuthorId = a.Id
    INNER JOIN dbo.Categories c ON b.CategoryId = c.Id
    WHERE (@Status IS NULL OR b.Status = @Status)
      AND (@AuthorId IS NULL OR b.AuthorId = @AuthorId)
      AND (@CategoryId IS NULL OR b.CategoryId = @CategoryId)
    ORDER BY b.Id
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO
