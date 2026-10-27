using System;

namespace Legado.Negocio
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Telefone { get; set; }
    }

    public enum StatusPedido
    {
        Aberto,
        Pago,
        Entregue
    }

    public enum TipoCertidao
    {
        Nascimento,
        Casamento,
        Obito
    }

    public class PedidoCertidao
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }

        /// <summary>Texto como está no banco (ex.: "Certidao de nascimento").</summary>
        public string Tipo { get; set; }

        /// <summary>decimal, não double: dinheiro não pode ter erro de arredondamento binário.</summary>
        public decimal Valor { get; set; }

        public StatusPedido Status { get; set; }

        /// <summary>Nulo se a data gravada no banco não estiver no formato dd/MM/yyyy.</summary>
        public DateTime? DataPedido { get; set; }
    }
}
