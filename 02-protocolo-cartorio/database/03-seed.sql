USE CartorioProtocolo;
GO

MERGE dbo.StatusProtocolo AS alvo
USING (VALUES
    (1, N'Recebido'),
    (2, N'Em análise'),
    (3, N'Pronto'),
    (4, N'Entregue')
) AS origem (Id, Nome)
ON alvo.Id = origem.Id
WHEN NOT MATCHED THEN INSERT (Id, Nome) VALUES (origem.Id, origem.Nome);
GO

-- CPFs fictícios, mas com dígitos verificadores válidos (de uso comum em testes).
MERGE dbo.Cliente AS alvo
USING (VALUES
    (N'Maria Aparecida Souza',  '52998224725', N'maria.souza@example.com',  '(11) 98888-1111'),
    (N'João Carlos Pereira',    '11144477735', N'joao.pereira@example.com', '(31) 97777-2222'),
    (N'Ana Beatriz Lima',       '12345678909', NULL,                        '(41) 96666-3333')
) AS origem (Nome, Cpf, Email, Telefone)
ON alvo.Cpf = origem.Cpf
WHEN NOT MATCHED THEN
    INSERT (Nome, Cpf, Email, Telefone) VALUES (origem.Nome, origem.Cpf, origem.Email, origem.Telefone);
GO

MERGE dbo.Protocolo AS alvo
USING (
    SELECT v.Numero, c.Id AS ClienteId, v.TipoServico, v.Descricao, v.StatusId
    FROM (VALUES
        ('2026-000001', '52998224725', N'Escritura de compra e venda', N'Imóvel residencial', 2),
        ('2026-000002', '11144477735', N'Procuração',                  N'Procuração para venda de veículo', 3),
        ('2026-000003', '12345678909', N'Reconhecimento de firma',     NULL, 1)
    ) AS v (Numero, Cpf, TipoServico, Descricao, StatusId)
    JOIN dbo.Cliente c ON c.Cpf = v.Cpf
) AS origem
ON alvo.Numero = origem.Numero
WHEN NOT MATCHED THEN
    INSERT (Numero, ClienteId, TipoServico, Descricao, StatusId)
    VALUES (origem.Numero, origem.ClienteId, origem.TipoServico, origem.Descricao, origem.StatusId);
GO

-- Histórico inicial: só para protocolos que ainda não têm nenhum registro.
INSERT INTO dbo.ProtocoloHistorico (ProtocoloId, StatusAnteriorId, StatusNovoId, Observacao)
SELECT p.Id, NULL, 1, N'Protocolo aberto (seed)'
FROM dbo.Protocolo p
WHERE NOT EXISTS (SELECT 1 FROM dbo.ProtocoloHistorico h WHERE h.ProtocoloId = p.Id);

INSERT INTO dbo.ProtocoloHistorico (ProtocoloId, StatusAnteriorId, StatusNovoId, Observacao)
SELECT p.Id, 1, 2, N'Encaminhado para análise (seed)'
FROM dbo.Protocolo p
WHERE p.StatusId >= 2
  AND NOT EXISTS (SELECT 1 FROM dbo.ProtocoloHistorico h WHERE h.ProtocoloId = p.Id AND h.StatusNovoId = 2);

INSERT INTO dbo.ProtocoloHistorico (ProtocoloId, StatusAnteriorId, StatusNovoId, Observacao)
SELECT p.Id, 2, 3, N'Documento pronto (seed)'
FROM dbo.Protocolo p
WHERE p.StatusId >= 3
  AND NOT EXISTS (SELECT 1 FROM dbo.ProtocoloHistorico h WHERE h.ProtocoloId = p.Id AND h.StatusNovoId = 3);
GO
