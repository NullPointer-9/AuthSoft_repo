using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Legado.Apresentacao;
using Legado.Documentos;
using Legado.Negocio;

namespace Legado.WinForms
{
    public partial class FormBalcao : Form, IBalcaoView
    {
        private readonly EmissorRecibo _emissorRecibo;

        // Usado apenas pelo designer do Visual Studio.
        public FormBalcao()
        {
            InitializeComponent();
        }

        /// <param name="emissorRecibo">Usado só para montar a prévia no ReportViewer.</param>
        public FormBalcao(EmissorRecibo emissorRecibo) : this()
        {
            _emissorRecibo = emissorRecibo;
        }

        public event EventHandler Carregada;
        public event EventHandler BuscarClientesSolicitado;
        public event EventHandler ClienteSelecionado;
        public event EventHandler NovoClienteSolicitado;
        public event EventHandler SalvarClienteSolicitado;
        public event EventHandler ExcluirClienteSolicitado;
        public event EventHandler NovoPedidoSolicitado;
        public event EventHandler MarcarPagoSolicitado;
        public event EventHandler EntregarSolicitado;
        public event EventHandler ReciboSolicitado;

        public string TextoBusca => txtBusca.Text;
        public int? ClienteSelecionadoId => (dgvClientes.CurrentRow?.DataBoundItem as ClienteLinha)?.Id;
        public int? PedidoSelecionadoId => (dgvPedidos.CurrentRow?.DataBoundItem as PedidoLinha)?.Id;
        public TipoCertidao? TipoSelecionado => (cmbTipo.SelectedItem as OpcaoTipoCertidao)?.Tipo;
        public bool Urgente => chkUrgente.Checked;

        public string Nome
        {
            get => txtNome.Text;
            set => txtNome.Text = value;
        }

        public string Cpf
        {
            get => txtCpf.Text;
            set => txtCpf.Text = value;
        }

        public string Telefone
        {
            get => txtTelefone.Text;
            set => txtTelefone.Text = value;
        }

        public void ExibirTipos(IList<OpcaoTipoCertidao> tipos)
        {
            cmbTipo.Items.Clear();
            cmbTipo.Items.AddRange(tipos.Cast<object>().ToArray());
            if (cmbTipo.Items.Count > 0)
                cmbTipo.SelectedIndex = 0;
        }

        public void ExibirClientes(IList<ClienteLinha> clientes)
        {
            dgvClientes.DataSource = clientes;
            dgvClientes.ClearSelection();
        }

        public void ExibirPedidos(IList<PedidoLinha> pedidos) => dgvPedidos.DataSource = pedidos;

        public void ExibirTotalEmAberto(string total) => lblTotal.Text = "Total em aberto: " + total;

        public void ExibirMensagem(string mensagem) =>
            MessageBox.Show(this, mensagem, "Balcão", MessageBoxButtons.OK, MessageBoxIcon.Information);

        public void ExibirAviso(string mensagem) =>
            MessageBox.Show(this, mensagem, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        public void ExibirErro(string mensagem) =>
            MessageBox.Show(this, mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

        public bool Confirmar(string pergunta) =>
            MessageBox.Show(this, pergunta, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

        public bool ExibirPreviaRecibo(ReciboViewModel recibo)
        {
            using (var previa = new FormPreviaRecibo(_emissorRecibo, recibo))
                return previa.ShowDialog(this) == DialogResult.OK;
        }

        public string SolicitarCaminhoDoArquivo(string nomeSugerido)
        {
            using (var dialogo = new SaveFileDialog())
            {
                dialogo.Title = "Salvar recibo";
                dialogo.Filter = "Documento PDF (*.pdf)|*.pdf";
                dialogo.FileName = nomeSugerido;
                return dialogo.ShowDialog(this) == DialogResult.OK ? dialogo.FileName : null;
            }
        }

        public void AbrirDocumento(string caminho) => Process.Start(new ProcessStartInfo(caminho) { UseShellExecute = true });

        private void btnRecibo_Click(object sender, EventArgs e) => ReciboSolicitado?.Invoke(this, EventArgs.Empty);

        private void FormBalcao_Load(object sender, EventArgs e) => Carregada?.Invoke(this, EventArgs.Empty);

        private void btnBuscar_Click(object sender, EventArgs e) => BuscarClientesSolicitado?.Invoke(this, EventArgs.Empty);

        private void dgvClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                ClienteSelecionado?.Invoke(this, EventArgs.Empty);
        }

        private void btnNovoCliente_Click(object sender, EventArgs e) => NovoClienteSolicitado?.Invoke(this, EventArgs.Empty);

        private void btnSalvarCliente_Click(object sender, EventArgs e) => SalvarClienteSolicitado?.Invoke(this, EventArgs.Empty);

        private void btnExcluirCliente_Click(object sender, EventArgs e) => ExcluirClienteSolicitado?.Invoke(this, EventArgs.Empty);

        private void btnNovoPedido_Click(object sender, EventArgs e) => NovoPedidoSolicitado?.Invoke(this, EventArgs.Empty);

        private void btnMarcarPago_Click(object sender, EventArgs e) => MarcarPagoSolicitado?.Invoke(this, EventArgs.Empty);

        private void btnEntregar_Click(object sender, EventArgs e) => EntregarSolicitado?.Invoke(this, EventArgs.Empty);
    }
}
