-- Migração de dados: ON CONFLICT ignora a inserção se o CPF já existir (usa o UNIQUE uq_cliente_cpf).
INSERT INTO cliente (nome, cpf, email)
VALUES ('Cliente de demonstração', '52998224725', 'demo@example.com')
ON CONFLICT (cpf) DO NOTHING;
