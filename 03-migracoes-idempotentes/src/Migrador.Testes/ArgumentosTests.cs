using System;
using Migrador.Console;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Migrador.Testes
{
    [TestClass]
    public class ArgumentosTests
    {
        [TestMethod]
        public void LeTodasAsOpcoes()
        {
            var argumentos = Argumentos.Interpretar(new[]
            {
                "--banco", "PostgreS", "--conexao", "Host=x", "--pasta", @"c:\scripts", "--simular"
            });

            Assert.AreEqual("postgres", argumentos.Banco);
            Assert.AreEqual("Host=x", argumentos.Conexao);
            Assert.AreEqual(@"c:\scripts", argumentos.Pasta);
            Assert.IsTrue(argumentos.Simular);
        }

        [TestMethod]
        public void SoOBanco_DeixaOResto_ComPadrao()
        {
            var argumentos = Argumentos.Interpretar(new[] { "--banco", "sqlserver" });

            Assert.IsNull(argumentos.Conexao);
            Assert.IsNull(argumentos.Pasta);
            Assert.IsFalse(argumentos.Simular);
        }

        [TestMethod]
        public void Ajuda_NaoExigeBanco()
        {
            Assert.IsTrue(Argumentos.Interpretar(new[] { "--ajuda" }).Ajuda);
        }

        [DataTestMethod]
        [DataRow(new string[0], "Informe o banco")]
        [DataRow(new[] { "--banco", "oracle" }, "não suportado")]
        [DataRow(new[] { "--banco" }, "Falta o valor")]
        [DataRow(new[] { "--banco", "--simular" }, "Falta o valor")]
        [DataRow(new[] { "--banco", "sqlserver", "--forcar" }, "desconhecida")]
        public void ArgumentosInvalidos_ExplicamOProblema(string[] args, string trecho)
        {
            var ex = Assert.ThrowsException<ArgumentException>(() => Argumentos.Interpretar(args));
            StringAssert.Contains(ex.Message, trecho);
        }
    }
}
