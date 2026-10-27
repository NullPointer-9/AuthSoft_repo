using System.Globalization;
using Emolumentos.Apresentacao;
using Emolumentos.Dominio;
using Emolumentos.Testes.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emolumentos.Testes
{
    [TestClass]
    public class CalculoPresenterTests
    {
        private static readonly CultureInfo PtBr = new CultureInfo("pt-BR");

        private CalculoViewFalsa _view;
        private TabelaEmolumentosEmMemoria _repositorio;
        private EmissorDemonstrativoFalso _emissor;

        [TestInitialize]
        public void Inicializar()
        {
            _view = new CalculoViewFalsa();
            _repositorio = new TabelaEmolumentosEmMemoria();
            _emissor = new EmissorDemonstrativoFalso();
            new CalculoPresenter(_view, _repositorio, new CalculadoraEmolumentos(_repositorio), _emissor);
        }

        private void CalcularEscrituraSp()
        {
            _view.SimularCarregamento();
            _view.UfSelecionada = "SP";
            _view.TipoAtoSelecionadoId = TabelaEmolumentosEmMemoria.Escritura.Id;
            _view.ValorNegocioTexto = "150.000,00";
            _view.SimularCliqueCalcular();
        }

        #region Emissão do demonstrativo

        [TestMethod]
        public void Emissao_ComecaDesabilitada_EHabilitaDepoisDoCalculo()
        {
            _view.SimularCarregamento();
            Assert.AreEqual(false, _view.EmissaoHabilitada);

            CalcularEscrituraSp();
            Assert.AreEqual(true, _view.EmissaoHabilitada);
        }

        [TestMethod]
        public void Emissao_CalculoComErro_DesabilitaDeNovo()
        {
            CalcularEscrituraSp();

            _view.UfSelecionada = "RJ";
            _view.SimularCliqueCalcular();

            Assert.AreEqual(false, _view.EmissaoHabilitada);
        }

        [TestMethod]
        public void Emitir_SalvaNoCaminhoEscolhidoEAbreODocumento()
        {
            CalcularEscrituraSp();
            _view.CaminhoEscolhido = @"C:\temp\demo.pdf";

            _view.SimularCliqueEmitir();

            Assert.AreEqual("demonstrativo-emolumentos-SP.pdf", _view.NomeSugerido);
            Assert.AreEqual(@"C:\temp\demo.pdf", _emissor.CaminhoEmitido);
            Assert.AreEqual(5400m.ToString("C", PtBr), _emissor.ResultadoEmitido.Total);
            Assert.AreEqual(@"C:\temp\demo.pdf", _view.DocumentoAberto);
        }

        [TestMethod]
        public void Emitir_UsuarioCancelaODialogo_NaoEmite()
        {
            CalcularEscrituraSp();
            _view.CaminhoEscolhido = null;

            _view.SimularCliqueEmitir();

            Assert.AreEqual(0, _emissor.Emissoes);
            Assert.IsNull(_view.DocumentoAberto);
        }

        [TestMethod]
        public void Emitir_SemCalculo_ExibeErro()
        {
            _view.SimularCarregamento();
            _view.CaminhoEscolhido = @"C:\temp\demo.pdf";

            _view.SimularCliqueEmitir();

            Assert.AreEqual(0, _emissor.Emissoes);
            StringAssert.Contains(_view.UltimoErro, "Calcule");
        }

        [TestMethod]
        public void Emitir_FalhaAoGerar_ExibeErroENaoAbre()
        {
            CalcularEscrituraSp();
            _view.CaminhoEscolhido = @"C:\temp\demo.pdf";
            _emissor.Falhar = true;

            _view.SimularCliqueEmitir();

            StringAssert.Contains(_view.UltimoErro, "Não foi possível emitir");
            Assert.IsNull(_view.DocumentoAberto);
        }

        [TestMethod]
        public void Previa_MostraOResultadoDoUltimoCalculo()
        {
            CalcularEscrituraSp();

            _view.SimularCliquePrevia();

            Assert.AreEqual(5400m.ToString("C", PtBr), _view.PreviaExibida.Total);
        }

        [TestMethod]
        public void Previa_FechadaSemEmitir_NaoGeraDocumento()
        {
            CalcularEscrituraSp();
            _view.CaminhoEscolhido = @"C:\temp\demo.pdf";
            _view.RespostaDaPrevia = false;

            _view.SimularCliquePrevia();

            Assert.AreEqual(0, _emissor.Emissoes);
            Assert.IsNull(_view.NomeSugerido, "não deveria nem perguntar onde salvar");
        }

        [TestMethod]
        public void Previa_EmitirAPartirDela_SegueOFluxoNormalDeEmissao()
        {
            CalcularEscrituraSp();
            _view.CaminhoEscolhido = @"C:\temp\demo.pdf";
            _view.RespostaDaPrevia = true;

            _view.SimularCliquePrevia();

            Assert.AreEqual(1, _emissor.Emissoes);
            Assert.AreEqual(@"C:\temp\demo.pdf", _view.DocumentoAberto);
        }

        [TestMethod]
        public void Previa_SemCalculo_ExibeErro()
        {
            _view.SimularCarregamento();

            _view.SimularCliquePrevia();

            Assert.IsNull(_view.PreviaExibida);
            StringAssert.Contains(_view.UltimoErro, "Calcule");
        }

        #endregion

        [TestMethod]
        public void AoCarregar_ExibeUfsETiposDeAto()
        {
            _view.SimularCarregamento();

            Assert.AreEqual(3, _view.UfsExibidas.Count);
            Assert.AreEqual(3, _view.TiposAtoExibidos.Count);
            Assert.IsNull(_view.UltimoErro);
        }

        [TestMethod]
        public void AoCarregar_ComBancoFora_ExibeErroAmigavel()
        {
            _repositorio.FalharAoListar = true;

            _view.SimularCarregamento();

            StringAssert.Contains(_view.UltimoErro, "Não foi possível carregar");
        }

        [TestMethod]
        public void Calcular_ComDadosValidos_ExibeResultadoFormatado()
        {
            _view.SimularCarregamento();
            _view.UfSelecionada = "SP";
            _view.TipoAtoSelecionadoId = TabelaEmolumentosEmMemoria.Escritura.Id;
            _view.ValorNegocioTexto = "150.000,00";

            _view.SimularCliqueCalcular();

            Assert.IsNull(_view.UltimoErro);
            Assert.AreEqual(4100m.ToString("C", PtBr), _view.ResultadoExibido.Emolumento);
            Assert.AreEqual(5400m.ToString("C", PtBr), _view.ResultadoExibido.Total);
            StringAssert.StartsWith(_view.ResultadoExibido.Faixa, "De ");
        }

        [TestMethod]
        public void Calcular_ComValorInvalido_ExibeErroENaoExibeResultado()
        {
            _view.SimularCarregamento();
            _view.UfSelecionada = "SP";
            _view.TipoAtoSelecionadoId = TabelaEmolumentosEmMemoria.Escritura.Id;
            _view.ValorNegocioTexto = "cem mil";

            _view.SimularCliqueCalcular();

            Assert.IsNotNull(_view.UltimoErro);
            Assert.IsNull(_view.ResultadoExibido);
        }

        [TestMethod]
        public void Calcular_UfSemTabela_ExibeMensagemDoDominio()
        {
            _view.SimularCarregamento();
            _view.UfSelecionada = "RJ";
            _view.TipoAtoSelecionadoId = TabelaEmolumentosEmMemoria.Escritura.Id;
            _view.ValorNegocioTexto = "1000";

            _view.SimularCliqueCalcular();

            StringAssert.Contains(_view.UltimoErro, "RJ");
            Assert.IsNull(_view.ResultadoExibido);
        }

        [TestMethod]
        public void TrocarParaAtoSemValor_DesabilitaCampoDeValor()
        {
            _view.SimularCarregamento();

            _view.SimularTrocaDeAto(TabelaEmolumentosEmMemoria.Procuracao.Id);
            Assert.AreEqual(false, _view.ValorHabilitado);

            _view.SimularTrocaDeAto(TabelaEmolumentosEmMemoria.Escritura.Id);
            Assert.AreEqual(true, _view.ValorHabilitado);
        }

        [TestMethod]
        public void Calcular_AtoSemValor_NaoExigeValorDigitado()
        {
            _view.SimularCarregamento();
            _view.UfSelecionada = "SP";
            _view.TipoAtoSelecionadoId = TabelaEmolumentosEmMemoria.Firma.Id;
            _view.ValorNegocioTexto = "";

            _view.SimularCliqueCalcular();

            Assert.IsNull(_view.UltimoErro);
            Assert.AreEqual("Valor fixo", _view.ResultadoExibido.Faixa);
        }

        [DataTestMethod]
        [DataRow("150000", "150000")]
        [DataRow("150.000,00", "150000")]
        [DataRow("R$ 150.000,50", "150000.50")]
        [DataRow("  1.234,5 ", "1234.5")]
        public void TentarLerValor_AceitaFormatosBrasileiros(string texto, string esperado)
        {
            decimal valor;

            Assert.IsTrue(CalculoPresenter.TentarLerValor(texto, out valor));
            Assert.AreEqual(decimal.Parse(esperado, CultureInfo.InvariantCulture), valor);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("abc")]
        public void TentarLerValor_RejeitaTextoInvalido(string texto)
        {
            decimal valor;

            Assert.IsFalse(CalculoPresenter.TentarLerValor(texto, out valor));
        }
    }
}
