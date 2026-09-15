-- Phase: User Lookup by Username Stored Procedure (Dapper reads)
-- Problem: Retrieve user credentials and profile for login verification.
CREATE OR ALTER PROCEDURE dbo.sp_User_GetByUsername
    @Username NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Username, Email, PasswordHash, CreatedAt
    FROM dbo.Users
    WHERE Username = @Username;
END;
GO
