CREATE OR ALTER PROCEDURE dbo.sp_Category_Update
    @Id INT,
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT OFF;
    UPDATE dbo.Categories
    SET Name = @Name
    WHERE Id = @Id;
END;
GO
