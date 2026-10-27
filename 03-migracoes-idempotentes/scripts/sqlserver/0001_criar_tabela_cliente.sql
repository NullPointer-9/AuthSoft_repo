IF OBJECT_ID(N'dbo.cliente', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.cliente (
        id         INT IDENTITY(1,1) NOT NULL CONSTRAINT pk_cliente PRIMARY KEY,
        nome       NVARCHAR(150) NOT NULL,
        cpf        CHAR(11)      NOT NULL CONSTRAINT uq_cliente_cpf UNIQUE,
        criado_em  DATETIME2(0)  NOT NULL CONSTRAINT df_cliente_criado_em DEFAULT SYSDATETIME()
    );
END
