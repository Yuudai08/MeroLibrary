-- Phase 4: Seed Authors.
-- Id is IDENTITY, so we insert Name/Biography only.
-- Names are distinct in this dataset (no UNIQUE constraint by design).
INSERT INTO dbo.Authors (Name, Biography) VALUES
(N'Robert C. Martin', N'Software engineer known for Clean Code and SOLID principles.'),
(N'Martin Fowler', N'Software engineer known for Refactoring and enterprise patterns.'),
(N'Andrew Hunt', N'Co-author of The Pragmatic Programmer.'),
(N'Herman Melville', N'American novelist known for Moby-Dick.'),
(N'Jane Austen', N'English novelist known for Pride and Prejudice.');
GO
