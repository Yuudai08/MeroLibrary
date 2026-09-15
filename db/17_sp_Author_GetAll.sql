CREATE OR ALTER PROCEDURE dbo.sp_Author_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Biography FROM dbo.Authors ORDER BY Id;
END;
GO
