-- CREATE VIEW precisa ser o primeiro comando do lote; o GO separa os lotes.
-- CREATE OR ALTER (SQL Server 2016 SP1+) já é idempotente: cria ou substitui.
IF OBJECT_ID(N'dbo.vw_cliente_contato', N'V') IS NOT NULL
    PRINT 'vw_cliente_contato já existe e será atualizada.';
GO

CREATE OR ALTER VIEW dbo.vw_cliente_contato
AS
SELECT id, nome, email
FROM dbo.cliente
WHERE email IS NOT NULL;
GO
