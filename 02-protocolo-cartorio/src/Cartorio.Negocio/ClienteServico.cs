using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cartorio.Entidades;

namespace Cartorio.Negocio
{
    public class ClienteServico
    {
        private static readonly Regex EmailSimples = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        private readonly IClienteRepositorio _repositorio;
        private readonly IRelogio _relogio;

        public ClienteServico(IClienteRepositorio repositorio, IRelogio relogio)
        {
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
            _relogio = relogio ?? throw new ArgumentNullException(nameof(relogio));
        }

        public Cliente Cadastrar(string nome, string cpf, string email, string telefone)
        {
            var cpfNormalizado = ValidadorCpf.Normalizar(cpf);
            if (!ValidadorCpf.EhValido(cpfNormalizado))
                throw new NegocioException("CPF inválido. Confira os números digitados.");

            if (_repositorio.ObterPorCpf(cpfNormalizado) != null)
                throw new NegocioException("Já existe um cliente cadastrado com o CPF " + ValidadorCpf.Formatar(cpfNormalizado) + ".");

            var cliente = new Cliente
            {
                Cpf = cpfNormalizado,
                DataCadastro = _relogio.Agora
            };
            PreencherDados(cliente, nome, email, telefone);

            _repositorio.Adicionar(cliente);
            return cliente;
        }

        /// <summary>Atualiza os dados de contato. O CPF não muda depois do cadastro.</summary>
        public Cliente Atualizar(int id, string nome, string email, string telefone)
        {
            var cliente = _repositorio.ObterPorId(id);
            if (cliente == null)
                throw new RegistroNaoEncontradoException("Cliente não encontrado. Ele pode ter sido removido.");

            PreencherDados(cliente, nome, email, telefone);

            _repositorio.Atualizar(cliente);
            return cliente;
        }

        public Cliente ObterPorCpf(string cpf)
        {
            var cpfNormalizado = ValidadorCpf.Normalizar(cpf);
            if (!ValidadorCpf.EhValido(cpfNormalizado))
                throw new NegocioException("CPF inválido. Confira os números digitados.");

            var cliente = _repositorio.ObterPorCpf(cpfNormalizado);
            if (cliente == null)
                throw new RegistroNaoEncontradoException("Nenhum cliente cadastrado com o CPF " + ValidadorCpf.Formatar(cpfNormalizado) + ".");

            return cliente;
        }

        /// <summary>
        /// Pesquisa por CPF quando o termo tem 11 dígitos e por trecho do nome nos outros casos.
        /// Termo vazio lista todos.
        /// </summary>
        public IList<Cliente> Pesquisar(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
                return _repositorio.Listar();

            var digitos = ValidadorCpf.Normalizar(termo);
            var pareceCpf = digitos.Length == 11 && termo.Trim().All(c => char.IsDigit(c) || c == '.' || c == '-');
            if (pareceCpf)
            {
                var cliente = _repositorio.ObterPorCpf(digitos);
                return cliente == null ? new List<Cliente>() : new List<Cliente> { cliente };
            }

            return _repositorio.BuscarPorNome(termo.Trim());
        }

        private static void PreencherDados(Cliente cliente, string nome, string email, string telefone)
        {
            nome = (nome ?? string.Empty).Trim();
            email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
            telefone = string.IsNullOrWhiteSpace(telefone) ? null : telefone.Trim();

            if (nome.Length < 3)
                throw new NegocioException("Informe o nome completo do cliente.");
            if (nome.Length > 150)
                throw new NegocioException("O nome pode ter no máximo 150 caracteres.");
            if (email != null && (email.Length > 150 || !EmailSimples.IsMatch(email)))
                throw new NegocioException("E-mail inválido.");
            if (telefone != null && telefone.Length > 20)
                throw new NegocioException("O telefone pode ter no máximo 20 caracteres.");

            cliente.Nome = nome;
            cliente.Email = email;
            cliente.Telefone = telefone;
        }
    }
}
