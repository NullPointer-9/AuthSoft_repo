using System;

namespace Cartorio.Entidades
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        /// <summary>Somente os 11 dígitos, sem pontuação.</summary>
        public string Cpf { get; set; }

        public string Email { get; set; }
        public string Telefone { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
