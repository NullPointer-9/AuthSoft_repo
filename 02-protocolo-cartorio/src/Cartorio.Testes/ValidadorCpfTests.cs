using Cartorio.Negocio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cartorio.Testes
{
    [TestClass]
    public class ValidadorCpfTests
    {
        [DataTestMethod]
        [DataRow("52998224725")]
        [DataRow("529.982.247-25")]
        [DataRow(" 111.444.777-35 ")]
        [DataRow("12345678909")]
        public void CpfsValidos_SaoAceitos(string cpf)
        {
            Assert.IsTrue(ValidadorCpf.EhValido(cpf));
        }

        [DataTestMethod]
        [DataRow("52998224724")]        // último dígito errado
        [DataRow("52998224715")]        // penúltimo dígito errado
        [DataRow("11111111111")]        // sequência repetida passa no cálculo, mas é inválida
        [DataRow("00000000000")]
        [DataRow("5299822472")]         // 10 dígitos
        [DataRow("529982247250")]       // 12 dígitos
        [DataRow("")]
        [DataRow(null)]
        [DataRow("abc.def.ghi-jk")]
        public void CpfsInvalidos_SaoRejeitados(string cpf)
        {
            Assert.IsFalse(ValidadorCpf.EhValido(cpf));
        }

        [TestMethod]
        public void Normalizar_RemovePontuacaoEEspacos()
        {
            Assert.AreEqual("52998224725", ValidadorCpf.Normalizar(" 529.982.247-25 "));
        }

        [TestMethod]
        public void Formatar_ColocaMascara()
        {
            Assert.AreEqual("529.982.247-25", ValidadorCpf.Formatar("52998224725"));
        }

        [TestMethod]
        public void Formatar_TextoSemOnzeDigitos_DevolveOriginal()
        {
            Assert.AreEqual("123", ValidadorCpf.Formatar("123"));
        }
    }
}
