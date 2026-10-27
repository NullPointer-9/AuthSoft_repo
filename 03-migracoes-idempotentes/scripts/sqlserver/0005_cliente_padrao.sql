-- Migração de dados: só insere se o registro ainda não existir.
IF NOT EXISTS (SELECT 1 FROM dbo.cliente WHERE cpf = '52998224725')
    INSERT INTO dbo.cliente (nome, cpf, email)
    VALUES (N'Cliente de demonstração', '52998224725', N'demo@example.com');
