using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Legado.Negocio;

namespace Legado.Dados
{
    /// <summary>O esquema não muda; a conversão para decimal, enum e DateTime acontece aqui.</summary>
    public class PedidoRepositorio : IPedidoRepositorio
    {
        private const string Colunas = "id, id_cliente, tipo, valor, status, dt_pedido";

        private readonly string _conexao;

        public PedidoRepositorio(string conexao)
        {
            _conexao = conexao ?? throw new ArgumentNullException(nameof(conexao));
        }

        public IList<PedidoCertidao> ListarPorCliente(int idCliente)
        {
            return Consultar("select " + Colunas + " from tb_pedido_certidao where id_cliente = @id order by id", idCliente);
        }

        public PedidoCertidao ObterPorId(int idPedido)
        {
            var encontrados = Consultar("select " + Colunas + " from tb_pedido_certidao where id = @id", idPedido);
            return encontrados.Count == 0 ? null : encontrados[0];
        }

        public int ContarPorCliente(int idCliente)
        {
            using (var con = new SqlConnection(_conexao))
            using (var cmd = new SqlCommand("select count(*) from tb_pedido_certidao where id_cliente = @id", con))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = idCliente;
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public void Inserir(PedidoCertidao pedido)
        {
            const string sql = @"
insert into tb_pedido_certidao (id_cliente, tipo, valor, status, dt_pedido)
values (@id_cliente, @tipo, @valor, @status, @dt_pedido);
select cast(scope_identity() as int)";

            using (var con = new SqlConnection(_conexao))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.Add("@id_cliente", SqlDbType.Int).Value = pedido.IdCliente;
                cmd.Parameters.Add("@tipo", SqlDbType.VarChar, 50).Value = pedido.Tipo;
                cmd.Parameters.Add("@valor", SqlDbType.Float).Value = (double)pedido.Valor;
                cmd.Parameters.Add("@status", SqlDbType.VarChar, 30).Value = FormatosLegados.EscreverStatus(pedido.Status);
                cmd.Parameters.Add("@dt_pedido", SqlDbType.VarChar, 20).Value =
                    pedido.DataPedido.HasValue ? (object)FormatosLegados.EscreverData(pedido.DataPedido.Value) : DBNull.Value;
                con.Open();
                pedido.Id = (int)cmd.ExecuteScalar();
            }
        }

        public void AtualizarStatus(int idPedido, StatusPedido status)
        {
            using (var con = new SqlConnection(_conexao))
            using (var cmd = new SqlCommand("update tb_pedido_certidao set status = @status where id = @id", con))
            {
                cmd.Parameters.Add("@status", SqlDbType.VarChar, 30).Value = FormatosLegados.EscreverStatus(status);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = idPedido;
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private IList<PedidoCertidao> Consultar(string sql, int id)
        {
            var pedidos = new List<PedidoCertidao>();
            using (var con = new SqlConnection(_conexao))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                con.Open();
                using (var leitor = cmd.ExecuteReader())
                {
                    while (leitor.Read())
                    {
                        pedidos.Add(new PedidoCertidao
                        {
                            Id = leitor.GetInt32(0),
                            IdCliente = leitor.IsDBNull(1) ? 0 : leitor.GetInt32(1),
                            Tipo = leitor.IsDBNull(2) ? string.Empty : leitor.GetString(2),
                            // FLOAT do banco → decimal em centavos. A conversão usa 15 dígitos significativos,
                            // então 78.52499999... vira 78,525 → 78,53, o mesmo que o legado exibia com ToString("N2").
                            Valor = leitor.IsDBNull(3) ? 0m : Math.Round((decimal)leitor.GetDouble(3), 2, MidpointRounding.AwayFromZero),
                            Status = FormatosLegados.LerStatus(leitor.IsDBNull(4) ? null : leitor.GetString(4)),
                            DataPedido = FormatosLegados.LerData(leitor.IsDBNull(5) ? null : leitor.GetString(5))
                        });
                    }
                }
            }
            return pedidos;
        }
    }
}
