-- Phase: User Registration Stored Procedure (EF Core writes)
-- Problem: Securely register a user and return newly generated Id.
-- Concepts: INSERT, SCOPE_IDENTITY(), SET NOCOUNT ON, TRY/CATCH.
CREATE OR ALTER PROCEDURE dbo.sp_User_Create
    @Username NVARCHAR(50),
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.Users (Username, Email, PasswordHash)
        VALUES (@Username, @Email, @PasswordHash);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO
