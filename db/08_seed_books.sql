-- Phase 4: Seed Books.
-- Resolves AuthorId/CategoryId by Name lookup (no hardcoded IDENTITY values).
-- Covers all statuses: 0=WantToRead, 1=Reading, 2=Completed, 3=Dropped.
-- Includes 3x 'Clean' titles for later search test (?term=clean).
DECLARE @UncleBob INT = (SELECT TOP (1) Id FROM dbo.Authors WHERE Name = N'Robert C. Martin' ORDER BY Id);
DECLARE @Fowler INT = (SELECT TOP (1) Id FROM dbo.Authors WHERE Name = N'Martin Fowler' ORDER BY Id);
DECLARE @Hunt INT = (SELECT TOP (1) Id FROM dbo.Authors WHERE Name = N'Andrew Hunt' ORDER BY Id);
DECLARE @Melville INT = (SELECT TOP (1) Id FROM dbo.Authors WHERE Name = N'Herman Melville' ORDER BY Id);
DECLARE @Austen INT = (SELECT TOP (1) Id FROM dbo.Authors WHERE Name = N'Jane Austen' ORDER BY Id);

DECLARE @Prog INT = (SELECT Id FROM dbo.Categories WHERE Name = N'Programming');
DECLARE @Design INT = (SELECT Id FROM dbo.Categories WHERE Name = N'Software Design');
DECLARE @Fiction INT = (SELECT Id FROM dbo.Categories WHERE Name = N'Fiction');
DECLARE @Classic INT = (SELECT Id FROM dbo.Categories WHERE Name = N'Classic');

INSERT INTO dbo.Books (Title, ISBN, AuthorId, CategoryId, PublishedYear, Status) VALUES
(N'Clean Code', N'9780132350884', @UncleBob, @Prog, 2008, 2),
(N'Clean Architecture', N'9780134494166', @UncleBob, @Design, 2017, 1),
(N'The Clean Coder', N'9780137081073', @UncleBob, @Prog, 2011, 0),
(N'Refactoring', N'9780201485677', @Fowler, @Design, 1999, 2),
(N'Patterns of Enterprise Application Architecture', N'9780321127426', @Fowler, @Design, 2002, 1),
(N'The Pragmatic Programmer', N'9780201616224', @Hunt, @Prog, 1999, 2),
(N'Moby-Dick', N'9780142437247', @Melville, @Classic, 1851, 3),
(N'Pride and Prejudice', N'9780141439518', @Austen, @Fiction, 1813, 2);
GO
