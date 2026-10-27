namespace Emolumentos.Apresentacao
{
    /// <summary>Gera o demonstrativo do cálculo em PDF assinado.</summary>
    public interface IEmissorDemonstrativo
    {
        void Emitir(ResultadoCalculoViewModel resultado, string caminhoDestino);
    }
}
