using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Cartorio.Entidades;
using Cartorio.Negocio;

namespace Cartorio.WinForms
{
    public partial class FormClientes : Form
    {
        private readonly ClienteServico _servico;
        private IList<Cliente> _clientes = new List<Cliente>();
        private int? _clienteEmEdicaoId;
        private bool _carregandoGrade;

        // Usado apenas pelo designer do Visual Studio.
        public FormClientes()
        {
            InitializeComponent();
        }

        public FormClientes(ClienteServico servico) : this()
        {
            _servico = servico;
        }

        // Shown e não Load: o grid só termina de montar as linhas quando a janela aparece.
        private void FormClientes_Shown(object sender, EventArgs e)
        {
            PrepararNovoCadastro();
            Pesquisar();
        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            dgvClientes.ClearSelection();
            PrepararNovoCadastro();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            Cliente salvo = null;
            var sucesso = TratamentoDeErros.Executar(this, () =>
            {
                salvo = _clienteEmEdicaoId.HasValue
                    ? _servico.Atualizar(_clienteEmEdicaoId.Value, txtNome.Text, txtEmail.Text, txtTelefone.Text)
                    : _servico.Cadastrar(txtNome.Text, mtbCpf.Text, txtEmail.Text, txtTelefone.Text);
            });

            if (!sucesso)
                return;

            txtPesquisa.Text = salvo.Cpf;
            Pesquisar();
            PrepararNovoCadastro();
            lblStatus.Text = "Cliente " + salvo.Nome + " salvo.";
        }

        private void dgvClientes_SelectionChanged(object sender, EventArgs e)
        {
            if (_carregandoGrade)
                return;

            var linha = dgvClientes.CurrentRow?.DataBoundItem as ClienteLinha;
            if (linha == null || !dgvClientes.CurrentRow.Selected)
                return;

            var cliente = _clientes.FirstOrDefault(c => c.Id == linha.Id);
            if (cliente != null)
                CarregarParaEdicao(cliente);
        }

        private void Pesquisar()
        {
            TratamentoDeErros.Executar(this, () =>
            {
                _clientes = _servico.Pesquisar(txtPesquisa.Text);

                // Ao trocar o DataSource o grid seleciona a primeira linha sozinho;
                // isso não deve abrir um cliente para edição sem o usuário clicar.
                _carregandoGrade = true;
                try
                {
                    dgvClientes.DataSource = _clientes.Select(c => new ClienteLinha(c)).ToList();
                    dgvClientes.ClearSelection();
                }
                finally
                {
                    _carregandoGrade = false;
                }

                lblStatus.Text = _clientes.Count == 0
                    ? "Nenhum cliente encontrado."
                    : _clientes.Count + " cliente(s) encontrado(s).";
            });
        }

        private void PrepararNovoCadastro()
        {
            _clienteEmEdicaoId = null;
            grpDados.Text = "Novo cliente";
            txtNome.Clear();
            mtbCpf.Clear();
            mtbCpf.Enabled = true;
            txtEmail.Clear();
            txtTelefone.Clear();
            txtNome.Focus();
        }

        private void CarregarParaEdicao(Cliente cliente)
        {
            _clienteEmEdicaoId = cliente.Id;
            grpDados.Text = "Editando cliente";
            txtNome.Text = cliente.Nome;
            mtbCpf.Text = cliente.Cpf;
            mtbCpf.Enabled = false; // CPF não muda depois do cadastro
            txtEmail.Text = cliente.Email;
            txtTelefone.Text = cliente.Telefone;
        }
    }
}
