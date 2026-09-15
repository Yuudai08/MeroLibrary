-- Phase: Alter Books to add UserId and optional BookNumber
-- Problem: Associate books with their owners (UserId) for personal library isolation,
--          and allow tracking an optional book number for books being read / in a series.

-- 1. Ensure seed user exists so existing books have an owner
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'merouser')
BEGIN
    INSERT INTO dbo.Users (Username, Email, PasswordHash)
    VALUES ('merouser', 'merouser@merolibrary.local', 'AQAAAAIAAYagAAAAEG3c/kL927Z5...DefaultSeedHashPlaceholder');
END;
GO

-- 2. Add UserId to Books if not present
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Books') AND name = 'UserId')
BEGIN
    ALTER TABLE dbo.Books ADD UserId INT NULL;
END;
GO

-- 3. Populate existing books with default user
DECLARE @DefaultUserId INT;
SELECT TOP 1 @DefaultUserId = Id FROM dbo.Users WHERE Username = 'merouser';
UPDATE dbo.Books SET UserId = @DefaultUserId WHERE UserId IS NULL;
GO

-- 4. Enforce NOT NULL and add foreign key
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Books') AND name = 'UserId' AND is_nullable = 1)
BEGIN
    ALTER TABLE dbo.Books ALTER COLUMN UserId INT NOT NULL;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Books_Users')
BEGIN
    ALTER TABLE dbo.Books ADD CONSTRAINT FK_Books_Users FOREIGN KEY (UserId) REFERENCES dbo.Users (Id) ON DELETE CASCADE;
END;
GO

-- 5. Add BookNumber to Books if not present
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Books') AND name = 'BookNumber')
BEGIN
    ALTER TABLE dbo.Books ADD BookNumber INT NULL;
END;
GO
