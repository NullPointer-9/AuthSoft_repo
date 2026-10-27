#!/bin/bash
set -euo pipefail

SQLCMD=/opt/mssql-tools18/bin/sqlcmd

for arquivo in /scripts/*.sql; do
  echo ">> Executando $(basename "$arquivo")"
  "$SQLCMD" -S "${DB_HOST:-sqlserver}" -U sa -P "$MSSQL_SA_PASSWORD" -C -b -f 65001 -i "$arquivo"
done

echo ">> Banco inicializado."
