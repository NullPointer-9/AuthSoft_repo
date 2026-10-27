using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Migrador.Core
{
    public static class LeitorDeScripts
    {
        // 0001_criar_tabela_cliente.sql → versão 1, descrição "criar_tabela_cliente"
        private static readonly Regex PadraoNome = new Regex(@"^(?<versao>\d+)_(?<descricao>[^.]+)\.sql$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Lê os .sql em ordem de versão numérica (0010 depois de 0002). Nome fora do padrão
        /// é erro, para nenhum script ser ignorado sem aviso.
        /// </summary>
        public static IList<Migracao> Ler(string pasta)
        {
            if (!Directory.Exists(pasta))
                throw new DirectoryNotFoundException("Pasta de scripts não encontrada: " + Path.GetFullPath(pasta));

            var migracoes = new List<Migracao>();
            foreach (var caminho in Directory.GetFiles(pasta, "*.sql"))
            {
                var nome = Path.GetFileName(caminho);
                var partes = PadraoNome.Match(nome);
                if (!partes.Success)
                {
                    throw new ScriptInvalidoException(
                        "Nome de script fora do padrão: \"" + nome + "\". Use NNNN_descricao.sql, ex.: 0004_criar_tabela_pedido.sql");
                }

                var versao = int.Parse(partes.Groups["versao"].Value, CultureInfo.InvariantCulture);
                var conteudo = File.ReadAllText(caminho, Encoding.UTF8);
                migracoes.Add(new Migracao(versao, partes.Groups["descricao"].Value, nome, conteudo));
            }

            var repetida = migracoes.GroupBy(m => m.Versao).FirstOrDefault(g => g.Count() > 1);
            if (repetida != null)
            {
                throw new ScriptInvalidoException(
                    "Versão " + repetida.Key + " repetida em: " + string.Join(", ", repetida.Select(m => m.NomeArquivo)));
            }

            return migracoes.OrderBy(m => m.Versao).ToList();
        }
    }
}
