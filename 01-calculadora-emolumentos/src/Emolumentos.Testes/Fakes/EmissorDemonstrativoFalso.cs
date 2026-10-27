using System;
using Emolumentos.Apresentacao;

namespace Emolumentos.Testes.Fakes
{
    public class EmissorDemonstrativoFalso : IEmissorDemonstrativo
    {
        public ResultadoCalculoViewModel ResultadoEmitido { get; private set; }
        public string CaminhoEmitido { get; private set; }
        public int Emissoes { get; private set; }
        public bool Falhar { get; set; }

        public void Emitir(ResultadoCalculoViewModel resultado, string caminhoDestino)
        {
            if (Falhar)
                throw new InvalidOperationException("Disco cheio");

            ResultadoEmitido = resultado;
            CaminhoEmitido = caminhoDestino;
            Emissoes++;
        }
    }
}
