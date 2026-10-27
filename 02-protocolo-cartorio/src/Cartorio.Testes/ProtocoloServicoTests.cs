using System;
using System.Linq;
using Cartorio.Entidades;
using Cartorio.Negocio;
using Cartorio.Testes.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cartorio.Testes
{
    [TestClass]
    public class ProtocoloServicoTests
    {
        private ClienteRepositorioEmMemoria _clientes;
        private ProtocoloRepositorioEmMemoria _protocolos;
        private RelogioFixo _relogio;
        private ProtocoloServico _servico;
        private Cliente _maria;

        [TestInitialize]
        public void Inicializar()
        {
            _clientes = new ClienteRepositorioEmMemoria();
            _protocolos = new ProtocoloRepositorioEmMemoria();
            _relogio = new RelogioFixo(new DateTime(2026, 5, 20, 14, 30, 0));
            _servico = new ProtocoloServico(_protocolos, _clientes, _relogio);

            _maria = new Cliente { Nome = "Maria Souza", Cpf = "52998224725" };
            _clientes.Adicionar(_maria);
        }

        #region Abertura

        [TestMethod]
        public void Abrir_CriaProtocoloRecebidoComHistoricoInicial()
        {
            var protocolo = _servico.Abrir(_maria.Id, "Procuração", "Venda de veículo");

            Assert.AreEqual("2026-000001", protocolo.Numero);
            Assert.AreEqual(StatusProtocolo.Recebido, protocolo.Status);
            Assert.AreEqual(_relogio.Agora, protocolo.DataAbertura);

            var historico = _protocolos.ListarHistorico(protocolo.Id).Single();
            Assert.IsNull(historico.StatusAnterior);
            Assert.AreEqual(StatusProtocolo.Recebido, historico.StatusNovo);
        }

        [TestMethod]
        public void Abrir_DuasVezes_NumeraEmSequencia()
        {
            _servico.Abrir(_maria.Id, "Procuração", null);
            var segundo = _servico.Abrir(_maria.Id, "Certidão", null);

            Assert.AreEqual("2026-000002", segundo.Numero);
        }

        [TestMethod]
        public void Abrir_NaViradaDoAno_ReiniciaANumeracao()
        {
            _servico.Abrir(_maria.Id, "Procuração", null);
            _relogio.Agora = new DateTime(2027, 1, 2, 8, 0, 0);

            var protocolo = _servico.Abrir(_maria.Id, "Procuração", null);

            Assert.AreEqual("2027-000001", protocolo.Numero);
        }

        [TestMethod]
        public void Abrir_ClienteInexistente_LancaRegistroNaoEncontrado()
        {
            Assert.ThrowsException<RegistroNaoEncontradoException>(() => _servico.Abrir(999, "Procuração", null));
            Assert.AreEqual(0, _protocolos.Protocolos.Count);
        }

        [TestMethod]
        public void Abrir_SemTipoDeServico_LancaNegocioException()
        {
            Assert.ThrowsException<NegocioException>(() => _servico.Abrir(_maria.Id, "   ", null));
        }

        #endregion

        #region Status

        [TestMethod]
        public void AvancarStatus_SegueAOrdemAteEntregue()
        {
            var protocolo = _servico.Abrir(_maria.Id, "Procuração", null);

            Assert.AreEqual(StatusProtocolo.EmAnalise, _servico.AvancarStatus(protocolo.Id, null).Status);
            Assert.AreEqual(StatusProtocolo.Pronto, _servico.AvancarStatus(protocolo.Id, null).Status);
            Assert.AreEqual(StatusProtocolo.Entregue, _servico.AvancarStatus(protocolo.Id, null).Status);
        }

        [TestMethod]
        public void AvancarStatus_GravaHistoricoComStatusAnteriorENovo()
        {
            var protocolo = _servico.Abrir(_maria.Id, "Procuração", null);

            _servico.AvancarStatus(protocolo.Id, "  Conferido pelo escrevente  ");

            var ultimo = _protocolos.ListarHistorico(protocolo.Id).Last();
            Assert.AreEqual(StatusProtocolo.Recebido, ultimo.StatusAnterior);
            Assert.AreEqual(StatusProtocolo.EmAnalise, ultimo.StatusNovo);
            Assert.AreEqual("Conferido pelo escrevente", ultimo.Observacao);
        }

        [TestMethod]
        public void AvancarStatus_AteEntregue_PreencheDataDeEntrega()
        {
            var protocolo = _servico.Abrir(_maria.Id, "Procuração", null);
            _servico.AvancarStatus(protocolo.Id, null);
            _servico.AvancarStatus(protocolo.Id, null);
            Assert.IsNull(protocolo.DataEntrega);

            _relogio.Agora = new DateTime(2026, 5, 22, 10, 0, 0);
            _servico.AvancarStatus(protocolo.Id, null);

            Assert.AreEqual(new DateTime(2026, 5, 22, 10, 0, 0), protocolo.DataEntrega);
            Assert.AreEqual(protocolo.DataEntrega, protocolo.DataAtualizacao);
        }

        [TestMethod]
        public void AvancarStatus_ProtocoloEntregue_LancaNegocioExceptionSemGravarHistorico()
        {
            var protocolo = _servico.Abrir(_maria.Id, "Procuração", null);
            _servico.AvancarStatus(protocolo.Id, null);
            _servico.AvancarStatus(protocolo.Id, null);
            _servico.AvancarStatus(protocolo.Id, null);
            var registrosAntes = _protocolos.Historicos.Count;

            var ex = Assert.ThrowsException<NegocioException>(() => _servico.AvancarStatus(protocolo.Id, null));

            StringAssert.Contains(ex.Message, "já foi entregue");
            Assert.AreEqual(registrosAntes, _protocolos.Historicos.Count);
        }

        [TestMethod]
        public void AvancarStatus_ProtocoloInexistente_LancaRegistroNaoEncontrado()
        {
            Assert.ThrowsException<RegistroNaoEncontradoException>(() => _servico.AvancarStatus(42, null));
        }

        [DataTestMethod]
        [DataRow(StatusProtocolo.Recebido, StatusProtocolo.EmAnalise)]
        [DataRow(StatusProtocolo.EmAnalise, StatusProtocolo.Pronto)]
        [DataRow(StatusProtocolo.Pronto, StatusProtocolo.Entregue)]
        public void RegrasStatus_Proximo(StatusProtocolo atual, StatusProtocolo esperado)
        {
            Assert.AreEqual(esperado, RegrasStatus.Proximo(atual));
        }

        [TestMethod]
        public void RegrasStatus_EntregueNaoTemProximo()
        {
            Assert.IsNull(RegrasStatus.Proximo(StatusProtocolo.Entregue));
        }

        #endregion

        #region Busca

        [TestMethod]
        public void BuscarPorNumero_Inexistente_LancaRegistroNaoEncontrado()
        {
            Assert.ThrowsException<RegistroNaoEncontradoException>(() => _servico.BuscarPorNumero("2026-999999"));
        }

        [TestMethod]
        public void BuscarPorNumero_IgnoraEspacos()
        {
            var protocolo = _servico.Abrir(_maria.Id, "Procuração", null);

            Assert.AreSame(protocolo, _servico.BuscarPorNumero("  2026-000001 "));
        }

        [TestMethod]
        public void BuscarPorCpf_AceitaCpfFormatado()
        {
            _servico.Abrir(_maria.Id, "Procuração", null);
            _servico.Abrir(_maria.Id, "Certidão", null);

            Assert.AreEqual(2, _servico.BuscarPorCpf("529.982.247-25").Count);
        }

        [TestMethod]
        public void BuscarPorCpf_Invalido_LancaNegocioException()
        {
            Assert.ThrowsException<NegocioException>(() => _servico.BuscarPorCpf("123"));
        }

        #endregion

        #region Comprovantes e relatório

        [TestMethod]
        public void ObterParaComprovante_Abertura_ValeParaQualquerStatus()
        {
            var protocolo = _servico.Abrir(_maria.Id, "Procuração", null);

            Assert.AreSame(protocolo, _servico.ObterParaComprovante(protocolo.Id, TipoComprovante.Abertura));
        }

        [TestMethod]
        public void ObterParaComprovante_EntregaAntesDeEntregar_LancaNegocioException()
        {
            var protocolo = _servico.Abrir(_maria.Id, "Procuração", null);
            _servico.AvancarStatus(protocolo.Id, null);
            _servico.AvancarStatus(protocolo.Id, null);

            var ex = Assert.ThrowsException<NegocioException>(
                () => _servico.ObterParaComprovante(protocolo.Id, TipoComprovante.Entrega));

            StringAssert.Contains(ex.Message, "depois que o protocolo");
        }

        [TestMethod]
        public void ObterParaComprovante_EntregaDeProtocoloEntregue_Libera()
        {
            var protocolo = _servico.Abrir(_maria.Id, "Procuração", null);
            _servico.AvancarStatus(protocolo.Id, null);
            _servico.AvancarStatus(protocolo.Id, null);
            _servico.AvancarStatus(protocolo.Id, null);

            Assert.AreSame(protocolo, _servico.ObterParaComprovante(protocolo.Id, TipoComprovante.Entrega));
        }

        [TestMethod]
        public void ObterParaComprovante_ProtocoloInexistente_LancaRegistroNaoEncontrado()
        {
            Assert.ThrowsException<RegistroNaoEncontradoException>(
                () => _servico.ObterParaComprovante(99, TipoComprovante.Abertura));
        }

        [TestMethod]
        public void ConsultarParaRelatorio_IncluiODiaFinalInteiro()
        {
            _relogio.Agora = new DateTime(2026, 5, 20, 23, 59, 0);
            _servico.Abrir(_maria.Id, "Procuração", null);
            _relogio.Agora = new DateTime(2026, 5, 21, 0, 0, 0);
            _servico.Abrir(_maria.Id, "Certidão", null);

            var resultado = _servico.ConsultarParaRelatorio(new DateTime(2026, 5, 1), new DateTime(2026, 5, 20), null);

            Assert.AreEqual(1, resultado.Count);
            Assert.AreEqual("Procuração", resultado[0].TipoServico);
        }

        [TestMethod]
        public void ConsultarParaRelatorio_FiltraPorStatus()
        {
            var primeiro = _servico.Abrir(_maria.Id, "Procuração", null);
            _servico.Abrir(_maria.Id, "Certidão", null);
            _servico.AvancarStatus(primeiro.Id, null);

            var emAnalise = _servico.ConsultarParaRelatorio(new DateTime(2026, 5, 1), new DateTime(2026, 5, 31), StatusProtocolo.EmAnalise);

            Assert.AreEqual(1, emAnalise.Count);
            Assert.AreEqual(primeiro.Numero, emAnalise[0].Numero);
        }

        [TestMethod]
        public void ConsultarParaRelatorio_DataFinalAntesDaInicial_LancaNegocioException()
        {
            Assert.ThrowsException<NegocioException>(
                () => _servico.ConsultarParaRelatorio(new DateTime(2026, 5, 10), new DateTime(2026, 5, 9), null));
        }

        [TestMethod]
        public void ConsultarParaRelatorio_PeriodoMaiorQueUmAno_LancaNegocioException()
        {
            Assert.ThrowsException<NegocioException>(
                () => _servico.ConsultarParaRelatorio(new DateTime(2025, 1, 1), new DateTime(2026, 1, 2), null));
        }

        #endregion

        #region Numeração

        [DataTestMethod]
        [DataRow(null, "2026-000001")]
        [DataRow("2026-000001", "2026-000002")]
        [DataRow("2026-000099", "2026-000100")]
        [DataRow("2026-123456", "2026-123457")]
        public void GerarProximoNumero(string ultimo, string esperado)
        {
            Assert.AreEqual(esperado, ProtocoloServico.GerarProximoNumero(2026, ultimo));
        }

        [TestMethod]
        public void GerarProximoNumero_NoLimite_LancaNegocioException()
        {
            Assert.ThrowsException<NegocioException>(() => ProtocoloServico.GerarProximoNumero(2026, "2026-999999"));
        }

        [DataTestMethod]
        [DataRow("2025-000010")]    // número de outro ano
        [DataRow("2026000010")]     // sem hífen
        [DataRow("2026-00A010")]
        public void GerarProximoNumero_ForaDoPadrao_LancaInvalidOperation(string ultimo)
        {
            Assert.ThrowsException<InvalidOperationException>(() => ProtocoloServico.GerarProximoNumero(2026, ultimo));
        }

        #endregion
    }
}
