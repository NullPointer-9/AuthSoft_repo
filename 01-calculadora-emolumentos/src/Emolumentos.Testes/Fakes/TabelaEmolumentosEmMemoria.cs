using System.Collections.Generic;
using System.Linq;
using Emolumentos.Dominio;

namespace Emolumentos.Testes.Fakes
{
    /// <summary>
    /// Repositório em memória com os mesmos números do seed de SP, mais casos montados para teste:
    /// RJ sem tabela, MG só com firma e XX com um buraco entre faixas.
    /// </summary>
    public class TabelaEmolumentosEmMemoria : ITabelaEmolumentosRepositorio
    {
        public static readonly TipoAto Escritura = new TipoAto(1, "ESCRITURA", "Escritura pública com valor declarado", true);
        public static readonly TipoAto Procuracao = new TipoAto(2, "PROCURACAO", "Procuração", false);
        public static readonly TipoAto Firma = new TipoAto(3, "RECONHECIMENTO_FIRMA", "Reconhecimento de firma por assinatura", false);

        private readonly Dictionary<string, List<FaixaEmolumento>> _faixas = new Dictionary<string, List<FaixaEmolumento>>
        {
            ["SP"] = new List<FaixaEmolumento>
            {
                new FaixaEmolumento(1, 0.00m, 20000.00m, 1200.00m, 380.00m),
                new FaixaEmolumento(1, 20000.01m, 100000.00m, 2400.00m, 760.00m),
                new FaixaEmolumento(1, 100000.01m, 500000.00m, 4100.00m, 1300.00m),
                new FaixaEmolumento(1, 500000.01m, null, 6800.00m, 2150.00m),
                new FaixaEmolumento(2, 0.00m, null, 210.00m, 66.00m),
                new FaixaEmolumento(3, 0.00m, null, 9.50m, 3.00m)
            },
            ["MG"] = new List<FaixaEmolumento>
            {
                new FaixaEmolumento(3, 0.00m, null, 7.80m, 2.30m)
            },
            ["XX"] = new List<FaixaEmolumento>
            {
                new FaixaEmolumento(1, 0.00m, 1000.00m, 100.00m, 10.00m),
                new FaixaEmolumento(1, 2000.00m, null, 300.00m, 30.00m)
            }
        };

        public bool FalharAoListar { get; set; }

        public IList<Uf> ListarUfs()
        {
            if (FalharAoListar)
                throw new System.InvalidOperationException("Banco fora do ar");

            return new List<Uf>
            {
                new Uf("MG", "Minas Gerais"),
                new Uf("RJ", "Rio de Janeiro"),
                new Uf("SP", "São Paulo")
            };
        }

        public IList<TipoAto> ListarTiposAto() => new List<TipoAto> { Escritura, Procuracao, Firma };

        public IList<FaixaEmolumento> ObterFaixas(string siglaUf)
        {
            List<FaixaEmolumento> faixas;
            return _faixas.TryGetValue(siglaUf, out faixas) ? faixas.ToList() : new List<FaixaEmolumento>();
        }
    }
}
