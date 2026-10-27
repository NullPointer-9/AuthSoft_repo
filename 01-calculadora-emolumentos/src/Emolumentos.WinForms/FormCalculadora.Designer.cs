namespace Emolumentos.WinForms
{
    partial class FormCalculadora
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
            this.lblUf = new System.Windows.Forms.Label();
            this.cmbUf = new System.Windows.Forms.ComboBox();
            this.lblTipoAto = new System.Windows.Forms.Label();
            this.cmbTipoAto = new System.Windows.Forms.ComboBox();
            this.lblValor = new System.Windows.Forms.Label();
            this.txtValor = new System.Windows.Forms.TextBox();
            this.btnCalcular = new System.Windows.Forms.Button();
            this.grpResultado = new System.Windows.Forms.GroupBox();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lvResultado = new System.Windows.Forms.ListView();
            this.colItem = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colValor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnEmitir = new System.Windows.Forms.Button();
            this.btnPrevia = new System.Windows.Forms.Button();
            this.grpResultado.SuspendLayout();
            this.SuspendLayout();
            //
            // lblUf
            //
            this.lblUf.AutoSize = true;
            this.lblUf.Location = new System.Drawing.Point(16, 18);
            this.lblUf.Name = "lblUf";
            this.lblUf.Size = new System.Drawing.Size(24, 13);
            this.lblUf.TabIndex = 0;
            this.lblUf.Text = "UF:";
            //
            // cmbUf
            //
            this.cmbUf.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUf.FormattingEnabled = true;
            this.cmbUf.Location = new System.Drawing.Point(130, 15);
            this.cmbUf.Name = "cmbUf";
            this.cmbUf.Size = new System.Drawing.Size(260, 21);
            this.cmbUf.TabIndex = 1;
            //
            // lblTipoAto
            //
            this.lblTipoAto.AutoSize = true;
            this.lblTipoAto.Location = new System.Drawing.Point(16, 51);
            this.lblTipoAto.Name = "lblTipoAto";
            this.lblTipoAto.Size = new System.Drawing.Size(70, 13);
            this.lblTipoAto.TabIndex = 2;
            this.lblTipoAto.Text = "Tipo de ato:";
            //
            // cmbTipoAto
            //
            this.cmbTipoAto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoAto.FormattingEnabled = true;
            this.cmbTipoAto.Location = new System.Drawing.Point(130, 48);
            this.cmbTipoAto.Name = "cmbTipoAto";
            this.cmbTipoAto.Size = new System.Drawing.Size(260, 21);
            this.cmbTipoAto.TabIndex = 3;
            this.cmbTipoAto.SelectedIndexChanged += new System.EventHandler(this.cmbTipoAto_SelectedIndexChanged);
            //
            // lblValor
            //
            this.lblValor.AutoSize = true;
            this.lblValor.Location = new System.Drawing.Point(16, 84);
            this.lblValor.Name = "lblValor";
            this.lblValor.Size = new System.Drawing.Size(98, 13);
            this.lblValor.TabIndex = 4;
            this.lblValor.Text = "Valor do negócio:";
            //
            // txtValor
            //
            this.txtValor.Location = new System.Drawing.Point(130, 81);
            this.txtValor.Name = "txtValor";
            this.txtValor.Size = new System.Drawing.Size(160, 20);
            this.txtValor.TabIndex = 5;
            this.txtValor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //
            // btnCalcular
            //
            this.btnCalcular.Location = new System.Drawing.Point(300, 79);
            this.btnCalcular.Name = "btnCalcular";
            this.btnCalcular.Size = new System.Drawing.Size(90, 24);
            this.btnCalcular.TabIndex = 6;
            this.btnCalcular.Text = "&Calcular";
            this.btnCalcular.UseVisualStyleBackColor = true;
            this.btnCalcular.Click += new System.EventHandler(this.btnCalcular_Click);
            //
            // grpResultado
            //
            this.grpResultado.Controls.Add(this.lblTotal);
            this.grpResultado.Controls.Add(this.lvResultado);
            this.grpResultado.Location = new System.Drawing.Point(16, 118);
            this.grpResultado.Name = "grpResultado";
            this.grpResultado.Size = new System.Drawing.Size(374, 222);
            this.grpResultado.TabIndex = 7;
            this.grpResultado.TabStop = false;
            this.grpResultado.Text = "Resultado";
            //
            // lvResultado
            //
            this.lvResultado.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colItem,
            this.colValor});
            this.lvResultado.FullRowSelect = true;
            this.lvResultado.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvResultado.HideSelection = false;
            this.lvResultado.Location = new System.Drawing.Point(10, 20);
            this.lvResultado.MultiSelect = false;
            this.lvResultado.Name = "lvResultado";
            this.lvResultado.Size = new System.Drawing.Size(354, 160);
            this.lvResultado.TabIndex = 0;
            this.lvResultado.UseCompatibleStateImageBehavior = false;
            this.lvResultado.View = System.Windows.Forms.View.Details;
            //
            // colItem
            //
            this.colItem.Text = "Item";
            this.colItem.Width = 120;
            //
            // colValor
            //
            this.colValor.Text = "Valor";
            this.colValor.Width = 228;
            //
            // lblTotal
            //
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(10, 188);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(354, 24);
            this.lblTotal.TabIndex = 1;
            this.lblTotal.Text = "Total: -";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // btnEmitir
            //
            this.btnEmitir.Enabled = false;
            this.btnEmitir.Location = new System.Drawing.Point(230, 348);
            this.btnEmitir.Name = "btnEmitir";
            this.btnEmitir.Size = new System.Drawing.Size(160, 28);
            this.btnEmitir.TabIndex = 9;
            this.btnEmitir.Text = "&Emitir PDF assinado";
            this.btnEmitir.UseVisualStyleBackColor = true;
            this.btnEmitir.Click += new System.EventHandler(this.btnEmitir_Click);
            //
            // btnPrevia
            //
            this.btnPrevia.Enabled = false;
            this.btnPrevia.Location = new System.Drawing.Point(104, 348);
            this.btnPrevia.Name = "btnPrevia";
            this.btnPrevia.Size = new System.Drawing.Size(120, 28);
            this.btnPrevia.TabIndex = 8;
            this.btnPrevia.Text = "&Visualizar prévia";
            this.btnPrevia.UseVisualStyleBackColor = true;
            this.btnPrevia.Click += new System.EventHandler(this.btnPrevia_Click);
            //
            // FormCalculadora
            //
            this.AcceptButton = this.btnCalcular;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 390);
            this.Controls.Add(this.btnPrevia);
            this.Controls.Add(this.btnEmitir);
            this.Controls.Add(this.grpResultado);
            this.Controls.Add(this.btnCalcular);
            this.Controls.Add(this.txtValor);
            this.Controls.Add(this.lblValor);
            this.Controls.Add(this.cmbTipoAto);
            this.Controls.Add(this.lblTipoAto);
            this.Controls.Add(this.cmbUf);
            this.Controls.Add(this.lblUf);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormCalculadora";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Calculadora de Emolumentos";
            this.Load += new System.EventHandler(this.FormCalculadora_Load);
            this.grpResultado.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblUf;
        private System.Windows.Forms.ComboBox cmbUf;
        private System.Windows.Forms.Label lblTipoAto;
        private System.Windows.Forms.ComboBox cmbTipoAto;
        private System.Windows.Forms.Label lblValor;
        private System.Windows.Forms.TextBox txtValor;
        private System.Windows.Forms.Button btnCalcular;
        private System.Windows.Forms.GroupBox grpResultado;
        private System.Windows.Forms.ListView lvResultado;
        private System.Windows.Forms.ColumnHeader colItem;
        private System.Windows.Forms.ColumnHeader colValor;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnEmitir;
        private System.Windows.Forms.Button btnPrevia;
    }
}
