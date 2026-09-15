CREATE OR ALTER PROCEDURE dbo.sp_Author_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Biography FROM dbo.Authors WHERE Id = @Id;
END;
GO
