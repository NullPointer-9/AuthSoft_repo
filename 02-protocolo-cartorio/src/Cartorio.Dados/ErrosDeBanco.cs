using System;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using Cartorio.Negocio;

namespace Cartorio.Dados
{
    internal static class ErrosDeBanco
    {
        // 2601: índice único duplicado. 2627: violação de UNIQUE/PRIMARY KEY.
        private const int ChaveDuplicadaIndice = 2601;
        private const int ChaveDuplicadaRestricao = 2627;

        /// <summary>
        /// Converte violação de chave única em <see cref="NegocioException"/>. Cobre a corrida
        /// em que duas estações gravam o mesmo CPF ou número de protocolo ao mesmo tempo.
        /// </summary>
        public static Exception Traduzir(DbUpdateException ex, string mensagemDuplicado)
        {
            for (Exception atual = ex; atual != null; atual = atual.InnerException)
            {
                var sql = atual as SqlException;
                if (sql != null && (sql.Number == ChaveDuplicadaIndice || sql.Number == ChaveDuplicadaRestricao))
                    return new NegocioException(mensagemDuplicado, ex);
            }
            return ex;
        }
    }
}
