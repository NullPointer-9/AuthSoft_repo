using System;
using System.Windows.Forms;
using Cartorio.Documentos;
using Cartorio.Entidades;
using Cartorio.Negocio;
using Microsoft.Reporting.WinForms;

namespace Cartorio.WinForms
{
    public partial class FormRelatorioProtocolos : Form
    {
        private readonly ProtocoloServico _protocoloServico;
        private readonly GeradorRelatorioProtocolos _gerador;

        private class OpcaoStatus
        {
            public OpcaoStatus(string texto, StatusProtocolo? valor)
            {
                Texto = texto;
                Valor = valor;
            }

            public string Texto { get; }
            public StatusProtocolo? Valor { get; }
            public override string ToString() => Texto;
        }

        // Usado apenas pelo designer do Visual Studio.
        public FormRelatorioProtocolos()
        {
            InitializeComponent();
        }

        public FormRelatorioProtocolos(ProtocoloServico protocoloServico, GeradorRelatorioProtocolos gerador) : this()
        {
            _protocoloServico = protocoloServico;
            _gerador = gerador;
        }

        private void FormRelatorioProtocolos_Shown(object sender, EventArgs e)
        {
            cmbStatus.Items.Add(new OpcaoStatus("Todos", null));
            foreach (StatusProtocolo status in Enum.GetValues(typeof(StatusProtocolo)))
                cmbStatus.Items.Add(new OpcaoStatus(RegrasStatus.Descricao(status), status));
            cmbStatus.SelectedIndex = 0;

            var hoje = DateTime.Today;
            dtpInicio.Value = new DateTime(hoje.Year, hoje.Month, 1);
            dtpFim.Value = hoje;

            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer.ZoomMode = ZoomMode.PageWidth;

            Gerar();
        }

        private void btnGerar_Click(object sender, EventArgs e)
        {
            Gerar();
        }

        private void Gerar()
        {
            var inicio = dtpInicio.Value.Date;
            var fim = dtpFim.Value.Date;
            var status = (cmbStatus.SelectedItem as OpcaoStatus)?.Valor;

            TratamentoDeErros.Executar(this, () =>
            {
                var protocolos = _protocoloServico.ConsultarParaRelatorio(inicio, fim, status);
                _gerador.ConfigurarRelatorio(reportViewer.LocalReport, protocolos,
                    GeradorRelatorioProtocolos.DescreverFiltro(inicio, fim, status));
                reportViewer.RefreshReport();
            });
        }
    }
}
