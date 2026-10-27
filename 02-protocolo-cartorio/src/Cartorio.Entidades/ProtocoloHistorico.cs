using System;

namespace Cartorio.Entidades
{
    public class ProtocoloHistorico
    {
        public int Id { get; set; }
        public int ProtocoloId { get; set; }

        /// <summary>Nulo no registro de abertura do protocolo.</summary>
        public StatusProtocolo? StatusAnterior { get; set; }

        public StatusProtocolo StatusNovo { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string Observacao { get; set; }
    }
}
