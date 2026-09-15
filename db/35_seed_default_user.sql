-- Phase: Seed default user if not exists
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'merouser')
BEGIN
    INSERT INTO dbo.Users (Username, Email, PasswordHash)
    VALUES ('merouser', 'merouser@merolibrary.local', 'DefaultSeedUserHash');
END;
GO
