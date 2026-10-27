Executáveis compilados em Release dos quatro projetos.

| Arquivo | Conteúdo |
|---|---|
| `calculadora-emolumentos-*.zip` | Calculadora de emolumentos (WinForms) |
| `protocolo-cartorio-*.zip` | Protocolo de cartório (WinForms) |
| `migrador-sql-*.zip` | Migrador de scripts SQL (console), com a pasta `scripts` de exemplo |
| `balcao-certidoes-*.zip` | Balcão de certidões, versão refatorada (WinForms) |

Requisitos: Windows com .NET Framework 4.8.

Os aplicativos usam os bancos de dados do repositório. Antes de abrir, suba o banco do projeto correspondente com `docker compose up -d`, na pasta do projeto. A connection string fica no arquivo `.exe.config` de cada aplicativo.

Os PDFs são assinados com um certificado autoassinado de teste, criado na primeira execução.
