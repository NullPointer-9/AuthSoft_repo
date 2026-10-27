namespace Legado.WinForms
{
    partial class FormBalcao
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.grpClientes = new System.Windows.Forms.GroupBox();
            this.lblBusca = new System.Windows.Forms.Label();
            this.txtBusca = new System.Windows.Forms.TextBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.dgvClientes = new System.Windows.Forms.DataGridView();
            this.lblNome = new System.Windows.Forms.Label();
            this.txtNome = new System.Windows.Forms.TextBox();
            this.lblCpf = new System.Windows.Forms.Label();
            this.txtCpf = new System.Windows.Forms.TextBox();
            this.lblTelefone = new System.Windows.Forms.Label();
            this.txtTelefone = new System.Windows.Forms.TextBox();
            this.btnNovoCliente = new System.Windows.Forms.Button();
            this.btnSalvarCliente = new System.Windows.Forms.Button();
            this.btnExcluirCliente = new System.Windows.Forms.Button();
            this.grpPedidos = new System.Windows.Forms.GroupBox();
            this.dgvPedidos = new System.Windows.Forms.DataGridView();
            this.cmbTipo = new System.Windows.Forms.ComboBox();
            this.chkUrgente = new System.Windows.Forms.CheckBox();
            this.btnNovoPedido = new System.Windows.Forms.Button();
            this.btnMarcarPago = new System.Windows.Forms.Button();
            this.btnEntregar = new System.Windows.Forms.Button();
            this.lblTotal = new System.Windows.Forms.Label();
            this.grpClientes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).BeginInit();
            this.grpPedidos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPedidos)).BeginInit();
            this.SuspendLayout();
            //
            // grpClientes
            //
            this.grpClientes.Controls.Add(this.lblBusca);
            this.grpClientes.Controls.Add(this.txtBusca);
            this.grpClientes.Controls.Add(this.btnBuscar);
            this.grpClientes.Controls.Add(this.dgvClientes);
            this.grpClientes.Controls.Add(this.lblNome);
            this.grpClientes.Controls.Add(this.txtNome);
            this.grpClientes.Controls.Add(this.lblCpf);
            this.grpClientes.Controls.Add(this.txtCpf);
            this.grpClientes.Controls.Add(this.lblTelefone);
            this.grpClientes.Controls.Add(this.txtTelefone);
            this.grpClientes.Controls.Add(this.btnNovoCliente);
            this.grpClientes.Controls.Add(this.btnSalvarCliente);
            this.grpClientes.Controls.Add(this.btnExcluirCliente);
            this.grpClientes.Location = new System.Drawing.Point(12, 12);
            this.grpClientes.Name = "grpClientes";
            this.grpClientes.Size = new System.Drawing.Size(470, 430);
            this.grpClientes.TabIndex = 0;
            this.grpClientes.TabStop = false;
            this.grpClientes.Text = "Clientes";
            //
            // lblBusca
            //
            this.lblBusca.AutoSize = true;
            this.lblBusca.Location = new System.Drawing.Point(10, 25);
            this.lblBusca.Name = "lblBusca";
            this.lblBusca.Size = new System.Drawing.Size(38, 13);
            this.lblBusca.TabIndex = 0;
            this.lblBusca.Text = "Nome:";
            //
            // txtBusca
            //
            this.txtBusca.Location = new System.Drawing.Point(60, 22);
            this.txtBusca.Name = "txtBusca";
            this.txtBusca.Size = new System.Drawing.Size(300, 20);
            this.txtBusca.TabIndex = 1;
            //
            // btnBuscar
            //
            this.btnBuscar.Location = new System.Drawing.Point(370, 20);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(90, 24);
            this.btnBuscar.TabIndex = 2;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            //
            // dgvClientes
            //
            this.dgvClientes.AllowUserToAddRows = false;
            this.dgvClientes.AllowUserToDeleteRows = false;
            this.dgvClientes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvClientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClientes.Location = new System.Drawing.Point(10, 52);
            this.dgvClientes.MultiSelect = false;
            this.dgvClientes.Name = "dgvClientes";
            this.dgvClientes.ReadOnly = true;
            this.dgvClientes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvClientes.Size = new System.Drawing.Size(450, 250);
            this.dgvClientes.TabIndex = 3;
            this.dgvClientes.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvClientes_CellClick);
            //
            // lblNome
            //
            this.lblNome.AutoSize = true;
            this.lblNome.Location = new System.Drawing.Point(10, 315);
            this.lblNome.Name = "lblNome";
            this.lblNome.Size = new System.Drawing.Size(35, 13);
            this.lblNome.TabIndex = 4;
            this.lblNome.Text = "Nome";
            //
            // txtNome
            //
            this.txtNome.Location = new System.Drawing.Point(80, 312);
            this.txtNome.Name = "txtNome";
            this.txtNome.Size = new System.Drawing.Size(380, 20);
            this.txtNome.TabIndex = 5;
            //
            // lblCpf
            //
            this.lblCpf.AutoSize = true;
            this.lblCpf.Location = new System.Drawing.Point(10, 345);
            this.lblCpf.Name = "lblCpf";
            this.lblCpf.Size = new System.Drawing.Size(27, 13);
            this.lblCpf.TabIndex = 6;
            this.lblCpf.Text = "CPF";
            //
            // txtCpf
            //
            this.txtCpf.Location = new System.Drawing.Point(80, 342);
            this.txtCpf.Name = "txtCpf";
            this.txtCpf.Size = new System.Drawing.Size(150, 20);
            this.txtCpf.TabIndex = 7;
            //
            // lblTelefone
            //
            this.lblTelefone.AutoSize = true;
            this.lblTelefone.Location = new System.Drawing.Point(240, 345);
            this.lblTelefone.Name = "lblTelefone";
            this.lblTelefone.Size = new System.Drawing.Size(49, 13);
            this.lblTelefone.TabIndex = 8;
            this.lblTelefone.Text = "Telefone";
            //
            // txtTelefone
            //
            this.txtTelefone.Location = new System.Drawing.Point(300, 342);
            this.txtTelefone.Name = "txtTelefone";
            this.txtTelefone.Size = new System.Drawing.Size(160, 20);
            this.txtTelefone.TabIndex = 9;
            //
            // btnNovoCliente
            //
            this.btnNovoCliente.Location = new System.Drawing.Point(180, 380);
            this.btnNovoCliente.Name = "btnNovoCliente";
            this.btnNovoCliente.Size = new System.Drawing.Size(90, 28);
            this.btnNovoCliente.TabIndex = 10;
            this.btnNovoCliente.Text = "Novo";
            this.btnNovoCliente.UseVisualStyleBackColor = true;
            this.btnNovoCliente.Click += new System.EventHandler(this.btnNovoCliente_Click);
            //
            // btnSalvarCliente
            //
            this.btnSalvarCliente.Location = new System.Drawing.Point(275, 380);
            this.btnSalvarCliente.Name = "btnSalvarCliente";
            this.btnSalvarCliente.Size = new System.Drawing.Size(90, 28);
            this.btnSalvarCliente.TabIndex = 11;
            this.btnSalvarCliente.Text = "Salvar";
            this.btnSalvarCliente.UseVisualStyleBackColor = true;
            this.btnSalvarCliente.Click += new System.EventHandler(this.btnSalvarCliente_Click);
            //
            // btnExcluirCliente
            //
            this.btnExcluirCliente.Location = new System.Drawing.Point(370, 380);
            this.btnExcluirCliente.Name = "btnExcluirCliente";
            this.btnExcluirCliente.Size = new System.Drawing.Size(90, 28);
            this.btnExcluirCliente.TabIndex = 12;
            this.btnExcluirCliente.Text = "Excluir";
            this.btnExcluirCliente.UseVisualStyleBackColor = true;
            this.btnExcluirCliente.Click += new System.EventHandler(this.btnExcluirCliente_Click);
            //
            // grpPedidos
            //
            this.grpPedidos.Controls.Add(this.dgvPedidos);
            this.grpPedidos.Controls.Add(this.cmbTipo);
            this.grpPedidos.Controls.Add(this.chkUrgente);
            this.grpPedidos.Controls.Add(this.btnNovoPedido);
            this.grpPedidos.Controls.Add(this.btnMarcarPago);
            this.grpPedidos.Controls.Add(this.btnEntregar);
            this.grpPedidos.Controls.Add(this.lblTotal);
            this.grpPedidos.Location = new System.Drawing.Point(494, 12);
            this.grpPedidos.Name = "grpPedidos";
            this.grpPedidos.Size = new System.Drawing.Size(494, 430);
            this.grpPedidos.TabIndex = 1;
            this.grpPedidos.TabStop = false;
            this.grpPedidos.Text = "Pedidos de certidão";
            //
            // dgvPedidos
            //
            this.dgvPedidos.AllowUserToAddRows = false;
            this.dgvPedidos.AllowUserToDeleteRows = false;
            this.dgvPedidos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPedidos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPedidos.Location = new System.Drawing.Point(10, 22);
            this.dgvPedidos.MultiSelect = false;
            this.dgvPedidos.Name = "dgvPedidos";
            this.dgvPedidos.ReadOnly = true;
            this.dgvPedidos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPedidos.Size = new System.Drawing.Size(474, 300);
            this.dgvPedidos.TabIndex = 0;
            //
            // cmbTipo
            //
            this.cmbTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipo.FormattingEnabled = true;
            this.cmbTipo.Location = new System.Drawing.Point(10, 335);
            this.cmbTipo.Name = "cmbTipo";
            this.cmbTipo.Size = new System.Drawing.Size(250, 21);
            this.cmbTipo.TabIndex = 1;
            //
            // chkUrgente
            //
            this.chkUrgente.AutoSize = true;
            this.chkUrgente.Location = new System.Drawing.Point(270, 337);
            this.chkUrgente.Name = "chkUrgente";
            this.chkUrgente.Size = new System.Drawing.Size(103, 17);
            this.chkUrgente.TabIndex = 2;
            this.chkUrgente.Text = "Urgente (+50%)";
            this.chkUrgente.UseVisualStyleBackColor = true;
            //
            // btnNovoPedido
            //
            this.btnNovoPedido.Location = new System.Drawing.Point(384, 333);
            this.btnNovoPedido.Name = "btnNovoPedido";
            this.btnNovoPedido.Size = new System.Drawing.Size(100, 26);
            this.btnNovoPedido.TabIndex = 3;
            this.btnNovoPedido.Text = "Novo pedido";
            this.btnNovoPedido.UseVisualStyleBackColor = true;
            this.btnNovoPedido.Click += new System.EventHandler(this.btnNovoPedido_Click);
            //
            // btnMarcarPago
            //
            this.btnMarcarPago.Location = new System.Drawing.Point(10, 370);
            this.btnMarcarPago.Name = "btnMarcarPago";
            this.btnMarcarPago.Size = new System.Drawing.Size(120, 28);
            this.btnMarcarPago.TabIndex = 4;
            this.btnMarcarPago.Text = "Marcar pago";
            this.btnMarcarPago.UseVisualStyleBackColor = true;
            this.btnMarcarPago.Click += new System.EventHandler(this.btnMarcarPago_Click);
            //
            // btnEntregar
            //
            this.btnEntregar.Location = new System.Drawing.Point(136, 370);
            this.btnEntregar.Name = "btnEntregar";
            this.btnEntregar.Size = new System.Drawing.Size(120, 28);
            this.btnEntregar.TabIndex = 5;
            this.btnEntregar.Text = "Entregar";
            this.btnEntregar.UseVisualStyleBackColor = true;
            this.btnEntregar.Click += new System.EventHandler(this.btnEntregar_Click);
            //
            // lblTotal
            //
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(10, 410);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(185, 16);
            this.lblTotal.TabIndex = 6;
            this.lblTotal.Text = "Total em aberto: R$ 0,00";
            //
            // FormBalcao
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 454);
            this.Controls.Add(this.grpClientes);
            this.Controls.Add(this.grpPedidos);
            this.Name = "FormBalcao";
            this.Text = "Balcão de Certidões";
            this.Load += new System.EventHandler(this.FormBalcao_Load);
            this.grpClientes.ResumeLayout(false);
            this.grpClientes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).EndInit();
            this.grpPedidos.ResumeLayout(false);
            this.grpPedidos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPedidos)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpClientes;
        private System.Windows.Forms.Label lblBusca;
        private System.Windows.Forms.TextBox txtBusca;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.DataGridView dgvClientes;
        private System.Windows.Forms.Label lblNome;
        private System.Windows.Forms.TextBox txtNome;
        private System.Windows.Forms.Label lblCpf;
        private System.Windows.Forms.TextBox txtCpf;
        private System.Windows.Forms.Label lblTelefone;
        private System.Windows.Forms.TextBox txtTelefone;
        private System.Windows.Forms.Button btnNovoCliente;
        private System.Windows.Forms.Button btnSalvarCliente;
        private System.Windows.Forms.Button btnExcluirCliente;
        private System.Windows.Forms.GroupBox grpPedidos;
        private System.Windows.Forms.DataGridView dgvPedidos;
        private System.Windows.Forms.ComboBox cmbTipo;
        private System.Windows.Forms.CheckBox chkUrgente;
        private System.Windows.Forms.Button btnNovoPedido;
        private System.Windows.Forms.Button btnMarcarPago;
        private System.Windows.Forms.Button btnEntregar;
        private System.Windows.Forms.Label lblTotal;
    }
}
