using System;
using System.Collections.Generic;
using System.Diagnostics;
using Migrador.Core;
using Npgsql;
using NpgsqlTypes;

namespace Migrador.Postgres
{
    public class ExecutorPostgres : IExecutorBanco
    {
        private const string CriarTabelaDeControle = @"
CREATE TABLE IF NOT EXISTS historico_migracao (
    versao        INTEGER       NOT NULL CONSTRAINT pk_historico_migracao PRIMARY KEY,
    nome_arquivo  VARCHAR(260)  NOT NULL,
    checksum      CHAR(64)      NOT NULL,
    aplicado_em   TIMESTAMP(0)  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    duracao_ms    INTEGER       NOT NULL
)";

        private readonly string _connectionString;

        public ExecutorPostgres(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string não informada.", nameof(connectionString));

            _connectionString = connectionString;
        }

        public string NomeBanco => "PostgreSQL";

        public void GarantirTabelaDeControle()
        {
            using (var conexao = new NpgsqlConnection(_connectionString))
            using (var comando = new NpgsqlCommand(CriarTabelaDeControle, conexao))
            {
                conexao.Open();
                comando.ExecuteNonQuery();
            }
        }

        public IList<MigracaoAplicada> ListarAplicadas()
        {
            var aplicadas = new List<MigracaoAplicada>();
            using (var conexao = new NpgsqlConnection(_connectionString))
            {
                conexao.Open();

                using (var existe = new NpgsqlCommand("SELECT to_regclass('historico_migracao') IS NOT NULL", conexao))
                {
                    if (!(bool)existe.ExecuteScalar())
                        return aplicadas;
                }

                const string sql = "SELECT versao, nome_arquivo, checksum, aplicado_em, duracao_ms FROM historico_migracao ORDER BY versao";
                using (var comando = new NpgsqlCommand(sql, conexao))
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

            using (var conexao = new NpgsqlConnection(_connectionString))
            {
                conexao.Open();

                // No PostgreSQL o DDL também é transacional.
                using (var transacao = conexao.BeginTransaction())
                {
                    // O Npgsql aceita vários comandos separados por ";" num único NpgsqlCommand.
                    using (var comando = new NpgsqlCommand(migracao.Conteudo, conexao, transacao) { CommandTimeout = 0 })
                    {
                        try
                        {
                            comando.ExecuteNonQuery();
                        }
                        catch (PostgresException ex)
                        {
                            throw new InvalidOperationException(ex.MessageText + " (código " + ex.SqlState + ")", ex);
                        }
                    }

                    const string registrar = @"
INSERT INTO historico_migracao (versao, nome_arquivo, checksum, duracao_ms)
VALUES (@versao, @nome_arquivo, @checksum, @duracao_ms)";
                    using (var comando = new NpgsqlCommand(registrar, conexao, transacao))
                    {
                        comando.Parameters.Add("versao", NpgsqlDbType.Integer).Value = migracao.Versao;
                        comando.Parameters.Add("nome_arquivo", NpgsqlDbType.Varchar).Value = migracao.NomeArquivo;
                        comando.Parameters.Add("checksum", NpgsqlDbType.Char).Value = migracao.Checksum;
                        comando.Parameters.Add("duracao_ms", NpgsqlDbType.Integer).Value = (int)cronometro.ElapsedMilliseconds;
                        comando.ExecuteNonQuery();
                    }

                    transacao.Commit();
                }
            }
        }
    }
}
