using System;
using System.IO;
using Legado.Apresentacao;
using Microsoft.Reporting.WinForms;

namespace Legado.Documentos
{
    /// <summary>Recibo do pedido: RDLC → PDF → assinatura digital.</summary>
    public class EmissorRecibo : IEmissorRecibo
    {
        private const string RecursoRelatorio = "Legado.Documentos.Relatorios.ReciboPedidoCertidao.rdlc";

        private readonly AssinadorPdf _assinador;
        private readonly Func<DateTime> _agora;

        public EmissorRecibo(AssinadorPdf assinador) : this(assinador, () => DateTime.Now)
        {
        }

        public EmissorRecibo(AssinadorPdf assinador, Func<DateTime> agora)
        {
            _assinador = assinador ?? throw new ArgumentNullException(nameof(assinador));
            _agora = agora ?? throw new ArgumentNullException(nameof(agora));
        }

        public void Emitir(ReciboViewModel recibo, string caminhoDestino)
        {
            File.WriteAllBytes(caminhoDestino, GerarPdfAssinado(recibo));
        }

        public byte[] GerarPdfAssinado(ReciboViewModel recibo)
        {
            return _assinador.Assinar(RenderizarPdf(recibo), "Recibo do pedido " + recibo.NumeroPedido, "Balcão de Certidões");
        }

        public byte[] RenderizarPdf(ReciboViewModel recibo)
        {
            using (var relatorio = new LocalReport())
            {
                ConfigurarRelatorio(relatorio, recibo);
                return relatorio.Render("PDF");
            }
        }

        /// <summary>Usado pelo PDF e pela prévia no ReportViewer.</summary>
        public void ConfigurarRelatorio(LocalReport relatorio, ReciboViewModel recibo)
        {
            if (relatorio == null)
                throw new ArgumentNullException(nameof(relatorio));
            if (recibo == null)
                throw new ArgumentNullException(nameof(recibo));

            using (var definicao = typeof(EmissorRecibo).Assembly.GetManifestResourceStream(RecursoRelatorio))
            {
                if (definicao == null)
                    throw new InvalidOperationException("Relatório não encontrado nos recursos: " + RecursoRelatorio);
                relatorio.LoadReportDefinition(definicao);
            }

            relatorio.DisplayName = recibo.NomeArquivoSugerido.Replace(".pdf", string.Empty);
            relatorio.SetParameters(new[]
            {
                new ReportParameter("NumeroPedido", recibo.NumeroPedido),
                new ReportParameter("Cliente", recibo.Cliente),
                new ReportParameter("Cpf", recibo.Cpf),
                new ReportParameter("Tipo", recibo.Tipo),
                new ReportParameter("Valor", recibo.Valor),
                new ReportParameter("Status", recibo.Status),
                new ReportParameter("DataPedido", recibo.DataPedido),
                new ReportParameter("DataEmissao", _agora().ToString("dd/MM/yyyy HH:mm")),
                new ReportParameter("Assinante", _assinador.NomeTitular)
            });
        }
    }
}
