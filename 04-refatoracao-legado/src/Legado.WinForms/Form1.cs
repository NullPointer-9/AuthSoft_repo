using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Legado.WinForms
{
    public partial class Form1 : Form
    {
        string conexao = ConfigurationManager.ConnectionStrings["LegadoCartorio"].ConnectionString;
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
                using (SqlConnection con = new SqlConnection(conexao))
                using (SqlDataAdapter da = new SqlDataAdapter("select id, nome, cpf, telefone from tb_cliente where nome like @filtro order by nome", con))
                {
                    da.SelectCommand.Parameters.Add("@filtro", SqlDbType.VarChar, 102).Value = "%" + filtro + "%";
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
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
                using (SqlConnection con = new SqlConnection(conexao))
                {
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter("select id, tipo, valor, status, dt_pedido from tb_pedido_certidao where id_cliente = @id_cliente order by id", con))
                    {
                        da.SelectCommand.Parameters.Add("@id_cliente", SqlDbType.Int).Value = idClienteSelecionado;
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView2.DataSource = dt;
                    }

                    using (SqlCommand cmd = new SqlCommand("select sum(valor) from tb_pedido_certidao where id_cliente = @id_cliente and status = 'aberto'", con))
                    {
                        cmd.Parameters.Add("@id_cliente", SqlDbType.Int).Value = idClienteSelecionado;
                        object total = cmd.ExecuteScalar();
                        if (total == DBNull.Value)
                            label5.Text = "Total em aberto: R$ 0,00";
                        else
                            label5.Text = "Total em aberto: R$ " + Convert.ToDouble(total).ToString("N2");
                    }
                }
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
                using (SqlConnection con = new SqlConnection(conexao))
                {
                    con.Open();
                    if (idClienteSelecionado == 0)
                    {
                        using (SqlCommand cmd = new SqlCommand("select count(*) from tb_cliente where cpf = @cpf", con))
                        {
                            cmd.Parameters.Add("@cpf", SqlDbType.VarChar, 20).Value = textBox3.Text;
                            int qtd = (int)cmd.ExecuteScalar();
                            if (qtd > 0)
                            {
                                MessageBox.Show("CPF ja cadastrado");
                                return;
                            }
                        }
                        using (SqlCommand cmd = new SqlCommand("insert into tb_cliente (nome, cpf, telefone) values (@nome, @cpf, @telefone)", con))
                        {
                            AdicionarParametrosCliente(cmd);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Cliente salvo!");
                    }
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand("update tb_cliente set nome = @nome, cpf = @cpf, telefone = @telefone where id = @id", con))
                        {
                            AdicionarParametrosCliente(cmd);
                            cmd.Parameters.Add("@id", SqlDbType.Int).Value = idClienteSelecionado;
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Cliente alterado!");
                    }
                }
                CarregarClientes("");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        void AdicionarParametrosCliente(SqlCommand cmd)
        {
            cmd.Parameters.Add("@nome", SqlDbType.VarChar, 100).Value = textBox2.Text;
            cmd.Parameters.Add("@cpf", SqlDbType.VarChar, 20).Value = textBox3.Text;
            cmd.Parameters.Add("@telefone", SqlDbType.VarChar, 30).Value = textBox4.Text;
        }

        // excluir cliente
        private void button3_Click(object sender, EventArgs e)
        {
            if (idClienteSelecionado == 0) return;
            if (MessageBox.Show("Deseja excluir o cliente?", "Excluir", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(conexao))
                    using (SqlCommand cmd = new SqlCommand("delete from tb_cliente where id = @id", con))
                    {
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = idClienteSelecionado;
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
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
                using (SqlConnection con = new SqlConnection(conexao))
                using (SqlCommand cmd = new SqlCommand("insert into tb_pedido_certidao (id_cliente, tipo, valor, status, dt_pedido) values (@id_cliente, @tipo, @valor, 'aberto', @dt_pedido)", con))
                {
                    cmd.Parameters.Add("@id_cliente", SqlDbType.Int).Value = idClienteSelecionado;
                    cmd.Parameters.Add("@tipo", SqlDbType.VarChar, 50).Value = comboBox1.Text;
                    cmd.Parameters.Add("@valor", SqlDbType.Float).Value = valor;
                    cmd.Parameters.Add("@dt_pedido", SqlDbType.VarChar, 20).Value = DateTime.Now.ToString("dd/MM/yyyy");
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
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

            AtualizarStatus(idPedido, "pago");
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

            AtualizarStatus(idPedido, "entregue");
        }

        void AtualizarStatus(int idPedido, string novoStatus)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conexao))
                using (SqlCommand cmd = new SqlCommand("update tb_pedido_certidao set status = @status where id = @id", con))
                {
                    cmd.Parameters.Add("@status", SqlDbType.VarChar, 30).Value = novoStatus;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = idPedido;
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                CarregarPedidos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }
    }
}
