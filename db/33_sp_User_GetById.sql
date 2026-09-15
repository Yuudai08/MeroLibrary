-- Phase: User Lookup by Id Stored Procedure (Dapper reads)
-- Problem: Retrieve public user profile by user Id.
CREATE OR ALTER PROCEDURE dbo.sp_User_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Username, Email, CreatedAt
    FROM dbo.Users
    WHERE Id = @Id;
END;
GO
