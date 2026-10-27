<#
    Compila os aplicativos em Release e gera um zip para cada um em ./pacotes.
    Usado pelo workflow de release; também pode rodar localmente.

    O balcão de certidões é empacotado na versão refatorada, a partir de uma cópia da branch
    refatoracao/05-emissao-documentos em -BalcaoRefatorado.
#>
param(
    [Parameter(Mandatory)][string]$Versao,
    [Parameter(Mandatory)][string]$BalcaoRefatorado
)
$ErrorActionPreference = 'Stop'

$raiz = Split-Path $PSScriptRoot -Parent
$saida = Join-Path $raiz 'pacotes'
if (Test-Path $saida) { Remove-Item $saida -Recurse -Force }
New-Item -ItemType Directory $saida | Out-Null

$aplicativos = @(
    @{ Nome = 'calculadora-emolumentos'; Projeto = "$raiz/01-calculadora-emolumentos/src/Emolumentos.WinForms/Emolumentos.WinForms.csproj" },
    @{ Nome = 'protocolo-cartorio';      Projeto = "$raiz/02-protocolo-cartorio/src/Cartorio.WinForms/Cartorio.WinForms.csproj" },
    @{ Nome = 'migrador-sql';            Projeto = "$raiz/03-migracoes-idempotentes/src/Migrador.Console/Migrador.Console.csproj"; Extra = "$raiz/03-migracoes-idempotentes/scripts" },
    @{ Nome = 'balcao-certidoes';        Projeto = "$BalcaoRefatorado/04-refatoracao-legado/src/Legado.WinForms/Legado.WinForms.csproj" }
)

Add-Type -AssemblyName System.IO.Compression.FileSystem

foreach ($app in $aplicativos) {
    $pasta = Join-Path $saida $app.Nome
    # Sem nós do MSBuild residentes: eles seguram as DLLs copiadas e impedem a compactação.
    dotnet build $app.Projeto -c Release -o $pasta -nologo -v q --disable-build-servers -nodeReuse:false
    if ($LASTEXITCODE -ne 0) { throw "Falha ao compilar $($app.Nome)" }

    if ($app.Extra) { Copy-Item $app.Extra (Join-Path $pasta 'scripts') -Recurse }
    Get-ChildItem $pasta -Include *.pdb, *.xml -Recurse | Remove-Item -Force

    $zip = Join-Path $saida "$($app.Nome)-$Versao.zip"
    [System.IO.Compression.ZipFile]::CreateFromDirectory($pasta, $zip)
    Write-Host "$zip ($([math]::Round((Get-Item $zip).Length / 1MB, 1)) MB)"
}
