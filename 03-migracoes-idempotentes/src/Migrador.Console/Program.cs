using System;
using System.Configuration;
using System.IO;
using System.Text;
using Migrador.Core;
using Migrador.Postgres;
using Migrador.SqlServer;

namespace Migrador.Console
{
    internal static class Program
    {
        private const int Sucesso = 0;
        private const int ErroDeUso = 1;
        private const int FalhaNaMigracao = 2;

        private static int Main(string[] args)
        {
            System.Console.OutputEncoding = Encoding.UTF8;

            Argumentos argumentos;
            try
            {
                argumentos = Argumentos.Interpretar(args);
            }
            catch (ArgumentException ex)
            {
                Escrever(ConsoleColor.Red, ex.Message);
                MostrarAjuda();
                return ErroDeUso;
            }

            if (argumentos.Ajuda)
            {
                MostrarAjuda();
                return Sucesso;
            }

            var conexao = argumentos.Conexao ?? ConfigurationManager.ConnectionStrings[argumentos.Banco]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(conexao))
            {
                Escrever(ConsoleColor.Red, "Sem connection string: use --conexao ou configure \"" + argumentos.Banco + "\" no Migrador.exe.config.");
                return ErroDeUso;
            }

            var pasta = argumentos.Pasta ?? Path.Combine("scripts", argumentos.Banco);
            var executor = argumentos.Banco == Argumentos.SqlServer
                ? (IExecutorBanco)new ExecutorSqlServer(conexao)
                : new ExecutorPostgres(conexao);

            try
            {
                var migracoes = LeitorDeScripts.Ler(pasta);
                System.Console.WriteLine(executor.NomeBanco + " | " + pasta + " | " + migracoes.Count + " script(s)");

                var resultado = new MotorMigracao(executor, mensagem => Escrever(ConsoleColor.Cyan, "  " + mensagem))
                    .Executar(migracoes, argumentos.Simular);

                foreach (var alerta in resultado.Alertas)
                    Escrever(ConsoleColor.Yellow, "AVISO: " + alerta);

                var verbo = resultado.Simulacao ? "seriam aplicadas" : "aplicadas agora";
                Escrever(ConsoleColor.Green,
                    resultado.Aplicadas.Count + " migração(ões) " + verbo + ", " + resultado.JaAplicadas + " já estavam aplicadas.");
                return Sucesso;
            }
            catch (Exception ex) when (ex is ScriptInvalidoException || ex is MigracaoException || ex is DirectoryNotFoundException)
            {
                Escrever(ConsoleColor.Red, "ERRO: " + ex.Message);
                return FalhaNaMigracao;
            }
            catch (Exception ex)
            {
                // Normalmente falha de conexão: banco fora do ar, senha errada, porta errada.
                Escrever(ConsoleColor.Red, "ERRO ao acessar o " + executor.NomeBanco + ": " + ex.Message);
                return FalhaNaMigracao;
            }
        }

        private static void Escrever(ConsoleColor cor, string texto)
        {
            var anterior = System.Console.ForegroundColor;
            System.Console.ForegroundColor = cor;
            System.Console.WriteLine(texto);
            System.Console.ForegroundColor = anterior;
        }

        private static void MostrarAjuda()
        {
            System.Console.WriteLine(@"
Aplica scripts de migração em ordem e registra os aplicados na tabela historico_migracao.

Uso:
  Migrador.exe --banco sqlserver|postgres [--conexao ""...""] [--pasta caminho] [--simular]

Opções:
  --banco     sqlserver ou postgres (obrigatório)
  --conexao   connection string; se omitida, usa a do Migrador.exe.config com o nome do banco
  --pasta     pasta dos scripts; padrão: scripts\<banco>
  --simular   mostra o que seria aplicado, sem alterar o banco
  --ajuda     mostra esta ajuda

Códigos de saída: 0 sucesso, 1 erro de uso, 2 falha na migração.");
        }
    }
}
