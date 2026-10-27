namespace Cartorio.WinForms
{
    partial class FormPrincipal
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
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnClientes = new System.Windows.Forms.Button();
            this.btnProtocolos = new System.Windows.Forms.Button();
            this.btnRelatorios = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(12, 15);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(316, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Atendimento do Cartório";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // btnClientes
            //
            this.btnClientes.Location = new System.Drawing.Point(40, 60);
            this.btnClientes.Name = "btnClientes";
            this.btnClientes.Size = new System.Drawing.Size(260, 40);
            this.btnClientes.TabIndex = 1;
            this.btnClientes.Text = "&Clientes";
            this.btnClientes.UseVisualStyleBackColor = true;
            this.btnClientes.Click += new System.EventHandler(this.btnClientes_Click);
            //
            // btnProtocolos
            //
            this.btnProtocolos.Location = new System.Drawing.Point(40, 110);
            this.btnProtocolos.Name = "btnProtocolos";
            this.btnProtocolos.Size = new System.Drawing.Size(260, 40);
            this.btnProtocolos.TabIndex = 2;
            this.btnProtocolos.Text = "&Protocolos";
            this.btnProtocolos.UseVisualStyleBackColor = true;
            this.btnProtocolos.Click += new System.EventHandler(this.btnProtocolos_Click);
            //
            // btnRelatorios
            //
            this.btnRelatorios.Location = new System.Drawing.Point(40, 160);
            this.btnRelatorios.Name = "btnRelatorios";
            this.btnRelatorios.Size = new System.Drawing.Size(260, 40);
            this.btnRelatorios.TabIndex = 3;
            this.btnRelatorios.Text = "&Relatórios";
            this.btnRelatorios.UseVisualStyleBackColor = true;
            this.btnRelatorios.Click += new System.EventHandler(this.btnRelatorios_Click);
            //
            // FormPrincipal
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 225);
            this.Controls.Add(this.btnRelatorios);
            this.Controls.Add(this.btnProtocolos);
            this.Controls.Add(this.btnClientes);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Protocolo de Cartório";
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnClientes;
        private System.Windows.Forms.Button btnProtocolos;
        private System.Windows.Forms.Button btnRelatorios;
    }
}
