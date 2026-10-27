using System;
using Cartorio.Entidades;
using Cartorio.Negocio;
using Cartorio.Testes.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cartorio.Testes
{
    [TestClass]
    public class ClienteServicoTests
    {
        private ClienteRepositorioEmMemoria _repositorio;
        private ClienteServico _servico;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new ClienteRepositorioEmMemoria();
            _servico = new ClienteServico(_repositorio, new RelogioFixo(new DateTime(2026, 3, 10, 9, 0, 0)));
        }

        [TestMethod]
        public void Cadastrar_GuardaCpfSemPontuacaoEDataDoRelogio()
        {
            var cliente = _servico.Cadastrar("  Maria Souza  ", "529.982.247-25", "maria@example.com", "(11) 98888-1111");

            Assert.AreEqual("52998224725", cliente.Cpf);
            Assert.AreEqual("Maria Souza", cliente.Nome);
            Assert.AreEqual(new DateTime(2026, 3, 10, 9, 0, 0), cliente.DataCadastro);
            Assert.AreEqual(1, _repositorio.Clientes.Count);
        }

        [TestMethod]
        public void Cadastrar_CamposOpcionaisEmBranco_ViramNulo()
        {
            var cliente = _servico.Cadastrar("Maria Souza", "52998224725", "   ", "");

            Assert.IsNull(cliente.Email);
            Assert.IsNull(cliente.Telefone);
        }

        [TestMethod]
        public void Cadastrar_CpfInvalido_LancaNegocioException()
        {
            var ex = Assert.ThrowsException<NegocioException>(
                () => _servico.Cadastrar("Maria Souza", "123.456.789-00", null, null));

            StringAssert.Contains(ex.Message, "CPF inválido");
            Assert.AreEqual(0, _repositorio.Clientes.Count);
        }

        [TestMethod]
        public void Cadastrar_CpfDuplicado_LancaNegocioException()
        {
            _servico.Cadastrar("Maria Souza", "52998224725", null, null);

            var ex = Assert.ThrowsException<NegocioException>(
                () => _servico.Cadastrar("Outra Pessoa", "529.982.247-25", null, null));

            StringAssert.Contains(ex.Message, "529.982.247-25");
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("  ")]
        [DataRow("Al")]
        public void Cadastrar_NomeCurto_LancaNegocioException(string nome)
        {
            Assert.ThrowsException<NegocioException>(() => _servico.Cadastrar(nome, "52998224725", null, null));
        }

        [DataTestMethod]
        [DataRow("maria")]
        [DataRow("maria@")]
        [DataRow("maria@example")]
        [DataRow("ma ria@example.com")]
        public void Cadastrar_EmailInvalido_LancaNegocioException(string email)
        {
            Assert.ThrowsException<NegocioException>(() => _servico.Cadastrar("Maria Souza", "52998224725", email, null));
        }

        [TestMethod]
        public void Atualizar_MudaContato_MantemCpf()
        {
            var cliente = _servico.Cadastrar("Maria Souza", "52998224725", null, null);

            var atualizado = _servico.Atualizar(cliente.Id, "Maria Souza Lima", "maria@example.com", "11 99999-0000");

            Assert.AreEqual("Maria Souza Lima", atualizado.Nome);
            Assert.AreEqual("maria@example.com", atualizado.Email);
            Assert.AreEqual("52998224725", atualizado.Cpf);
        }

        [TestMethod]
        public void Atualizar_ClienteInexistente_LancaRegistroNaoEncontrado()
        {
            Assert.ThrowsException<RegistroNaoEncontradoException>(
                () => _servico.Atualizar(999, "Maria Souza", null, null));
        }

        [TestMethod]
        public void Pesquisar_ComCpfFormatado_BuscaPorCpf()
        {
            _servico.Cadastrar("Maria Souza", "52998224725", null, null);
            _servico.Cadastrar("João Pereira", "11144477735", null, null);

            var encontrados = _servico.Pesquisar("111.444.777-35");

            Assert.AreEqual(1, encontrados.Count);
            Assert.AreEqual("João Pereira", encontrados[0].Nome);
        }

        [TestMethod]
        public void Pesquisar_ComTrechoDoNome_BuscaPorNome()
        {
            _servico.Cadastrar("Maria Souza", "52998224725", null, null);
            _servico.Cadastrar("João Pereira", "11144477735", null, null);

            var encontrados = _servico.Pesquisar("souza");

            Assert.AreEqual(1, encontrados.Count);
            Assert.AreEqual("Maria Souza", encontrados[0].Nome);
        }

        [TestMethod]
        public void Pesquisar_Vazio_ListaTodos()
        {
            _servico.Cadastrar("Maria Souza", "52998224725", null, null);
            _servico.Cadastrar("João Pereira", "11144477735", null, null);

            Assert.AreEqual(2, _servico.Pesquisar("").Count);
        }

        [TestMethod]
        public void ObterPorCpf_NaoCadastrado_LancaRegistroNaoEncontrado()
        {
            Assert.ThrowsException<RegistroNaoEncontradoException>(() => _servico.ObterPorCpf("52998224725"));
        }

        [TestMethod]
        public void ObterPorCpf_Cadastrado_DevolveCliente()
        {
            _repositorio.Clientes.Add(new Cliente { Id = 7, Nome = "Ana Lima", Cpf = "12345678909" });

            Assert.AreEqual(7, _servico.ObterPorCpf("123.456.789-09").Id);
        }
    }
}
