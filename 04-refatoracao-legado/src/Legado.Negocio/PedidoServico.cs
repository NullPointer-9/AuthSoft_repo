using System;
using System.Collections.Generic;
using System.Linq;

namespace Legado.Negocio
{
    /// <summary>O pedido segue Aberto → Pago → Entregue, um passo por vez.</summary>
    public class PedidoServico
    {
        private readonly IPedidoRepositorio _pedidos;
        private readonly IRelogio _relogio;

        public PedidoServico(IPedidoRepositorio pedidos, IRelogio relogio)
        {
            _pedidos = pedidos ?? throw new ArgumentNullException(nameof(pedidos));
            _relogio = relogio ?? throw new ArgumentNullException(nameof(relogio));
        }

        public IList<PedidoCertidao> ListarPorCliente(int idCliente) => _pedidos.ListarPorCliente(idCliente);

        public PedidoCertidao Criar(int idCliente, TipoCertidao tipo, bool urgente)
        {
            if (idCliente == 0)
                throw new NegocioException("Selecione um cliente.");

            var pedido = new PedidoCertidao
            {
                IdCliente = idCliente,
                Tipo = FormatosLegados.DescricaoTipo(tipo),
                Valor = TabelaPrecos.Preco(tipo, urgente),
                Status = StatusPedido.Aberto,
                DataPedido = _relogio.Agora.Date
            };
            _pedidos.Inserir(pedido);
            return pedido;
        }

        public void MarcarPago(int idPedido)
        {
            var pedido = Obter(idPedido);
            if (pedido.Status != StatusPedido.Aberto)
                throw new NegocioException("Só pedido em aberto pode ser marcado como pago.");

            _pedidos.AtualizarStatus(idPedido, StatusPedido.Pago);
        }

        public void Entregar(int idPedido)
        {
            var pedido = Obter(idPedido);
            if (pedido.Status != StatusPedido.Pago)
                throw new NegocioException(pedido.Status == StatusPedido.Entregue
                    ? "Pedido já foi entregue."
                    : "O pedido precisa estar pago para ser entregue.");

            _pedidos.AtualizarStatus(idPedido, StatusPedido.Entregue);
        }

        /// <summary>Soma dos pedidos em aberto do cliente.</summary>
        public decimal TotalEmAberto(int idCliente)
        {
            return _pedidos.ListarPorCliente(idCliente).Where(p => p.Status == StatusPedido.Aberto).Sum(p => p.Valor);
        }

        private PedidoCertidao Obter(int idPedido)
        {
            var pedido = _pedidos.ObterPorId(idPedido);
            if (pedido == null)
                throw new NegocioException("Pedido não encontrado.");
            return pedido;
        }
    }
}
