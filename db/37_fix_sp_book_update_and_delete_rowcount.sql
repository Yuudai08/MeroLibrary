-- Fix sp_Book_Update and sp_Book_Delete rowcount reporting
-- Problem: SET NOCOUNT ON suppresses the affected row count in ADO.NET/EF Core ExecuteNonQueryAsync.
-- Fix: Use SET NOCOUNT OFF and remove SELECT @@ROWCOUNT so ExecuteSqlRawAsync returns the exact rows affected (1 on success, 0 on not found).

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
