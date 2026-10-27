namespace Legado.Dados
{
    // Espelham as tabelas como estão hoje (valor em double, data em texto).

    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Telefone { get; set; }
    }

    public class PedidoCertidao
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public string Tipo { get; set; }
        public double Valor { get; set; }
        public string Status { get; set; }
        public string DataPedido { get; set; }
    }
}
