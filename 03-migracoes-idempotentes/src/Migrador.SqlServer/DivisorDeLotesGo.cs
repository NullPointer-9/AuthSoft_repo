using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Migrador.SqlServer
{
    /// <summary>
    /// Divide um script T-SQL nos lotes separados por GO.
    /// GO não é T-SQL: é um comando do SSMS/sqlcmd. Mandar "GO" para o SqlCommand dá erro de sintaxe,
    /// e comandos como CREATE VIEW/PROCEDURE precisam ser os primeiros do lote.
    /// </summary>
    public static class DivisorDeLotesGo
    {
        // "GO", "go", "GO 3" (repete o lote 3 vezes), "GO -- comentário". Precisa estar sozinho na linha.
        private static readonly Regex SeparadorGo = new Regex(@"^\s*GO(?:\s+(?<repeticoes>\d+))?\s*(?:--.*)?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static IList<string> Dividir(string script)
        {
            var lotes = new List<string>();
            var atual = new StringBuilder();
            var dentroDeComentarioDeBloco = false;

            foreach (var linha in (script ?? string.Empty).Replace("\r\n", "\n").Split('\n'))
            {
                var separador = dentroDeComentarioDeBloco ? Match.Empty : SeparadorGo.Match(linha);
                if (separador.Success)
                {
                    var repeticoes = separador.Groups["repeticoes"].Success
                        ? int.Parse(separador.Groups["repeticoes"].Value, CultureInfo.InvariantCulture)
                        : 1;
                    FecharLote(lotes, atual, repeticoes);
                    continue;
                }

                dentroDeComentarioDeBloco = TerminaDentroDeComentarioDeBloco(linha, dentroDeComentarioDeBloco);
                atual.Append(linha).Append('\n');
            }

            FecharLote(lotes, atual, 1);
            return lotes;
        }

        private static void FecharLote(List<string> lotes, StringBuilder atual, int repeticoes)
        {
            var texto = atual.ToString().Trim();
            atual.Clear();

            if (texto.Length == 0)
                return;

            for (var i = 0; i < repeticoes; i++)
                lotes.Add(texto);
        }

        /// <summary>
        /// Acompanha /* ... */ que atravessa linhas, para um "GO" dentro de comentário não virar separador.
        /// Ignora o que estiver em comentário de linha (--) e em literais de texto ('...').
        /// </summary>
        private static bool TerminaDentroDeComentarioDeBloco(string linha, bool comecaDentro)
        {
            var dentro = comecaDentro;
            for (var i = 0; i < linha.Length; i++)
            {
                var atual = linha[i];
                var proximo = i + 1 < linha.Length ? linha[i + 1] : '\0';

                if (dentro)
                {
                    if (atual == '*' && proximo == '/')
                    {
                        dentro = false;
                        i++;
                    }
                    continue;
                }

                if (atual == '-' && proximo == '-')
                    break;

                if (atual == '\'')
                {
                    var fim = linha.IndexOf('\'', i + 1);
                    if (fim < 0)
                        break;
                    i = fim;
                    continue;
                }

                if (atual == '/' && proximo == '*')
                {
                    dentro = true;
                    i++;
                }
            }
            return dentro;
        }
    }
}
