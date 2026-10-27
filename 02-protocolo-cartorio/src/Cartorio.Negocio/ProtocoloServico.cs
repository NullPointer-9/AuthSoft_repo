using System;
using System.Collections.Generic;
using System.Globalization;
using Cartorio.Entidades;

namespace Cartorio.Negocio
{
    public class ProtocoloServico
    {
        private readonly IProtocoloRepositorio _protocolos;
        private readonly IClienteRepositorio _clientes;
        private readonly IRelogio _relogio;

        public ProtocoloServico(IProtocoloRepositorio protocolos, IClienteRepositorio clientes, IRelogio relogio)
        {
            _protocolos = protocolos ?? throw new ArgumentNullException(nameof(protocolos));
            _clientes = clientes ?? throw new ArgumentNullException(nameof(clientes));
            _relogio = relogio ?? throw new ArgumentNullException(nameof(relogio));
        }

        public Protocolo Abrir(int clienteId, string tipoServico, string descricao)
        {
            tipoServico = (tipoServico ?? string.Empty).Trim();
            descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();

            if (tipoServico.Length == 0)
                throw new NegocioException("Informe o tipo de serviço.");
            if (tipoServico.Length > 80)
                throw new NegocioException("O tipo de serviço pode ter no máximo 80 caracteres.");
            if (descricao != null && descricao.Length > 500)
                throw new NegocioException("A descrição pode ter no máximo 500 caracteres.");

            var cliente = _clientes.ObterPorId(clienteId);
            if (cliente == null)
                throw new RegistroNaoEncontradoException("Cliente não encontrado. Localize o cliente pelo CPF antes de abrir o protocolo.");

            var agora = _relogio.Agora;
            var protocolo = new Protocolo
            {
                Numero = GerarProximoNumero(agora.Year, _protocolos.ObterUltimoNumeroDoAno(agora.Year)),
                ClienteId = cliente.Id,
                Cliente = cliente,
                TipoServico = tipoServico,
                Descricao = descricao,
                Status = StatusProtocolo.Recebido,
                DataAbertura = agora,
                DataAtualizacao = agora
            };

            var historico = new ProtocoloHistorico
            {
                StatusAnterior = null,
                StatusNovo = StatusProtocolo.Recebido,
                DataAlteracao = agora,
                Observacao = "Protocolo aberto"
            };

            _protocolos.Adicionar(protocolo, historico);
            return protocolo;
        }

        /// <summary>Move o protocolo para o próximo status e registra no histórico.</summary>
        public Protocolo AvancarStatus(int protocoloId, string observacao)
        {
            var protocolo = _protocolos.ObterPorId(protocoloId);
            if (protocolo == null)
                throw new RegistroNaoEncontradoException("Protocolo não encontrado.");

            var proximo = RegrasStatus.Proximo(protocolo.Status);
            if (!proximo.HasValue)
                throw new NegocioException("O protocolo " + protocolo.Numero + " já foi entregue e não pode mais mudar de status.");

            observacao = string.IsNullOrWhiteSpace(observacao) ? null : observacao.Trim();
            if (observacao != null && observacao.Length > 300)
                throw new NegocioException("A observação pode ter no máximo 300 caracteres.");

            var agora = _relogio.Agora;
            var historico = new ProtocoloHistorico
            {
                ProtocoloId = protocolo.Id,
                StatusAnterior = protocolo.Status,
                StatusNovo = proximo.Value,
                DataAlteracao = agora,
                Observacao = observacao
            };

            protocolo.Status = proximo.Value;
            protocolo.DataAtualizacao = agora;
            if (proximo.Value == StatusProtocolo.Entregue)
                protocolo.DataEntrega = agora;

            _protocolos.AtualizarStatus(protocolo, historico);
            return protocolo;
        }

        public Protocolo BuscarPorNumero(string numero)
        {
            numero = (numero ?? string.Empty).Trim();
            if (numero.Length == 0)
                throw new NegocioException("Informe o número do protocolo.");

            var protocolo = _protocolos.ObterPorNumero(numero);
            if (protocolo == null)
                throw new RegistroNaoEncontradoException("Nenhum protocolo encontrado com o número " + numero + ".");

            return protocolo;
        }

        public IList<Protocolo> BuscarPorCpf(string cpf)
        {
            var cpfNormalizado = ValidadorCpf.Normalizar(cpf);
            if (!ValidadorCpf.EhValido(cpfNormalizado))
                throw new NegocioException("CPF inválido. Confira os números digitados.");

            return _protocolos.ListarPorCpfCliente(cpfNormalizado);
        }

        public IList<Protocolo> ListarRecentes() => _protocolos.ListarRecentes(100);

        public IList<ProtocoloHistorico> ListarHistorico(int protocoloId) => _protocolos.ListarHistorico(protocoloId);

        /// <summary>Período máximo do relatório, para não puxar a base inteira por engano.</summary>
        public const int DiasMaximosRelatorio = 366;

        /// <summary>
        /// Protocolos abertos entre as duas datas (inclusive), opcionalmente de um status só.
        /// </summary>
        public IList<Protocolo> ConsultarParaRelatorio(DateTime dataInicial, DateTime dataFinal, StatusProtocolo? status)
        {
            var inicio = dataInicial.Date;
            var fim = dataFinal.Date;

            if (fim < inicio)
                throw new NegocioException("A data final não pode ser anterior à data inicial.");
            if ((fim - inicio).TotalDays >= DiasMaximosRelatorio)
                throw new NegocioException("O período do relatório pode ter no máximo " + DiasMaximosRelatorio + " dias.");

            return _protocolos.ListarPorPeriodo(inicio, fim.AddDays(1), status);
        }

        /// <summary>
        /// Busca o protocolo atualizado e confere se o comprovante pedido pode ser emitido.
        /// </summary>
        public Protocolo ObterParaComprovante(int protocoloId, TipoComprovante tipo)
        {
            var protocolo = _protocolos.ObterPorId(protocoloId);
            if (protocolo == null)
                throw new RegistroNaoEncontradoException("Protocolo não encontrado.");

            if (tipo == TipoComprovante.Entrega && protocolo.Status != StatusProtocolo.Entregue)
                throw new NegocioException("O comprovante de entrega só pode ser emitido depois que o protocolo " +
                                           protocolo.Numero + " for entregue.");

            return protocolo;
        }

        /// <summary>
        /// "2026-000041" vira "2026-000042". Sem número no ano, começa em "AAAA-000001".
        /// </summary>
        public static string GerarProximoNumero(int ano, string ultimoNumeroDoAno)
        {
            var sequencia = 0;
            if (!string.IsNullOrEmpty(ultimoNumeroDoAno))
            {
                var partes = ultimoNumeroDoAno.Split('-');
                if (partes.Length != 2 || partes[0] != ano.ToString(CultureInfo.InvariantCulture) ||
                    !int.TryParse(partes[1], NumberStyles.None, CultureInfo.InvariantCulture, out sequencia))
                {
                    throw new InvalidOperationException("Número de protocolo fora do padrão no banco: " + ultimoNumeroDoAno);
                }
            }

            if (sequencia >= 999999)
                throw new NegocioException("Limite de protocolos do ano atingido.");

            return string.Format(CultureInfo.InvariantCulture, "{0}-{1:000000}", ano, sequencia + 1);
        }
    }
}
