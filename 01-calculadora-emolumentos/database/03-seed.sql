USE Emolumentos;
GO

-- ATENÇÃO: valores ILUSTRATIVOS e simplificados, só para desenvolvimento.
-- Troque pelas tabelas oficiais publicadas pelos tribunais de cada UF
-- e preencha FonteTabela / VigenciaTabela com a referência real.

MERGE dbo.Uf AS alvo
USING (VALUES
    ('SP', N'São Paulo',       N'Tabela simplificada (substituir pela oficial)', NULL),
    ('MG', N'Minas Gerais',    N'Tabela simplificada (substituir pela oficial)', NULL),
    ('PR', N'Paraná',          N'Tabela simplificada (substituir pela oficial)', NULL),
    ('RJ', N'Rio de Janeiro',  NULL, NULL)   -- de propósito sem faixas: caso "UF sem tabela"
) AS origem (Sigla, Nome, FonteTabela, VigenciaTabela)
ON alvo.Sigla = origem.Sigla
WHEN NOT MATCHED THEN
    INSERT (Sigla, Nome, FonteTabela, VigenciaTabela)
    VALUES (origem.Sigla, origem.Nome, origem.FonteTabela, origem.VigenciaTabela);
GO

MERGE dbo.TipoAto AS alvo
USING (VALUES
    (1, 'ESCRITURA',            N'Escritura pública com valor declarado', 1),
    (2, 'PROCURACAO',           N'Procuração',                             0),
    (3, 'RECONHECIMENTO_FIRMA', N'Reconhecimento de firma por assinatura', 0)
) AS origem (Id, Codigo, Descricao, ExigeValor)
ON alvo.Id = origem.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Codigo, Descricao, ExigeValor) VALUES (origem.Id, origem.Codigo, origem.Descricao, origem.ExigeValor);
GO

MERGE dbo.FaixaEmolumento AS alvo
USING (
    SELECT v.UfSigla,
           t.Id AS TipoAtoId,
           CAST(v.ValorDe    AS DECIMAL(18,2)) AS ValorDe,
           CAST(v.ValorAte   AS DECIMAL(18,2)) AS ValorAte,
           CAST(v.Emolumento AS DECIMAL(18,2)) AS Emolumento,
           CAST(v.Custas     AS DECIMAL(18,2)) AS Custas
    FROM (VALUES
        -- SP
        ('SP', 'ESCRITURA',                 0.00,  20000.00, 1200.00,  380.00),
        ('SP', 'ESCRITURA',             20000.01, 100000.00, 2400.00,  760.00),
        ('SP', 'ESCRITURA',            100000.01, 500000.00, 4100.00, 1300.00),
        ('SP', 'ESCRITURA',            500000.01,      NULL, 6800.00, 2150.00),
        ('SP', 'PROCURACAO',                0.00,      NULL,  210.00,   66.00),
        ('SP', 'RECONHECIMENTO_FIRMA',      0.00,      NULL,    9.50,    3.00),
        -- MG
        ('MG', 'ESCRITURA',                 0.00,  30000.00, 1050.00,  310.00),
        ('MG', 'ESCRITURA',             30000.01, 150000.00, 2250.00,  660.00),
        ('MG', 'ESCRITURA',            150000.01,      NULL, 5300.00, 1560.00),
        ('MG', 'PROCURACAO',                0.00,      NULL,  180.00,   53.00),
        ('MG', 'RECONHECIMENTO_FIRMA',      0.00,      NULL,    7.80,    2.30),
        -- PR
        ('PR', 'ESCRITURA',                 0.00,  25000.00,  980.00,  250.00),
        ('PR', 'ESCRITURA',             25000.01, 200000.00, 2600.00,  670.00),
        ('PR', 'ESCRITURA',            200000.01,      NULL, 5900.00, 1520.00),
        ('PR', 'PROCURACAO',                0.00,      NULL,  160.00,   41.00),
        ('PR', 'RECONHECIMENTO_FIRMA',      0.00,      NULL,    6.90,    1.80)
    ) AS v (UfSigla, CodigoAto, ValorDe, ValorAte, Emolumento, Custas)
    JOIN dbo.TipoAto t ON t.Codigo = v.CodigoAto
) AS origem
ON  alvo.UfSigla   = origem.UfSigla
AND alvo.TipoAtoId = origem.TipoAtoId
AND alvo.ValorDe   = origem.ValorDe
WHEN MATCHED THEN
    UPDATE SET ValorAte   = origem.ValorAte,
               Emolumento = origem.Emolumento,
               Custas     = origem.Custas
WHEN NOT MATCHED THEN
    INSERT (UfSigla, TipoAtoId, ValorDe, ValorAte, Emolumento, Custas)
    VALUES (origem.UfSigla, origem.TipoAtoId, origem.ValorDe, origem.ValorAte, origem.Emolumento, origem.Custas);
GO
