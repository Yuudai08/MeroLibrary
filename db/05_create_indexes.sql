-- Phase 3: FK indexes for JOINs and GetByAuthor/GetByCategory.
-- Unique constraints already index ISBN and Category.Name.
CREATE NONCLUSTERED INDEX IX_Books_AuthorId ON dbo.Books (AuthorId);
GO
CREATE NONCLUSTERED INDEX IX_Books_CategoryId ON dbo.Books (CategoryId);
GO
