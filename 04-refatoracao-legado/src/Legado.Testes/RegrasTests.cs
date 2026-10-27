using System;
using Legado.Negocio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Legado.Testes
{
    [TestClass]
    public class TabelaPrecosTests
    {
        [DataTestMethod]
        [DataRow(TipoCertidao.Nascimento, false, "45.90")]
        [DataRow(TipoCertidao.Casamento, false, "45.90")]
        [DataRow(TipoCertidao.Obito, false, "52.35")]
        [DataRow(TipoCertidao.Nascimento, true, "68.85")]
        [DataRow(TipoCertidao.Obito, true, "78.53")]   // 52,35 × 1,5 = 78,525 → 78,53
        public void Preco(TipoCertidao tipo, bool urgente, string esperado)
        {
            Assert.AreEqual(decimal.Parse(esperado, System.Globalization.CultureInfo.InvariantCulture), TabelaPrecos.Preco(tipo, urgente));
        }

        [TestMethod]
        public void ComDouble_SomaDeCentavosNaoEhExata()
        {
            // Motivo de trocar double por decimal: o total em aberto é uma soma de valores,
            // e em ponto flutuante binário 0,10 + 0,20 não dá exatamente 0,30.
            Assert.AreNotEqual(0.30, 0.10 + 0.20);
            Assert.AreEqual(0.30m, 0.10m + 0.20m);
        }
    }

    [TestClass]
    public class FormatosLegadosTests
    {
        [DataTestMethod]
        [DataRow("aberto", StatusPedido.Aberto)]
        [DataRow("ABERTO", StatusPedido.Aberto)]
        [DataRow(" Aberto ", StatusPedido.Aberto)]
        [DataRow("PAGO", StatusPedido.Pago)]
        [DataRow("entregue", StatusPedido.Entregue)]
        public void LerStatus_AceitaAsGrafiasDoBanco(string texto, StatusPedido esperado)
        {
            Assert.AreEqual(esperado, FormatosLegados.LerStatus(texto));
        }

        [TestMethod]
        public void LerStatus_Desconhecido_LancaFormatException()
        {
            Assert.ThrowsException<FormatException>(() => FormatosLegados.LerStatus("cancelado"));
        }

        [TestMethod]
        public void EscreverStatus_MantemOFormatoAntigoEmMinusculas()
        {
            Assert.AreEqual("pago", FormatosLegados.EscreverStatus(StatusPedido.Pago));
        }

        [TestMethod]
        public void Data_IdaEVoltaNoFormatoDoBanco()
        {
            Assert.AreEqual(new DateTime(2026, 2, 3), FormatosLegados.LerData("03/02/2026"));
            Assert.AreEqual("03/02/2026", FormatosLegados.EscreverData(new DateTime(2026, 2, 3, 15, 0, 0)));
        }

        [DataTestMethod]
        [DataRow("2026-02-03")]
        [DataRow("")]
        [DataRow(null)]
        public void LerData_ForaDoFormato_DevolveNulo(string texto)
        {
            Assert.IsNull(FormatosLegados.LerData(texto));
        }
    }

    [TestClass]
    public class ValidadorCpfTests
    {
        [DataTestMethod]
        [DataRow("529.982.247-25", true)]
        [DataRow("11144477735", true)]
        [DataRow("123.456.789-09", true)]
        [DataRow("123.456.789-00", false)]
        [DataRow("00000000000", false)]
        [DataRow("", false)]
        public void EhValido(string cpf, bool esperado)
        {
            Assert.AreEqual(esperado, ValidadorCpf.EhValido(cpf));
        }
    }
}
