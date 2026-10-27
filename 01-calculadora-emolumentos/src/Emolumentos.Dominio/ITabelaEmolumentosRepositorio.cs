using System.Collections.Generic;

namespace Emolumentos.Dominio
{
    public interface ITabelaEmolumentosRepositorio
    {
        IList<Uf> ListarUfs();

        IList<TipoAto> ListarTiposAto();

        /// <summary>Todas as faixas da UF, de todos os tipos de ato. Lista vazia se a UF não tem tabela.</summary>
        IList<FaixaEmolumento> ObterFaixas(string siglaUf);
    }
}
