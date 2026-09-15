-- Phase 5 WRITE (EF Core calls this).
-- Problem: delete a book by Id.
-- SQL concepts: DELETE...WHERE.
-- Not-found signal: @@ROWCOUNT = 0 -> 404.
CREATE OR ALTER PROCEDURE dbo.sp_Book_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT OFF;
    DELETE FROM dbo.Books WHERE Id = @Id;
END;
GO
