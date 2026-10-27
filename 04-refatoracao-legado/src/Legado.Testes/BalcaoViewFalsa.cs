using System;
using System.Collections.Generic;
using Legado.Apresentacao;
using Legado.Negocio;

namespace Legado.Testes
{
    /// <summary>Simula a tela: guarda o que o Presenter mostrou e dispara os "cliques".</summary>
    public class BalcaoViewFalsa : IBalcaoView
    {
        public event EventHandler Carregada;
        public event EventHandler BuscarClientesSolicitado;
        public event EventHandler ClienteSelecionado;
        public event EventHandler NovoClienteSolicitado;
        public event EventHandler SalvarClienteSolicitado;
        public event EventHandler ExcluirClienteSolicitado;
        public event EventHandler NovoPedidoSolicitado;
        public event EventHandler MarcarPagoSolicitado;
        public event EventHandler EntregarSolicitado;
        public event EventHandler ReciboSolicitado;

        /// <summary>Resposta da "janela de prévia": verdadeiro = usuário clicou em Emitir.</summary>
        public bool RespostaDaPrevia { get; set; } = true;
        public ReciboViewModel PreviaExibida { get; private set; }

        /// <summary>O que o "diálogo de salvar" devolve; nulo simula cancelar.</summary>
        public string CaminhoEscolhido { get; set; } = @"C:\temp\recibo.pdf";
        public string DocumentoAberto { get; private set; }

        public string TextoBusca { get; set; } = string.Empty;
        public int? ClienteSelecionadoId { get; set; }
        public int? PedidoSelecionadoId { get; set; }
        public TipoCertidao? TipoSelecionado { get; set; } = TipoCertidao.Nascimento;
        public bool Urgente { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Telefone { get; set; }

        public IList<OpcaoTipoCertidao> Tipos { get; private set; }
        public IList<ClienteLinha> Clientes { get; private set; }
        public IList<PedidoLinha> Pedidos { get; private set; }
        public string Total { get; private set; }
        public string UltimaMensagem { get; private set; }
        public string UltimoAviso { get; private set; }
        public string UltimoErro { get; private set; }
        public bool RespostaConfirmacao { get; set; } = true;

        public void ExibirTipos(IList<OpcaoTipoCertidao> tipos) => Tipos = tipos;
        public void ExibirClientes(IList<ClienteLinha> clientes) => Clientes = clientes;
        public void ExibirPedidos(IList<PedidoLinha> pedidos) => Pedidos = pedidos;
        public void ExibirTotalEmAberto(string total) => Total = total;
        public void ExibirMensagem(string mensagem) => UltimaMensagem = mensagem;
        public void ExibirAviso(string mensagem) => UltimoAviso = mensagem;
        public void ExibirErro(string mensagem) => UltimoErro = mensagem;
        public bool Confirmar(string pergunta) => RespostaConfirmacao;

        public bool ExibirPreviaRecibo(ReciboViewModel recibo)
        {
            PreviaExibida = recibo;
            return RespostaDaPrevia;
        }

        public string SolicitarCaminhoDoArquivo(string nomeSugerido) => CaminhoEscolhido;
        public void AbrirDocumento(string caminho) => DocumentoAberto = caminho;
        public void Recibo() => ReciboSolicitado?.Invoke(this, EventArgs.Empty);

        public void Carregar() => Carregada?.Invoke(this, EventArgs.Empty);
        public void Buscar() => BuscarClientesSolicitado?.Invoke(this, EventArgs.Empty);
        public void NovoCliente() => NovoClienteSolicitado?.Invoke(this, EventArgs.Empty);
        public void SalvarCliente() => SalvarClienteSolicitado?.Invoke(this, EventArgs.Empty);
        public void ExcluirCliente() => ExcluirClienteSolicitado?.Invoke(this, EventArgs.Empty);
        public void NovoPedido() => NovoPedidoSolicitado?.Invoke(this, EventArgs.Empty);
        public void MarcarPago() => MarcarPagoSolicitado?.Invoke(this, EventArgs.Empty);
        public void Entregar() => EntregarSolicitado?.Invoke(this, EventArgs.Empty);

        public void SelecionarCliente(int id)
        {
            ClienteSelecionadoId = id;
            ClienteSelecionado?.Invoke(this, EventArgs.Empty);
        }
    }
}
