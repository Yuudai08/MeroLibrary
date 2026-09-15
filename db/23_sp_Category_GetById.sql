CREATE OR ALTER PROCEDURE dbo.sp_Category_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name FROM dbo.Categories WHERE Id = @Id;
END;
GO
