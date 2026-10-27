using System.Security.Cryptography;
using System.Text;

namespace Migrador.Core
{
    public static class Checksum
    {
        /// <summary>
        /// SHA-256 em hexadecimal (64 caracteres). As quebras de linha são normalizadas antes,
        /// para o mesmo script dar o mesmo checksum no Windows (CRLF) e no Linux (LF), já que
        /// o Git pode converter as quebras ao fazer checkout.
        /// </summary>
        public static string Calcular(string conteudo)
        {
            var normalizado = (conteudo ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n').TrimEnd();

            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(normalizado));
                var hex = new StringBuilder(hash.Length * 2);
                foreach (var b in hash)
                    hex.Append(b.ToString("x2"));
                return hex.ToString();
            }
        }
    }
}
