namespace Cartorio.WinForms
{
    partial class FormRelatorioProtocolos
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
            this.pnlFiltros = new System.Windows.Forms.Panel();
            this.lblInicio = new System.Windows.Forms.Label();
            this.dtpInicio = new System.Windows.Forms.DateTimePicker();
            this.lblFim = new System.Windows.Forms.Label();
            this.dtpFim = new System.Windows.Forms.DateTimePicker();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.btnGerar = new System.Windows.Forms.Button();
            this.reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
            this.pnlFiltros.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlFiltros
            //
            this.pnlFiltros.Controls.Add(this.lblInicio);
            this.pnlFiltros.Controls.Add(this.dtpInicio);
            this.pnlFiltros.Controls.Add(this.lblFim);
            this.pnlFiltros.Controls.Add(this.dtpFim);
            this.pnlFiltros.Controls.Add(this.lblStatus);
            this.pnlFiltros.Controls.Add(this.cmbStatus);
            this.pnlFiltros.Controls.Add(this.btnGerar);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Location = new System.Drawing.Point(0, 0);
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(884, 44);
            this.pnlFiltros.TabIndex = 0;
            //
            // lblInicio
            //
            this.lblInicio.AutoSize = true;
            this.lblInicio.Location = new System.Drawing.Point(12, 15);
            this.lblInicio.Name = "lblInicio";
            this.lblInicio.Size = new System.Drawing.Size(66, 13);
            this.lblInicio.TabIndex = 0;
            this.lblInicio.Text = "Abertos de:";
            //
            // dtpInicio
            //
            this.dtpInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpInicio.Location = new System.Drawing.Point(80, 12);
            this.dtpInicio.Name = "dtpInicio";
            this.dtpInicio.Size = new System.Drawing.Size(100, 20);
            this.dtpInicio.TabIndex = 1;
            //
            // lblFim
            //
            this.lblFim.AutoSize = true;
            this.lblFim.Location = new System.Drawing.Point(190, 15);
            this.lblFim.Name = "lblFim";
            this.lblFim.Size = new System.Drawing.Size(16, 13);
            this.lblFim.TabIndex = 2;
            this.lblFim.Text = "a:";
            //
            // dtpFim
            //
            this.dtpFim.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFim.Location = new System.Drawing.Point(210, 12);
            this.dtpFim.Name = "dtpFim";
            this.dtpFim.Size = new System.Drawing.Size(100, 20);
            this.dtpFim.TabIndex = 3;
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(330, 15);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(40, 13);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Status:";
            //
            // cmbStatus
            //
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Location = new System.Drawing.Point(375, 12);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(130, 21);
            this.cmbStatus.TabIndex = 5;
            //
            // btnGerar
            //
            this.btnGerar.Location = new System.Drawing.Point(520, 10);
            this.btnGerar.Name = "btnGerar";
            this.btnGerar.Size = new System.Drawing.Size(120, 25);
            this.btnGerar.TabIndex = 6;
            this.btnGerar.Text = "&Gerar relatório";
            this.btnGerar.UseVisualStyleBackColor = true;
            this.btnGerar.Click += new System.EventHandler(this.btnGerar_Click);
            //
            // reportViewer
            //
            this.reportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportViewer.Location = new System.Drawing.Point(0, 44);
            this.reportViewer.Name = "reportViewer";
            this.reportViewer.ShowBackButton = false;
            this.reportViewer.ShowDocumentMapButton = false;
            this.reportViewer.ShowParameterPrompts = false;
            this.reportViewer.ShowRefreshButton = false;
            this.reportViewer.ShowStopButton = false;
            this.reportViewer.Size = new System.Drawing.Size(884, 617);
            this.reportViewer.TabIndex = 1;
            //
            // FormRelatorioProtocolos
            //
            this.AcceptButton = this.btnGerar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 661);
            this.Controls.Add(this.reportViewer);
            this.Controls.Add(this.pnlFiltros);
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "FormRelatorioProtocolos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Relatório de protocolos";
            this.Shown += new System.EventHandler(this.FormRelatorioProtocolos_Shown);
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.Label lblInicio;
        private System.Windows.Forms.DateTimePicker dtpInicio;
        private System.Windows.Forms.Label lblFim;
        private System.Windows.Forms.DateTimePicker dtpFim;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Button btnGerar;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer;
    }
}
