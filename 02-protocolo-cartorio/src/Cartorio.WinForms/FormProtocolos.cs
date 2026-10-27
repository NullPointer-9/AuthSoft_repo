using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Cartorio.Documentos;
using Cartorio.Entidades;
using Cartorio.Negocio;

namespace Cartorio.WinForms
{
    public partial class FormProtocolos : Form
    {
        private readonly ProtocoloServico _protocoloServico;
        private readonly ClienteServico _clienteServico;
        private readonly EmissorComprovante _emissorComprovante;

        private IList<Protocolo> _protocolosExibidos = new List<Protocolo>();
        private Func<IList<Protocolo>> _ultimaConsulta;
        private Protocolo _protocoloSelecionado;
        private Cliente _clienteDoNovoProtocolo;

        // Usado apenas pelo designer do Visual Studio.
        public FormProtocolos()
        {
            InitializeComponent();
        }

        public FormProtocolos(ProtocoloServico protocoloServico, ClienteServico clienteServico, EmissorComprovante emissorComprovante) : this()
        {
            _protocoloServico = protocoloServico;
            _clienteServico = clienteServico;
            _emissorComprovante = emissorComprovante;
        }

        private void FormProtocolos_Shown(object sender, EventArgs e)
        {
            Consultar(() => _protocoloServico.ListarRecentes());
        }

        #region Busca

        private void btnRecentes_Click(object sender, EventArgs e)
        {
            Consultar(() => _protocoloServico.ListarRecentes());
        }

        private void btnBuscarNumero_Click(object sender, EventArgs e)
        {
            var numero = txtBusca.Text;
            Consultar(() => new List<Protocolo> { _protocoloServico.BuscarPorNumero(numero) });
        }

        private void btnBuscarCpf_Click(object sender, EventArgs e)
        {
            var cpf = txtBusca.Text;
            Consultar(() => _protocoloServico.BuscarPorCpf(cpf));
        }

        private void Consultar(Func<IList<Protocolo>> consulta, int? selecionarProtocoloId = null)
        {
            TratamentoDeErros.Executar(this, () =>
            {
                _protocolosExibidos = consulta();
                _ultimaConsulta = consulta;

                dgvProtocolos.DataSource = _protocolosExibidos.Select(p => new ProtocoloLinha(p)).ToList();
                lblResultado.Text = _protocolosExibidos.Count == 0
                    ? "Nenhum protocolo encontrado."
                    : _protocolosExibidos.Count + " protocolo(s).";

                if (selecionarProtocoloId.HasValue)
                    SelecionarLinha(selecionarProtocoloId.Value);
            });

            AtualizarPainelDeStatus();
        }

        private void SelecionarLinha(int protocoloId)
        {
            foreach (DataGridViewRow linha in dgvProtocolos.Rows)
            {
                var item = linha.DataBoundItem as ProtocoloLinha;
                if (item != null && item.Id == protocoloId)
                {
                    dgvProtocolos.CurrentCell = linha.Cells[0];
                    return;
                }
            }
        }

        #endregion

        #region Status

        private void dgvProtocolos_SelectionChanged(object sender, EventArgs e)
        {
            AtualizarPainelDeStatus();
        }

        private void AtualizarPainelDeStatus()
        {
            var linha = dgvProtocolos.CurrentRow?.DataBoundItem as ProtocoloLinha;
            var selecionado = linha == null ? null : _protocolosExibidos.FirstOrDefault(p => p.Id == linha.Id);

            // O SelectionChanged dispara várias vezes ao trocar o DataSource; evita reconsultar o histórico à toa.
            if (selecionado != null && _protocoloSelecionado != null && selecionado == _protocoloSelecionado)
                return;

            _protocoloSelecionado = selecionado;

            if (selecionado == null)
            {
                lblSelecionado.Text = "Selecione um protocolo na lista.";
                btnAvancar.Text = "Avançar status";
                btnAvancar.Enabled = false;
                btnComprovanteAbertura.Enabled = false;
                btnComprovanteEntrega.Enabled = false;
                dgvHistorico.DataSource = null;
                return;
            }

            btnComprovanteAbertura.Enabled = true;
            btnComprovanteEntrega.Enabled = selecionado.Status == StatusProtocolo.Entregue;

            lblSelecionado.Text = "Protocolo " + selecionado.Numero + " - " + selecionado.Cliente?.Nome +
                                  " - status atual: " + RegrasStatus.Descricao(selecionado.Status);

            var proximo = RegrasStatus.Proximo(selecionado.Status);
            btnAvancar.Enabled = proximo.HasValue;
            btnAvancar.Text = proximo.HasValue
                ? "Avançar para: " + RegrasStatus.Descricao(proximo.Value)
                : "Protocolo já entregue";

            TratamentoDeErros.Executar(this, () =>
            {
                dgvHistorico.DataSource = _protocoloServico.ListarHistorico(selecionado.Id)
                    .Select(h => new HistoricoLinha(h))
                    .ToList();
            });
        }

        private void btnAvancar_Click(object sender, EventArgs e)
        {
            var protocolo = _protocoloSelecionado;
            if (protocolo == null)
                return;

            var proximo = RegrasStatus.Proximo(protocolo.Status);
            if (proximo == StatusProtocolo.Entregue)
            {
                var confirmacao = MessageBox.Show(this,
                    "Confirmar a entrega do protocolo " + protocolo.Numero + " para " + protocolo.Cliente?.Nome + "?",
                    "Entrega", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmacao != DialogResult.Yes)
                    return;
            }

            var observacao = txtObservacao.Text;
            if (!TratamentoDeErros.Executar(this, () => _protocoloServico.AvancarStatus(protocolo.Id, observacao)))
                return;

            txtObservacao.Clear();
            _protocoloSelecionado = null;
            Consultar(_ultimaConsulta ?? (() => _protocoloServico.ListarRecentes()), protocolo.Id);

            if (proximo == StatusProtocolo.Entregue && Perguntar("Deseja emitir o comprovante de entrega agora?"))
                EmitirComprovante(protocolo.Id, TipoComprovante.Entrega);
        }

        #endregion

        #region Comprovantes

        private void btnComprovanteAbertura_Click(object sender, EventArgs e)
        {
            if (_protocoloSelecionado != null)
                EmitirComprovante(_protocoloSelecionado.Id, TipoComprovante.Abertura);
        }

        private void btnComprovanteEntrega_Click(object sender, EventArgs e)
        {
            if (_protocoloSelecionado != null)
                EmitirComprovante(_protocoloSelecionado.Id, TipoComprovante.Entrega);
        }

        /// <summary>Prévia e, se confirmado, PDF assinado. A regra de quando pode emitir fica no Negócio.</summary>
        private void EmitirComprovante(int protocoloId, TipoComprovante tipo)
        {
            Protocolo protocolo = null;
            if (!TratamentoDeErros.Executar(this, () => protocolo = _protocoloServico.ObterParaComprovante(protocoloId, tipo)))
                return;

            var titulo = (tipo == TipoComprovante.Entrega ? "Comprovante de entrega" : "Comprovante de protocolo") +
                         " - " + protocolo.Numero;

            using (var previa = new FormPreviaDocumento(titulo, relatorio => _emissorComprovante.ConfigurarRelatorio(relatorio, protocolo, tipo)))
            {
                if (previa.ShowDialog(this) != DialogResult.OK)
                    return;
            }

            string caminho;
            using (var dialogo = new SaveFileDialog())
            {
                dialogo.Title = "Salvar comprovante";
                dialogo.Filter = "Documento PDF (*.pdf)|*.pdf";
                dialogo.FileName = EmissorComprovante.NomeSugerido(protocolo, tipo);
                if (dialogo.ShowDialog(this) != DialogResult.OK)
                    return;
                caminho = dialogo.FileName;
            }

            if (TratamentoDeErros.Executar(this, () => _emissorComprovante.Emitir(protocolo, tipo, caminho)))
                Process.Start(new ProcessStartInfo(caminho) { UseShellExecute = true });
        }

        private bool Perguntar(string pergunta)
        {
            return MessageBox.Show(this, pergunta, "Comprovante", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        #endregion

        #region Novo protocolo

        private void mtbCpfCliente_TextChanged(object sender, EventArgs e)
        {
            // Mudou o CPF: o cliente localizado antes não vale mais.
            _clienteDoNovoProtocolo = null;
            lblClienteEncontrado.Text = "Nenhum cliente localizado.";
        }

        private void btnLocalizarCliente_Click(object sender, EventArgs e)
        {
            Cliente cliente = null;
            var cpf = mtbCpfCliente.Text;
            if (!TratamentoDeErros.Executar(this, () => cliente = _clienteServico.ObterPorCpf(cpf)))
                return;

            _clienteDoNovoProtocolo = cliente;
            lblClienteEncontrado.Text = "Cliente: " + cliente.Nome;
            cmbTipoServico.Focus();
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            var cliente = _clienteDoNovoProtocolo;
            if (cliente == null)
            {
                MessageBox.Show(this, "Localize o cliente pelo CPF antes de abrir o protocolo.",
                    "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                mtbCpfCliente.Focus();
                return;
            }

            Protocolo aberto = null;
            var tipoServico = cmbTipoServico.Text;
            var descricao = txtDescricao.Text;
            if (!TratamentoDeErros.Executar(this, () => aberto = _protocoloServico.Abrir(cliente.Id, tipoServico, descricao)))
                return;

            txtDescricao.Clear();
            Consultar(() => _protocoloServico.ListarRecentes(), aberto.Id);

            if (Perguntar("Protocolo " + aberto.Numero + " aberto para " + cliente.Nome + ".\n\n" +
                          "Deseja emitir o comprovante de protocolo para o cliente?"))
            {
                EmitirComprovante(aberto.Id, TipoComprovante.Abertura);
            }
        }

        #endregion
    }
}
