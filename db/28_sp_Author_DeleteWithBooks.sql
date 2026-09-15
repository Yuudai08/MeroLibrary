-- Phase 18 TRANSACTION (EF Core calls this).
-- Problem: delete an author and all their associated books atomically.
-- SQL concepts: BEGIN TRANSACTION, COMMIT, ROLLBACK, TRY/CATCH, atomicity.
CREATE OR ALTER PROCEDURE dbo.sp_Author_DeleteWithBooks
    @AuthorId INT
AS
BEGIN
    SET NOCOUNT OFF;

    BEGIN TRANSACTION;

    BEGIN TRY
        -- 1. Delete all books written by this author
        DELETE FROM dbo.Books
        WHERE AuthorId = @AuthorId;

        -- 2. Delete the author
        DELETE FROM dbo.Authors
        WHERE Id = @AuthorId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO
