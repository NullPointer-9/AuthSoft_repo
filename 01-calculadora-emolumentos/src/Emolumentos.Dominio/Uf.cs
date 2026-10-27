namespace Emolumentos.Dominio
{
    public class Uf
    {
        public Uf(string sigla, string nome)
        {
            Sigla = sigla;
            Nome = nome;
        }

        public string Sigla { get; }
        public string Nome { get; }

        public override string ToString() => Sigla + " - " + Nome;
    }
}
