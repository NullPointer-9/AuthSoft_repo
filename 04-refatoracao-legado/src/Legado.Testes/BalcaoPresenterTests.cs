using System;
using System.Globalization;
using System.Linq;
using Legado.Apresentacao;
using Legado.Negocio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Legado.Testes
{
    [TestClass]
    public class BalcaoPresenterTests
    {
        private static readonly CultureInfo PtBr = new CultureInfo("pt-BR");

        private BalcaoViewFalsa _view;
        private ClienteRepositorioEmMemoria _clientes;
        private PedidoRepositorioEmMemoria _pedidos;
        private EmissorReciboFalso _emissor;
        private BalcaoPresenter _presenter;

        [TestInitialize]
        public void Inicializar()
        {
            _view = new BalcaoViewFalsa();
            _clientes = new ClienteRepositorioEmMemoria();
            _pedidos = new PedidoRepositorioEmMemoria();
            _emissor = new EmissorReciboFalso();
            _presenter = new BalcaoPresenter(
                _view,
                new ClienteServico(_clientes, _pedidos),
                new PedidoServico(_pedidos, new RelogioFixo()),
                _emissor);

            _clientes.Clientes.Add(new Cliente { Id = 1, Nome = "MARIA APARECIDA SOUZA", Cpf = "529.982.247-25", Telefone = "11988881111" });
            _clientes.Clientes.Add(new Cliente { Id = 2, Nome = "joao carlos pereira", Cpf = "11144477735", Telefone = "" });
        }

        private static string Moeda(decimal valor) => valor.ToString("C", PtBr);

        [TestMethod]
        public void AoCarregar_MostraTiposEClientes()
        {
            _view.Carregar();

            Assert.AreEqual(3, _view.Tipos.Count);
            Assert.AreEqual(2, _view.Clientes.Count);
        }

        [TestMethod]
        public void SelecionarCliente_PreencheCamposComCpfFormatadoEMostraPedidos()
        {
            _pedidos.Existente(2, "aberto", 45.90m);
            _view.Carregar();

            _view.SelecionarCliente(2);

            Assert.AreEqual("joao carlos pereira", _view.Nome);
            Assert.AreEqual("111.444.777-35", _view.Cpf);
            Assert.AreEqual(1, _view.Pedidos.Count);
            Assert.AreEqual(Moeda(45.90m), _view.Total);
        }

        [TestMethod]
        public void SalvarClienteNovo_CadastraEFicaEditandoOCliente()
        {
            _view.Carregar();
            _view.NovoCliente();
            _view.Nome = "Ana Beatriz Lima";
            _view.Cpf = "12345678909";

            _view.SalvarCliente();

            Assert.AreEqual("Cliente cadastrado.", _view.UltimaMensagem);
            Assert.AreEqual("123.456.789-09", _view.Cpf);
            Assert.AreNotEqual(0, _presenter.IdClienteAtual);
            Assert.AreEqual(3, _view.Clientes.Count);
        }

        [TestMethod]
        public void SalvarComCpfDuplicado_MostraAvisoENaoCadastra()
        {
            _view.Carregar();
            _view.NovoCliente();
            _view.Nome = "Outra Maria";
            _view.Cpf = "52998224725";

            _view.SalvarCliente();

            StringAssert.Contains(_view.UltimoAviso, "já cadastrado");
            Assert.AreEqual(2, _clientes.Clientes.Count);
        }

        [TestMethod]
        public void ExcluirSemClienteSelecionado_Avisa()
        {
            _view.Carregar();

            _view.ExcluirCliente();

            Assert.AreEqual("Selecione um cliente.", _view.UltimoAviso);
        }

        [TestMethod]
        public void ExcluirSemConfirmar_NaoExclui()
        {
            _view.Carregar();
            _view.SelecionarCliente(2);
            _view.RespostaConfirmacao = false;

            _view.ExcluirCliente();

            Assert.AreEqual(2, _clientes.Clientes.Count);
        }

        [TestMethod]
        public void NovoPedido_CriaParaOClienteSelecionadoEAtualizaOTotal()
        {
            _view.Carregar();
            _view.SelecionarCliente(1);
            _view.TipoSelecionado = TipoCertidao.Obito;
            _view.Urgente = true;

            _view.NovoPedido();

            var pedido = _pedidos.Pedidos.Single();
            Assert.AreEqual(1, pedido.IdCliente);
            Assert.AreEqual(78.53m, pedido.Valor);
            Assert.AreEqual(Moeda(78.53m), _view.Total);
        }

        [TestMethod]
        public void NovoPedidoSemCliente_Avisa()
        {
            _view.Carregar();

            _view.NovoPedido();

            Assert.AreEqual("Selecione um cliente.", _view.UltimoAviso);
            Assert.AreEqual(0, _pedidos.Pedidos.Count);
        }

        [TestMethod]
        public void MarcarPagoEEntregar_SeguemOFluxoEZeramOTotal()
        {
            var pedido = _pedidos.Existente(1, "ABERTO", 45.90m);
            _view.Carregar();
            _view.SelecionarCliente(1);
            _view.PedidoSelecionadoId = pedido.Id;

            _view.MarcarPago();
            _view.Entregar();

            Assert.AreEqual(StatusPedido.Entregue, pedido.Status);
            Assert.AreEqual(Moeda(0m), _view.Total);
            Assert.IsNull(_view.UltimoAviso);
        }

        [TestMethod]
        public void EntregarSemPagar_MostraAvisoDaRegra()
        {
            var pedido = _pedidos.Existente(1, "aberto");
            _view.Carregar();
            _view.SelecionarCliente(1);
            _view.PedidoSelecionadoId = pedido.Id;

            _view.Entregar();

            StringAssert.Contains(_view.UltimoAviso, "precisa estar pago");
        }

        [TestMethod]
        public void AcaoNoPedidoSemSelecionar_Avisa()
        {
            _view.Carregar();
            _view.SelecionarCliente(1);

            _view.MarcarPago();

            Assert.AreEqual("Selecione um pedido.", _view.UltimoAviso);
        }

        #region Recibo

        private PedidoCertidao PedidoPagoSelecionado()
        {
            var pedido = _pedidos.Existente(1, "pago", 68.85m);
            _view.Carregar();
            _view.SelecionarCliente(1);
            _view.PedidoSelecionadoId = pedido.Id;
            return pedido;
        }

        [TestMethod]
        public void Recibo_MostraPreviaComDadosDoClienteEDoPedido()
        {
            PedidoPagoSelecionado();
            _view.RespostaDaPrevia = false;

            _view.Recibo();

            Assert.AreEqual("MARIA APARECIDA SOUZA", _view.PreviaExibida.Cliente);
            Assert.AreEqual("529.982.247-25", _view.PreviaExibida.Cpf);
            Assert.AreEqual(Moeda(68.85m), _view.PreviaExibida.Valor);
            Assert.AreEqual(0, _emissor.Emissoes, "fechar a prévia não gera arquivo");
        }

        [TestMethod]
        public void Recibo_ConfirmadoNaPrevia_EmiteNoCaminhoEscolhidoEAbre()
        {
            PedidoPagoSelecionado();

            _view.Recibo();

            Assert.AreEqual(1, _emissor.Emissoes);
            Assert.AreEqual(@"C:\temp\recibo.pdf", _emissor.CaminhoEmitido);
            Assert.AreEqual(@"C:\temp\recibo.pdf", _view.DocumentoAberto);
        }

        [TestMethod]
        public void Recibo_CancelarODialogoDeSalvar_NaoEmite()
        {
            PedidoPagoSelecionado();
            _view.CaminhoEscolhido = null;

            _view.Recibo();

            Assert.AreEqual(0, _emissor.Emissoes);
            Assert.IsNull(_view.DocumentoAberto);
        }

        [TestMethod]
        public void Recibo_PedidoEmAberto_AvisaENaoMostraPrevia()
        {
            var pedido = _pedidos.Existente(1, "aberto");
            _view.Carregar();
            _view.SelecionarCliente(1);
            _view.PedidoSelecionadoId = pedido.Id;

            _view.Recibo();

            StringAssert.Contains(_view.UltimoAviso, "depois que o pedido for pago");
            Assert.IsNull(_view.PreviaExibida);
        }

        [TestMethod]
        public void Recibo_SemPedidoSelecionado_Avisa()
        {
            _view.Carregar();

            _view.Recibo();

            Assert.AreEqual("Selecione um pedido.", _view.UltimoAviso);
        }

        #endregion

        [TestMethod]
        public void ErroInesperado_ViraMensagemDeErroSemDerrubarATela()
        {
            _view.Carregar();
            _view.SelecionarCliente(1);
            _pedidos.Pedidos.Add(null); // dado corrompido: o repositório devolve algo inválido

            _view.NovoPedido();

            StringAssert.StartsWith(_view.UltimoErro, "Ocorreu um erro inesperado");
        }
    }
}
