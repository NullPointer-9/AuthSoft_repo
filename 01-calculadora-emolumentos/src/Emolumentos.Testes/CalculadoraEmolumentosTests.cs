using Emolumentos.Dominio;
using Emolumentos.Testes.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emolumentos.Testes
{
    [TestClass]
    public class CalculadoraEmolumentosTests
    {
        private CalculadoraEmolumentos _calculadora;

        [TestInitialize]
        public void Inicializar()
        {
            _calculadora = new CalculadoraEmolumentos(new TabelaEmolumentosEmMemoria());
        }

        [DataTestMethod]
        [DataRow("0.01", "1200.00")]        // primeiro centavo da primeira faixa
        [DataRow("20000.00", "1200.00")]    // limite superior é inclusivo
        [DataRow("20000.01", "2400.00")]    // um centavo acima já é a próxima faixa
        [DataRow("100000.00", "2400.00")]
        [DataRow("100000.01", "4100.00")]
        [DataRow("500000.00", "4100.00")]
        [DataRow("500000.01", "6800.00")]
        public void Escritura_SP_UsaAFaixaCertaNosLimites(string valor, string emolumentoEsperado)
        {
            var resultado = _calculadora.Calcular("SP", TabelaEmolumentosEmMemoria.Escritura, decimal.Parse(valor, System.Globalization.CultureInfo.InvariantCulture));

            Assert.AreEqual(decimal.Parse(emolumentoEsperado, System.Globalization.CultureInfo.InvariantCulture), resultado.Emolumento);
        }

        [TestMethod]
        public void UltimaFaixa_SemTeto_AceitaValoresMuitoAltos()
        {
            var resultado = _calculadora.Calcular("SP", TabelaEmolumentosEmMemoria.Escritura, 250000000m);

            Assert.AreEqual(6800.00m, resultado.Emolumento);
            Assert.IsNull(resultado.Faixa.ValorAte);
        }

        [TestMethod]
        public void Total_EhEmolumentoMaisCustas()
        {
            var resultado = _calculadora.Calcular("SP", TabelaEmolumentosEmMemoria.Escritura, 150000m);

            Assert.AreEqual(4100.00m, resultado.Emolumento);
            Assert.AreEqual(1300.00m, resultado.Custas);
            Assert.AreEqual(5400.00m, resultado.Total);
        }

        [DataTestMethod]
        [DataRow("20000.004", "1200.00")]   // arredonda para 20.000,00
        [DataRow("20000.005", "2400.00")]   // arredonda para 20.000,01 (não cai no buraco entre as faixas)
        public void ValorComMaisDeDuasCasas_EhArredondadoAntesDeEscolherAFaixa(string valor, string emolumentoEsperado)
        {
            var resultado = _calculadora.Calcular("SP", TabelaEmolumentosEmMemoria.Escritura, decimal.Parse(valor, System.Globalization.CultureInfo.InvariantCulture));

            Assert.AreEqual(decimal.Parse(emolumentoEsperado, System.Globalization.CultureInfo.InvariantCulture), resultado.Emolumento);
        }

        [TestMethod]
        public void SiglaEmMinusculasEComEspacos_FuncionaIgual()
        {
            var resultado = _calculadora.Calcular(" sp ", TabelaEmolumentosEmMemoria.Escritura, 1000m);

            Assert.AreEqual("SP", resultado.SiglaUf);
            Assert.AreEqual(1200.00m, resultado.Emolumento);
        }

        [TestMethod]
        public void AtoSemValorDeclarado_UsaValorFixo_IgnorandoOValorInformado()
        {
            var resultado = _calculadora.Calcular("SP", TabelaEmolumentosEmMemoria.Procuracao, 999999m);

            Assert.AreEqual(210.00m, resultado.Emolumento);
            Assert.AreEqual(66.00m, resultado.Custas);
            Assert.AreEqual(0m, resultado.ValorNegocio);
        }

        [TestMethod]
        public void AtoSemValorDeclarado_AceitaValorZero()
        {
            var resultado = _calculadora.Calcular("SP", TabelaEmolumentosEmMemoria.Firma, 0m);

            Assert.AreEqual(12.50m, resultado.Total);
        }

        [TestMethod]
        public void UfSemTabela_LancaUfSemTabelaException()
        {
            var ex = Assert.ThrowsException<UfSemTabelaException>(
                () => _calculadora.Calcular("RJ", TabelaEmolumentosEmMemoria.Escritura, 1000m));

            Assert.AreEqual("RJ", ex.SiglaUf);
            StringAssert.Contains(ex.Message, "RJ");
        }

        [TestMethod]
        public void UfComTabela_MasSemOAto_LancaAtoSemTabelaException()
        {
            var ex = Assert.ThrowsException<AtoSemTabelaException>(
                () => _calculadora.Calcular("MG", TabelaEmolumentosEmMemoria.Escritura, 1000m));

            Assert.AreEqual(TabelaEmolumentosEmMemoria.Escritura.Id, ex.TipoAtoId);
        }

        [TestMethod]
        public void ValorNoBuracoEntreFaixas_LancaFaixaNaoEncontradaException()
        {
            Assert.ThrowsException<FaixaNaoEncontradaException>(
                () => _calculadora.Calcular("XX", TabelaEmolumentosEmMemoria.Escritura, 1500m));
        }

        [TestMethod]
        public void ValorNegativo_EhRejeitado()
        {
            Assert.ThrowsException<DadosCalculoInvalidosException>(
                () => _calculadora.Calcular("SP", TabelaEmolumentosEmMemoria.Escritura, -1m));
        }

        [TestMethod]
        public void ValorZero_EmAtoQueExigeValor_EhRejeitado()
        {
            Assert.ThrowsException<DadosCalculoInvalidosException>(
                () => _calculadora.Calcular("SP", TabelaEmolumentosEmMemoria.Escritura, 0m));
        }

        [TestMethod]
        public void SemUf_EhRejeitado()
        {
            Assert.ThrowsException<DadosCalculoInvalidosException>(
                () => _calculadora.Calcular("", TabelaEmolumentosEmMemoria.Escritura, 1000m));
        }

        [TestMethod]
        public void SemTipoDeAto_EhRejeitado()
        {
            Assert.ThrowsException<DadosCalculoInvalidosException>(
                () => _calculadora.Calcular("SP", null, 1000m));
        }
    }
}
