using Cartorio.Entidades;

namespace Cartorio.Negocio
{
    /// <summary>
    /// O protocolo anda sempre um passo por vez: Recebido → Em análise → Pronto → Entregue.
    /// </summary>
    public static class RegrasStatus
    {
        public static StatusProtocolo? Proximo(StatusProtocolo atual)
        {
            switch (atual)
            {
                case StatusProtocolo.Recebido: return StatusProtocolo.EmAnalise;
                case StatusProtocolo.EmAnalise: return StatusProtocolo.Pronto;
                case StatusProtocolo.Pronto: return StatusProtocolo.Entregue;
                default: return null;
            }
        }

        public static string Descricao(StatusProtocolo status)
        {
            switch (status)
            {
                case StatusProtocolo.Recebido: return "Recebido";
                case StatusProtocolo.EmAnalise: return "Em análise";
                case StatusProtocolo.Pronto: return "Pronto";
                case StatusProtocolo.Entregue: return "Entregue";
                default: return status.ToString();
            }
        }
    }
}
