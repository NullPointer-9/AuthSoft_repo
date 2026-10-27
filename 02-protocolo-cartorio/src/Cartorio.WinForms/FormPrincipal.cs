using System;
using System.Windows.Forms;
using Cartorio.Documentos;
using Cartorio.Negocio;

namespace Cartorio.WinForms
{
    public partial class FormPrincipal : Form
    {
        private readonly ClienteServico _clienteServico;
        private readonly ProtocoloServico _protocoloServico;
        private readonly EmissorComprovante _emissorComprovante;
        private readonly GeradorRelatorioProtocolos _geradorRelatorio;

        // Usado apenas pelo designer do Visual Studio.
        public FormPrincipal()
        {
            InitializeComponent();
        }

        public FormPrincipal(
            ClienteServico clienteServico,
            ProtocoloServico protocoloServico,
            EmissorComprovante emissorComprovante,
            GeradorRelatorioProtocolos geradorRelatorio) : this()
        {
            _clienteServico = clienteServico;
            _protocoloServico = protocoloServico;
            _emissorComprovante = emissorComprovante;
            _geradorRelatorio = geradorRelatorio;
        }

        private void btnRelatorios_Click(object sender, EventArgs e)
        {
            using (var form = new FormRelatorioProtocolos(_protocoloServico, _geradorRelatorio))
                form.ShowDialog(this);
        }

        private void btnClientes_Click(object sender, EventArgs e)
        {
            using (var form = new FormClientes(_clienteServico))
                form.ShowDialog(this);
        }

        private void btnProtocolos_Click(object sender, EventArgs e)
        {
            using (var form = new FormProtocolos(_protocoloServico, _clienteServico, _emissorComprovante))
                form.ShowDialog(this);
        }
    }
}
