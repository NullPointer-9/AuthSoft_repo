namespace Emolumentos.Dominio
{
    public class ResultadoCalculo
    {
        public ResultadoCalculo(string siglaUf, TipoAto tipoAto, decimal valorNegocio, FaixaEmolumento faixa)
        {
            SiglaUf = siglaUf;
            TipoAto = tipoAto;
            ValorNegocio = valorNegocio;
            Faixa = faixa;
        }

        public string SiglaUf { get; }
        public TipoAto TipoAto { get; }
        public decimal ValorNegocio { get; }
        public FaixaEmolumento Faixa { get; }

        public decimal Emolumento => Faixa.Emolumento;
        public decimal Custas => Faixa.Custas;
        public decimal Total => Faixa.Emolumento + Faixa.Custas;
    }
}
