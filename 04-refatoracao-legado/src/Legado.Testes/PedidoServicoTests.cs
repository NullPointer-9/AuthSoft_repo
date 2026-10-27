using System;
using Legado.Negocio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Legado.Testes
{
    [TestClass]
    public class PedidoServicoTests
    {
        private PedidoRepositorioEmMemoria _pedidos;
        private RelogioFixo _relogio;
        private PedidoServico _servico;

        [TestInitialize]
        public void Inicializar()
        {
            _pedidos = new PedidoRepositorioEmMemoria();
            _relogio = new RelogioFixo();
            _servico = new PedidoServico(_pedidos, _relogio);
        }

        [TestMethod]
        public void Criar_GravaAbertoComPrecoDaTabelaEDataDoDia()
        {
            var pedido = _servico.Criar(1, TipoCertidao.Obito, urgente: false);

            Assert.AreEqual(StatusPedido.Aberto, pedido.Status);
            Assert.AreEqual(52.35m, pedido.Valor);
            Assert.AreEqual("Certidao de obito", pedido.Tipo);
            Assert.AreEqual(new DateTime(2026, 10, 27), pedido.DataPedido);
        }

        [TestMethod]
        public void Criar_SemCliente_LancaNegocioException()
        {
            Assert.ThrowsException<NegocioException>(() => _servico.Criar(0, TipoCertidao.Nascimento, false));
        }

        #region Bug 1: "Aberto" com maiúscula não podia ser pago

        [DataTestMethod]
        [DataRow("aberto")]
        [DataRow("Aberto")]
        [DataRow("ABERTO")]
        [DataRow(" aberto ")]
        public void MarcarPago_AceitaQualquerGrafiaDeAbertoGravadaPeloLegado(string statusNoBanco)
        {
            var pedido = _pedidos.Existente(1, statusNoBanco);

            _servico.MarcarPago(pedido.Id);

            Assert.AreEqual(StatusPedido.Pago, pedido.Status);
        }

        [TestMethod]
        public void MarcarPago_PedidoJaPago_LancaNegocioException()
        {
            var pedido = _pedidos.Existente(1, "PAGO");

            Assert.ThrowsException<NegocioException>(() => _servico.MarcarPago(pedido.Id));
        }

        #endregion

        #region Bug 2: dava para entregar pedido não pago

        [TestMethod]
        public void Entregar_PedidoEmAberto_LancaNegocioExceptionENaoMudaStatus()
        {
            var pedido = _pedidos.Existente(1, "aberto");

            var ex = Assert.ThrowsException<NegocioException>(() => _servico.Entregar(pedido.Id));

            StringAssert.Contains(ex.Message, "precisa estar pago");
            Assert.AreEqual(StatusPedido.Aberto, pedido.Status);
        }

        [TestMethod]
        public void Entregar_PedidoPago_Entrega()
        {
            var pedido = _pedidos.Existente(1, "pago");

            _servico.Entregar(pedido.Id);

            Assert.AreEqual(StatusPedido.Entregue, pedido.Status);
        }

        [TestMethod]
        public void Entregar_PedidoJaEntregue_AvisaQueJaFoiEntregue()
        {
            var pedido = _pedidos.Existente(1, "entregue");

            var ex = Assert.ThrowsException<NegocioException>(() => _servico.Entregar(pedido.Id));

            StringAssert.Contains(ex.Message, "já foi entregue");
        }

        #endregion

        [DataTestMethod]
        [DataRow("pago")]
        [DataRow("Entregue")]
        public void ObterParaRecibo_PedidoPagoOuEntregue_Libera(string status)
        {
            var pedido = _pedidos.Existente(1, status);

            Assert.AreSame(pedido, _servico.ObterParaRecibo(pedido.Id));
        }

        [TestMethod]
        public void ObterParaRecibo_PedidoEmAberto_LancaNegocioException()
        {
            var pedido = _pedidos.Existente(1, "aberto");

            Assert.ThrowsException<NegocioException>(() => _servico.ObterParaRecibo(pedido.Id));
        }

        [TestMethod]
        public void PedidoInexistente_LancaNegocioException()
        {
            Assert.ThrowsException<NegocioException>(() => _servico.MarcarPago(999));
            Assert.ThrowsException<NegocioException>(() => _servico.Entregar(999));
        }

        [TestMethod]
        public void TotalEmAberto_SomaSoOsAbertosDoCliente()
        {
            _pedidos.Existente(1, "aberto", 45.90m);
            _pedidos.Existente(1, "Aberto", 68.85m);
            _pedidos.Existente(1, "pago", 52.35m);
            _pedidos.Existente(2, "aberto", 100m);

            Assert.AreEqual(114.75m, _servico.TotalEmAberto(1));
        }

        [TestMethod]
        public void TotalEmAberto_SemPedidos_EhZero()
        {
            Assert.AreEqual(0m, _servico.TotalEmAberto(1));
        }
    }
}
