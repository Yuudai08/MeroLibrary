CREATE OR ALTER PROCEDURE dbo.sp_Author_Update
    @Id INT,
    @Name NVARCHAR(200),
    @Biography NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT OFF;
    UPDATE dbo.Authors
    SET Name = @Name,
        Biography = @Biography
    WHERE Id = @Id;
END;
GO
