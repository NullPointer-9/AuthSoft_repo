using System.Linq;

namespace Cartorio.Negocio
{
    public static class ValidadorCpf
    {
        /// <summary>Remove tudo que não for dígito. Retorna string vazia para nulo.</summary>
        public static string Normalizar(string cpf)
        {
            return cpf == null ? string.Empty : new string(cpf.Where(char.IsDigit).ToArray());
        }

        public static bool EhValido(string cpf)
        {
            var digitos = Normalizar(cpf);

            if (digitos.Length != 11)
                return false;

            // 000.000.000-00, 111.111.111-11 etc. passam no cálculo, mas não são CPFs válidos.
            if (digitos.All(c => c == digitos[0]))
                return false;

            return digitos[9] - '0' == CalcularDigito(digitos, 9)
                && digitos[10] - '0' == CalcularDigito(digitos, 10);
        }

        /// <summary>Formata como 000.000.000-00. Se não tiver 11 dígitos, devolve o texto original.</summary>
        public static string Formatar(string cpf)
        {
            var digitos = Normalizar(cpf);
            if (digitos.Length != 11)
                return cpf;

            return digitos.Substring(0, 3) + "." + digitos.Substring(3, 3) + "." +
                   digitos.Substring(6, 3) + "-" + digitos.Substring(9, 2);
        }

        private static int CalcularDigito(string digitos, int quantidade)
        {
            var soma = 0;
            for (var i = 0; i < quantidade; i++)
                soma += (digitos[i] - '0') * (quantidade + 1 - i);

            var resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }
    }
}
