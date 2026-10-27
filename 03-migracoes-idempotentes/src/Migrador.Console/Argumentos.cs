using System;

namespace Migrador.Console
{
    public class Argumentos
    {
        public const string SqlServer = "sqlserver";
        public const string Postgres = "postgres";

        public string Banco { get; private set; }

        /// <summary>Nulo quando não informado: usa a connection string do App.config com o nome do banco.</summary>
        public string Conexao { get; private set; }

        /// <summary>Nulo quando não informado: usa scripts\&lt;banco&gt; a partir da pasta atual.</summary>
        public string Pasta { get; private set; }

        public bool Simular { get; private set; }
        public bool Ajuda { get; private set; }

        public static Argumentos Interpretar(string[] args)
        {
            var resultado = new Argumentos();

            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--banco":
                        resultado.Banco = Valor(args, ref i).ToLowerInvariant();
                        break;
                    case "--conexao":
                        resultado.Conexao = Valor(args, ref i);
                        break;
                    case "--pasta":
                        resultado.Pasta = Valor(args, ref i);
                        break;
                    case "--simular":
                        resultado.Simular = true;
                        break;
                    case "--ajuda":
                    case "-h":
                    case "/?":
                        resultado.Ajuda = true;
                        break;
                    default:
                        throw new ArgumentException("Opção desconhecida: " + args[i]);
                }
            }

            if (resultado.Ajuda)
                return resultado;

            if (string.IsNullOrEmpty(resultado.Banco))
                throw new ArgumentException("Informe o banco com --banco sqlserver ou --banco postgres.");
            if (resultado.Banco != SqlServer && resultado.Banco != Postgres)
                throw new ArgumentException("Banco não suportado: \"" + resultado.Banco + "\". Use sqlserver ou postgres.");

            return resultado;
        }

        private static string Valor(string[] args, ref int i)
        {
            if (i + 1 >= args.Length || args[i + 1].StartsWith("--", StringComparison.Ordinal))
                throw new ArgumentException("Falta o valor de " + args[i] + ".");

            i++;
            return args[i];
        }
    }
}
