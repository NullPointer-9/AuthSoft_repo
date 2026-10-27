using System;
using System.Configuration;
using System.Windows.Forms;
using Legado.Dados;
using Legado.Negocio;

namespace Legado.WinForms
{
    public partial class Form1 : Form
    {
        readonly ClienteServico clientes;
        readonly PedidoServico pedidos;
        int idClienteSelecionado = 0;

        public Form1()
        {
            InitializeComponent();

            var conexao = ConfigurationManager.ConnectionStrings["LegadoCartorio"].ConnectionString;
            var clienteRepositorio = new ClienteRepositorio(conexao);
            var pedidoRepositorio = new PedidoRepositorio(conexao);
            clientes = new ClienteServico(clienteRepositorio, pedidoRepositorio);
            pedidos = new PedidoServico(pedidoRepositorio, new RelogioSistema());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // A ordem dos itens segue o enum TipoCertidao.
            foreach (TipoCertidao tipo in Enum.GetValues(typeof(TipoCertidao)))
                comboBox1.Items.Add(FormatosLegados.DescricaoTipo(tipo));
            comboBox1.SelectedIndex = 0;
            CarregarClientes("");
        }

        /// <summary>Regra violada vira aviso; qualquer outra coisa vira erro.</summary>
        void Executar(Action acao)
        {
            try
            {
                acao();
            }
            catch (NegocioException ex)
            {
                MessageBox.Show(ex.Message, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void CarregarClientes(string filtro)
        {
            Executar(() => dataGridView1.DataSource = clientes.Buscar(filtro));
        }

        void CarregarPedidos()
        {
            Executar(() =>
            {
                dataGridView2.DataSource = pedidos.ListarPorCliente(idClienteSelecionado);
                label5.Text = "Total em aberto: " + pedidos.TotalEmAberto(idClienteSelecionado).ToString("C");
            });
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
            label5.Text = "Total em aberto: " + 0m.ToString("C");
        }

        // salvar cliente
        private void button2_Click(object sender, EventArgs e)
        {
            Executar(() =>
            {
                var novo = idClienteSelecionado == 0;
                clientes.Salvar(idClienteSelecionado, textBox2.Text, textBox3.Text, textBox4.Text);
                MessageBox.Show(novo ? "Cliente salvo!" : "Cliente alterado!");
                CarregarClientes("");
            });
        }

        // excluir cliente
        private void button3_Click(object sender, EventArgs e)
        {
            if (idClienteSelecionado == 0) return;
            if (MessageBox.Show("Deseja excluir o cliente?", "Excluir", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            Executar(() =>
            {
                clientes.Excluir(idClienteSelecionado);
                button7_Click(null, null);
                CarregarClientes("");
            });
        }

        // novo pedido
        private void button4_Click(object sender, EventArgs e)
        {
            Executar(() =>
            {
                pedidos.Criar(idClienteSelecionado, (TipoCertidao)comboBox1.SelectedIndex, checkBox1.Checked);
                CarregarPedidos();
            });
        }

        // marcar pago
        private void button5_Click(object sender, EventArgs e)
        {
            var pedido = PedidoSelecionado();
            if (pedido == null) return;
            Executar(() =>
            {
                pedidos.MarcarPago(pedido.Id);
                CarregarPedidos();
            });
        }

        // entregar
        private void button6_Click(object sender, EventArgs e)
        {
            var pedido = PedidoSelecionado();
            if (pedido == null) return;
            Executar(() =>
            {
                pedidos.Entregar(pedido.Id);
                CarregarPedidos();
            });
        }
    }
}
