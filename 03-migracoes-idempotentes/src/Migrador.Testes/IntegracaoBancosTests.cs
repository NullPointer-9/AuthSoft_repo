using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Migrador.Core;
using Migrador.Postgres;
using Migrador.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;

namespace Migrador.Testes
{
    /// <summary>
    /// Roda os scripts reais da pasta scripts/ contra os bancos do docker-compose.
    /// Cada teste cria um banco temporário e apaga no final.
    ///
    /// Só roda com as variáveis de ambiente (conexão ao servidor, não a um banco específico):
    ///   MIGRADOR_TESTE_SQLSERVER = Server=localhost,14333;User Id=sa;Password=...;TrustServerCertificate=True
    ///   MIGRADOR_TESTE_POSTGRES  = Host=localhost;Port=54333;Username=postgres;Password=...
    /// Sem elas, os testes ficam como "inconclusivos" em vez de falhar.
    /// </summary>
    [TestClass]
    [TestCategory("Integracao")]
    public class IntegracaoBancosTests
    {
        private const string ScriptComErro = "CREATE TABLE tabela_que_nao_deve_ficar (id INT);\nSELECT * FROM tabela_inexistente;";

        #region SQL Server

        [TestMethod]
        public void SqlServer_ScriptsReais_AplicaUmaVezEDepoisNaoFazNada()
        {
            ComBancoSqlServer(conexao =>
            {
                var motor = new MotorMigracao(new ExecutorSqlServer(conexao));
                var scripts = LeitorDeScripts.Ler(PastaDeScripts("sqlserver"));

                var primeira = motor.Executar(scripts);
                var segunda = motor.Executar(scripts);

                Assert.AreEqual(scripts.Count, primeira.Aplicadas.Count);
                Assert.AreEqual(0, segunda.Aplicadas.Count);
                Assert.AreEqual(scripts.Count, Escalar<int>(conexao, "SELECT COUNT(*) FROM dbo.historico_migracao"));
                Assert.AreEqual(1, Escalar<int>(conexao, "SELECT COUNT(*) FROM dbo.vw_cliente_contato"));
            });
        }

        [TestMethod]
        public void SqlServer_CadaScriptRodaDuasVezesSemErro()
        {
            ComBancoSqlServer(conexao =>
            {
                foreach (var script in LeitorDeScripts.Ler(PastaDeScripts("sqlserver")))
                {
                    foreach (var lote in DivisorDeLotesGo.Dividir(script.Conteudo).Concat(DivisorDeLotesGo.Dividir(script.Conteudo)))
                        Executar(conexao, lote);
                }

                Assert.AreEqual(1, Escalar<int>(conexao, "SELECT COUNT(*) FROM dbo.cliente"), "a migração de dados não pode duplicar o cliente");
            });
        }

        [TestMethod]
        public void SqlServer_ScriptComErro_DesfazTudoDaqueleScript()
        {
            ComBancoSqlServer(conexao =>
            {
                var executor = new ExecutorSqlServer(conexao);
                executor.GarantirTabelaDeControle();

                Assert.ThrowsException<InvalidOperationException>(
                    () => executor.Aplicar(new Migracao(1, "com_erro", "0001_com_erro.sql", ScriptComErro)));

                Assert.AreEqual(0, Escalar<int>(conexao, "SELECT COUNT(*) FROM sys.tables WHERE name = 'tabela_que_nao_deve_ficar'"));
                Assert.AreEqual(0, executor.ListarAplicadas().Count);
            });
        }

        private static void ComBancoSqlServer(Action<string> teste)
        {
            var servidor = Environment.GetEnvironmentVariable("MIGRADOR_TESTE_SQLSERVER");
            if (string.IsNullOrWhiteSpace(servidor))
                Assert.Inconclusive("Defina MIGRADOR_TESTE_SQLSERVER para rodar os testes de integração.");

            var nome = "migrador_teste_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            Executar(servidor, "CREATE DATABASE " + nome);
            try
            {
                teste(new SqlConnectionStringBuilder(servidor) { InitialCatalog = nome }.ConnectionString);
            }
            finally
            {
                SqlConnection.ClearAllPools();
                Executar(servidor, "ALTER DATABASE " + nome + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE " + nome);
            }
        }

        private static void Executar(string conexao, string sql)
        {
            using (var c = new SqlConnection(conexao))
            using (var comando = new SqlCommand(sql, c))
            {
                c.Open();
                comando.ExecuteNonQuery();
            }
        }

        private static T Escalar<T>(string conexao, string sql)
        {
            using (var c = new SqlConnection(conexao))
            using (var comando = new SqlCommand(sql, c))
            {
                c.Open();
                return (T)comando.ExecuteScalar();
            }
        }

        #endregion

        #region PostgreSQL

        [TestMethod]
        public void Postgres_ScriptsReais_AplicaUmaVezEDepoisNaoFazNada()
        {
            ComBancoPostgres(conexao =>
            {
                var motor = new MotorMigracao(new ExecutorPostgres(conexao));
                var scripts = LeitorDeScripts.Ler(PastaDeScripts("postgres"));

                var primeira = motor.Executar(scripts);
                var segunda = motor.Executar(scripts);

                Assert.AreEqual(scripts.Count, primeira.Aplicadas.Count);
                Assert.AreEqual(0, segunda.Aplicadas.Count);
                Assert.AreEqual((long)scripts.Count, EscalarPg<long>(conexao, "SELECT COUNT(*) FROM historico_migracao"));
                Assert.AreEqual(1L, EscalarPg<long>(conexao, "SELECT COUNT(*) FROM vw_cliente_contato"));
            });
        }

        [TestMethod]
        public void Postgres_CadaScriptRodaDuasVezesSemErro()
        {
            ComBancoPostgres(conexao =>
            {
                foreach (var script in LeitorDeScripts.Ler(PastaDeScripts("postgres")))
                {
                    ExecutarPg(conexao, script.Conteudo);
                    ExecutarPg(conexao, script.Conteudo);
                }

                Assert.AreEqual(1L, EscalarPg<long>(conexao, "SELECT COUNT(*) FROM cliente"), "a migração de dados não pode duplicar o cliente");
            });
        }

        [TestMethod]
        public void Postgres_ScriptComErro_DesfazTudoDaqueleScript()
        {
            ComBancoPostgres(conexao =>
            {
                var executor = new ExecutorPostgres(conexao);
                executor.GarantirTabelaDeControle();

                Assert.ThrowsException<InvalidOperationException>(
                    () => executor.Aplicar(new Migracao(1, "com_erro", "0001_com_erro.sql", ScriptComErro)));

                Assert.IsFalse(EscalarPg<bool>(conexao, "SELECT to_regclass('tabela_que_nao_deve_ficar') IS NOT NULL"),
                    "no PostgreSQL o CREATE TABLE também deveria ter sido desfeito");
                Assert.AreEqual(0, executor.ListarAplicadas().Count);
            });
        }

        private static void ComBancoPostgres(Action<string> teste)
        {
            var servidor = Environment.GetEnvironmentVariable("MIGRADOR_TESTE_POSTGRES");
            if (string.IsNullOrWhiteSpace(servidor))
                Assert.Inconclusive("Defina MIGRADOR_TESTE_POSTGRES para rodar os testes de integração.");

            var administracao = new NpgsqlConnectionStringBuilder(servidor) { Database = "postgres" }.ConnectionString;
            var nome = "migrador_teste_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            ExecutarPg(administracao, "CREATE DATABASE " + nome);
            try
            {
                teste(new NpgsqlConnectionStringBuilder(servidor) { Database = nome }.ConnectionString);
            }
            finally
            {
                NpgsqlConnection.ClearAllPools();
                ExecutarPg(administracao, "DROP DATABASE " + nome + " WITH (FORCE)");
            }
        }

        private static void ExecutarPg(string conexao, string sql)
        {
            using (var c = new NpgsqlConnection(conexao))
            using (var comando = new NpgsqlCommand(sql, c))
            {
                c.Open();
                comando.ExecuteNonQuery();
            }
        }

        private static T EscalarPg<T>(string conexao, string sql)
        {
            using (var c = new NpgsqlConnection(conexao))
            using (var comando = new NpgsqlCommand(sql, c))
            {
                c.Open();
                return (T)comando.ExecuteScalar();
            }
        }

        #endregion

        /// <summary>Sobe a partir da pasta do teste até achar scripts/&lt;banco&gt; na raiz do projeto.</summary>
        private static string PastaDeScripts(string banco)
        {
            for (var pasta = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory); pasta != null; pasta = pasta.Parent)
            {
                var candidata = Path.Combine(pasta.FullName, "scripts", banco);
                if (Directory.Exists(candidata))
                    return candidata;
            }
            throw new DirectoryNotFoundException("Pasta scripts/" + banco + " não encontrada acima de " + AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
