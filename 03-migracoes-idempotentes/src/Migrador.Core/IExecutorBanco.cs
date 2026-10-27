using System.Collections.Generic;

namespace Migrador.Core
{
    /// <summary>O que o motor precisa de cada banco. SQL Server e PostgreSQL implementam.</summary>
    public interface IExecutorBanco
    {
        /// <summary>Nome para exibição, ex.: "SQL Server".</summary>
        string NomeBanco { get; }

        /// <summary>Cria a tabela historico_migracao se ela ainda não existir.</summary>
        void GarantirTabelaDeControle();

        /// <summary>Migrações já registradas. Lista vazia se a tabela de controle ainda não existe.</summary>
        IList<MigracaoAplicada> ListarAplicadas();

        /// <summary>
        /// Roda o script e grava o registro na tabela de controle numa única transação:
        /// ou fica tudo, ou nada.
        /// </summary>
        void Aplicar(Migracao migracao);
    }
}
