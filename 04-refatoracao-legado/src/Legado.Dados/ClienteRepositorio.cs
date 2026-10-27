using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Legado.Negocio;

namespace Legado.Dados
{
    public class ClienteRepositorio : IClienteRepositorio
    {
        private readonly string _conexao;

        public ClienteRepositorio(string conexao)
        {
            _conexao = conexao ?? throw new ArgumentNullException(nameof(conexao));
        }

        public IList<Cliente> BuscarPorNome(string trecho)
        {
            var clientes = new List<Cliente>();
            using (var con = new SqlConnection(_conexao))
            using (var cmd = new SqlCommand("select id, nome, cpf, telefone from tb_cliente where nome like @filtro order by nome", con))
            {
                cmd.Parameters.Add("@filtro", SqlDbType.VarChar, 102).Value = "%" + (trecho ?? string.Empty) + "%";
                con.Open();
                using (var leitor = cmd.ExecuteReader())
                {
                    while (leitor.Read())
                    {
                        clientes.Add(new Cliente
                        {
                            Id = leitor.GetInt32(0),
                            Nome = leitor.IsDBNull(1) ? string.Empty : leitor.GetString(1),
                            Cpf = leitor.IsDBNull(2) ? string.Empty : leitor.GetString(2),
                            Telefone = leitor.IsDBNull(3) ? string.Empty : leitor.GetString(3)
                        });
                    }
                }
            }
            return clientes;
        }

        public bool ExisteCpf(string cpfSomenteDigitos, int ignorarIdCliente)
        {
            // O banco tem "529.982.247-25" e "52998224725": compara sem a pontuação.
            const string sql = @"
select count(*) from tb_cliente
where replace(replace(replace(cpf, '.', ''), '-', ''), ' ', '') = @cpf
  and id <> @ignorar";

            using (var con = new SqlConnection(_conexao))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.Add("@cpf", SqlDbType.VarChar, 20).Value = cpfSomenteDigitos;
                cmd.Parameters.Add("@ignorar", SqlDbType.Int).Value = ignorarIdCliente;
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public void Inserir(Cliente cliente)
        {
            Executar("insert into tb_cliente (nome, cpf, telefone) values (@nome, @cpf, @telefone); select cast(scope_identity() as int)", cliente, novoId => cliente.Id = novoId);
        }

        public void Atualizar(Cliente cliente)
        {
            Executar("update tb_cliente set nome = @nome, cpf = @cpf, telefone = @telefone where id = @id", cliente, null);
        }

        public void Excluir(int idCliente)
        {
            using (var con = new SqlConnection(_conexao))
            using (var cmd = new SqlCommand("delete from tb_cliente where id = @id", con))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = idCliente;
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void Executar(string sql, Cliente cliente, Action<int> aoGerarId)
        {
            using (var con = new SqlConnection(_conexao))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = cliente.Id;
                cmd.Parameters.Add("@nome", SqlDbType.VarChar, 100).Value = cliente.Nome;
                cmd.Parameters.Add("@cpf", SqlDbType.VarChar, 20).Value = cliente.Cpf;
                cmd.Parameters.Add("@telefone", SqlDbType.VarChar, 30).Value = (object)cliente.Telefone ?? DBNull.Value;
                con.Open();

                if (aoGerarId == null)
                    cmd.ExecuteNonQuery();
                else
                    aoGerarId((int)cmd.ExecuteScalar());
            }
        }
    }
}
