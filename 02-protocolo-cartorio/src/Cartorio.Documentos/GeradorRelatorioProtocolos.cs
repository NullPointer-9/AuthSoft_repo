using System;
using System.Collections.Generic;
using System.Linq;
using Cartorio.Entidades;
using Cartorio.Negocio;
using Microsoft.Reporting.WinForms;

namespace Cartorio.Documentos
{
    /// <summary>Uma linha da tabela do relatório, já formatada. Precisa ser pública para o RDLC enxergar.</summary>
    public class LinhaRelatorioProtocolo
    {
        public LinhaRelatorioProtocolo(Protocolo protocolo)
        {
            Numero = protocolo.Numero;
            Cliente = protocolo.Cliente?.Nome;
            Cpf = ValidadorCpf.Formatar(protocolo.Cliente?.Cpf);
            Servico = protocolo.TipoServico;
            Status = RegrasStatus.Descricao(protocolo.Status);
            Abertura = protocolo.DataAbertura.ToString("dd/MM/yyyy HH:mm");
        }

        public string Numero { get; }
        public string Cliente { get; }
        public string Cpf { get; }
        public string Servico { get; }
        public string Status { get; }
        public string Abertura { get; }
    }

    /// <summary>Relatório interno de protocolos, sem assinatura.</summary>
    public class GeradorRelatorioProtocolos
    {
        private readonly Func<DateTime> _agora;

        public GeradorRelatorioProtocolos() : this(() => DateTime.Now)
        {
        }

        public GeradorRelatorioProtocolos(Func<DateTime> agora)
        {
            _agora = agora ?? throw new ArgumentNullException(nameof(agora));
        }

        public void ConfigurarRelatorio(LocalReport relatorio, IList<Protocolo> protocolos, string descricaoFiltro)
        {
            if (relatorio == null)
                throw new ArgumentNullException(nameof(relatorio));
            if (protocolos == null)
                throw new ArgumentNullException(nameof(protocolos));

            DefinicaoRelatorio.Carregar(relatorio, "RelatorioProtocolos.rdlc");

            relatorio.DisplayName = "relatorio-protocolos";
            relatorio.DataSources.Clear();
            relatorio.DataSources.Add(new ReportDataSource(
                "Protocolos",
                protocolos.Select(p => new LinhaRelatorioProtocolo(p)).ToList()));
            relatorio.SetParameters(new[]
            {
                new ReportParameter("Filtro", descricaoFiltro ?? string.Empty),
                new ReportParameter("DataEmissao", _agora().ToString("dd/MM/yyyy HH:mm"))
            });
        }

        public byte[] RenderizarPdf(IList<Protocolo> protocolos, string descricaoFiltro)
        {
            using (var relatorio = new LocalReport())
            {
                ConfigurarRelatorio(relatorio, protocolos, descricaoFiltro);
                return relatorio.Render("PDF");
            }
        }

        public static string DescreverFiltro(DateTime dataInicial, DateTime dataFinal, StatusProtocolo? status)
        {
            return "Abertos de " + dataInicial.ToString("dd/MM/yyyy") + " a " + dataFinal.ToString("dd/MM/yyyy") +
                   " | Status: " + (status.HasValue ? RegrasStatus.Descricao(status.Value) : "todos");
        }
    }
}
