using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Legado.Dados
{
    public class PedidoRepositorio
    {
        private readonly string _conexao;

        public PedidoRepositorio(string conexao)
        {
            _conexao = conexao ?? throw new ArgumentNullException(nameof(conexao));
        }

        public IList<PedidoCertidao> ListarPorCliente(int idCliente)
        {
            var pedidos = new List<PedidoCertidao>();
            using (var con = new SqlConnection(_conexao))
            using (var cmd = new SqlCommand("select id, id_cliente, tipo, valor, status, dt_pedido from tb_pedido_certidao where id_cliente = @id_cliente order by id", con))
            {
                cmd.Parameters.Add("@id_cliente", SqlDbType.Int).Value = idCliente;
                con.Open();
                using (var leitor = cmd.ExecuteReader())
                {
                    while (leitor.Read())
                    {
                        pedidos.Add(new PedidoCertidao
                        {
                            Id = leitor.GetInt32(0),
                            IdCliente = leitor.GetInt32(1),
                            Tipo = leitor.IsDBNull(2) ? string.Empty : leitor.GetString(2),
                            Valor = leitor.IsDBNull(3) ? 0 : leitor.GetDouble(3),
                            Status = leitor.IsDBNull(4) ? string.Empty : leitor.GetString(4),
                            DataPedido = leitor.IsDBNull(5) ? string.Empty : leitor.GetString(5)
                        });
                    }
                }
            }
            return pedidos;
        }

        public double TotalEmAberto(int idCliente)
        {
            using (var con = new SqlConnection(_conexao))
            using (var cmd = new SqlCommand("select sum(valor) from tb_pedido_certidao where id_cliente = @id_cliente and status = 'aberto'", con))
            {
                cmd.Parameters.Add("@id_cliente", SqlDbType.Int).Value = idCliente;
                con.Open();
                var total = cmd.ExecuteScalar();
                return total == DBNull.Value ? 0 : Convert.ToDouble(total);
            }
        }

        public void Inserir(PedidoCertidao pedido)
        {
            using (var con = new SqlConnection(_conexao))
            using (var cmd = new SqlCommand("insert into tb_pedido_certidao (id_cliente, tipo, valor, status, dt_pedido) values (@id_cliente, @tipo, @valor, @status, @dt_pedido)", con))
            {
                cmd.Parameters.Add("@id_cliente", SqlDbType.Int).Value = pedido.IdCliente;
                cmd.Parameters.Add("@tipo", SqlDbType.VarChar, 50).Value = pedido.Tipo;
                cmd.Parameters.Add("@valor", SqlDbType.Float).Value = pedido.Valor;
                cmd.Parameters.Add("@status", SqlDbType.VarChar, 30).Value = pedido.Status;
                cmd.Parameters.Add("@dt_pedido", SqlDbType.VarChar, 20).Value = pedido.DataPedido;
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void AtualizarStatus(int idPedido, string status)
        {
            using (var con = new SqlConnection(_conexao))
            using (var cmd = new SqlCommand("update tb_pedido_certidao set status = @status where id = @id", con))
            {
                cmd.Parameters.Add("@status", SqlDbType.VarChar, 30).Value = status;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = idPedido;
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
