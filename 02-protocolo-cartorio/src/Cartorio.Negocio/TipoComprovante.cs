namespace Cartorio.Negocio
{
    public enum TipoComprovante
    {
        /// <summary>Entregue ao cliente quando o protocolo é aberto. Pode ser reimpresso a qualquer momento.</summary>
        Abertura,

        /// <summary>Atesta a entrega do documento. Só existe para protocolo com status Entregue.</summary>
        Entrega
    }
}
