using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Emolumentos.Dominio;

namespace Emolumentos.Dados
{
    public class TabelaEmolumentosRepositorio : ITabelaEmolumentosRepositorio
    {
        private readonly string _connectionString;

        public TabelaEmolumentosRepositorio(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string não informada.", nameof(connectionString));

            _connectionString = connectionString;
        }

        public IList<Uf> ListarUfs()
        {
            const string sql = "SELECT Sigla, Nome FROM dbo.Uf ORDER BY Sigla";

            var ufs = new List<Uf>();
            using (var conexao = new SqlConnection(_connectionString))
            using (var comando = new SqlCommand(sql, conexao))
            {
                conexao.Open();
                using (var leitor = comando.ExecuteReader())
                {
                    while (leitor.Read())
                        ufs.Add(new Uf(leitor.GetString(0).Trim(), leitor.GetString(1)));
                }
            }
            return ufs;
        }

        public IList<TipoAto> ListarTiposAto()
        {
            const string sql = "SELECT Id, Codigo, Descricao, ExigeValor FROM dbo.TipoAto ORDER BY Id";

            var tipos = new List<TipoAto>();
            using (var conexao = new SqlConnection(_connectionString))
            using (var comando = new SqlCommand(sql, conexao))
            {
                conexao.Open();
                using (var leitor = comando.ExecuteReader())
                {
                    while (leitor.Read())
                    {
                        tipos.Add(new TipoAto(
                            leitor.GetInt32(0),
                            leitor.GetString(1),
                            leitor.GetString(2),
                            leitor.GetBoolean(3)));
                    }
                }
            }
            return tipos;
        }

        public IList<FaixaEmolumento> ObterFaixas(string siglaUf)
        {
            const string sql = @"
                SELECT TipoAtoId, ValorDe, ValorAte, Emolumento, Custas
                FROM dbo.FaixaEmolumento
                WHERE UfSigla = @UfSigla
                ORDER BY TipoAtoId, ValorDe";

            var faixas = new List<FaixaEmolumento>();
            using (var conexao = new SqlConnection(_connectionString))
            using (var comando = new SqlCommand(sql, conexao))
            {
                comando.Parameters.Add("@UfSigla", SqlDbType.Char, 2).Value = siglaUf;

                conexao.Open();
                using (var leitor = comando.ExecuteReader())
                {
                    while (leitor.Read())
                    {
                        faixas.Add(new FaixaEmolumento(
                            leitor.GetInt32(0),
                            leitor.GetDecimal(1),
                            leitor.IsDBNull(2) ? (decimal?)null : leitor.GetDecimal(2),
                            leitor.GetDecimal(3),
                            leitor.GetDecimal(4)));
                    }
                }
            }
            return faixas;
        }
    }
}
