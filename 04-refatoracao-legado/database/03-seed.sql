USE LegadoCartorio;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.tb_cliente)
BEGIN
    INSERT INTO dbo.tb_cliente (nome, cpf, telefone, obs) VALUES
        ('MARIA APARECIDA SOUZA', '529.982.247-25', '11988881111', NULL),
        ('joao carlos pereira',   '11144477735',    '(31) 97777-2222', 'cliente antigo'),
        ('Ana Beatriz Lima',      '123.456.789-09', NULL, '');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.tb_pedido_certidao)
BEGIN
    INSERT INTO dbo.tb_pedido_certidao (id_cliente, tipo, valor, status, dt_pedido) VALUES
        (1, 'Certidao de nascimento', 45.9,  'aberto',   '03/02/2026'),
        (1, 'Certidao de casamento',  45.9,  'PAGO',     '10/02/2026'),
        (2, 'Certidao de obito',      52.35, 'entregue', '15/03/2026'),
        (3, 'certidao de nascimento', 45.9,  'Aberto',   '01/04/2026'),
        (99, 'Certidao de casamento', 45.9,  'aberto',   '02/04/2026');  -- cliente inexistente (sem FK)
END
GO
