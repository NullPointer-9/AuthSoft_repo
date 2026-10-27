using System;
using System.Collections.Generic;
using Legado.Negocio;

namespace Legado.Apresentacao
{
    public interface IBalcaoView
    {
        event EventHandler Carregada;
        event EventHandler BuscarClientesSolicitado;
        event EventHandler ClienteSelecionado;
        event EventHandler NovoClienteSolicitado;
        event EventHandler SalvarClienteSolicitado;
        event EventHandler ExcluirClienteSolicitado;
        event EventHandler NovoPedidoSolicitado;
        event EventHandler MarcarPagoSolicitado;
        event EventHandler EntregarSolicitado;

        string TextoBusca { get; }
        int? ClienteSelecionadoId { get; }
        int? PedidoSelecionadoId { get; }
        TipoCertidao? TipoSelecionado { get; }
        bool Urgente { get; }

        string Nome { get; set; }
        string Cpf { get; set; }
        string Telefone { get; set; }

        void ExibirTipos(IList<OpcaoTipoCertidao> tipos);
        void ExibirClientes(IList<ClienteLinha> clientes);
        void ExibirPedidos(IList<PedidoLinha> pedidos);
        void ExibirTotalEmAberto(string total);

        void ExibirMensagem(string mensagem);
        void ExibirAviso(string mensagem);
        void ExibirErro(string mensagem);
        bool Confirmar(string pergunta);
    }
}
