<#
.SYNOPSIS
    Prova que cada script pode rodar duas vezes sem erro, nos dois bancos, sem passar pelo migrador.

.DESCRIPTION
    Cria um banco temporário em cada servidor (containers do docker-compose), roda cada script
    DUAS vezes direto no sqlcmd / psql e apaga o banco no final. Se algum script não for
    idempotente, a segunda execução falha e o script para com código de saída 1.

    Uso (na raiz do projeto, com os containers no ar):
        docker compose up -d
        .\tools\verificar-idempotencia.ps1
#>
# Continue (e não Stop): sqlcmd e psql escrevem avisos no stderr, e no Windows PowerShell isso
# viraria erro. Quem decide se falhou é o código de saída ($LASTEXITCODE).
$ErrorActionPreference = 'Continue'

$senha = 'Cartorio@2026'
$bancoTeste = 'idempotencia_teste'
$falhas = 0

function Rodar([string]$descricao, [scriptblock]$comando) {
    $saida = & $comando 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  FALHOU  $descricao" -ForegroundColor Red
        $saida | ForEach-Object { Write-Host "          $_" -ForegroundColor DarkRed }
        $script:falhas++
    } else {
        Write-Host "  ok      $descricao" -ForegroundColor Green
    }
}

function SqlServer([string[]]$argumentos) {
    docker exec migracoes-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $senha -C -b @argumentos
}

function Postgres([string[]]$argumentos) {
    # Esconde os NOTICE do tipo "relation already exists, skipping": são esperados na 2ª execução.
    docker exec -e PGOPTIONS=--client-min-messages=warning migracoes-postgres psql -U postgres -v ON_ERROR_STOP=1 -q @argumentos
}

Write-Host "SQL Server" -ForegroundColor Cyan
SqlServer @('-Q', "IF DB_ID('$bancoTeste') IS NOT NULL DROP DATABASE $bancoTeste; CREATE DATABASE $bancoTeste;") | Out-Null
foreach ($arquivo in Get-ChildItem "$PSScriptRoot\..\scripts\sqlserver\*.sql" | Sort-Object Name) {
    foreach ($vez in 1, 2) {
        Rodar "$($arquivo.Name) (execução $vez)" { SqlServer @('-d', $bancoTeste, '-i', "/migracoes/sqlserver/$($arquivo.Name)") }
    }
}
SqlServer @('-Q', "DROP DATABASE $bancoTeste;") | Out-Null

Write-Host "PostgreSQL" -ForegroundColor Cyan
Postgres @('-d', 'postgres', '-c', "DROP DATABASE IF EXISTS $bancoTeste;") | Out-Null
Postgres @('-d', 'postgres', '-c', "CREATE DATABASE $bancoTeste;") | Out-Null
foreach ($arquivo in Get-ChildItem "$PSScriptRoot\..\scripts\postgres\*.sql" | Sort-Object Name) {
    foreach ($vez in 1, 2) {
        Rodar "$($arquivo.Name) (execução $vez)" { Postgres @('-d', $bancoTeste, '-f', "/migracoes/postgres/$($arquivo.Name)") }
    }
}
Postgres @('-d', 'postgres', '-c', "DROP DATABASE $bancoTeste;") | Out-Null

if ($falhas -gt 0) {
    Write-Host "$falhas execução(ões) falharam: há script que não é idempotente." -ForegroundColor Red
    exit 1
}
Write-Host "Todos os scripts rodaram duas vezes sem erro nos dois bancos." -ForegroundColor Green
