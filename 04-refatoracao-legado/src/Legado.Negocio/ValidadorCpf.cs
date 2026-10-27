using System.Linq;

namespace Legado.Negocio
{
    public static class ValidadorCpf
    {
        /// <summary>Só os dígitos. O legado gravou CPFs com e sem pontuação.</summary>
        public static string Normalizar(string cpf)
        {
            return cpf == null ? string.Empty : new string(cpf.Where(char.IsDigit).ToArray());
        }

        public static bool EhValido(string cpf)
        {
            var digitos = Normalizar(cpf);
            if (digitos.Length != 11 || digitos.All(c => c == digitos[0]))
                return false;

            return digitos[9] - '0' == Digito(digitos, 9) && digitos[10] - '0' == Digito(digitos, 10);
        }

        private static int Digito(string digitos, int quantidade)
        {
            var soma = 0;
            for (var i = 0; i < quantidade; i++)
                soma += (digitos[i] - '0') * (quantidade + 1 - i);

            var resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }
    }
}
