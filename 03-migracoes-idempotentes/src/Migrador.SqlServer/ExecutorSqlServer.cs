using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Migrador.Core;

namespace Migrador.SqlServer
{
    public class ExecutorSqlServer : IExecutorBanco
    {
        private const string CriarTabelaDeControle = @"
IF OBJECT_ID(N'dbo.historico_migracao', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.historico_migracao (
        versao        INT            NOT NULL CONSTRAINT pk_historico_migracao PRIMARY KEY,
        nome_arquivo  NVARCHAR(260)  NOT NULL,
        checksum      CHAR(64)       NOT NULL,
        aplicado_em   DATETIME2(0)   NOT NULL CONSTRAINT df_historico_migracao_aplicado_em DEFAULT SYSDATETIME(),
        duracao_ms    INT            NOT NULL
    );
END";

        private readonly string _connectionString;

        public ExecutorSqlServer(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string não informada.", nameof(connectionString));

            _connectionString = connectionString;
        }

        public string NomeBanco => "SQL Server";

        public void GarantirTabelaDeControle()
        {
            using (var conexao = new SqlConnection(_connectionString))
            using (var comando = new SqlCommand(CriarTabelaDeControle, conexao))
            {
                conexao.Open();
                comando.ExecuteNonQuery();
            }
        }

        public IList<MigracaoAplicada> ListarAplicadas()
        {
            var aplicadas = new List<MigracaoAplicada>();
            using (var conexao = new SqlConnection(_connectionString))
            {
                conexao.Open();

                using (var existe = new SqlCommand("SELECT OBJECT_ID(N'dbo.historico_migracao', N'U')", conexao))
                {
                    if (existe.ExecuteScalar() is DBNull)
                        return aplicadas;
                }

                const string sql = "SELECT versao, nome_arquivo, checksum, aplicado_em, duracao_ms FROM dbo.historico_migracao ORDER BY versao";
                using (var comando = new SqlCommand(sql, conexao))
                using (var leitor = comando.ExecuteReader())
                {
                    while (leitor.Read())
                    {
                        aplicadas.Add(new MigracaoAplicada
                        {
                            Versao = leitor.GetInt32(0),
                            NomeArquivo = leitor.GetString(1),
                            Checksum = leitor.GetString(2),
                            AplicadoEm = leitor.GetDateTime(3),
                            DuracaoMs = leitor.GetInt32(4)
                        });
                    }
                }
            }
            return aplicadas;
        }

        public void Aplicar(Migracao migracao)
        {
            var cronometro = Stopwatch.StartNew();
            var lotes = DivisorDeLotesGo.Dividir(migracao.Conteudo);

            using (var conexao = new SqlConnection(_connectionString))
            {
                conexao.Open();
                using (var transacao = conexao.BeginTransaction())
                {
                    for (var i = 0; i < lotes.Count; i++)
                    {
                        using (var comando = new SqlCommand(lotes[i], conexao, transacao) { CommandTimeout = 0 })
                        {
                            try
                            {
                                comando.ExecuteNonQuery();
                            }
                            catch (SqlException ex)
                            {
                                throw new InvalidOperationException(
                                    "Erro no lote " + (i + 1) + " de " + lotes.Count + ", linha " + ex.LineNumber + ": " + ex.Message, ex);
                            }
                        }
                    }

                    const string registrar = @"
INSERT INTO dbo.historico_migracao (versao, nome_arquivo, checksum, duracao_ms)
VALUES (@versao, @nome_arquivo, @checksum, @duracao_ms)";
                    using (var comando = new SqlCommand(registrar, conexao, transacao))
                    {
                        comando.Parameters.Add("@versao", SqlDbType.Int).Value = migracao.Versao;
                        comando.Parameters.Add("@nome_arquivo", SqlDbType.NVarChar, 260).Value = migracao.NomeArquivo;
                        comando.Parameters.Add("@checksum", SqlDbType.Char, 64).Value = migracao.Checksum;
                        comando.Parameters.Add("@duracao_ms", SqlDbType.Int).Value = (int)cronometro.ElapsedMilliseconds;
                        comando.ExecuteNonQuery();
                    }

                    transacao.Commit();
                }
            }
        }
    }
}
