using System;

namespace Cartorio.Negocio
{
    /// <summary>Abstrai DateTime.Now para os testes poderem fixar a data (ex.: virada de ano na numeração).</summary>
    public interface IRelogio
    {
        DateTime Agora { get; }
    }

    public class RelogioSistema : IRelogio
    {
        public DateTime Agora => DateTime.Now;
    }
}
