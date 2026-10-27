namespace Cartorio.Entidades
{
    /// <summary>Os valores batem com a tabela dbo.StatusProtocolo (coluna TINYINT).</summary>
    public enum StatusProtocolo : byte
    {
        Recebido = 1,
        EmAnalise = 2,
        Pronto = 3,
        Entregue = 4
    }
}
