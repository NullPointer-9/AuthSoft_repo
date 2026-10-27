using System;
using System.IO;
using System.Linq;
using Migrador.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Migrador.Testes
{
    [TestClass]
    public class LeitorDeScriptsTests
    {
        private string _pasta;

        [TestInitialize]
        public void Inicializar()
        {
            _pasta = Path.Combine(Path.GetTempPath(), "migrador-" + Guid.NewGuid());
            Directory.CreateDirectory(_pasta);
        }

        [TestCleanup]
        public void Limpar()
        {
            Directory.Delete(_pasta, true);
        }

        private void Criar(string nome, string conteudo = "SELECT 1;") => File.WriteAllText(Path.Combine(_pasta, nome), conteudo);

        [TestMethod]
        public void OrdenaPelaVersaoNumerica_NaoPeloTexto()
        {
            Criar("10_dez.sql");
            Criar("0002_dois.sql");
            Criar("1_um.sql");

            var versoes = LeitorDeScripts.Ler(_pasta).Select(m => m.Versao).ToArray();

            CollectionAssert.AreEqual(new[] { 1, 2, 10 }, versoes);
        }

        [TestMethod]
        public void ExtraiVersaoDescricaoEConteudo()
        {
            Criar("0003_indice_cliente_nome.sql", "CREATE INDEX ...");

            var migracao = LeitorDeScripts.Ler(_pasta).Single();

            Assert.AreEqual(3, migracao.Versao);
            Assert.AreEqual("indice_cliente_nome", migracao.Descricao);
            Assert.AreEqual("0003_indice_cliente_nome.sql", migracao.NomeArquivo);
            Assert.AreEqual("CREATE INDEX ...", migracao.Conteudo);
        }

        [TestMethod]
        public void IgnoraArquivosQueNaoSaoSql()
        {
            Criar("0001_um.sql");
            Criar("LEIA-ME.txt", "anotações");

            Assert.AreEqual(1, LeitorDeScripts.Ler(_pasta).Count);
        }

        [DataTestMethod]
        [DataRow("criar_tabela.sql")]
        [DataRow("V1__criar_tabela.sql")]
        [DataRow("0001-criar_tabela.sql")]
        public void SqlForaDoPadrao_EhErro(string nome)
        {
            Criar(nome);

            var ex = Assert.ThrowsException<ScriptInvalidoException>(() => LeitorDeScripts.Ler(_pasta));
            StringAssert.Contains(ex.Message, nome);
        }

        [TestMethod]
        public void VersaoRepetida_EhErro()
        {
            Criar("0002_um.sql");
            Criar("2_outro.sql");

            var ex = Assert.ThrowsException<ScriptInvalidoException>(() => LeitorDeScripts.Ler(_pasta));
            StringAssert.Contains(ex.Message, "Versão 2 repetida");
        }

        [TestMethod]
        public void PastaInexistente_EhErro()
        {
            Assert.ThrowsException<DirectoryNotFoundException>(() => LeitorDeScripts.Ler(Path.Combine(_pasta, "nao-existe")));
        }
    }

    [TestClass]
    public class ChecksumTests
    {
        [TestMethod]
        public void TemSessentaEQuatroCaracteresHexadecimais()
        {
            var checksum = Checksum.Calcular("SELECT 1;");

            Assert.AreEqual(64, checksum.Length);
            Assert.IsTrue(checksum.All(c => "0123456789abcdef".IndexOf(c) >= 0));
        }

        [TestMethod]
        public void CrlfELf_DaoOMesmoChecksum()
        {
            Assert.AreEqual(Checksum.Calcular("A\nB\n"), Checksum.Calcular("A\r\nB\r\n"));
        }

        [TestMethod]
        public void ConteudoDiferente_DaChecksumDiferente()
        {
            Assert.AreNotEqual(Checksum.Calcular("SELECT 1;"), Checksum.Calcular("SELECT 2;"));
        }
    }
}
