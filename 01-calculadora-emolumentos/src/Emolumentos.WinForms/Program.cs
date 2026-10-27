using System;
using System.Configuration;
using System.Windows.Forms;
using Emolumentos.Apresentacao;
using Emolumentos.Dados;
using Emolumentos.Documentos;
using Emolumentos.Dominio;

namespace Emolumentos.WinForms
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var configuracao = ConfigurationManager.ConnectionStrings["Emolumentos"];
            if (configuracao == null)
            {
                MessageBox.Show("Connection string \"Emolumentos\" não encontrada no App.config.",
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

            var repositorio = new TabelaEmolumentosRepositorio(configuracao.ConnectionString);
            var calculadora = new CalculadoraEmolumentos(repositorio);
            var emissor = new EmissorDemonstrativo(new AssinadorPdf(certificado));
            var form = new FormCalculadora(emissor);
            var presenter = new CalculoPresenter(form, repositorio, calculadora, emissor);

            Application.Run(form);
            GC.KeepAlive(presenter);
        }
    }
}
