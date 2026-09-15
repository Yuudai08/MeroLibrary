-- Phase 4: Seed Categories.
-- UQ_Categories_Name makes re-runs fail on duplicates (intentional, tests uniqueness).
INSERT INTO dbo.Categories (Name) VALUES
(N'Programming'),
(N'Software Design'),
(N'Fiction'),
(N'Classic');
GO
