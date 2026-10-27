IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'ix_cliente_nome' AND object_id = OBJECT_ID(N'dbo.cliente')
)
    CREATE INDEX ix_cliente_nome ON dbo.cliente (nome);
