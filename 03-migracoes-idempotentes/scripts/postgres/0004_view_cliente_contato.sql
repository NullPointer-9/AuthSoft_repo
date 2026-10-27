-- CREATE OR REPLACE já é idempotente: cria ou substitui a view.
CREATE OR REPLACE VIEW vw_cliente_contato AS
SELECT id, nome, email
FROM cliente
WHERE email IS NOT NULL;
