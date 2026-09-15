-- Phase 3: clean rebuild of incorrect Phase-2-deviation schema.
-- Tables are empty (verified 0 rows), so DROP is safe.
-- Order matters: drop dependent table first.
IF OBJECT_ID(N'dbo.Books', N'U') IS NOT NULL DROP TABLE dbo.Books;
IF OBJECT_ID(N'dbo.Authors', N'U') IS NOT NULL DROP TABLE dbo.Authors;
IF OBJECT_ID(N'dbo.Categories', N'U') IS NOT NULL DROP TABLE dbo.Categories;
GO
