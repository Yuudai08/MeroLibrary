-- Phase 3: Authors per Phase 2 approved design.
-- No CreatedAt/UpdatedAt by intentional requirement.
CREATE TABLE dbo.Authors (
    Id INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Biography NVARCHAR(MAX) NULL,
    CONSTRAINT PK_Authors PRIMARY KEY CLUSTERED (Id)
);
GO
