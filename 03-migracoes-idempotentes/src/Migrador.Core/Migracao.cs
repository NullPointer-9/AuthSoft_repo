using System;

namespace Migrador.Core
{
    /// <summary>Um script de migração lido do disco, ex.: 0003_indice_cliente_nome.sql.</summary>
    public class Migracao
    {
        public Migracao(int versao, string descricao, string nomeArquivo, string conteudo)
        {
            Versao = versao;
            Descricao = descricao;
            NomeArquivo = nomeArquivo;
            Conteudo = conteudo ?? throw new ArgumentNullException(nameof(conteudo));
            Checksum = Core.Checksum.Calcular(conteudo);
        }

        public int Versao { get; }

        /// <summary>Parte do nome depois do número, ex.: "indice_cliente_nome".</summary>
        public string Descricao { get; }

        public string NomeArquivo { get; }
        public string Conteudo { get; }

        /// <summary>SHA-256 do conteúdo; serve para detectar script editado depois de aplicado.</summary>
        public string Checksum { get; }

        public override string ToString() => NomeArquivo;
    }

    /// <summary>Registro da tabela de controle (historico_migracao).</summary>
    public class MigracaoAplicada
    {
        public int Versao { get; set; }
        public string NomeArquivo { get; set; }
        public string Checksum { get; set; }
        public DateTime AplicadoEm { get; set; }
        public int DuracaoMs { get; set; }
    }
}
