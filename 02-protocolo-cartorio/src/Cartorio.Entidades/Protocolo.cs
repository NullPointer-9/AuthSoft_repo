using System;

namespace Cartorio.Entidades
{
    public class Protocolo
    {
        public int Id { get; set; }

        /// <summary>Formato AAAA-NNNNNN, ex.: 2026-000001.</summary>
        public string Numero { get; set; }

        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        public string TipoServico { get; set; }
        public string Descricao { get; set; }
        public StatusProtocolo Status { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public DateTime? DataEntrega { get; set; }
    }
}
