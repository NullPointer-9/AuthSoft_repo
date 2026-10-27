using Legado.Negocio;

namespace Legado.Apresentacao
{
    /// <summary>Dados do recibo já formatados para o documento.</summary>
    public class ReciboViewModel
    {
        public ReciboViewModel(PedidoCertidao pedido, Cliente cliente)
        {
            NumeroPedido = pedido.Id.ToString();
            Cliente = cliente.Nome;
            Cpf = Formatacao.Cpf(cliente.Cpf);
            Tipo = pedido.Tipo;
            Valor = Formatacao.Moeda(pedido.Valor);
            Status = pedido.Status.ToString();
            DataPedido = pedido.DataPedido?.ToString("dd/MM/yyyy") ?? "-";
        }

        public string NumeroPedido { get; }
        public string Cliente { get; }
        public string Cpf { get; }
        public string Tipo { get; }
        public string Valor { get; }
        public string Status { get; }
        public string DataPedido { get; }

        public string NomeArquivoSugerido => "recibo-pedido-" + NumeroPedido + ".pdf";
    }

    /// <summary>Gera o recibo em PDF assinado. Definido aqui, implementado no projeto Documentos.</summary>
    public interface IEmissorRecibo
    {
        void Emitir(ReciboViewModel recibo, string caminhoDestino);
    }
}
