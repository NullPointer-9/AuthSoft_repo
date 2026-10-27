using System.Collections.Generic;
using System.Linq;
using Migrador.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Migrador.Testes
{
    [TestClass]
    public class MotorMigracaoTests
    {
        private ExecutorFalso _banco;
        private MotorMigracao _motor;

        [TestInitialize]
        public void Inicializar()
        {
            _banco = new ExecutorFalso();
            _motor = new MotorMigracao(_banco);
        }

        private static Migracao Script(int versao, string conteudo = null)
        {
            return new Migracao(versao, "script_" + versao, versao.ToString("0000") + "_script_" + versao + ".sql",
                conteudo ?? "SELECT " + versao + ";");
        }

        private static List<Migracao> Scripts(params int[] versoes) => versoes.Select(v => Script(v)).ToList();

        [TestMethod]
        public void PrimeiraExecucao_CriaTabelaDeControleEAplicaTudoEmOrdem()
        {
            var resultado = _motor.Executar(Scripts(1, 2, 3));

            Assert.IsTrue(_banco.TabelaDeControleCriada);
            CollectionAssert.AreEqual(new[] { "0001_script_1.sql", "0002_script_2.sql", "0003_script_3.sql" }, _banco.Executados);
            Assert.AreEqual(3, resultado.Aplicadas.Count);
            Assert.AreEqual(0, resultado.JaAplicadas);
        }

        [TestMethod]
        public void SegundaExecucao_NaoAplicaNada()
        {
            _motor.Executar(Scripts(1, 2, 3));
            _banco.Executados.Clear();

            var resultado = _motor.Executar(Scripts(1, 2, 3));

            Assert.AreEqual(0, _banco.Executados.Count);
            Assert.AreEqual(0, resultado.Aplicadas.Count);
            Assert.AreEqual(3, resultado.JaAplicadas);
            Assert.AreEqual(0, resultado.Alertas.Count);
        }

        [TestMethod]
        public void ScriptNovo_SoEleEhAplicado()
        {
            _motor.Executar(Scripts(1, 2));
            _banco.Executados.Clear();

            _motor.Executar(Scripts(1, 2, 3));

            CollectionAssert.AreEqual(new[] { "0003_script_3.sql" }, _banco.Executados);
        }

        [TestMethod]
        public void ScriptAlteradoDepoisDeAplicado_GeraAlertaENaoRoda()
        {
            _motor.Executar(new List<Migracao> { Script(1, "CREATE TABLE a (id INT);") });
            _banco.Executados.Clear();

            var resultado = _motor.Executar(new List<Migracao> { Script(1, "CREATE TABLE a (id INT, nome VARCHAR(10));") });

            Assert.AreEqual(0, _banco.Executados.Count);
            StringAssert.Contains(resultado.Alertas.Single(), "alterado depois de aplicado");
        }

        [TestMethod]
        public void SoMudarQuebraDeLinha_NaoContaComoAlteracao()
        {
            _motor.Executar(new List<Migracao> { Script(1, "CREATE TABLE a (\n id INT\n);") });

            var resultado = _motor.Executar(new List<Migracao> { Script(1, "CREATE TABLE a (\r\n id INT\r\n);\r\n") });

            Assert.AreEqual(0, resultado.Alertas.Count);
        }

        [TestMethod]
        public void ArquivoApagadoDepoisDeAplicado_GeraAlerta()
        {
            _motor.Executar(Scripts(1, 2));

            var resultado = _motor.Executar(Scripts(1));

            StringAssert.Contains(resultado.Alertas.Single(), "não existe mais");
        }

        [TestMethod]
        public void ScriptComVersaoMenorQueAUltimaAplicada_ParaSemAplicarNada()
        {
            _motor.Executar(Scripts(1, 5));
            _banco.Executados.Clear();

            var ex = Assert.ThrowsException<MigracaoException>(() => _motor.Executar(Scripts(1, 3, 5, 6)));

            StringAssert.Contains(ex.Message, "Renumere");
            Assert.AreEqual(3, ex.Migracao.Versao);
            Assert.AreEqual(0, _banco.Executados.Count, "não deve aplicar nem o 0006");
        }

        [TestMethod]
        public void FalhaNoMeio_ParaNoScriptComErroEMantemOsAnteriores()
        {
            _banco.FalharNaVersao = 2;

            var ex = Assert.ThrowsException<MigracaoException>(() => _motor.Executar(Scripts(1, 2, 3)));

            Assert.AreEqual(2, ex.Migracao.Versao);
            StringAssert.Contains(ex.Message, "TABLEE");
            CollectionAssert.AreEqual(new[] { "0001_script_1.sql" }, _banco.Executados);
            CollectionAssert.AreEqual(new[] { 1 }, _banco.Registros.Select(r => r.Versao).ToList());
        }

        [TestMethod]
        public void DepoisDeCorrigirOScript_RetomaDeOndeParou()
        {
            _banco.FalharNaVersao = 2;
            Assert.ThrowsException<MigracaoException>(() => _motor.Executar(Scripts(1, 2, 3)));
            _banco.FalharNaVersao = null;
            _banco.Executados.Clear();

            _motor.Executar(Scripts(1, 2, 3));

            CollectionAssert.AreEqual(new[] { "0002_script_2.sql", "0003_script_3.sql" }, _banco.Executados);
        }

        [TestMethod]
        public void Simulacao_NaoTocaNoBanco()
        {
            var mensagens = new List<string>();
            var motor = new MotorMigracao(_banco, mensagens.Add);

            var resultado = motor.Executar(Scripts(1, 2), simular: true);

            Assert.IsFalse(_banco.TabelaDeControleCriada);
            Assert.AreEqual(0, _banco.Executados.Count);
            Assert.AreEqual(2, resultado.Aplicadas.Count);
            Assert.IsTrue(resultado.Simulacao);
            StringAssert.StartsWith(mensagens[0], "[simulação]");
        }

        [TestMethod]
        public void PastaVazia_NaoFazNada()
        {
            var resultado = _motor.Executar(new List<Migracao>());

            Assert.AreEqual(0, resultado.Aplicadas.Count);
            Assert.AreEqual(0, resultado.Alertas.Count);
        }
    }
}
