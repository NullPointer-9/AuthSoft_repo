using System.ComponentModel;
using System.Globalization;
using Legado.Negocio;

namespace Legado.Apresentacao
{
    // Dados já formatados para a tela. O DisplayName vira o cabeçalho da coluna no DataGridView.

    public class OpcaoTipoCertidao
    {
        public OpcaoTipoCertidao(TipoCertidao tipo)
        {
            Tipo = tipo;
            Descricao = FormatosLegados.DescricaoTipo(tipo);
        }

        public TipoCertidao Tipo { get; }
        public string Descricao { get; }
        public override string ToString() => Descricao;
    }

    public class ClienteLinha
    {
        public ClienteLinha(Cliente cliente)
        {
            Id = cliente.Id;
            Nome = cliente.Nome;
            Cpf = Formatacao.Cpf(cliente.Cpf);
            Telefone = cliente.Telefone;
        }

        [Browsable(false)]
        public int Id { get; }

        public string Nome { get; }

        [DisplayName("CPF")]
        public string Cpf { get; }

        public string Telefone { get; }
    }

    public class PedidoLinha
    {
        public PedidoLinha(PedidoCertidao pedido)
        {
            Id = pedido.Id;
            Tipo = pedido.Tipo;
            Valor = Formatacao.Moeda(pedido.Valor);
            Status = pedido.Status.ToString();
            Data = pedido.DataPedido?.ToString("dd/MM/yyyy") ?? "-";
        }

        [DisplayName("Nº")]
        public int Id { get; }

        public string Tipo { get; }
        public string Valor { get; }
        public string Status { get; }
        public string Data { get; }
    }

    public static class Formatacao
    {
        private static readonly CultureInfo PtBr = new CultureInfo("pt-BR");

        public static string Moeda(decimal valor) => valor.ToString("C", PtBr);

        /// <summary>000.000.000-00 quando tiver 11 dígitos; senão, o texto como veio do banco.</summary>
        public static string Cpf(string cpf)
        {
            var d = ValidadorCpf.Normalizar(cpf);
            return d.Length == 11
                ? d.Substring(0, 3) + "." + d.Substring(3, 3) + "." + d.Substring(6, 3) + "-" + d.Substring(9, 2)
                : cpf;
        }
    }
}
