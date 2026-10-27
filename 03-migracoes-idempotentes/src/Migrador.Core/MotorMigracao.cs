using System;
using System.Collections.Generic;
using System.Linq;

namespace Migrador.Core
{
    public class ResultadoExecucao
    {
        public bool Simulacao { get; set; }
        public IList<Migracao> Aplicadas { get; } = new List<Migracao>();
        public int JaAplicadas { get; set; }
        public IList<string> Alertas { get; } = new List<string>();
    }

    /// <summary>
    /// Decide o que aplicar comparando os scripts da pasta com a tabela de controle do banco.
    /// </summary>
    public class MotorMigracao
    {
        private readonly IExecutorBanco _executor;
        private readonly Action<string> _log;

        public MotorMigracao(IExecutorBanco executor, Action<string> log = null)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _log = log ?? (_ => { });
        }

        /// <param name="simular">Só mostra o que seria aplicado, sem tocar no banco (nem criar a tabela de controle).</param>
        public ResultadoExecucao Executar(IList<Migracao> migracoes, bool simular = false)
        {
            if (migracoes == null)
                throw new ArgumentNullException(nameof(migracoes));

            var resultado = new ResultadoExecucao { Simulacao = simular };

            if (!simular)
                _executor.GarantirTabelaDeControle();

            var aplicadas = _executor.ListarAplicadas().ToDictionary(a => a.Versao);
            var noDisco = migracoes.ToDictionary(m => m.Versao);

            foreach (var registro in aplicadas.Values.OrderBy(a => a.Versao))
            {
                Migracao arquivo;
                if (!noDisco.TryGetValue(registro.Versao, out arquivo))
                {
                    resultado.Alertas.Add("A versão " + registro.Versao + " (" + registro.NomeArquivo +
                                          ") está aplicada no banco, mas o arquivo não existe mais na pasta.");
                }
                else if (!string.Equals(arquivo.Checksum, registro.Checksum, StringComparison.OrdinalIgnoreCase))
                {
                    resultado.Alertas.Add("O script " + arquivo.NomeArquivo + " foi alterado depois de aplicado. " +
                                          "Ele não será executado de novo: crie um script novo com a mudança.");
                }
            }

            resultado.JaAplicadas = migracoes.Count(m => aplicadas.ContainsKey(m.Versao));

            var pendentes = migracoes.Where(m => !aplicadas.ContainsKey(m.Versao)).OrderBy(m => m.Versao).ToList();
            var ultimaAplicada = aplicadas.Count == 0 ? 0 : aplicadas.Keys.Max();

            // Versão menor que a última aplicada (típico de merge de branches): para antes de mexer no banco.
            var foraDeOrdem = pendentes.FirstOrDefault(m => m.Versao < ultimaAplicada);
            if (foraDeOrdem != null)
            {
                throw new MigracaoException(
                    "O script " + foraDeOrdem.NomeArquivo + " tem versão menor que a última aplicada (" + ultimaAplicada +
                    "). Renumere-o com uma versão maior que " + ultimaAplicada + ".", foraDeOrdem);
            }

            foreach (var migracao in pendentes)
            {
                _log((simular ? "[simulação] Aplicaria " : "Aplicando ") + migracao.NomeArquivo);
                if (!simular)
                {
                    try
                    {
                        _executor.Aplicar(migracao);
                    }
                    catch (Exception ex)
                    {
                        throw new MigracaoException(
                            "Falha ao aplicar " + migracao.NomeArquivo + " no " + _executor.NomeBanco + ": " + ex.Message +
                            " Nada deste script foi gravado; as migrações anteriores continuam aplicadas.",
                            migracao, ex);
                    }
                }
                resultado.Aplicadas.Add(migracao);
            }

            return resultado;
        }
    }
}
