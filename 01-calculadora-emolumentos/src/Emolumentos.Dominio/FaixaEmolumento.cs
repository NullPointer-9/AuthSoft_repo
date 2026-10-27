namespace Emolumentos.Dominio
{
    /// <summary>
    /// Uma faixa da tabela de emolumentos. Os limites são inclusivos e
    /// <see cref="ValorAte"/> nulo significa que a faixa não tem teto.
    /// </summary>
    public class FaixaEmolumento
    {
        public FaixaEmolumento(int tipoAtoId, decimal valorDe, decimal? valorAte, decimal emolumento, decimal custas)
        {
            TipoAtoId = tipoAtoId;
            ValorDe = valorDe;
            ValorAte = valorAte;
            Emolumento = emolumento;
            Custas = custas;
        }

        public int TipoAtoId { get; }
        public decimal ValorDe { get; }
        public decimal? ValorAte { get; }
        public decimal Emolumento { get; }
        public decimal Custas { get; }

        public bool Contem(decimal valor)
        {
            return valor >= ValorDe && (!ValorAte.HasValue || valor <= ValorAte.Value);
        }
    }
}
