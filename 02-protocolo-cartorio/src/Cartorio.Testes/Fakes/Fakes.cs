using System;
using System.Collections.Generic;
using System.Linq;
using Cartorio.Entidades;
using Cartorio.Negocio;

namespace Cartorio.Testes.Fakes
{
    public class RelogioFixo : IRelogio
    {
        public RelogioFixo(DateTime agora)
        {
            Agora = agora;
        }

        public DateTime Agora { get; set; }
    }

    public class ClienteRepositorioEmMemoria : IClienteRepositorio
    {
        public readonly List<Cliente> Clientes = new List<Cliente>();

        public Cliente ObterPorId(int id) => Clientes.FirstOrDefault(c => c.Id == id);

        public Cliente ObterPorCpf(string cpf) => Clientes.FirstOrDefault(c => c.Cpf == cpf);

        public IList<Cliente> BuscarPorNome(string trecho) =>
            Clientes.Where(c => c.Nome.IndexOf(trecho, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

        public IList<Cliente> Listar() => Clientes.ToList();

        public void Adicionar(Cliente cliente)
        {
            cliente.Id = Clientes.Count == 0 ? 1 : Clientes.Max(c => c.Id) + 1;
            Clientes.Add(cliente);
        }

        public void Atualizar(Cliente cliente)
        {
            var indice = Clientes.FindIndex(c => c.Id == cliente.Id);
            Clientes[indice] = cliente;
        }
    }

    public class ProtocoloRepositorioEmMemoria : IProtocoloRepositorio
    {
        public readonly List<Protocolo> Protocolos = new List<Protocolo>();
        public readonly List<ProtocoloHistorico> Historicos = new List<ProtocoloHistorico>();

        public Protocolo ObterPorId(int id) => Protocolos.FirstOrDefault(p => p.Id == id);

        public Protocolo ObterPorNumero(string numero) => Protocolos.FirstOrDefault(p => p.Numero == numero);

        public IList<Protocolo> ListarPorCpfCliente(string cpf) =>
            Protocolos.Where(p => p.Cliente != null && p.Cliente.Cpf == cpf).ToList();

        public IList<Protocolo> ListarRecentes(int quantidade) =>
            Protocolos.OrderByDescending(p => p.DataAtualizacao).Take(quantidade).ToList();

        public IList<ProtocoloHistorico> ListarHistorico(int protocoloId) =>
            Historicos.Where(h => h.ProtocoloId == protocoloId).ToList();

        public IList<Protocolo> ListarPorPeriodo(DateTime inicio, DateTime fimExclusivo, StatusProtocolo? status) =>
            Protocolos.Where(p => p.DataAbertura >= inicio && p.DataAbertura < fimExclusivo)
                .Where(p => !status.HasValue || p.Status == status.Value)
                .OrderBy(p => p.Numero, StringComparer.Ordinal)
                .ToList();

        public string ObterUltimoNumeroDoAno(int ano) =>
            Protocolos.Select(p => p.Numero)
                .Where(n => n.StartsWith(ano + "-", StringComparison.Ordinal))
                .OrderByDescending(n => n, StringComparer.Ordinal)
                .FirstOrDefault();

        public void Adicionar(Protocolo protocolo, ProtocoloHistorico historicoInicial)
        {
            protocolo.Id = Protocolos.Count + 1;
            Protocolos.Add(protocolo);
            historicoInicial.ProtocoloId = protocolo.Id;
            historicoInicial.Id = Historicos.Count + 1;
            Historicos.Add(historicoInicial);
        }

        public void AtualizarStatus(Protocolo protocolo, ProtocoloHistorico historico)
        {
            historico.Id = Historicos.Count + 1;
            Historicos.Add(historico);
        }
    }
}
