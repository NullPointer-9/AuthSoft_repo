using System;
using System.Collections.Generic;

namespace Legado.Negocio
{
    /// <summary>Violação de regra; a mensagem é para o usuário.</summary>
    public class NegocioException : Exception
    {
        public NegocioException(string mensagem) : base(mensagem)
        {
        }
    }

    public interface IRelogio
    {
        DateTime Agora { get; }
    }

    public class RelogioSistema : IRelogio
    {
        public DateTime Agora => DateTime.Now;
    }

    public interface IClienteRepositorio
    {
        IList<Cliente> BuscarPorNome(string trecho);

        /// <summary>Compara só os dígitos do CPF, porque o banco tem CPFs com e sem pontuação.</summary>
        bool ExisteCpf(string cpfSomenteDigitos, int ignorarIdCliente);

        void Inserir(Cliente cliente);
        void Atualizar(Cliente cliente);
        void Excluir(int idCliente);
    }

    public interface IPedidoRepositorio
    {
        IList<PedidoCertidao> ListarPorCliente(int idCliente);
        PedidoCertidao ObterPorId(int idPedido);
        int ContarPorCliente(int idCliente);
        void Inserir(PedidoCertidao pedido);
        void AtualizarStatus(int idPedido, StatusPedido status);
    }
}
