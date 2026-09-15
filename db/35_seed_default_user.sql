-- Phase: Seed default user if not exists
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'merouser')
BEGIN
    INSERT INTO dbo.Users (Username, Email, PasswordHash)
    VALUES ('merouser', 'merouser@merolibrary.local', 'YI7nEbZtH8hAo9n5QNA0sw==:cZib5u4oaAGl9iQTjhrT0sx3mczqkk8ACsI9jePud2E=');
END;
GO
