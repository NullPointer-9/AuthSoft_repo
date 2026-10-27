using System;
using System.Globalization;

namespace Legado.Negocio
{
    /// <summary>
    /// Converte os textos gravados pelo sistema antigo. A leitura aceita as variações que existem
    /// no banco ("aberto", "Aberto", "ABERTO") e a escrita mantém o formato antigo.
    /// </summary>
    public static class FormatosLegados
    {
        public const string FormatoData = "dd/MM/yyyy";

        public static StatusPedido LerStatus(string texto)
        {
            switch ((texto ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "aberto": return StatusPedido.Aberto;
                case "pago": return StatusPedido.Pago;
                case "entregue": return StatusPedido.Entregue;
                default: throw new FormatException("Status de pedido desconhecido no banco: \"" + texto + "\".");
            }
        }

        public static string EscreverStatus(StatusPedido status)
        {
            return status.ToString().ToLowerInvariant();
        }

        public static DateTime? LerData(string texto)
        {
            DateTime data;
            return DateTime.TryParseExact((texto ?? string.Empty).Trim(), FormatoData, CultureInfo.InvariantCulture, DateTimeStyles.None, out data)
                ? data
                : (DateTime?)null;
        }

        public static string EscreverData(DateTime data)
        {
            return data.ToString(FormatoData, CultureInfo.InvariantCulture);
        }

        public static string DescricaoTipo(TipoCertidao tipo)
        {
            switch (tipo)
            {
                case TipoCertidao.Nascimento: return "Certidao de nascimento";
                case TipoCertidao.Casamento: return "Certidao de casamento";
                case TipoCertidao.Obito: return "Certidao de obito";
                default: throw new ArgumentOutOfRangeException(nameof(tipo));
            }
        }
    }
}
