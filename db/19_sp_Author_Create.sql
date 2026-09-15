CREATE OR ALTER PROCEDURE dbo.sp_Author_Create
    @Name NVARCHAR(200),
    @Biography NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Authors (Name, Biography)
    VALUES (@Name, @Biography);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END;
GO
