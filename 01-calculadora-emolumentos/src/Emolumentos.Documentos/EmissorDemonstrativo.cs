using System;
using System.IO;
using Emolumentos.Apresentacao;
using Microsoft.Reporting.WinForms;

namespace Emolumentos.Documentos
{
    /// <summary>
    /// Renderiza o relatório RDLC do demonstrativo em PDF e assina o resultado.
    /// </summary>
    public class EmissorDemonstrativo : IEmissorDemonstrativo
    {
        private const string RecursoRelatorio = "Emolumentos.Documentos.Relatorios.DemonstrativoEmolumentos.rdlc";

        private readonly AssinadorPdf _assinador;
        private readonly Func<DateTime> _agora;

        public EmissorDemonstrativo(AssinadorPdf assinador) : this(assinador, () => DateTime.Now)
        {
        }

        public EmissorDemonstrativo(AssinadorPdf assinador, Func<DateTime> agora)
        {
            _assinador = assinador ?? throw new ArgumentNullException(nameof(assinador));
            _agora = agora ?? throw new ArgumentNullException(nameof(agora));
        }

        public void Emitir(ResultadoCalculoViewModel resultado, string caminhoDestino)
        {
            File.WriteAllBytes(caminhoDestino, GerarPdfAssinado(resultado));
        }

        public byte[] GerarPdfAssinado(ResultadoCalculoViewModel resultado)
        {
            var pdf = RenderizarPdf(resultado);
            return _assinador.Assinar(pdf, "Demonstrativo de emolumentos", resultado.Uf);
        }

        /// <summary>PDF sem assinatura, direto do RDLC.</summary>
        public byte[] RenderizarPdf(ResultadoCalculoViewModel resultado)
        {
            using (var relatorio = new LocalReport())
            {
                ConfigurarRelatorio(relatorio, resultado);
                return relatorio.Render("PDF");
            }
        }

        /// <summary>Carrega o RDLC e os parâmetros. Usado pelo PDF e pela prévia no ReportViewer.</summary>
        public void ConfigurarRelatorio(LocalReport relatorio, ResultadoCalculoViewModel resultado)
        {
            if (relatorio == null)
                throw new ArgumentNullException(nameof(relatorio));
            if (resultado == null)
                throw new ArgumentNullException(nameof(resultado));

            using (var definicao = typeof(EmissorDemonstrativo).Assembly.GetManifestResourceStream(RecursoRelatorio))
            {
                if (definicao == null)
                    throw new InvalidOperationException("Relatório não encontrado nos recursos: " + RecursoRelatorio);

                relatorio.LoadReportDefinition(definicao);
            }

            relatorio.DisplayName = "Demonstrativo de emolumentos";
            relatorio.SetParameters(new[]
            {
                new ReportParameter("Uf", resultado.Uf),
                new ReportParameter("TipoAto", resultado.TipoAto),
                new ReportParameter("ValorNegocio", resultado.ValorNegocio),
                new ReportParameter("Faixa", resultado.Faixa),
                new ReportParameter("Emolumento", resultado.Emolumento),
                new ReportParameter("Custas", resultado.Custas),
                new ReportParameter("Total", resultado.Total),
                new ReportParameter("DataEmissao", _agora().ToString("dd/MM/yyyy HH:mm")),
                new ReportParameter("Assinante", _assinador.NomeTitular)
            });
        }
    }
}
