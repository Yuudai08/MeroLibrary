CREATE OR ALTER PROCEDURE dbo.sp_Category_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT OFF;
    DELETE FROM dbo.Categories WHERE Id = @Id;
END;
GO
