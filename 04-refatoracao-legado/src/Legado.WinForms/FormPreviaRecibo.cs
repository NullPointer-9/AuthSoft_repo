using System;
using System.Windows.Forms;
using Legado.Apresentacao;
using Legado.Documentos;
using Microsoft.Reporting.WinForms;

namespace Legado.WinForms
{
    /// <summary>Prévia no ReportViewer. Fecha com DialogResult.OK quando o usuário escolhe emitir.</summary>
    public partial class FormPreviaRecibo : Form
    {
        private readonly EmissorRecibo _emissor;
        private readonly ReciboViewModel _recibo;

        // Usado apenas pelo designer do Visual Studio.
        public FormPreviaRecibo()
        {
            InitializeComponent();
        }

        public FormPreviaRecibo(EmissorRecibo emissor, ReciboViewModel recibo) : this()
        {
            _emissor = emissor;
            _recibo = recibo;
        }

        private void FormPreviaRecibo_Load(object sender, EventArgs e)
        {
            reportViewer.ProcessingMode = ProcessingMode.Local;
            _emissor.ConfigurarRelatorio(reportViewer.LocalReport, _recibo);

            reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer.ZoomMode = ZoomMode.PageWidth;
            reportViewer.RefreshReport();
        }
    }
}
