using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Emolumentos.Apresentacao;
using Emolumentos.Documentos;
using Emolumentos.Dominio;

namespace Emolumentos.WinForms
{
    public partial class FormCalculadora : Form, ICalculoView
    {
        private readonly EmissorDemonstrativo _emissor;

        // Usado apenas pelo designer do Visual Studio.
        public FormCalculadora()
        {
            InitializeComponent();
        }

        /// <param name="emissor">Usado só para montar a prévia no ReportViewer.</param>
        public FormCalculadora(EmissorDemonstrativo emissor) : this()
        {
            _emissor = emissor;
        }

        public event EventHandler Carregada;
        public event EventHandler TipoAtoAlterado;
        public event EventHandler CalcularSolicitado;
        public event EventHandler EmitirDocumentoSolicitado;
        public event EventHandler PreviaSolicitada;

        public string UfSelecionada => (cmbUf.SelectedItem as Uf)?.Sigla;

        public int? TipoAtoSelecionadoId => (cmbTipoAto.SelectedItem as TipoAto)?.Id;

        public string ValorNegocioTexto => txtValor.Text;

        public void ExibirUfs(IList<Uf> ufs)
        {
            cmbUf.Items.Clear();
            cmbUf.Items.AddRange(ufs.Cast<object>().ToArray());
            if (cmbUf.Items.Count > 0)
                cmbUf.SelectedIndex = 0;
        }

        public void ExibirTiposAto(IList<TipoAto> tiposAto)
        {
            cmbTipoAto.Items.Clear();
            cmbTipoAto.Items.AddRange(tiposAto.Cast<object>().ToArray());
            if (cmbTipoAto.Items.Count > 0)
                cmbTipoAto.SelectedIndex = 0;
        }

        public void HabilitarValorNegocio(bool habilitado)
        {
            txtValor.Enabled = habilitado;
            if (!habilitado)
                txtValor.Clear();
        }

        public void ExibirResultado(ResultadoCalculoViewModel resultado)
        {
            lvResultado.BeginUpdate();
            lvResultado.Items.Clear();
            AdicionarLinha("UF", resultado.Uf);
            AdicionarLinha("Tipo de ato", resultado.TipoAto);
            AdicionarLinha("Valor do negócio", resultado.ValorNegocio);
            AdicionarLinha("Faixa aplicada", resultado.Faixa);
            AdicionarLinha("Emolumentos", resultado.Emolumento);
            AdicionarLinha("Custas", resultado.Custas);
            lvResultado.EndUpdate();

            lblTotal.Text = "Total: " + resultado.Total;
        }

        public void LimparResultado()
        {
            lvResultado.Items.Clear();
            lblTotal.Text = "Total: -";
        }

        public void ExibirErro(string mensagem)
        {
            MessageBox.Show(this, mensagem, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void HabilitarEmissao(bool habilitada)
        {
            btnEmitir.Enabled = habilitada;
            btnPrevia.Enabled = habilitada;
        }

        public bool ExibirPrevia(ResultadoCalculoViewModel resultado)
        {
            using (var previa = new FormPreviaDemonstrativo(_emissor, resultado))
                return previa.ShowDialog(this) == DialogResult.OK;
        }

        public string SolicitarCaminhoDoArquivo(string nomeSugerido)
        {
            using (var dialogo = new SaveFileDialog())
            {
                dialogo.Title = "Salvar demonstrativo";
                dialogo.Filter = "Documento PDF (*.pdf)|*.pdf";
                dialogo.FileName = nomeSugerido;
                dialogo.OverwritePrompt = true;
                return dialogo.ShowDialog(this) == DialogResult.OK ? dialogo.FileName : null;
            }
        }

        public void AbrirDocumento(string caminho)
        {
            Process.Start(new ProcessStartInfo(caminho) { UseShellExecute = true });
        }

        private void AdicionarLinha(string item, string valor)
        {
            lvResultado.Items.Add(new ListViewItem(new[] { item, valor }));
        }

        private void FormCalculadora_Load(object sender, EventArgs e)
        {
            Carregada?.Invoke(this, EventArgs.Empty);
        }

        private void cmbTipoAto_SelectedIndexChanged(object sender, EventArgs e)
        {
            TipoAtoAlterado?.Invoke(this, EventArgs.Empty);
        }

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            CalcularSolicitado?.Invoke(this, EventArgs.Empty);
        }

        private void btnEmitir_Click(object sender, EventArgs e)
        {
            EmitirDocumentoSolicitado?.Invoke(this, EventArgs.Empty);
        }

        private void btnPrevia_Click(object sender, EventArgs e)
        {
            PreviaSolicitada?.Invoke(this, EventArgs.Empty);
        }
    }
}
