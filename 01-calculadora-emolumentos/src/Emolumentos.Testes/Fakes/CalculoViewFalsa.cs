using System;
using System.Collections.Generic;
using Emolumentos.Apresentacao;
using Emolumentos.Dominio;

namespace Emolumentos.Testes.Fakes
{
    /// <summary>View falsa: guarda o que o Presenter mandou mostrar e permite simular cliques.</summary>
    public class CalculoViewFalsa : ICalculoView
    {
        public event EventHandler Carregada;
        public event EventHandler TipoAtoAlterado;
        public event EventHandler CalcularSolicitado;
        public event EventHandler EmitirDocumentoSolicitado;
        public event EventHandler PreviaSolicitada;

        /// <summary>O que a "janela de prévia" responde: verdadeiro = usuário clicou em Emitir.</summary>
        public bool RespostaDaPrevia { get; set; }
        public ResultadoCalculoViewModel PreviaExibida { get; private set; }

        /// <summary>O que o "diálogo de salvar" vai devolver. Nulo simula o usuário cancelando.</summary>
        public string CaminhoEscolhido { get; set; }
        public string NomeSugerido { get; private set; }
        public string DocumentoAberto { get; private set; }
        public bool? EmissaoHabilitada { get; private set; }

        public string UfSelecionada { get; set; }
        public int? TipoAtoSelecionadoId { get; set; }
        public string ValorNegocioTexto { get; set; }

        public IList<Uf> UfsExibidas { get; private set; }
        public IList<TipoAto> TiposAtoExibidos { get; private set; }
        public bool? ValorHabilitado { get; private set; }
        public ResultadoCalculoViewModel ResultadoExibido { get; private set; }
        public string UltimoErro { get; private set; }

        public void ExibirUfs(IList<Uf> ufs) => UfsExibidas = ufs;
        public void ExibirTiposAto(IList<TipoAto> tiposAto) => TiposAtoExibidos = tiposAto;
        public void HabilitarValorNegocio(bool habilitado) => ValorHabilitado = habilitado;
        public void ExibirResultado(ResultadoCalculoViewModel resultado) => ResultadoExibido = resultado;
        public void LimparResultado() => ResultadoExibido = null;
        public void ExibirErro(string mensagem) => UltimoErro = mensagem;
        public void HabilitarEmissao(bool habilitada) => EmissaoHabilitada = habilitada;
        public void AbrirDocumento(string caminho) => DocumentoAberto = caminho;

        public string SolicitarCaminhoDoArquivo(string nomeSugerido)
        {
            NomeSugerido = nomeSugerido;
            return CaminhoEscolhido;
        }

        public bool ExibirPrevia(ResultadoCalculoViewModel resultado)
        {
            PreviaExibida = resultado;
            return RespostaDaPrevia;
        }

        public void SimularCliqueEmitir() => EmitirDocumentoSolicitado?.Invoke(this, EventArgs.Empty);

        public void SimularCliquePrevia() => PreviaSolicitada?.Invoke(this, EventArgs.Empty);

        public void SimularCarregamento() => Carregada?.Invoke(this, EventArgs.Empty);

        public void SimularTrocaDeAto(int tipoAtoId)
        {
            TipoAtoSelecionadoId = tipoAtoId;
            TipoAtoAlterado?.Invoke(this, EventArgs.Empty);
        }

        public void SimularCliqueCalcular() => CalcularSolicitado?.Invoke(this, EventArgs.Empty);
    }
}
