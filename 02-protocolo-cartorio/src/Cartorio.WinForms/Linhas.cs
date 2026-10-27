using System.ComponentModel;
using Cartorio.Entidades;
using Cartorio.Negocio;

namespace Cartorio.WinForms
{
    // Linhas já formatadas para os DataGridView. O DisplayName vira o cabeçalho da coluna.

    internal class ClienteLinha
    {
        public ClienteLinha(Cliente cliente)
        {
            Id = cliente.Id;
            Nome = cliente.Nome;
            Cpf = ValidadorCpf.Formatar(cliente.Cpf);
            Email = cliente.Email;
            Telefone = cliente.Telefone;
            Cadastro = cliente.DataCadastro.ToString("dd/MM/yyyy");
        }

        [Browsable(false)]
        public int Id { get; }

        public string Nome { get; }

        [DisplayName("CPF")]
        public string Cpf { get; }

        [DisplayName("E-mail")]
        public string Email { get; }

        public string Telefone { get; }

        public string Cadastro { get; }
    }

    internal class ProtocoloLinha
    {
        public ProtocoloLinha(Protocolo protocolo)
        {
            Id = protocolo.Id;
            Numero = protocolo.Numero;
            Cliente = protocolo.Cliente?.Nome;
            Cpf = ValidadorCpf.Formatar(protocolo.Cliente?.Cpf);
            Servico = protocolo.TipoServico;
            Status = RegrasStatus.Descricao(protocolo.Status);
            Abertura = protocolo.DataAbertura.ToString("dd/MM/yyyy HH:mm");
            Atualizacao = protocolo.DataAtualizacao.ToString("dd/MM/yyyy HH:mm");
        }

        [Browsable(false)]
        public int Id { get; }

        [DisplayName("Número")]
        public string Numero { get; }

        public string Cliente { get; }

        [DisplayName("CPF")]
        public string Cpf { get; }

        [DisplayName("Serviço")]
        public string Servico { get; }

        public string Status { get; }

        public string Abertura { get; }

        [DisplayName("Atualização")]
        public string Atualizacao { get; }
    }

    internal class HistoricoLinha
    {
        public HistoricoLinha(ProtocoloHistorico historico)
        {
            Data = historico.DataAlteracao.ToString("dd/MM/yyyy HH:mm");
            De = historico.StatusAnterior.HasValue ? RegrasStatus.Descricao(historico.StatusAnterior.Value) : "-";
            Para = RegrasStatus.Descricao(historico.StatusNovo);
            Observacao = historico.Observacao;
        }

        public string Data { get; }
        public string De { get; }
        public string Para { get; }

        [DisplayName("Observação")]
        public string Observacao { get; }
    }
}
