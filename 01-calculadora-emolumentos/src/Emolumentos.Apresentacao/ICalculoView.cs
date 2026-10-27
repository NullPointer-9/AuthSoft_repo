using System;
using System.Collections.Generic;
using Emolumentos.Dominio;

namespace Emolumentos.Apresentacao
{
    public interface ICalculoView
    {
        event EventHandler Carregada;
        event EventHandler TipoAtoAlterado;
        event EventHandler CalcularSolicitado;
        event EventHandler EmitirDocumentoSolicitado;
        event EventHandler PreviaSolicitada;

        string UfSelecionada { get; }
        int? TipoAtoSelecionadoId { get; }
        string ValorNegocioTexto { get; }

        void ExibirUfs(IList<Uf> ufs);
        void ExibirTiposAto(IList<TipoAto> tiposAto);
        void HabilitarValorNegocio(bool habilitado);
        void ExibirResultado(ResultadoCalculoViewModel resultado);
        void LimparResultado();
        void ExibirErro(string mensagem);

        void HabilitarEmissao(bool habilitada);

        /// <summary>Pergunta onde salvar o arquivo. Retorna nulo se o usuário cancelar.</summary>
        string SolicitarCaminhoDoArquivo(string nomeSugerido);

        void AbrirDocumento(string caminho);

        /// <summary>
        /// Mostra a prévia do demonstrativo (ainda sem assinatura).
        /// Retorna verdadeiro se o usuário pediu para emitir a partir da prévia.
        /// </summary>
        bool ExibirPrevia(ResultadoCalculoViewModel resultado);
    }
}
