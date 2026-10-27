using System;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using Cartorio.Negocio;

namespace Cartorio.WinForms
{
    /// <summary>Regra de negócio vira aviso; falha de conexão vira orientação; o resto vai para o log.</summary>
    internal static class TratamentoDeErros
    {
        public static readonly string CaminhoLog = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CartorioProtocolo", "erros.log");

        /// <summary>Executa a ação mostrando cursor de espera e tratando qualquer exceção.</summary>
        public static bool Executar(IWin32Window dono, Action acao)
        {
            var cursorAnterior = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                acao();
                return true;
            }
            catch (Exception ex)
            {
                Cursor.Current = cursorAnterior;
                Exibir(dono, ex);
                return false;
            }
            finally
            {
                Cursor.Current = cursorAnterior;
            }
        }

        public static void Exibir(IWin32Window dono, Exception ex)
        {
            if (ex is NegocioException)
            {
                MessageBox.Show(dono, ex.Message, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Registrar(ex);

            var mensagem = EhFalhaDeConexao(ex)
                ? "Não foi possível conectar ao banco de dados.\n\n" +
                  "Verifique se o container está rodando (docker compose up -d) e tente novamente."
                : "Ocorreu um erro inesperado e a operação não foi concluída.\n\n" +
                  "Os detalhes foram gravados em:\n" + CaminhoLog;

            MessageBox.Show(dono, mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void Registrar(Exception ex)
        {
            if (ex == null)
                return;

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(CaminhoLog));
                File.AppendAllText(CaminhoLog,
                    "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + ex + Environment.NewLine + Environment.NewLine);
            }
            catch (IOException)
            {
                // Falhar ao gravar o log não pode derrubar a aplicação.
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        private static bool EhFalhaDeConexao(Exception ex)
        {
            for (var atual = ex; atual != null; atual = atual.InnerException)
            {
                if (atual is SqlException || atual is EntityException)
                    return true;
            }
            return false;
        }
    }
}
