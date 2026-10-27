using System;
using System.Configuration;
using System.Windows.Forms;
using Legado.Apresentacao;
using Legado.Dados;
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

            var clienteRepositorio = new ClienteRepositorio(configuracao.ConnectionString);
            var pedidoRepositorio = new PedidoRepositorio(configuracao.ConnectionString);
            var clienteServico = new ClienteServico(clienteRepositorio, pedidoRepositorio);
            var pedidoServico = new PedidoServico(pedidoRepositorio, new RelogioSistema());

            var form = new FormBalcao();
            var presenter = new BalcaoPresenter(form, clienteServico, pedidoServico);

            Application.Run(form);
            GC.KeepAlive(presenter);
        }
    }
}
