using System;

namespace Legado.Negocio
{
    public static class TabelaPrecos
    {
        public const decimal AcrescimoUrgencia = 1.5m;

        public static decimal PrecoBase(TipoCertidao tipo)
        {
            switch (tipo)
            {
                case TipoCertidao.Nascimento: return 45.90m;
                case TipoCertidao.Casamento: return 45.90m;
                case TipoCertidao.Obito: return 52.35m;
                default: throw new ArgumentOutOfRangeException(nameof(tipo));
            }
        }

        /// <summary>
        /// Urgente custa 50% a mais, arredondado para centavos "para cima no meio" (78,525 → 78,53).
        /// Em decimal, e não double como no legado: dinheiro em ponto flutuante binário acumula erro nas somas.
        /// </summary>
        public static decimal Preco(TipoCertidao tipo, bool urgente)
        {
            var valor = PrecoBase(tipo) * (urgente ? AcrescimoUrgencia : 1m);
            return decimal.Round(valor, 2, MidpointRounding.AwayFromZero);
        }
    }
}
