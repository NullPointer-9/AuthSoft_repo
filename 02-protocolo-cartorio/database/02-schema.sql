USE CartorioProtocolo;
GO

IF OBJECT_ID(N'dbo.Cliente', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cliente (
        Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Cliente PRIMARY KEY,
        Nome          NVARCHAR(150)  NOT NULL,
        Cpf           CHAR(11)       NOT NULL CONSTRAINT UQ_Cliente_Cpf UNIQUE,  -- só dígitos
        Email         NVARCHAR(150)  NULL,
        Telefone      VARCHAR(20)    NULL,
        DataCadastro  DATETIME2(0)   NOT NULL CONSTRAINT DF_Cliente_DataCadastro DEFAULT SYSDATETIME()
    );
END
GO

IF OBJECT_ID(N'dbo.StatusProtocolo', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.StatusProtocolo (
        Id    TINYINT       NOT NULL CONSTRAINT PK_StatusProtocolo PRIMARY KEY,
        Nome  NVARCHAR(30)  NOT NULL CONSTRAINT UQ_StatusProtocolo_Nome UNIQUE
    );
END
GO

IF OBJECT_ID(N'dbo.Protocolo', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Protocolo (
        Id               INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Protocolo PRIMARY KEY,
        Numero           VARCHAR(20)    NOT NULL CONSTRAINT UQ_Protocolo_Numero UNIQUE,  -- ex.: 2026-000001
        ClienteId        INT            NOT NULL CONSTRAINT FK_Protocolo_Cliente REFERENCES dbo.Cliente (Id),
        TipoServico      NVARCHAR(80)   NOT NULL,
        Descricao        NVARCHAR(500)  NULL,
        StatusId         TINYINT        NOT NULL CONSTRAINT FK_Protocolo_Status REFERENCES dbo.StatusProtocolo (Id),
        DataAbertura     DATETIME2(0)   NOT NULL CONSTRAINT DF_Protocolo_DataAbertura DEFAULT SYSDATETIME(),
        DataAtualizacao  DATETIME2(0)   NOT NULL CONSTRAINT DF_Protocolo_DataAtualizacao DEFAULT SYSDATETIME(),
        DataEntrega      DATETIME2(0)   NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Protocolo_ClienteId' AND object_id = OBJECT_ID(N'dbo.Protocolo'))
    CREATE INDEX IX_Protocolo_ClienteId ON dbo.Protocolo (ClienteId);
GO

IF OBJECT_ID(N'dbo.ProtocoloHistorico', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProtocoloHistorico (
        Id                INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProtocoloHistorico PRIMARY KEY,
        ProtocoloId       INT            NOT NULL CONSTRAINT FK_Historico_Protocolo REFERENCES dbo.Protocolo (Id),
        StatusAnteriorId  TINYINT        NULL     CONSTRAINT FK_Historico_StatusAnterior REFERENCES dbo.StatusProtocolo (Id),
        StatusNovoId      TINYINT        NOT NULL CONSTRAINT FK_Historico_StatusNovo REFERENCES dbo.StatusProtocolo (Id),
        DataAlteracao     DATETIME2(0)   NOT NULL CONSTRAINT DF_Historico_DataAlteracao DEFAULT SYSDATETIME(),
        Observacao        NVARCHAR(300)  NULL
    );
END
GO
