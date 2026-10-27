using System;
using System.Configuration;
using System.Windows.Forms;
using Legado.Apresentacao;
using Legado.Dados;
using Legado.Documentos;
using Legado.Negocio;

namespace Legado.WinForms
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var configuracao = ConfigurationManager.ConnectionStrings["LegadoCartorio"];
            if (configuracao == null)
            {
                MessageBox.Show("Connection string \"LegadoCartorio\" não encontrada no App.config.",
                    "Configuração", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CertificadoAssinatura certificado;
            try
            {
                certificado = CertificadoAssinatura.ObterOuCriarDeTeste(
                    Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["CertificadoPfx"]),
                    ConfigurationManager.AppSettings["CertificadoSenha"],
                    ConfigurationManager.AppSettings["CertificadoTitular"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível carregar o certificado de assinatura.\n\nDetalhe: " + ex.Message,
                    "Certificado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var clienteRepositorio = new ClienteRepositorio(configuracao.ConnectionString);
            var pedidoRepositorio = new PedidoRepositorio(configuracao.ConnectionString);
            var clienteServico = new ClienteServico(clienteRepositorio, pedidoRepositorio);
            var pedidoServico = new PedidoServico(pedidoRepositorio, new RelogioSistema());

            var emissorRecibo = new EmissorRecibo(new AssinadorPdf(certificado));

            var form = new FormBalcao(emissorRecibo);
            var presenter = new BalcaoPresenter(form, clienteServico, pedidoServico, emissorRecibo);

            Application.Run(form);
            GC.KeepAlive(presenter);
        }
    }
}
