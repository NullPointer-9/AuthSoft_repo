using System;
using Microsoft.Reporting.WinForms;

namespace Cartorio.Documentos
{
    internal static class DefinicaoRelatorio
    {
        /// <summary>Carrega um .rdlc embutido como recurso desta DLL (pasta Relatorios).</summary>
        public static void Carregar(LocalReport relatorio, string arquivoRdlc)
        {
            var recurso = "Cartorio.Documentos.Relatorios." + arquivoRdlc;
            using (var definicao = typeof(DefinicaoRelatorio).Assembly.GetManifestResourceStream(recurso))
            {
                if (definicao == null)
                    throw new InvalidOperationException("Relatório não encontrado nos recursos: " + recurso);

                relatorio.LoadReportDefinition(definicao);
            }
        }
    }
}
