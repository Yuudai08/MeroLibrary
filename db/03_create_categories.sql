-- Phase 3: Categories per Phase 2 approved design.
-- Unique name enforced at DB level, not just app validation.
CREATE TABLE dbo.Categories (
    Id INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_Categories PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Categories_Name UNIQUE NONCLUSTERED (Name)
);
GO
