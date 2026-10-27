using System;

namespace Emolumentos.Dominio
{
    /// <summary>Falha de cálculo prevista; a mensagem é para o usuário.</summary>
    public abstract class CalculoEmolumentoException : Exception
    {
        protected CalculoEmolumentoException(string mensagem) : base(mensagem)
        {
        }
    }

    public class DadosCalculoInvalidosException : CalculoEmolumentoException
    {
        public DadosCalculoInvalidosException(string mensagem) : base(mensagem)
        {
        }
    }

    public class UfSemTabelaException : CalculoEmolumentoException
    {
        public UfSemTabelaException(string siglaUf)
            : base("Não há tabela de emolumentos cadastrada para " + siglaUf + ".")
        {
            SiglaUf = siglaUf;
        }

        public string SiglaUf { get; }
    }

    public class AtoSemTabelaException : CalculoEmolumentoException
    {
        public AtoSemTabelaException(string siglaUf, TipoAto tipoAto)
            : base("A tabela de " + siglaUf + " não tem valores para \"" + tipoAto.Descricao + "\".")
        {
            SiglaUf = siglaUf;
            TipoAtoId = tipoAto.Id;
        }

        public string SiglaUf { get; }
        public int TipoAtoId { get; }
    }

    /// <summary>
    /// A UF e o ato têm tabela, mas nenhuma faixa cobre o valor (buraco na tabela cadastrada).
    /// </summary>
    public class FaixaNaoEncontradaException : CalculoEmolumentoException
    {
        public FaixaNaoEncontradaException(string siglaUf, decimal valor)
            : base("Nenhuma faixa da tabela de " + siglaUf + " cobre o valor informado. Verifique a tabela cadastrada.")
        {
            SiglaUf = siglaUf;
            Valor = valor;
        }

        public string SiglaUf { get; }
        public decimal Valor { get; }
    }
}
