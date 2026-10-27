using System;
using System.Collections.Generic;
using System.Linq;
using Legado.Negocio;

namespace Legado.Apresentacao
{
    public class BalcaoPresenter
    {
        private readonly IBalcaoView _view;
        private readonly ClienteServico _clientes;
        private readonly PedidoServico _pedidos;

        private IList<Cliente> _clientesExibidos = new List<Cliente>();
        private int _idCliente;

        public BalcaoPresenter(IBalcaoView view, ClienteServico clientes, PedidoServico pedidos)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _clientes = clientes ?? throw new ArgumentNullException(nameof(clientes));
            _pedidos = pedidos ?? throw new ArgumentNullException(nameof(pedidos));

            _view.Carregada += (s, e) => Executar(Carregar);
            _view.BuscarClientesSolicitado += (s, e) => Executar(() => BuscarClientes(_view.TextoBusca));
            _view.ClienteSelecionado += (s, e) => Executar(SelecionarCliente);
            _view.NovoClienteSolicitado += (s, e) => NovoCliente();
            _view.SalvarClienteSolicitado += (s, e) => Executar(SalvarCliente);
            _view.ExcluirClienteSolicitado += (s, e) => Executar(ExcluirCliente);
            _view.NovoPedidoSolicitado += (s, e) => Executar(NovoPedido);
            _view.MarcarPagoSolicitado += (s, e) => Executar(() => AlterarPedido(_pedidos.MarcarPago));
            _view.EntregarSolicitado += (s, e) => Executar(() => AlterarPedido(_pedidos.Entregar));
        }

        /// <summary>Id do cliente em edição; 0 quando é um cadastro novo.</summary>
        public int IdClienteAtual => _idCliente;

        private void Carregar()
        {
            _view.ExibirTipos(Enum.GetValues(typeof(TipoCertidao)).Cast<TipoCertidao>().Select(t => new OpcaoTipoCertidao(t)).ToList());
            NovoCliente();
            BuscarClientes(string.Empty);
        }

        private void BuscarClientes(string trecho)
        {
            _clientesExibidos = _clientes.Buscar(trecho);
            _view.ExibirClientes(_clientesExibidos.Select(c => new ClienteLinha(c)).ToList());
        }

        private void SelecionarCliente()
        {
            var cliente = _clientesExibidos.FirstOrDefault(c => c.Id == _view.ClienteSelecionadoId);
            if (cliente == null)
                return;

            _idCliente = cliente.Id;
            _view.Nome = cliente.Nome;
            _view.Cpf = Formatacao.Cpf(cliente.Cpf);
            _view.Telefone = cliente.Telefone;
            CarregarPedidos();
        }

        private void NovoCliente()
        {
            _idCliente = 0;
            _view.Nome = string.Empty;
            _view.Cpf = string.Empty;
            _view.Telefone = string.Empty;
            _view.ExibirPedidos(new List<PedidoLinha>());
            _view.ExibirTotalEmAberto(Formatacao.Moeda(0m));
        }

        private void SalvarCliente()
        {
            var novo = _idCliente == 0;
            var cliente = _clientes.Salvar(_idCliente, _view.Nome, _view.Cpf, _view.Telefone);

            _idCliente = cliente.Id;
            _view.Cpf = Formatacao.Cpf(cliente.Cpf);
            BuscarClientes(_view.TextoBusca);
            _view.ExibirMensagem(novo ? "Cliente cadastrado." : "Cliente alterado.");
        }

        private void ExcluirCliente()
        {
            if (_idCliente == 0)
            {
                _view.ExibirAviso("Selecione um cliente.");
                return;
            }
            if (!_view.Confirmar("Deseja excluir o cliente " + _view.Nome + "?"))
                return;

            _clientes.Excluir(_idCliente);
            NovoCliente();
            BuscarClientes(_view.TextoBusca);
        }

        private void NovoPedido()
        {
            var tipo = _view.TipoSelecionado;
            if (!tipo.HasValue)
            {
                _view.ExibirAviso("Escolha o tipo de certidão.");
                return;
            }

            _pedidos.Criar(_idCliente, tipo.Value, _view.Urgente);
            CarregarPedidos();
        }

        private void AlterarPedido(Action<int> acao)
        {
            var idPedido = _view.PedidoSelecionadoId;
            if (!idPedido.HasValue)
            {
                _view.ExibirAviso("Selecione um pedido.");
                return;
            }

            acao(idPedido.Value);
            CarregarPedidos();
        }

        private void CarregarPedidos()
        {
            _view.ExibirPedidos(_pedidos.ListarPorCliente(_idCliente).Select(p => new PedidoLinha(p)).ToList());
            _view.ExibirTotalEmAberto(Formatacao.Moeda(_pedidos.TotalEmAberto(_idCliente)));
        }

        /// <summary>Regra violada vira aviso; qualquer outra falha vira erro, sem derrubar a tela.</summary>
        private void Executar(Action acao)
        {
            try
            {
                acao();
            }
            catch (NegocioException ex)
            {
                _view.ExibirAviso(ex.Message);
            }
            catch (Exception ex)
            {
                _view.ExibirErro("Ocorreu um erro inesperado: " + ex.Message);
            }
        }
    }
}
