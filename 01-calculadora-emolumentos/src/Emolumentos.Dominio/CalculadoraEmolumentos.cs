using System;
using System.Linq;

namespace Emolumentos.Dominio
{
    public class CalculadoraEmolumentos
    {
        private readonly ITabelaEmolumentosRepositorio _repositorio;

        public CalculadoraEmolumentos(ITabelaEmolumentosRepositorio repositorio)
        {
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        }

        public ResultadoCalculo Calcular(string siglaUf, TipoAto tipoAto, decimal valorNegocio)
        {
            if (string.IsNullOrWhiteSpace(siglaUf))
                throw new DadosCalculoInvalidosException("Selecione a UF.");
            if (tipoAto == null)
                throw new DadosCalculoInvalidosException("Selecione o tipo de ato.");
            if (valorNegocio < 0)
                throw new DadosCalculoInvalidosException("O valor do negócio não pode ser negativo.");

            // Arredonda antes de buscar a faixa: 20.000,005 cairia entre 20.000,00 e 20.000,01.
            var valor = decimal.Round(valorNegocio, 2, MidpointRounding.AwayFromZero);

            if (tipoAto.ExigeValor && valor <= 0)
                throw new DadosCalculoInvalidosException("Informe o valor do negócio para \"" + tipoAto.Descricao + "\".");

            var uf = siglaUf.Trim().ToUpperInvariant();

            var faixasDaUf = _repositorio.ObterFaixas(uf);
            if (faixasDaUf.Count == 0)
                throw new UfSemTabelaException(uf);

            var faixasDoAto = faixasDaUf.Where(f => f.TipoAtoId == tipoAto.Id).ToList();
            if (faixasDoAto.Count == 0)
                throw new AtoSemTabelaException(uf, tipoAto);

            // Atos sem valor declarado têm uma faixa única (0 até sem teto).
            var valorParaFaixa = tipoAto.ExigeValor ? valor : 0m;

            var faixa = faixasDoAto
                .Where(f => f.Contem(valorParaFaixa))
                .OrderBy(f => f.ValorDe)
                .FirstOrDefault();

            if (faixa == null)
                throw new FaixaNaoEncontradaException(uf, valorParaFaixa);

            return new ResultadoCalculo(uf, tipoAto, valorParaFaixa, faixa);
        }
    }
}
