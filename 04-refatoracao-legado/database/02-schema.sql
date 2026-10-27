USE LegadoCartorio;
GO

-- Esquema com "cara de legado" de propósito: prefixo tb_, status em texto livre,
-- valor em FLOAT, data em VARCHAR. Parte da refatoração é justificar (ou não) mudar isso.

IF OBJECT_ID(N'dbo.tb_cliente', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tb_cliente (
        id        INT IDENTITY(1,1) PRIMARY KEY,
        nome      VARCHAR(100),
        cpf       VARCHAR(20),     -- às vezes com pontuação, às vezes sem
        telefone  VARCHAR(30),
        obs       VARCHAR(MAX)
    );
END
GO

IF OBJECT_ID(N'dbo.tb_pedido_certidao', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tb_pedido_certidao (
        id          INT IDENTITY(1,1) PRIMARY KEY,
        id_cliente  INT,             -- sem FK
        tipo        VARCHAR(50),
        valor       FLOAT,
        status      VARCHAR(30),     -- 'aberto', 'ABERTO', 'pago', 'entregue'...
        dt_pedido   VARCHAR(20)      -- 'dd/MM/yyyy'
    );
END
GO
