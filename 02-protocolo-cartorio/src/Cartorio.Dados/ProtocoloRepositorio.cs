using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Cartorio.Entidades;
using Cartorio.Negocio;

namespace Cartorio.Dados
{
    public class ProtocoloRepositorio : IProtocoloRepositorio
    {
        private readonly string _connectionString;

        public ProtocoloRepositorio(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public Protocolo ObterPorId(int id)
        {
            using (var contexto = new CartorioContext(_connectionString))
                return contexto.Protocolos.AsNoTracking().Include(p => p.Cliente).FirstOrDefault(p => p.Id == id);
        }

        public Protocolo ObterPorNumero(string numero)
        {
            using (var contexto = new CartorioContext(_connectionString))
                return contexto.Protocolos.AsNoTracking().Include(p => p.Cliente).FirstOrDefault(p => p.Numero == numero);
        }

        public IList<Protocolo> ListarPorCpfCliente(string cpf)
        {
            using (var contexto = new CartorioContext(_connectionString))
            {
                return contexto.Protocolos.AsNoTracking()
                    .Include(p => p.Cliente)
                    .Where(p => p.Cliente.Cpf == cpf)
                    .OrderByDescending(p => p.DataAbertura)
                    .ToList();
            }
        }

        public IList<Protocolo> ListarRecentes(int quantidade)
        {
            using (var contexto = new CartorioContext(_connectionString))
            {
                return contexto.Protocolos.AsNoTracking()
                    .Include(p => p.Cliente)
                    .OrderByDescending(p => p.DataAtualizacao)
                    .Take(quantidade)
                    .ToList();
            }
        }

        public IList<ProtocoloHistorico> ListarHistorico(int protocoloId)
        {
            using (var contexto = new CartorioContext(_connectionString))
            {
                return contexto.Historicos.AsNoTracking()
                    .Where(h => h.ProtocoloId == protocoloId)
                    .OrderBy(h => h.DataAlteracao).ThenBy(h => h.Id)
                    .ToList();
            }
        }

        public IList<Protocolo> ListarPorPeriodo(DateTime inicio, DateTime fimExclusivo, StatusProtocolo? status)
        {
            using (var contexto = new CartorioContext(_connectionString))
            {
                var consulta = contexto.Protocolos.AsNoTracking()
                    .Include(p => p.Cliente)
                    .Where(p => p.DataAbertura >= inicio && p.DataAbertura < fimExclusivo);

                if (status.HasValue)
                {
                    var filtro = status.Value;
                    consulta = consulta.Where(p => p.Status == filtro);
                }

                return consulta.OrderBy(p => p.Numero).ToList();
            }
        }

        public string ObterUltimoNumeroDoAno(int ano)
        {
            var prefixo = ano + "-";
            using (var contexto = new CartorioContext(_connectionString))
            {
                // O número tem tamanho fixo (AAAA-NNNNNN), então a ordem de texto é a ordem numérica.
                return contexto.Protocolos.AsNoTracking()
                    .Where(p => p.Numero.StartsWith(prefixo))
                    .OrderByDescending(p => p.Numero)
                    .Select(p => p.Numero)
                    .FirstOrDefault();
            }
        }

        public void Adicionar(Protocolo protocolo, ProtocoloHistorico historicoInicial)
        {
            using (var contexto = new CartorioContext(_connectionString))
            {
                // O cliente já existe: não deixar o EF tentar inseri-lo de novo.
                var cliente = protocolo.Cliente;
                protocolo.Cliente = null;

                contexto.Protocolos.Add(protocolo);
                try
                {
                    // Um SaveChanges = uma transação. O histórico precisa do Id gerado do protocolo,
                    // por isso os dois vão juntos numa transação explícita.
                    using (var transacao = contexto.Database.BeginTransaction())
                    {
                        contexto.SaveChanges();

                        historicoInicial.ProtocoloId = protocolo.Id;
                        contexto.Historicos.Add(historicoInicial);
                        contexto.SaveChanges();

                        transacao.Commit();
                    }
                }
                catch (DbUpdateException ex)
                {
                    throw ErrosDeBanco.Traduzir(ex, "Outro atendente acabou de usar este número de protocolo. Tente abrir novamente.");
                }
                finally
                {
                    protocolo.Cliente = cliente;
                }
            }
        }

        public void AtualizarStatus(Protocolo protocolo, ProtocoloHistorico historico)
        {
            using (var contexto = new CartorioContext(_connectionString))
            {
                var registro = contexto.Protocolos.Find(protocolo.Id);
                if (registro == null)
                    throw new RegistroNaoEncontradoException("Protocolo não encontrado.");

                registro.Status = protocolo.Status;
                registro.DataAtualizacao = protocolo.DataAtualizacao;
                registro.DataEntrega = protocolo.DataEntrega;
                contexto.Historicos.Add(historico);

                // Status e histórico no mesmo SaveChanges: ou grava os dois, ou nenhum.
                contexto.SaveChanges();
            }
        }
    }
}
