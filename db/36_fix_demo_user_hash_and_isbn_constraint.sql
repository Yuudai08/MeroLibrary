-- Fix 1: Update merouser demo account with verified PBKDF2 hash of Password123!
UPDATE dbo.Users
SET PasswordHash = 'YI7nEbZtH8hAo9n5QNA0sw==:cZib5u4oaAGl9iQTjhrT0sx3mczqkk8ACsI9jePud2E='
WHERE Username = 'merouser';
GO

-- Fix 2: Drop global ISBN unique constraint and recreate it as (UserId, ISBN) unique constraint
-- This allows different users to own the same book while preventing duplicate ISBNs within a single user's library.
IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'UQ_Books_ISBN')
BEGIN
    ALTER TABLE dbo.Books DROP CONSTRAINT UQ_Books_ISBN;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'UQ_Books_UserId_ISBN')
BEGIN
    ALTER TABLE dbo.Books ADD CONSTRAINT UQ_Books_UserId_ISBN UNIQUE NONCLUSTERED (UserId, ISBN);
END;
GO
