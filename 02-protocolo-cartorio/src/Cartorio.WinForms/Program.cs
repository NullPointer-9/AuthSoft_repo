using System;
using System.Configuration;
using System.Windows.Forms;
using Cartorio.Dados;
using Cartorio.Documentos;
using Cartorio.Negocio;

namespace Cartorio.WinForms
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Exceção não tratada vira mensagem e log, sem derrubar o app.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => TratamentoDeErros.Exibir(null, e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => TratamentoDeErros.Registrar(e.ExceptionObject as Exception);

            var configuracao = ConfigurationManager.ConnectionStrings["CartorioContext"];
            if (configuracao == null)
            {
                MessageBox.Show("Connection string \"CartorioContext\" não encontrada no App.config.",
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
                TratamentoDeErros.Registrar(ex);
                MessageBox.Show("Não foi possível carregar o certificado de assinatura.\n\nDetalhe: " + ex.Message,
                    "Certificado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var relogio = new RelogioSistema();
            var clienteRepositorio = new ClienteRepositorio(configuracao.ConnectionString);
            var protocoloRepositorio = new ProtocoloRepositorio(configuracao.ConnectionString);

            var clienteServico = new ClienteServico(clienteRepositorio, relogio);
            var protocoloServico = new ProtocoloServico(protocoloRepositorio, clienteRepositorio, relogio);

            var emissorComprovante = new EmissorComprovante(new AssinadorPdf(certificado));
            var geradorRelatorio = new GeradorRelatorioProtocolos();

            Application.Run(new FormPrincipal(clienteServico, protocoloServico, emissorComprovante, geradorRelatorio));
        }
    }
}
