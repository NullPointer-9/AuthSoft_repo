using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Legado.WinForms
{
    public partial class Form1 : Form
    {
        // TODO: mover para o config
        string conexao = "Data Source=localhost,14334;Initial Catalog=LegadoCartorio;User ID=sa;Password=Cartorio@2026";
        int idClienteSelecionado = 0;

        public Form1()
        {
            InitializeComponent();
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
                SqlConnection con = new SqlConnection(conexao);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("select id, nome, cpf, telefone from tb_cliente where nome like '%" + filtro + "%' order by nome", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                con.Close();
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
                SqlConnection con = new SqlConnection(conexao);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("select id, tipo, valor, status, dt_pedido from tb_pedido_certidao where id_cliente = " + idClienteSelecionado + " order by id", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView2.DataSource = dt;

                SqlCommand cmd = new SqlCommand("select sum(valor) from tb_pedido_certidao where id_cliente = " + idClienteSelecionado + " and status = 'aberto'", con);
                object total = cmd.ExecuteScalar();
                if (total == DBNull.Value)
                    label5.Text = "Total em aberto: R$ 0,00";
                else
                    label5.Text = "Total em aberto: R$ " + Convert.ToDouble(total).ToString("N2");
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CarregarClientes(textBox1.Text);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            idClienteSelecionado = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
            textBox2.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            textBox3.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            textBox4.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
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
                SqlConnection con = new SqlConnection(conexao);
                con.Open();
                if (idClienteSelecionado == 0)
                {
                    SqlCommand cmd = new SqlCommand("select count(*) from tb_cliente where cpf = '" + textBox3.Text + "'", con);
                    int qtd = (int)cmd.ExecuteScalar();
                    if (qtd > 0)
                    {
                        MessageBox.Show("CPF ja cadastrado");
                        con.Close();
                        return;
                    }
                    cmd = new SqlCommand("insert into tb_cliente (nome, cpf, telefone) values ('" + textBox2.Text + "', '" + textBox3.Text + "', '" + textBox4.Text + "')", con);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cliente salvo!");
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("update tb_cliente set nome = '" + textBox2.Text + "', cpf = '" + textBox3.Text + "', telefone = '" + textBox4.Text + "' where id = " + idClienteSelecionado, con);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cliente alterado!");
                }
                con.Close();
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
                    SqlConnection con = new SqlConnection(conexao);
                    con.Open();
                    SqlCommand cmd = new SqlCommand("delete from tb_cliente where id = " + idClienteSelecionado, con);
                    cmd.ExecuteNonQuery();
                    con.Close();
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
                SqlConnection con = new SqlConnection(conexao);
                con.Open();
                SqlCommand cmd = new SqlCommand("insert into tb_pedido_certidao (id_cliente, tipo, valor, status, dt_pedido) values (" + idClienteSelecionado + ", '" + comboBox1.Text + "', " + valor.ToString().Replace(",", ".") + ", 'aberto', '" + DateTime.Now.ToString("dd/MM/yyyy") + "')", con);
                cmd.ExecuteNonQuery();
                con.Close();
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
            if (dataGridView2.CurrentRow == null) return;
            int idPedido = Convert.ToInt32(dataGridView2.CurrentRow.Cells[0].Value);
            string status = dataGridView2.CurrentRow.Cells[3].Value.ToString();
            if (status != "aberto")
            {
                MessageBox.Show("Pedido nao esta em aberto");
                return;
            }

            try
            {
                SqlConnection con = new SqlConnection(conexao);
                con.Open();
                SqlCommand cmd = new SqlCommand("update tb_pedido_certidao set status = 'pago' where id = " + idPedido, con);
                cmd.ExecuteNonQuery();
                con.Close();
                CarregarPedidos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        // entregar
        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow == null) return;
            int idPedido = Convert.ToInt32(dataGridView2.CurrentRow.Cells[0].Value);
            string status = dataGridView2.CurrentRow.Cells[3].Value.ToString();
            if (status == "entregue")
            {
                MessageBox.Show("Pedido ja foi entregue");
                return;
            }

            try
            {
                SqlConnection con = new SqlConnection(conexao);
                con.Open();
                SqlCommand cmd = new SqlCommand("update tb_pedido_certidao set status = 'entregue' where id = " + idPedido, con);
                cmd.ExecuteNonQuery();
                con.Close();
                CarregarPedidos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }
    }
}
