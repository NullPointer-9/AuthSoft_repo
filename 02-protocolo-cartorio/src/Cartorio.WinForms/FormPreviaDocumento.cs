using System;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace Cartorio.WinForms
{
    /// <summary>Prévia no ReportViewer. Fecha com DialogResult.OK quando o usuário escolhe emitir.</summary>
    public partial class FormPreviaDocumento : Form
    {
        private readonly Action<LocalReport> _configurarRelatorio;

        // Usado apenas pelo designer do Visual Studio.
        public FormPreviaDocumento()
        {
            InitializeComponent();
        }

        public FormPreviaDocumento(string titulo, Action<LocalReport> configurarRelatorio) : this()
        {
            Text = titulo;
            _configurarRelatorio = configurarRelatorio;
        }

        private void FormPreviaDocumento_Load(object sender, EventArgs e)
        {
            reportViewer.ProcessingMode = ProcessingMode.Local;
            _configurarRelatorio(reportViewer.LocalReport);

            reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer.ZoomMode = ZoomMode.PageWidth;
            reportViewer.RefreshReport();
        }
    }
}
