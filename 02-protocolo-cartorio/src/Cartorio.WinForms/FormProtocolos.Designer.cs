namespace Cartorio.WinForms
{
    partial class FormProtocolos
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
            this.lblBusca = new System.Windows.Forms.Label();
            this.txtBusca = new System.Windows.Forms.TextBox();
            this.btnBuscarNumero = new System.Windows.Forms.Button();
            this.btnBuscarCpf = new System.Windows.Forms.Button();
            this.btnRecentes = new System.Windows.Forms.Button();
            this.dgvProtocolos = new System.Windows.Forms.DataGridView();
            this.lblResultado = new System.Windows.Forms.Label();
            this.grpNovo = new System.Windows.Forms.GroupBox();
            this.lblCpfCliente = new System.Windows.Forms.Label();
            this.mtbCpfCliente = new System.Windows.Forms.MaskedTextBox();
            this.btnLocalizarCliente = new System.Windows.Forms.Button();
            this.lblClienteEncontrado = new System.Windows.Forms.Label();
            this.lblTipoServico = new System.Windows.Forms.Label();
            this.cmbTipoServico = new System.Windows.Forms.ComboBox();
            this.lblDescricao = new System.Windows.Forms.Label();
            this.txtDescricao = new System.Windows.Forms.TextBox();
            this.btnAbrir = new System.Windows.Forms.Button();
            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.lblSelecionado = new System.Windows.Forms.Label();
            this.lblObservacao = new System.Windows.Forms.Label();
            this.txtObservacao = new System.Windows.Forms.TextBox();
            this.btnAvancar = new System.Windows.Forms.Button();
            this.dgvHistorico = new System.Windows.Forms.DataGridView();
            this.btnComprovanteAbertura = new System.Windows.Forms.Button();
            this.btnComprovanteEntrega = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProtocolos)).BeginInit();
            this.grpNovo.SuspendLayout();
            this.grpStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorico)).BeginInit();
            this.SuspendLayout();
            //
            // lblBusca
            //
            this.lblBusca.AutoSize = true;
            this.lblBusca.Location = new System.Drawing.Point(12, 15);
            this.lblBusca.Name = "lblBusca";
            this.lblBusca.Size = new System.Drawing.Size(82, 13);
            this.lblBusca.TabIndex = 0;
            this.lblBusca.Text = "Número ou CPF:";
            //
            // txtBusca
            //
            this.txtBusca.Location = new System.Drawing.Point(100, 12);
            this.txtBusca.Name = "txtBusca";
            this.txtBusca.Size = new System.Drawing.Size(180, 20);
            this.txtBusca.TabIndex = 1;
            //
            // btnBuscarNumero
            //
            this.btnBuscarNumero.Location = new System.Drawing.Point(286, 10);
            this.btnBuscarNumero.Name = "btnBuscarNumero";
            this.btnBuscarNumero.Size = new System.Drawing.Size(110, 24);
            this.btnBuscarNumero.TabIndex = 2;
            this.btnBuscarNumero.Text = "Buscar por &número";
            this.btnBuscarNumero.UseVisualStyleBackColor = true;
            this.btnBuscarNumero.Click += new System.EventHandler(this.btnBuscarNumero_Click);
            //
            // btnBuscarCpf
            //
            this.btnBuscarCpf.Location = new System.Drawing.Point(400, 10);
            this.btnBuscarCpf.Name = "btnBuscarCpf";
            this.btnBuscarCpf.Size = new System.Drawing.Size(110, 24);
            this.btnBuscarCpf.TabIndex = 3;
            this.btnBuscarCpf.Text = "Buscar por &CPF";
            this.btnBuscarCpf.UseVisualStyleBackColor = true;
            this.btnBuscarCpf.Click += new System.EventHandler(this.btnBuscarCpf_Click);
            //
            // btnRecentes
            //
            this.btnRecentes.Location = new System.Drawing.Point(514, 10);
            this.btnRecentes.Name = "btnRecentes";
            this.btnRecentes.Size = new System.Drawing.Size(98, 24);
            this.btnRecentes.TabIndex = 4;
            this.btnRecentes.Text = "&Recentes";
            this.btnRecentes.UseVisualStyleBackColor = true;
            this.btnRecentes.Click += new System.EventHandler(this.btnRecentes_Click);
            //
            // dgvProtocolos
            //
            this.dgvProtocolos.AllowUserToAddRows = false;
            this.dgvProtocolos.AllowUserToDeleteRows = false;
            this.dgvProtocolos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProtocolos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProtocolos.Location = new System.Drawing.Point(12, 42);
            this.dgvProtocolos.MultiSelect = false;
            this.dgvProtocolos.Name = "dgvProtocolos";
            this.dgvProtocolos.ReadOnly = true;
            this.dgvProtocolos.RowHeadersVisible = false;
            this.dgvProtocolos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProtocolos.Size = new System.Drawing.Size(600, 290);
            this.dgvProtocolos.TabIndex = 5;
            this.dgvProtocolos.SelectionChanged += new System.EventHandler(this.dgvProtocolos_SelectionChanged);
            //
            // lblResultado
            //
            this.lblResultado.Location = new System.Drawing.Point(12, 336);
            this.lblResultado.Name = "lblResultado";
            this.lblResultado.Size = new System.Drawing.Size(600, 16);
            this.lblResultado.TabIndex = 6;
            //
            // grpNovo
            //
            this.grpNovo.Controls.Add(this.lblCpfCliente);
            this.grpNovo.Controls.Add(this.mtbCpfCliente);
            this.grpNovo.Controls.Add(this.btnLocalizarCliente);
            this.grpNovo.Controls.Add(this.lblClienteEncontrado);
            this.grpNovo.Controls.Add(this.lblTipoServico);
            this.grpNovo.Controls.Add(this.cmbTipoServico);
            this.grpNovo.Controls.Add(this.lblDescricao);
            this.grpNovo.Controls.Add(this.txtDescricao);
            this.grpNovo.Controls.Add(this.btnAbrir);
            this.grpNovo.Location = new System.Drawing.Point(624, 10);
            this.grpNovo.Name = "grpNovo";
            this.grpNovo.Size = new System.Drawing.Size(344, 342);
            this.grpNovo.TabIndex = 7;
            this.grpNovo.TabStop = false;
            this.grpNovo.Text = "Novo protocolo";
            //
            // lblCpfCliente
            //
            this.lblCpfCliente.AutoSize = true;
            this.lblCpfCliente.Location = new System.Drawing.Point(12, 26);
            this.lblCpfCliente.Name = "lblCpfCliente";
            this.lblCpfCliente.Size = new System.Drawing.Size(83, 13);
            this.lblCpfCliente.TabIndex = 0;
            this.lblCpfCliente.Text = "CPF do cliente:";
            //
            // mtbCpfCliente
            //
            this.mtbCpfCliente.Location = new System.Drawing.Point(110, 23);
            this.mtbCpfCliente.Mask = "000\\.000\\.000-00";
            this.mtbCpfCliente.Name = "mtbCpfCliente";
            this.mtbCpfCliente.Size = new System.Drawing.Size(110, 20);
            this.mtbCpfCliente.TabIndex = 1;
            this.mtbCpfCliente.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.mtbCpfCliente.TextChanged += new System.EventHandler(this.mtbCpfCliente_TextChanged);
            //
            // btnLocalizarCliente
            //
            this.btnLocalizarCliente.Location = new System.Drawing.Point(226, 21);
            this.btnLocalizarCliente.Name = "btnLocalizarCliente";
            this.btnLocalizarCliente.Size = new System.Drawing.Size(105, 24);
            this.btnLocalizarCliente.TabIndex = 2;
            this.btnLocalizarCliente.Text = "&Localizar";
            this.btnLocalizarCliente.UseVisualStyleBackColor = true;
            this.btnLocalizarCliente.Click += new System.EventHandler(this.btnLocalizarCliente_Click);
            //
            // lblClienteEncontrado
            //
            this.lblClienteEncontrado.Location = new System.Drawing.Point(12, 52);
            this.lblClienteEncontrado.Name = "lblClienteEncontrado";
            this.lblClienteEncontrado.Size = new System.Drawing.Size(320, 30);
            this.lblClienteEncontrado.TabIndex = 3;
            this.lblClienteEncontrado.Text = "Nenhum cliente localizado.";
            //
            // lblTipoServico
            //
            this.lblTipoServico.AutoSize = true;
            this.lblTipoServico.Location = new System.Drawing.Point(12, 90);
            this.lblTipoServico.Name = "lblTipoServico";
            this.lblTipoServico.Size = new System.Drawing.Size(46, 13);
            this.lblTipoServico.TabIndex = 4;
            this.lblTipoServico.Text = "Serviço:";
            //
            // cmbTipoServico
            //
            this.cmbTipoServico.FormattingEnabled = true;
            this.cmbTipoServico.Items.AddRange(new object[] {
            "Escritura de compra e venda",
            "Procuração",
            "Reconhecimento de firma",
            "Autenticação de documento",
            "Certidão",
            "Testamento"});
            this.cmbTipoServico.Location = new System.Drawing.Point(110, 87);
            this.cmbTipoServico.MaxLength = 80;
            this.cmbTipoServico.Name = "cmbTipoServico";
            this.cmbTipoServico.Size = new System.Drawing.Size(221, 21);
            this.cmbTipoServico.TabIndex = 5;
            //
            // lblDescricao
            //
            this.lblDescricao.AutoSize = true;
            this.lblDescricao.Location = new System.Drawing.Point(12, 120);
            this.lblDescricao.Name = "lblDescricao";
            this.lblDescricao.Size = new System.Drawing.Size(58, 13);
            this.lblDescricao.TabIndex = 6;
            this.lblDescricao.Text = "Descrição:";
            //
            // txtDescricao
            //
            this.txtDescricao.Location = new System.Drawing.Point(12, 138);
            this.txtDescricao.MaxLength = 500;
            this.txtDescricao.Multiline = true;
            this.txtDescricao.Name = "txtDescricao";
            this.txtDescricao.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescricao.Size = new System.Drawing.Size(319, 150);
            this.txtDescricao.TabIndex = 7;
            //
            // btnAbrir
            //
            this.btnAbrir.Location = new System.Drawing.Point(211, 300);
            this.btnAbrir.Name = "btnAbrir";
            this.btnAbrir.Size = new System.Drawing.Size(120, 30);
            this.btnAbrir.TabIndex = 8;
            this.btnAbrir.Text = "&Abrir protocolo";
            this.btnAbrir.UseVisualStyleBackColor = true;
            this.btnAbrir.Click += new System.EventHandler(this.btnAbrir_Click);
            //
            // grpStatus
            //
            this.grpStatus.Controls.Add(this.lblSelecionado);
            this.grpStatus.Controls.Add(this.lblObservacao);
            this.grpStatus.Controls.Add(this.txtObservacao);
            this.grpStatus.Controls.Add(this.btnAvancar);
            this.grpStatus.Controls.Add(this.dgvHistorico);
            this.grpStatus.Controls.Add(this.btnComprovanteAbertura);
            this.grpStatus.Controls.Add(this.btnComprovanteEntrega);
            this.grpStatus.Location = new System.Drawing.Point(12, 360);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Size = new System.Drawing.Size(956, 228);
            this.grpStatus.TabIndex = 8;
            this.grpStatus.TabStop = false;
            this.grpStatus.Text = "Status e histórico";
            //
            // lblSelecionado
            //
            this.lblSelecionado.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblSelecionado.Location = new System.Drawing.Point(12, 24);
            this.lblSelecionado.Name = "lblSelecionado";
            this.lblSelecionado.Size = new System.Drawing.Size(930, 18);
            this.lblSelecionado.TabIndex = 0;
            this.lblSelecionado.Text = "Selecione um protocolo na lista.";
            //
            // lblObservacao
            //
            this.lblObservacao.AutoSize = true;
            this.lblObservacao.Location = new System.Drawing.Point(12, 52);
            this.lblObservacao.Name = "lblObservacao";
            this.lblObservacao.Size = new System.Drawing.Size(68, 13);
            this.lblObservacao.TabIndex = 1;
            this.lblObservacao.Text = "Observação:";
            //
            // txtObservacao
            //
            this.txtObservacao.Location = new System.Drawing.Point(90, 49);
            this.txtObservacao.MaxLength = 300;
            this.txtObservacao.Name = "txtObservacao";
            this.txtObservacao.Size = new System.Drawing.Size(300, 20);
            this.txtObservacao.TabIndex = 2;
            //
            // btnAvancar
            //
            this.btnAvancar.Enabled = false;
            this.btnAvancar.Location = new System.Drawing.Point(400, 46);
            this.btnAvancar.Name = "btnAvancar";
            this.btnAvancar.Size = new System.Drawing.Size(200, 26);
            this.btnAvancar.TabIndex = 3;
            this.btnAvancar.Text = "Avançar status";
            this.btnAvancar.UseVisualStyleBackColor = true;
            this.btnAvancar.Click += new System.EventHandler(this.btnAvancar_Click);
            //
            // dgvHistorico
            //
            this.dgvHistorico.AllowUserToAddRows = false;
            this.dgvHistorico.AllowUserToDeleteRows = false;
            this.dgvHistorico.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHistorico.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistorico.Location = new System.Drawing.Point(12, 82);
            this.dgvHistorico.Name = "dgvHistorico";
            this.dgvHistorico.ReadOnly = true;
            this.dgvHistorico.RowHeadersVisible = false;
            this.dgvHistorico.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistorico.Size = new System.Drawing.Size(930, 135);
            this.dgvHistorico.TabIndex = 6;
            //
            // btnComprovanteAbertura
            //
            this.btnComprovanteAbertura.Enabled = false;
            this.btnComprovanteAbertura.Location = new System.Drawing.Point(610, 46);
            this.btnComprovanteAbertura.Name = "btnComprovanteAbertura";
            this.btnComprovanteAbertura.Size = new System.Drawing.Size(162, 26);
            this.btnComprovanteAbertura.TabIndex = 4;
            this.btnComprovanteAbertura.Text = "Comprovante de &protocolo";
            this.btnComprovanteAbertura.UseVisualStyleBackColor = true;
            this.btnComprovanteAbertura.Click += new System.EventHandler(this.btnComprovanteAbertura_Click);
            //
            // btnComprovanteEntrega
            //
            this.btnComprovanteEntrega.Enabled = false;
            this.btnComprovanteEntrega.Location = new System.Drawing.Point(778, 46);
            this.btnComprovanteEntrega.Name = "btnComprovanteEntrega";
            this.btnComprovanteEntrega.Size = new System.Drawing.Size(164, 26);
            this.btnComprovanteEntrega.TabIndex = 5;
            this.btnComprovanteEntrega.Text = "Comprovante de &entrega";
            this.btnComprovanteEntrega.UseVisualStyleBackColor = true;
            this.btnComprovanteEntrega.Click += new System.EventHandler(this.btnComprovanteEntrega_Click);
            //
            // FormProtocolos
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 600);
            this.Controls.Add(this.grpStatus);
            this.Controls.Add(this.grpNovo);
            this.Controls.Add(this.lblResultado);
            this.Controls.Add(this.dgvProtocolos);
            this.Controls.Add(this.btnRecentes);
            this.Controls.Add(this.btnBuscarCpf);
            this.Controls.Add(this.btnBuscarNumero);
            this.Controls.Add(this.txtBusca);
            this.Controls.Add(this.lblBusca);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormProtocolos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Protocolos";
            this.Shown += new System.EventHandler(this.FormProtocolos_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProtocolos)).EndInit();
            this.grpNovo.ResumeLayout(false);
            this.grpNovo.PerformLayout();
            this.grpStatus.ResumeLayout(false);
            this.grpStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorico)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblBusca;
        private System.Windows.Forms.TextBox txtBusca;
        private System.Windows.Forms.Button btnBuscarNumero;
        private System.Windows.Forms.Button btnBuscarCpf;
        private System.Windows.Forms.Button btnRecentes;
        private System.Windows.Forms.DataGridView dgvProtocolos;
        private System.Windows.Forms.Label lblResultado;
        private System.Windows.Forms.GroupBox grpNovo;
        private System.Windows.Forms.Label lblCpfCliente;
        private System.Windows.Forms.MaskedTextBox mtbCpfCliente;
        private System.Windows.Forms.Button btnLocalizarCliente;
        private System.Windows.Forms.Label lblClienteEncontrado;
        private System.Windows.Forms.Label lblTipoServico;
        private System.Windows.Forms.ComboBox cmbTipoServico;
        private System.Windows.Forms.Label lblDescricao;
        private System.Windows.Forms.TextBox txtDescricao;
        private System.Windows.Forms.Button btnAbrir;
        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.Label lblSelecionado;
        private System.Windows.Forms.Label lblObservacao;
        private System.Windows.Forms.TextBox txtObservacao;
        private System.Windows.Forms.Button btnAvancar;
        private System.Windows.Forms.DataGridView dgvHistorico;
        private System.Windows.Forms.Button btnComprovanteAbertura;
        private System.Windows.Forms.Button btnComprovanteEntrega;
    }
}
