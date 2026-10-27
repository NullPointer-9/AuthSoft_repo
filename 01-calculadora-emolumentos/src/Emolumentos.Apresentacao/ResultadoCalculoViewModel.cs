namespace Emolumentos.Apresentacao
{
    /// <summary>Resultado já formatado para exibição (moeda em pt-BR).</summary>
    public class ResultadoCalculoViewModel
    {
        public string Uf { get; set; }
        public string TipoAto { get; set; }
        public string ValorNegocio { get; set; }
        public string Faixa { get; set; }
        public string Emolumento { get; set; }
        public string Custas { get; set; }
        public string Total { get; set; }
    }
}
