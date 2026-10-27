using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Emolumentos.Dominio;

namespace Emolumentos.Apresentacao
{
    public class CalculoPresenter
    {
        private static readonly CultureInfo PtBr = new CultureInfo("pt-BR");

        private readonly ICalculoView _view;
        private readonly ITabelaEmolumentosRepositorio _repositorio;
        private readonly CalculadoraEmolumentos _calculadora;
        private readonly IEmissorDemonstrativo _emissor;
        private IList<TipoAto> _tiposAto = new List<TipoAto>();
        private ResultadoCalculoViewModel _ultimoResultado;

        public CalculoPresenter(
            ICalculoView view,
            ITabelaEmolumentosRepositorio repositorio,
            CalculadoraEmolumentos calculadora,
            IEmissorDemonstrativo emissor)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
            _calculadora = calculadora ?? throw new ArgumentNullException(nameof(calculadora));
            _emissor = emissor ?? throw new ArgumentNullException(nameof(emissor));

            _view.Carregada += (s, e) => Carregar();
            _view.TipoAtoAlterado += (s, e) => AtualizarCampoValor();
            _view.CalcularSolicitado += (s, e) => Calcular();
            _view.EmitirDocumentoSolicitado += (s, e) => EmitirDocumento();
            _view.PreviaSolicitada += (s, e) => ExibirPrevia();
        }

        private void Carregar()
        {
            try
            {
                _view.ExibirUfs(_repositorio.ListarUfs());
                _tiposAto = _repositorio.ListarTiposAto();
                _view.ExibirTiposAto(_tiposAto);
                AtualizarCampoValor();
                DefinirResultado(null);
            }
            catch (Exception ex)
            {
                _view.ExibirErro(
                    "Não foi possível carregar as tabelas de emolumentos. " +
                    "Verifique se o banco de dados está no ar (docker compose up -d).\n\nDetalhe: " + ex.Message);
            }
        }

        private void AtualizarCampoValor()
        {
            var ato = TipoAtoSelecionado();
            _view.HabilitarValorNegocio(ato == null || ato.ExigeValor);
        }

        private void Calcular()
        {
            _view.LimparResultado();
            DefinirResultado(null);

            var ato = TipoAtoSelecionado();
            if (string.IsNullOrWhiteSpace(_view.UfSelecionada))
            {
                _view.ExibirErro("Selecione a UF.");
                return;
            }
            if (ato == null)
            {
                _view.ExibirErro("Selecione o tipo de ato.");
                return;
            }

            var valor = 0m;
            if (ato.ExigeValor && !TentarLerValor(_view.ValorNegocioTexto, out valor))
            {
                _view.ExibirErro("Informe um valor válido para o negócio, por exemplo: 150.000,00");
                return;
            }

            try
            {
                var resultado = Formatar(_calculadora.Calcular(_view.UfSelecionada, ato, valor));
                _view.ExibirResultado(resultado);
                DefinirResultado(resultado);
            }
            catch (CalculoEmolumentoException ex)
            {
                _view.ExibirErro(ex.Message);
            }
            catch (Exception ex)
            {
                _view.ExibirErro("Erro inesperado ao calcular os emolumentos.\n\nDetalhe: " + ex.Message);
            }
        }

        private void ExibirPrevia()
        {
            var resultado = _ultimoResultado;
            if (resultado == null)
            {
                _view.ExibirErro("Calcule os emolumentos antes de visualizar o demonstrativo.");
                return;
            }

            bool emitir;
            try
            {
                emitir = _view.ExibirPrevia(resultado);
            }
            catch (Exception ex)
            {
                _view.ExibirErro("Não foi possível montar a prévia do demonstrativo.\n\nDetalhe: " + ex.Message);
                return;
            }

            if (emitir)
                EmitirDocumento();
        }

        private void EmitirDocumento()
        {
            var resultado = _ultimoResultado;
            if (resultado == null)
            {
                _view.ExibirErro("Calcule os emolumentos antes de emitir o demonstrativo.");
                return;
            }

            var caminho = _view.SolicitarCaminhoDoArquivo("demonstrativo-emolumentos-" + resultado.Uf + ".pdf");
            if (string.IsNullOrWhiteSpace(caminho))
                return;

            try
            {
                _emissor.Emitir(resultado, caminho);
                _view.AbrirDocumento(caminho);
            }
            catch (Exception ex)
            {
                _view.ExibirErro("Não foi possível emitir o demonstrativo.\n\nDetalhe: " + ex.Message);
            }
        }

        private void DefinirResultado(ResultadoCalculoViewModel resultado)
        {
            _ultimoResultado = resultado;
            _view.HabilitarEmissao(resultado != null);
        }

        private TipoAto TipoAtoSelecionado()
        {
            var id = _view.TipoAtoSelecionadoId;
            return id.HasValue ? _tiposAto.FirstOrDefault(t => t.Id == id.Value) : null;
        }

        /// <summary>Lê valores no formato brasileiro: "150000", "150.000,00" ou "R$ 150.000,00".</summary>
        public static bool TentarLerValor(string texto, out decimal valor)
        {
            valor = 0m;
            if (string.IsNullOrWhiteSpace(texto))
                return false;

            var limpo = texto.Replace("R$", string.Empty).Trim();
            return decimal.TryParse(limpo, NumberStyles.Number, PtBr, out valor);
        }

        private static ResultadoCalculoViewModel Formatar(ResultadoCalculo resultado)
        {
            return new ResultadoCalculoViewModel
            {
                Uf = resultado.SiglaUf,
                TipoAto = resultado.TipoAto.Descricao,
                ValorNegocio = resultado.TipoAto.ExigeValor ? Moeda(resultado.ValorNegocio) : "Não se aplica",
                Faixa = DescreverFaixa(resultado),
                Emolumento = Moeda(resultado.Emolumento),
                Custas = Moeda(resultado.Custas),
                Total = Moeda(resultado.Total)
            };
        }

        private static string DescreverFaixa(ResultadoCalculo resultado)
        {
            var faixa = resultado.Faixa;
            if (!resultado.TipoAto.ExigeValor)
                return "Valor fixo";
            if (!faixa.ValorAte.HasValue)
                return "Acima de " + Moeda(faixa.ValorDe);
            return "De " + Moeda(faixa.ValorDe) + " até " + Moeda(faixa.ValorAte.Value);
        }

        private static string Moeda(decimal valor) => valor.ToString("C", PtBr);
    }
}
