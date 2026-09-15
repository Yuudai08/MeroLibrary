-- Phase 5 WRITE (EF Core calls this).
-- Problem: update a book by Id, stamp UpdatedAt.
-- SQL concepts: UPDATE...WHERE, SYSUTCDATETIME().
-- Not-found signal: @@ROWCOUNT = 0 (EF ExecuteSql returns 0 -> API maps 404).
-- Single statement = atomic, no explicit transaction (Phase 18 topic).
CREATE OR ALTER PROCEDURE dbo.sp_Book_Update
    @Id INT,
    @Title NVARCHAR(300),
    @ISBN NVARCHAR(20),
    @AuthorId INT,
    @CategoryId INT,
    @PublishedYear INT,
    @Status TINYINT
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
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id;
END;
GO
