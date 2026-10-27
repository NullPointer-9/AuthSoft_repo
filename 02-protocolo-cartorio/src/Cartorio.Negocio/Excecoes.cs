using System;

namespace Cartorio.Negocio
{
    /// <summary>Regra de negócio violada; a mensagem é para o usuário.</summary>
    public class NegocioException : Exception
    {
        public NegocioException(string mensagem) : base(mensagem)
        {
        }

        public NegocioException(string mensagem, Exception interna) : base(mensagem, interna)
        {
        }
    }

    public class RegistroNaoEncontradoException : NegocioException
    {
        public RegistroNaoEncontradoException(string mensagem) : base(mensagem)
        {
        }
    }
}
