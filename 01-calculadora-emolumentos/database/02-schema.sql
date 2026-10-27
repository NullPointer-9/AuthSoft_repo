USE Emolumentos;
GO

IF OBJECT_ID(N'dbo.Uf', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Uf (
        Sigla           CHAR(2)        NOT NULL CONSTRAINT PK_Uf PRIMARY KEY,
        Nome            NVARCHAR(60)   NOT NULL,
        FonteTabela     NVARCHAR(200)  NULL,   -- ex.: "Lei estadual X / Provimento do TJ"
        VigenciaTabela  DATE           NULL
    );
END
GO

IF OBJECT_ID(N'dbo.TipoAto', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TipoAto (
        Id         INT            NOT NULL CONSTRAINT PK_TipoAto PRIMARY KEY,
        Codigo     VARCHAR(30)    NOT NULL CONSTRAINT UQ_TipoAto_Codigo UNIQUE,
        Descricao  NVARCHAR(120)  NOT NULL,
        ExigeValor BIT            NOT NULL CONSTRAINT DF_TipoAto_ExigeValor DEFAULT 1  -- 0 = ato sem valor declarado
    );
END
GO

-- Uma linha por faixa de valor. ValorDe e ValorAte são inclusivos;
-- ValorAte NULL significa "acima de ValorDe, sem teto".
-- Atos sem valor declarado (procuração, firma) têm uma única faixa 0..NULL.
IF OBJECT_ID(N'dbo.FaixaEmolumento', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FaixaEmolumento (
        Id          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_FaixaEmolumento PRIMARY KEY,
        UfSigla     CHAR(2)        NOT NULL CONSTRAINT FK_Faixa_Uf REFERENCES dbo.Uf (Sigla),
        TipoAtoId   INT            NOT NULL CONSTRAINT FK_Faixa_TipoAto REFERENCES dbo.TipoAto (Id),
        ValorDe     DECIMAL(18,2)  NOT NULL,
        ValorAte    DECIMAL(18,2)  NULL,
        Emolumento  DECIMAL(18,2)  NOT NULL,
        Custas      DECIMAL(18,2)  NOT NULL,
        CONSTRAINT UQ_Faixa UNIQUE (UfSigla, TipoAtoId, ValorDe),
        CONSTRAINT CK_Faixa_Intervalo CHECK (ValorAte IS NULL OR ValorAte > ValorDe),
        CONSTRAINT CK_Faixa_Valores CHECK (ValorDe >= 0 AND Emolumento >= 0 AND Custas >= 0)
    );
END
GO
