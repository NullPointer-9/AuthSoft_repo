using System;
using System.Windows.Forms;
using Emolumentos.Apresentacao;
using Emolumentos.Documentos;
using Microsoft.Reporting.WinForms;

namespace Emolumentos.WinForms
{
    /// <summary>Prévia no ReportViewer. Fecha com DialogResult.OK quando o usuário escolhe emitir.</summary>
    public partial class FormPreviaDemonstrativo : Form
    {
        private readonly EmissorDemonstrativo _emissor;
        private readonly ResultadoCalculoViewModel _resultado;

        // Usado apenas pelo designer do Visual Studio.
        public FormPreviaDemonstrativo()
        {
            InitializeComponent();
        }

        public FormPreviaDemonstrativo(EmissorDemonstrativo emissor, ResultadoCalculoViewModel resultado) : this()
        {
            _emissor = emissor;
            _resultado = resultado;
        }

        private void FormPreviaDemonstrativo_Load(object sender, EventArgs e)
        {
            reportViewer.ProcessingMode = ProcessingMode.Local;
            _emissor.ConfigurarRelatorio(reportViewer.LocalReport, _resultado);

            reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer.ZoomMode = ZoomMode.PageWidth;
            reportViewer.RefreshReport();
        }
    }
}
