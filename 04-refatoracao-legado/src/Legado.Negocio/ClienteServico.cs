using System;
using System.Collections.Generic;

namespace Legado.Negocio
{
    public class ClienteServico
    {
        private readonly IClienteRepositorio _clientes;
        private readonly IPedidoRepositorio _pedidos;

        public ClienteServico(IClienteRepositorio clientes, IPedidoRepositorio pedidos)
        {
            _clientes = clientes ?? throw new ArgumentNullException(nameof(clientes));
            _pedidos = pedidos ?? throw new ArgumentNullException(nameof(pedidos));
        }

        public IList<Cliente> Buscar(string trechoDoNome) => _clientes.BuscarPorNome(trechoDoNome ?? string.Empty);

        public Cliente Obter(int idCliente)
        {
            var cliente = _clientes.ObterPorId(idCliente);
            if (cliente == null)
                throw new NegocioException("Cliente não encontrado.");
            return cliente;
        }

        /// <summary>Id 0 cadastra; outro valor altera.</summary>
        public Cliente Salvar(int id, string nome, string cpf, string telefone)
        {
            nome = (nome ?? string.Empty).Trim();
            if (nome.Length == 0)
                throw new NegocioException("Preencha o nome.");
            if (nome.Length > 100)
                throw new NegocioException("O nome pode ter no máximo 100 caracteres.");

            var cpfDigitos = ValidadorCpf.Normalizar(cpf);
            if (!ValidadorCpf.EhValido(cpfDigitos))
                throw new NegocioException("CPF inválido.");

            if (_clientes.ExisteCpf(cpfDigitos, id))
                throw new NegocioException("CPF já cadastrado para outro cliente.");

            var cliente = new Cliente
            {
                Id = id,
                Nome = nome,
                Cpf = cpfDigitos,
                Telefone = string.IsNullOrWhiteSpace(telefone) ? string.Empty : telefone.Trim()
            };

            if (id == 0)
                _clientes.Inserir(cliente);
            else
                _clientes.Atualizar(cliente);

            return cliente;
        }

        public void Excluir(int idCliente)
        {
            if (idCliente == 0)
                throw new NegocioException("Selecione um cliente.");

            // tb_pedido_certidao não tem FK para tb_cliente.
            var pedidos = _pedidos.ContarPorCliente(idCliente);
            if (pedidos > 0)
                throw new NegocioException("O cliente tem " + pedidos + " pedido(s) de certidão e não pode ser excluído.");

            _clientes.Excluir(idCliente);
        }
    }
}
