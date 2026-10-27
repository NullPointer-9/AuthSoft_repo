using System;

namespace Migrador.Core
{
    /// <summary>Problema nos arquivos de script (nome fora do padrão, versão repetida...).</summary>
    public class ScriptInvalidoException : Exception
    {
        public ScriptInvalidoException(string mensagem) : base(mensagem)
        {
        }
    }

    /// <summary>Falha ao aplicar uma migração. As anteriores continuam aplicadas.</summary>
    public class MigracaoException : Exception
    {
        public MigracaoException(string mensagem, Migracao migracao, Exception interna = null)
            : base(mensagem, interna)
        {
            Migracao = migracao;
        }

        public Migracao Migracao { get; }
    }
}
