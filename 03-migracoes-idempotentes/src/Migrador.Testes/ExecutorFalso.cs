using System;
using System.Collections.Generic;
using System.Linq;
using Migrador.Core;

namespace Migrador.Testes
{
    /// <summary>Banco em memória: guarda o que foi "executado" e a tabela de controle.</summary>
    public class ExecutorFalso : IExecutorBanco
    {
        public string NomeBanco => "Banco falso";
        public bool TabelaDeControleCriada { get; private set; }
        public List<MigracaoAplicada> Registros { get; } = new List<MigracaoAplicada>();
        public List<string> Executados { get; } = new List<string>();

        /// <summary>Simula erro de SQL ao aplicar esta versão.</summary>
        public int? FalharNaVersao { get; set; }

        public void GarantirTabelaDeControle() => TabelaDeControleCriada = true;

        public IList<MigracaoAplicada> ListarAplicadas() => Registros.ToList();

        public void Aplicar(Migracao migracao)
        {
            if (migracao.Versao == FalharNaVersao)
                throw new InvalidOperationException("Sintaxe incorreta perto de 'TABLEE'.");

            Executados.Add(migracao.NomeArquivo);
            Registros.Add(new MigracaoAplicada
            {
                Versao = migracao.Versao,
                NomeArquivo = migracao.NomeArquivo,
                Checksum = migracao.Checksum,
                AplicadoEm = DateTime.Now
            });
        }
    }
}
