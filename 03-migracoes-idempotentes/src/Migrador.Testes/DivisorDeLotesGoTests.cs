using System.Linq;
using Migrador.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Migrador.Testes
{
    [TestClass]
    public class DivisorDeLotesGoTests
    {
        [TestMethod]
        public void SemGo_UmLote()
        {
            var lotes = DivisorDeLotesGo.Dividir("CREATE TABLE a (id INT);\nINSERT INTO a VALUES (1);");

            Assert.AreEqual(1, lotes.Count);
        }

        [TestMethod]
        public void GoSozinhoNaLinha_SeparaLotes()
        {
            var lotes = DivisorDeLotesGo.Dividir("SELECT 1;\nGO\nSELECT 2;\nGO\n");

            CollectionAssert.AreEqual(new[] { "SELECT 1;", "SELECT 2;" }, lotes.ToArray());
        }

        [DataTestMethod]
        [DataRow("go")]
        [DataRow("  GO  ")]
        [DataRow("Go -- fim da view")]
        [DataRow("GO\r")]
        public void VariacoesDeGo_SaoReconhecidas(string separador)
        {
            var lotes = DivisorDeLotesGo.Dividir("SELECT 1;\n" + separador + "\nSELECT 2;");

            Assert.AreEqual(2, lotes.Count);
        }

        [TestMethod]
        public void QuebraDeLinhaWindows_FuncionaIgual()
        {
            var lotes = DivisorDeLotesGo.Dividir("SELECT 1;\r\nGO\r\nSELECT 2;\r\n");

            CollectionAssert.AreEqual(new[] { "SELECT 1;", "SELECT 2;" }, lotes.ToArray());
        }

        [TestMethod]
        public void GoComNumero_RepeteOLote()
        {
            var lotes = DivisorDeLotesGo.Dividir("INSERT INTO a DEFAULT VALUES;\nGO 3");

            Assert.AreEqual(3, lotes.Count);
        }

        [DataTestMethod]
        [DataRow("GOTO fim;")]
        [DataRow("SELECT categoria AS GO")]
        [DataRow("-- GO")]
        public void GoDentroDeOutroTexto_NaoSepara(string linha)
        {
            var lotes = DivisorDeLotesGo.Dividir("SELECT 1;\n" + linha + "\nSELECT 2;");

            Assert.AreEqual(1, lotes.Count);
        }

        [TestMethod]
        public void GoDentroDeComentarioDeBloco_NaoSepara()
        {
            var script = "SELECT 1;\n/* exemplo antigo:\nGO\n*/\nSELECT 2;";

            Assert.AreEqual(1, DivisorDeLotesGo.Dividir(script).Count);
        }

        [TestMethod]
        public void ComentarioDeBlocoFechado_VoltaAReconhecerGo()
        {
            var script = "/* cabeçalho */\nSELECT 1;\nGO\nSELECT 2;";

            Assert.AreEqual(2, DivisorDeLotesGo.Dividir(script).Count);
        }

        [TestMethod]
        public void BarraAsteriscoDentroDeTexto_NaoAbreComentario()
        {
            var script = "SELECT '/*';\nGO\nSELECT 2;";

            Assert.AreEqual(2, DivisorDeLotesGo.Dividir(script).Count);
        }

        [TestMethod]
        public void GosSeguidosEVazios_NaoGeramLotesVazios()
        {
            var lotes = DivisorDeLotesGo.Dividir("GO\n\nSELECT 1;\nGO\nGO\n   \nGO");

            CollectionAssert.AreEqual(new[] { "SELECT 1;" }, lotes.ToArray());
        }
    }
}
