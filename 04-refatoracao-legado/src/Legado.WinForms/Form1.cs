using System;
using System.Configuration;
using System.Windows.Forms;
using Legado.Dados;

namespace Legado.WinForms
{
    public partial class Form1 : Form
    {
        readonly ClienteRepositorio clientes;
        readonly PedidoRepositorio pedidos;
        int idClienteSelecionado = 0;

        public Form1()
        {
            InitializeComponent();

            var conexao = ConfigurationManager.ConnectionStrings["LegadoCartorio"].ConnectionString;
            clientes = new ClienteRepositorio(conexao);
            pedidos = new PedidoRepositorio(conexao);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Certidao de nascimento");
            comboBox1.Items.Add("Certidao de casamento");
            comboBox1.Items.Add("Certidao de obito");
            comboBox1.SelectedIndex = 0;
            CarregarClientes("");
        }

        void CarregarClientes(string filtro)
        {
            try
            {
                dataGridView1.DataSource = clientes.BuscarPorNome(filtro);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        void CarregarPedidos()
        {
            try
            {
                dataGridView2.DataSource = pedidos.ListarPorCliente(idClienteSelecionado);
                label5.Text = "Total em aberto: R$ " + pedidos.TotalEmAberto(idClienteSelecionado).ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        PedidoCertidao PedidoSelecionado()
        {
            return dataGridView2.CurrentRow?.DataBoundItem as PedidoCertidao;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CarregarClientes(textBox1.Text);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var cliente = (Cliente)dataGridView1.Rows[e.RowIndex].DataBoundItem;
            idClienteSelecionado = cliente.Id;
            textBox2.Text = cliente.Nome;
            textBox3.Text = cliente.Cpf;
            textBox4.Text = cliente.Telefone;
            CarregarPedidos();
        }

        // novo cliente
        private void button7_Click(object sender, EventArgs e)
        {
            idClienteSelecionado = 0;
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            dataGridView2.DataSource = null;
            label5.Text = "Total em aberto: R$ 0,00";
        }

        // salvar cliente
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                MessageBox.Show("Preencha o nome");
                return;
            }
            if (textBox3.Text.Length < 11)
            {
                MessageBox.Show("CPF invalido");
                return;
            }

            try
            {
                var cliente = new Cliente { Id = idClienteSelecionado, Nome = textBox2.Text, Cpf = textBox3.Text, Telefone = textBox4.Text };
                if (idClienteSelecionado == 0)
                {
                    if (clientes.ContarPorCpf(textBox3.Text) > 0)
                    {
                        MessageBox.Show("CPF ja cadastrado");
                        return;
                    }
                    clientes.Inserir(cliente);
                    MessageBox.Show("Cliente salvo!");
                }
                else
                {
                    clientes.Atualizar(cliente);
                    MessageBox.Show("Cliente alterado!");
                }
                CarregarClientes("");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        // excluir cliente
        private void button3_Click(object sender, EventArgs e)
        {
            if (idClienteSelecionado == 0) return;
            if (MessageBox.Show("Deseja excluir o cliente?", "Excluir", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    clientes.Excluir(idClienteSelecionado);
                    button7_Click(null, null);
                    CarregarClientes("");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }

        // novo pedido
        private void button4_Click(object sender, EventArgs e)
        {
            if (idClienteSelecionado == 0)
            {
                MessageBox.Show("Selecione um cliente");
                return;
            }

            double valor = 0;
            if (comboBox1.Text == "Certidao de nascimento") valor = 45.9;
            else if (comboBox1.Text == "Certidao de casamento") valor = 45.9;
            else if (comboBox1.Text == "Certidao de obito") valor = 52.35;
            if (checkBox1.Checked) valor = valor * 1.5; // urgente

            try
            {
                pedidos.Inserir(new PedidoCertidao
                {
                    IdCliente = idClienteSelecionado,
                    Tipo = comboBox1.Text,
                    Valor = valor,
                    Status = "aberto",
                    DataPedido = DateTime.Now.ToString("dd/MM/yyyy")
                });
                CarregarPedidos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        // marcar pago
        private void button5_Click(object sender, EventArgs e)
        {
            var pedido = PedidoSelecionado();
            if (pedido == null) return;
            if (pedido.Status != "aberto")
            {
                MessageBox.Show("Pedido nao esta em aberto");
                return;
            }

            AtualizarStatus(pedido.Id, "pago");
        }

        // entregar
        private void button6_Click(object sender, EventArgs e)
        {
            var pedido = PedidoSelecionado();
            if (pedido == null) return;
            if (pedido.Status == "entregue")
            {
                MessageBox.Show("Pedido ja foi entregue");
                return;
            }

            AtualizarStatus(pedido.Id, "entregue");
        }

        void AtualizarStatus(int idPedido, string novoStatus)
        {
            try
            {
                pedidos.AtualizarStatus(idPedido, novoStatus);
                CarregarPedidos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }
    }
}
