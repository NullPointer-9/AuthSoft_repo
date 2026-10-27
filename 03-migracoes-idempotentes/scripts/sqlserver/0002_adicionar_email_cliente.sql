IF COL_LENGTH(N'dbo.cliente', N'email') IS NULL
    ALTER TABLE dbo.cliente ADD email NVARCHAR(150) NULL;
