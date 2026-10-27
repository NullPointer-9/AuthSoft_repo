using Legado.Negocio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Legado.Testes
{
    [TestClass]
    public class ClienteServicoTests
    {
        private ClienteRepositorioEmMemoria _clientes;
        private PedidoRepositorioEmMemoria _pedidos;
        private ClienteServico _servico;

        [TestInitialize]
        public void Inicializar()
        {
            _clientes = new ClienteRepositorioEmMemoria();
            _pedidos = new PedidoRepositorioEmMemoria();
            _servico = new ClienteServico(_clientes, _pedidos);
        }

        [TestMethod]
        public void Salvar_Novo_GravaCpfSoComDigitos()
        {
            var cliente = _servico.Salvar(0, "  Maria Souza ", "529.982.247-25", "");

            Assert.AreEqual("52998224725", cliente.Cpf);
            Assert.AreEqual("Maria Souza", cliente.Nome);
            Assert.AreEqual(1, _clientes.Clientes.Count);
        }

        [TestMethod]
        public void Salvar_CpfJaGravadoComPontuacaoPeloLegado_EhDetectadoComoDuplicado()
        {
            // O legado gravou com pontuação; o atendente digita sem.
            _clientes.Clientes.Add(new Cliente { Id = 1, Nome = "MARIA APARECIDA SOUZA", Cpf = "529.982.247-25" });

            var ex = Assert.ThrowsException<NegocioException>(() => _servico.Salvar(0, "Maria", "52998224725", ""));

            StringAssert.Contains(ex.Message, "já cadastrado");
        }

        [TestMethod]
        public void Salvar_AlterarOProprioCliente_NaoAcusaDuplicidade()
        {
            _clientes.Clientes.Add(new Cliente { Id = 1, Nome = "Maria", Cpf = "529.982.247-25" });

            var alterado = _servico.Salvar(1, "Maria Souza", "52998224725", "11 99999-0000");

            Assert.AreEqual("Maria Souza", alterado.Nome);
        }

        [TestMethod]
        public void Salvar_AlterarParaCpfDeOutroCliente_LancaNegocioException()
        {
            _clientes.Clientes.Add(new Cliente { Id = 1, Nome = "Maria", Cpf = "52998224725" });
            _clientes.Clientes.Add(new Cliente { Id = 2, Nome = "João", Cpf = "11144477735" });

            Assert.ThrowsException<NegocioException>(() => _servico.Salvar(2, "João", "529.982.247-25", ""));
        }

        [DataTestMethod]
        [DataRow("abcdefghijk")]      // o legado aceitava: só olhava se tinha 11 caracteres
        [DataRow("12345678900")]
        [DataRow("11111111111")]
        [DataRow("123")]
        public void Salvar_CpfInvalido_LancaNegocioException(string cpf)
        {
            Assert.ThrowsException<NegocioException>(() => _servico.Salvar(0, "Maria", cpf, ""));
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void Salvar_SemNome_LancaNegocioException(string nome)
        {
            Assert.ThrowsException<NegocioException>(() => _servico.Salvar(0, nome, "52998224725", ""));
        }

        [TestMethod]
        public void Excluir_ClienteComPedidos_NaoExcluiParaNaoDeixarPedidoOrfao()
        {
            _clientes.Clientes.Add(new Cliente { Id = 1, Nome = "Maria", Cpf = "52998224725" });
            _pedidos.Existente(1, "aberto");

            var ex = Assert.ThrowsException<NegocioException>(() => _servico.Excluir(1));

            StringAssert.Contains(ex.Message, "1 pedido");
            Assert.AreEqual(1, _clientes.Clientes.Count);
        }

        [TestMethod]
        public void Excluir_ClienteSemPedidos_Exclui()
        {
            _clientes.Clientes.Add(new Cliente { Id = 1, Nome = "Maria", Cpf = "52998224725" });

            _servico.Excluir(1);

            Assert.AreEqual(0, _clientes.Clientes.Count);
        }
    }
}
