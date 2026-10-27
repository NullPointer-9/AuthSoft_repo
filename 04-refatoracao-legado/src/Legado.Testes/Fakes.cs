using System;
using System.Collections.Generic;
using System.Linq;
using Legado.Negocio;

namespace Legado.Testes
{
    public class EmissorReciboFalso : Legado.Apresentacao.IEmissorRecibo
    {
        public Legado.Apresentacao.ReciboViewModel ReciboEmitido { get; private set; }
        public string CaminhoEmitido { get; private set; }
        public int Emissoes { get; private set; }

        public void Emitir(Legado.Apresentacao.ReciboViewModel recibo, string caminhoDestino)
        {
            ReciboEmitido = recibo;
            CaminhoEmitido = caminhoDestino;
            Emissoes++;
        }
    }

    public class RelogioFixo : IRelogio
    {
        public DateTime Agora { get; set; } = new DateTime(2026, 10, 27, 14, 30, 0);
    }

    public class ClienteRepositorioEmMemoria : IClienteRepositorio
    {
        public List<Cliente> Clientes { get; } = new List<Cliente>();

        public IList<Cliente> BuscarPorNome(string trecho) =>
            Clientes.Where(c => c.Nome.IndexOf(trecho, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

        public Cliente ObterPorId(int idCliente) => Clientes.FirstOrDefault(c => c.Id == idCliente);

        public bool ExisteCpf(string cpfSomenteDigitos, int ignorarIdCliente) =>
            Clientes.Any(c => ValidadorCpf.Normalizar(c.Cpf) == cpfSomenteDigitos && c.Id != ignorarIdCliente);

        public void Inserir(Cliente cliente)
        {
            cliente.Id = Clientes.Count == 0 ? 1 : Clientes.Max(c => c.Id) + 1;
            Clientes.Add(cliente);
        }

        public void Atualizar(Cliente cliente) => Clientes[Clientes.FindIndex(c => c.Id == cliente.Id)] = cliente;

        public void Excluir(int idCliente) => Clientes.RemoveAll(c => c.Id == idCliente);
    }

    public class PedidoRepositorioEmMemoria : IPedidoRepositorio
    {
        public List<PedidoCertidao> Pedidos { get; } = new List<PedidoCertidao>();

        public IList<PedidoCertidao> ListarPorCliente(int idCliente) => Pedidos.Where(p => p.IdCliente == idCliente).ToList();

        public PedidoCertidao ObterPorId(int idPedido) => Pedidos.FirstOrDefault(p => p.Id == idPedido);

        public int ContarPorCliente(int idCliente) => Pedidos.Count(p => p.IdCliente == idCliente);

        public void Inserir(PedidoCertidao pedido)
        {
            pedido.Id = Pedidos.Count == 0 ? 1 : Pedidos.Max(p => p.Id) + 1;
            Pedidos.Add(pedido);
        }

        public void AtualizarStatus(int idPedido, StatusPedido status) => ObterPorId(idPedido).Status = status;

        /// <summary>Simula um pedido que já estava no banco, gravado pelo sistema antigo.</summary>
        public PedidoCertidao Existente(int idCliente, string statusComoNoBanco, decimal valor = 45.90m)
        {
            var pedido = new PedidoCertidao
            {
                IdCliente = idCliente,
                Tipo = "Certidao de nascimento",
                Valor = valor,
                Status = FormatosLegados.LerStatus(statusComoNoBanco)
            };
            Inserir(pedido);
            return pedido;
        }
    }
}
