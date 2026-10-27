using System;
using System.Collections.Generic;
using Cartorio.Entidades;

namespace Cartorio.Negocio
{
    public interface IClienteRepositorio
    {
        Cliente ObterPorId(int id);
        Cliente ObterPorCpf(string cpf);
        IList<Cliente> BuscarPorNome(string trecho);
        IList<Cliente> Listar();
        void Adicionar(Cliente cliente);
        void Atualizar(Cliente cliente);
    }

    public interface IProtocoloRepositorio
    {
        /// <summary>Carrega o protocolo com o cliente.</summary>
        Protocolo ObterPorId(int id);

        Protocolo ObterPorNumero(string numero);
        IList<Protocolo> ListarPorCpfCliente(string cpf);
        IList<Protocolo> ListarRecentes(int quantidade);
        IList<ProtocoloHistorico> ListarHistorico(int protocoloId);

        /// <summary>Protocolos abertos em [inicio, fimExclusivo), opcionalmente filtrados por status.</summary>
        IList<Protocolo> ListarPorPeriodo(DateTime inicio, DateTime fimExclusivo, StatusProtocolo? status);

        /// <summary>Maior número já usado no ano (ex.: "2026-000041") ou nulo se não houver.</summary>
        string ObterUltimoNumeroDoAno(int ano);

        /// <summary>Grava o protocolo e o primeiro registro de histórico na mesma transação.</summary>
        void Adicionar(Protocolo protocolo, ProtocoloHistorico historicoInicial);

        /// <summary>Grava o novo status e o histórico na mesma transação.</summary>
        void AtualizarStatus(Protocolo protocolo, ProtocoloHistorico historico);
    }
}
