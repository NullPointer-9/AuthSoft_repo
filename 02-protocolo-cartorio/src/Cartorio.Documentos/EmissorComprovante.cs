using System;
using System.IO;
using Cartorio.Entidades;
using Cartorio.Negocio;
using Microsoft.Reporting.WinForms;

namespace Cartorio.Documentos
{
    /// <summary>
    /// Comprovante de abertura ou de entrega do protocolo: RDLC → PDF → assinatura digital.
    /// </summary>
    public class EmissorComprovante
    {
        private readonly AssinadorPdf _assinador;
        private readonly Func<DateTime> _agora;

        public EmissorComprovante(AssinadorPdf assinador) : this(assinador, () => DateTime.Now)
        {
        }

        public EmissorComprovante(AssinadorPdf assinador, Func<DateTime> agora)
        {
            _assinador = assinador ?? throw new ArgumentNullException(nameof(assinador));
            _agora = agora ?? throw new ArgumentNullException(nameof(agora));
        }

        public void Emitir(Protocolo protocolo, TipoComprovante tipo, string caminhoDestino)
        {
            File.WriteAllBytes(caminhoDestino, GerarPdfAssinado(protocolo, tipo));
        }

        public byte[] GerarPdfAssinado(Protocolo protocolo, TipoComprovante tipo)
        {
            var motivo = tipo == TipoComprovante.Entrega
                ? "Comprovante de entrega do protocolo " + protocolo.Numero
                : "Comprovante de abertura do protocolo " + protocolo.Numero;

            return _assinador.Assinar(RenderizarPdf(protocolo, tipo), motivo, "Cartório de Notas");
        }

        /// <summary>PDF sem assinatura, direto do RDLC.</summary>
        public byte[] RenderizarPdf(Protocolo protocolo, TipoComprovante tipo)
        {
            using (var relatorio = new LocalReport())
            {
                ConfigurarRelatorio(relatorio, protocolo, tipo);
                return relatorio.Render("PDF");
            }
        }

        /// <summary>Usado pelo PDF e pela prévia no ReportViewer.</summary>
        public void ConfigurarRelatorio(LocalReport relatorio, Protocolo protocolo, TipoComprovante tipo)
        {
            if (relatorio == null)
                throw new ArgumentNullException(nameof(relatorio));
            if (protocolo == null)
                throw new ArgumentNullException(nameof(protocolo));
            if (protocolo.Cliente == null)
                throw new ArgumentException("O protocolo precisa vir com o cliente carregado.", nameof(protocolo));

            DefinicaoRelatorio.Carregar(relatorio, "ComprovanteProtocolo.rdlc");

            var entrega = tipo == TipoComprovante.Entrega;
            relatorio.DisplayName = NomeSugerido(protocolo, tipo).Replace(".pdf", string.Empty);
            relatorio.SetParameters(new[]
            {
                new ReportParameter("Tipo", tipo.ToString()),
                new ReportParameter("Titulo", entrega ? "COMPROVANTE DE ENTREGA" : "COMPROVANTE DE PROTOCOLO"),
                new ReportParameter("Numero", protocolo.Numero),
                new ReportParameter("Cliente", protocolo.Cliente.Nome),
                new ReportParameter("Cpf", ValidadorCpf.Formatar(protocolo.Cliente.Cpf)),
                new ReportParameter("Servico", protocolo.TipoServico),
                new ReportParameter("Descricao", string.IsNullOrWhiteSpace(protocolo.Descricao) ? "-" : protocolo.Descricao),
                new ReportParameter("Status", RegrasStatus.Descricao(protocolo.Status)),
                new ReportParameter("DataAbertura", protocolo.DataAbertura.ToString("dd/MM/yyyy HH:mm")),
                new ReportParameter("DataEntrega", protocolo.DataEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "-"),
                new ReportParameter("Declaracao", Declaracao(protocolo, tipo)),
                new ReportParameter("DataEmissao", _agora().ToString("dd/MM/yyyy HH:mm")),
                new ReportParameter("Assinante", _assinador.NomeTitular)
            });
        }

        public static string NomeSugerido(Protocolo protocolo, TipoComprovante tipo)
        {
            var prefixo = tipo == TipoComprovante.Entrega ? "comprovante-entrega-" : "comprovante-protocolo-";
            return prefixo + protocolo.Numero + ".pdf";
        }

        private static string Declaracao(Protocolo protocolo, TipoComprovante tipo)
        {
            if (tipo == TipoComprovante.Entrega)
            {
                return "Declaramos que o serviço \"" + protocolo.TipoServico + "\", referente ao protocolo nº " +
                       protocolo.Numero + ", foi concluído e entregue em " +
                       (protocolo.DataEntrega?.ToString("dd/MM/yyyy 'às' HH:mm") ?? "-") + ".";
            }

            return "Declaramos que recebemos a solicitação de \"" + protocolo.TipoServico + "\" em nome de " +
                   protocolo.Cliente.Nome + ", registrada sob o protocolo nº " + protocolo.Numero +
                   ". Guarde este comprovante para acompanhar o andamento e retirar o documento.";
        }
    }
}
