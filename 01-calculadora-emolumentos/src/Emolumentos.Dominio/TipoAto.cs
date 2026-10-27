namespace Emolumentos.Dominio
{
    public class TipoAto
    {
        public TipoAto(int id, string codigo, string descricao, bool exigeValor)
        {
            Id = id;
            Codigo = codigo;
            Descricao = descricao;
            ExigeValor = exigeValor;
        }

        public int Id { get; }
        public string Codigo { get; }
        public string Descricao { get; }

        /// <summary>
        /// Falso para atos cobrados por valor fixo, sem valor de negócio declarado
        /// (procuração, reconhecimento de firma).
        /// </summary>
        public bool ExigeValor { get; }

        public override string ToString() => Descricao;
    }
}
