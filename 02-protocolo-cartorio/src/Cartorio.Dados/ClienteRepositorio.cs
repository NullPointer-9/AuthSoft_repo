using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Cartorio.Entidades;
using Cartorio.Negocio;

namespace Cartorio.Dados
{
    /// <summary>Um DbContext por operação, para não acumular entidades rastreadas durante a sessão.</summary>
    public class ClienteRepositorio : IClienteRepositorio
    {
        private readonly string _connectionString;

        public ClienteRepositorio(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public Cliente ObterPorId(int id)
        {
            using (var contexto = new CartorioContext(_connectionString))
                return contexto.Clientes.AsNoTracking().FirstOrDefault(c => c.Id == id);
        }

        public Cliente ObterPorCpf(string cpf)
        {
            using (var contexto = new CartorioContext(_connectionString))
                return contexto.Clientes.AsNoTracking().FirstOrDefault(c => c.Cpf == cpf);
        }

        public IList<Cliente> BuscarPorNome(string trecho)
        {
            using (var contexto = new CartorioContext(_connectionString))
            {
                return contexto.Clientes.AsNoTracking()
                    .Where(c => c.Nome.Contains(trecho))
                    .OrderBy(c => c.Nome)
                    .Take(200)
                    .ToList();
            }
        }

        public IList<Cliente> Listar()
        {
            using (var contexto = new CartorioContext(_connectionString))
                return contexto.Clientes.AsNoTracking().OrderBy(c => c.Nome).Take(200).ToList();
        }

        public void Adicionar(Cliente cliente)
        {
            using (var contexto = new CartorioContext(_connectionString))
            {
                contexto.Clientes.Add(cliente);
                try
                {
                    contexto.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    throw ErrosDeBanco.Traduzir(ex, "Já existe um cliente cadastrado com este CPF.");
                }
            }
        }

        public void Atualizar(Cliente cliente)
        {
            using (var contexto = new CartorioContext(_connectionString))
            {
                contexto.Clientes.Attach(cliente);
                var entrada = contexto.Entry(cliente);
                entrada.Property(c => c.Nome).IsModified = true;
                entrada.Property(c => c.Email).IsModified = true;
                entrada.Property(c => c.Telefone).IsModified = true;
                contexto.SaveChanges();
            }
        }
    }
}
