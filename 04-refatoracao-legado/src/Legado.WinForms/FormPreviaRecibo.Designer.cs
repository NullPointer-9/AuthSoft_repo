namespace Legado.WinForms
{
    partial class FormPreviaRecibo
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
            this.reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
            this.pnlBotoes = new System.Windows.Forms.Panel();
            this.lblAviso = new System.Windows.Forms.Label();
            this.btnEmitir = new System.Windows.Forms.Button();
            this.btnFechar = new System.Windows.Forms.Button();
            this.pnlBotoes.SuspendLayout();
            this.SuspendLayout();
            //
            // reportViewer
            //
            this.reportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportViewer.Location = new System.Drawing.Point(0, 0);
            this.reportViewer.Name = "reportViewer";
            this.reportViewer.ShowBackButton = false;
            this.reportViewer.ShowDocumentMapButton = false;
            this.reportViewer.ShowFindControls = false;
            this.reportViewer.ShowParameterPrompts = false;
            this.reportViewer.ShowRefreshButton = false;
            this.reportViewer.ShowStopButton = false;
            this.reportViewer.Size = new System.Drawing.Size(784, 613);
            this.reportViewer.TabIndex = 0;
            //
            // pnlBotoes
            //
            this.pnlBotoes.Controls.Add(this.lblAviso);
            this.pnlBotoes.Controls.Add(this.btnEmitir);
            this.pnlBotoes.Controls.Add(this.btnFechar);
            this.pnlBotoes.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBotoes.Location = new System.Drawing.Point(0, 613);
            this.pnlBotoes.Name = "pnlBotoes";
            this.pnlBotoes.Size = new System.Drawing.Size(784, 48);
            this.pnlBotoes.TabIndex = 1;
            //
            // lblAviso
            //
            this.lblAviso.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAviso.Location = new System.Drawing.Point(12, 10);
            this.lblAviso.Name = "lblAviso";
            this.lblAviso.Size = new System.Drawing.Size(460, 28);
            this.lblAviso.TabIndex = 0;
            this.lblAviso.Text = "Prévia sem assinatura. A assinatura digital é aplicada ao emitir o PDF.";
            this.lblAviso.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnEmitir
            //
            this.btnEmitir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEmitir.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnEmitir.Location = new System.Drawing.Point(506, 10);
            this.btnEmitir.Name = "btnEmitir";
            this.btnEmitir.Size = new System.Drawing.Size(160, 28);
            this.btnEmitir.TabIndex = 1;
            this.btnEmitir.Text = "&Emitir PDF assinado";
            this.btnEmitir.UseVisualStyleBackColor = true;
            //
            // btnFechar
            //
            this.btnFechar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFechar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnFechar.Location = new System.Drawing.Point(672, 10);
            this.btnFechar.Name = "btnFechar";
            this.btnFechar.Size = new System.Drawing.Size(100, 28);
            this.btnFechar.TabIndex = 2;
            this.btnFechar.Text = "&Fechar";
            this.btnFechar.UseVisualStyleBackColor = true;
            //
            // FormPreviaRecibo
            //
            this.AcceptButton = this.btnEmitir;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnFechar;
            this.ClientSize = new System.Drawing.Size(784, 661);
            this.Controls.Add(this.reportViewer);
            this.Controls.Add(this.pnlBotoes);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 500);
            this.Name = "FormPreviaRecibo";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Prévia do recibo";
            this.Load += new System.EventHandler(this.FormPreviaRecibo_Load);
            this.pnlBotoes.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer;
        private System.Windows.Forms.Panel pnlBotoes;
        private System.Windows.Forms.Label lblAviso;
        private System.Windows.Forms.Button btnEmitir;
        private System.Windows.Forms.Button btnFechar;
    }
}
