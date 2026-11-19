namespace HotDogBrasilia.UI
{
    partial class FrmRelatorioInsumos
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlTopo = new System.Windows.Forms.Panel();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.pnlFiltros = new System.Windows.Forms.Panel();
            this.btnLimpar = new System.Windows.Forms.Button();
            this.btnGerar = new System.Windows.Forms.Button();
            this.lblUnidades = new System.Windows.Forms.Label();
            this.txtLimite = new System.Windows.Forms.TextBox();
            this.lblLimite = new System.Windows.Forms.Label();
            this.lblDicaFiltro = new System.Windows.Forms.Label();
            this.tblResumo = new System.Windows.Forms.TableLayoutPanel();
            this.cardInsumos = new HotDogBrasilia.UI.CartaoResumo();
            this.cardBaixo = new HotDogBrasilia.UI.CartaoResumo();
            this.cardEsgotado = new HotDogBrasilia.UI.CartaoResumo();
            this.pnlGrade = new System.Windows.Forms.Panel();
            this.dgvInsumos = new System.Windows.Forms.DataGridView();
            this.lblVazio = new System.Windows.Forms.Label();
            this.pnlRodape = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblNota = new System.Windows.Forms.Label();
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnImprimir = new System.Windows.Forms.Button();
            this.btnFechar = new System.Windows.Forms.Button();
            this.dica = new System.Windows.Forms.ToolTip(this.components);
            this.pnlTopo.SuspendLayout();
            this.pnlFiltros.SuspendLayout();
            this.tblResumo.SuspendLayout();
            this.pnlGrade.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInsumos)).BeginInit();
            this.pnlRodape.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlTopo
            //
            this.pnlTopo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(48)))), ((int)(((byte)(43)))));
            this.pnlTopo.Controls.Add(this.lblSubtitulo);
            this.pnlTopo.Controls.Add(this.lblTitulo);
            this.pnlTopo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopo.Location = new System.Drawing.Point(0, 0);
            this.pnlTopo.Name = "pnlTopo";
            this.pnlTopo.Size = new System.Drawing.Size(880, 72);
            this.pnlTopo.TabIndex = 0;
            //
            // lblSubtitulo
            //
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(178)))));
            this.lblSubtitulo.Location = new System.Drawing.Point(22, 44);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(330, 17);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Quantidade disponível de cada insumo (pão, salsicha, molho...), com situação";
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(18, 10);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(225, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Relatório de Insumos";
            //
            // pnlFiltros
            //
            this.pnlFiltros.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(246)))), ((int)(((byte)(244)))));
            this.pnlFiltros.Controls.Add(this.lblDicaFiltro);
            this.pnlFiltros.Controls.Add(this.btnLimpar);
            this.pnlFiltros.Controls.Add(this.btnGerar);
            this.pnlFiltros.Controls.Add(this.lblUnidades);
            this.pnlFiltros.Controls.Add(this.txtLimite);
            this.pnlFiltros.Controls.Add(this.lblLimite);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Location = new System.Drawing.Point(0, 72);
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(880, 96);
            this.pnlFiltros.TabIndex = 1;
            //
            // lblLimite
            //
            this.lblLimite.AutoSize = true;
            this.lblLimite.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblLimite.Location = new System.Drawing.Point(20, 12);
            this.lblLimite.Name = "lblLimite";
            this.lblLimite.Size = new System.Drawing.Size(190, 19);
            this.lblLimite.TabIndex = 0;
            this.lblLimite.Text = "Considerar estoque &baixo até";
            //
            // txtLimite
            //
            this.txtLimite.AccessibleName = "Considerar estoque baixo até";
            this.txtLimite.Location = new System.Drawing.Point(20, 35);
            this.txtLimite.MaxLength = 6;
            this.txtLimite.Name = "txtLimite";
            this.txtLimite.Size = new System.Drawing.Size(150, 25);
            this.txtLimite.TabIndex = 1;
            this.txtLimite.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtLimite_KeyPress);
            //
            // lblUnidades
            //
            this.lblUnidades.AutoSize = true;
            this.lblUnidades.ForeColor = System.Drawing.Color.DimGray;
            this.lblUnidades.Location = new System.Drawing.Point(176, 38);
            this.lblUnidades.Name = "lblUnidades";
            this.lblUnidades.Size = new System.Drawing.Size(64, 19);
            this.lblUnidades.TabIndex = 2;
            this.lblUnidades.Text = "unidades";
            //
            // btnGerar
            //
            this.btnGerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(48)))), ((int)(((byte)(43)))));
            this.btnGerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGerar.FlatAppearance.BorderSize = 0;
            this.btnGerar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(30)))), ((int)(((byte)(28)))));
            this.btnGerar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(38)))), ((int)(((byte)(34)))));
            this.btnGerar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGerar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnGerar.ForeColor = System.Drawing.Color.White;
            this.btnGerar.Location = new System.Drawing.Point(266, 31);
            this.btnGerar.Name = "btnGerar";
            this.btnGerar.Size = new System.Drawing.Size(120, 32);
            this.btnGerar.TabIndex = 3;
            this.btnGerar.Text = "&Gerar";
            this.dica.SetToolTip(this.btnGerar, "Gerar o relatório (Enter ou F5)");
            this.btnGerar.UseVisualStyleBackColor = false;
            this.btnGerar.Click += new System.EventHandler(this.btnGerar_Click);
            //
            // btnLimpar
            //
            this.btnLimpar.BackColor = System.Drawing.Color.White;
            this.btnLimpar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLimpar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnLimpar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.btnLimpar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.btnLimpar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnLimpar.Location = new System.Drawing.Point(396, 31);
            this.btnLimpar.Name = "btnLimpar";
            this.btnLimpar.Size = new System.Drawing.Size(130, 32);
            this.btnLimpar.TabIndex = 4;
            this.btnLimpar.Text = "&Limpar filtros";
            this.dica.SetToolTip(this.btnLimpar, "Volta o limite para o padrão (10)");
            this.btnLimpar.UseVisualStyleBackColor = false;
            this.btnLimpar.Click += new System.EventHandler(this.btnLimpar_Click);
            //
            // lblDicaFiltro
            //
            this.lblDicaFiltro.AutoSize = true;
            this.lblDicaFiltro.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDicaFiltro.ForeColor = System.Drawing.Color.DimGray;
            this.lblDicaFiltro.Location = new System.Drawing.Point(20, 68);
            this.lblDicaFiltro.Name = "lblDicaFiltro";
            this.lblDicaFiltro.Size = new System.Drawing.Size(420, 15);
            this.lblDicaFiltro.TabIndex = 5;
            this.lblDicaFiltro.Text = "Deixe em branco para usar o padrão de 10 unidades.";
            //
            // tblResumo
            //
            this.tblResumo.BackColor = System.Drawing.Color.White;
            this.tblResumo.ColumnCount = 3;
            this.tblResumo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tblResumo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tblResumo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tblResumo.Controls.Add(this.cardInsumos, 0, 0);
            this.tblResumo.Controls.Add(this.cardBaixo, 1, 0);
            this.tblResumo.Controls.Add(this.cardEsgotado, 2, 0);
            this.tblResumo.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblResumo.Location = new System.Drawing.Point(0, 168);
            this.tblResumo.Name = "tblResumo";
            this.tblResumo.Padding = new System.Windows.Forms.Padding(14, 12, 14, 4);
            this.tblResumo.RowCount = 1;
            this.tblResumo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblResumo.Size = new System.Drawing.Size(880, 96);
            this.tblResumo.TabIndex = 2;
            //
            // cardInsumos
            //
            this.cardInsumos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardInsumos.Margin = new System.Windows.Forms.Padding(6);
            this.cardInsumos.Name = "cardInsumos";
            this.cardInsumos.Titulo = "Insumos cadastrados";
            this.cardInsumos.Valor = "0";
            //
            // cardBaixo
            //
            this.cardBaixo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardBaixo.Margin = new System.Windows.Forms.Padding(6);
            this.cardBaixo.Name = "cardBaixo";
            this.cardBaixo.Titulo = "Com estoque baixo";
            this.cardBaixo.Valor = "0";
            this.cardBaixo.CorDestaque = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(160)))), ((int)(((byte)(20)))));
            //
            // cardEsgotado
            //
            this.cardEsgotado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardEsgotado.Margin = new System.Windows.Forms.Padding(6);
            this.cardEsgotado.Name = "cardEsgotado";
            this.cardEsgotado.Titulo = "Esgotados";
            this.cardEsgotado.Valor = "0";
            this.cardEsgotado.CorDestaque = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(30)))), ((int)(((byte)(28)))));
            //
            // pnlGrade
            //
            this.pnlGrade.BackColor = System.Drawing.Color.White;
            this.pnlGrade.Controls.Add(this.lblVazio);
            this.pnlGrade.Controls.Add(this.dgvInsumos);
            this.pnlGrade.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGrade.Location = new System.Drawing.Point(0, 264);
            this.pnlGrade.Name = "pnlGrade";
            this.pnlGrade.Padding = new System.Windows.Forms.Padding(20, 8, 20, 8);
            this.pnlGrade.Size = new System.Drawing.Size(880, 262);
            this.pnlGrade.TabIndex = 3;
            //
            // dgvInsumos
            //
            this.dgvInsumos.AccessibleName = "Estoque por insumo";
            this.dgvInsumos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInsumos.Location = new System.Drawing.Point(20, 8);
            this.dgvInsumos.Name = "dgvInsumos";
            this.dgvInsumos.Size = new System.Drawing.Size(840, 246);
            this.dgvInsumos.TabIndex = 0;
            //
            // lblVazio
            //
            this.lblVazio.BackColor = System.Drawing.Color.White;
            this.lblVazio.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic);
            this.lblVazio.ForeColor = System.Drawing.Color.DimGray;
            this.lblVazio.Location = new System.Drawing.Point(21, 50);
            this.lblVazio.Name = "lblVazio";
            this.lblVazio.Size = new System.Drawing.Size(838, 60);
            this.lblVazio.TabIndex = 1;
            this.lblVazio.Text = "Nenhum registro encontrado.";
            this.lblVazio.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblVazio.Visible = false;
            //
            // pnlRodape
            //
            this.pnlRodape.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(246)))), ((int)(((byte)(244)))));
            this.pnlRodape.Controls.Add(this.lblStatus);
            this.pnlRodape.Controls.Add(this.lblNota);
            this.pnlRodape.Controls.Add(this.btnExportar);
            this.pnlRodape.Controls.Add(this.btnImprimir);
            this.pnlRodape.Controls.Add(this.btnFechar);
            this.pnlRodape.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRodape.Location = new System.Drawing.Point(0, 526);
            this.pnlRodape.Name = "pnlRodape";
            this.pnlRodape.Size = new System.Drawing.Size(880, 64);
            this.pnlRodape.TabIndex = 4;
            //
            // lblNota
            //
            this.lblNota.AutoSize = true;
            this.lblNota.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNota.ForeColor = System.Drawing.Color.DimGray;
            this.lblNota.Location = new System.Drawing.Point(20, 10);
            this.lblNota.Name = "lblNota";
            this.lblNota.Size = new System.Drawing.Size(460, 15);
            this.lblNota.TabIndex = 3;
            this.lblNota.Text = "ESGOTADO = 0 unid. · BAIXO = até o limite · OK = acima dele.";
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(110)))), ((int)(((byte)(46)))));
            this.lblStatus.Location = new System.Drawing.Point(20, 32);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 17);
            this.lblStatus.TabIndex = 4;
            //
            // btnExportar
            //
            this.btnExportar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportar.BackColor = System.Drawing.Color.White;
            this.btnExportar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(48)))), ((int)(((byte)(43)))));
            this.btnExportar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(238)))));
            this.btnExportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(48)))), ((int)(((byte)(43)))));
            this.btnExportar.Location = new System.Drawing.Point(510, 16);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(130, 32);
            this.btnExportar.TabIndex = 0;
            this.btnExportar.Text = "&Exportar CSV";
            this.dica.SetToolTip(this.btnExportar, "Salvar em CSV para abrir no Excel (Ctrl+E)");
            this.btnExportar.UseVisualStyleBackColor = false;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            //
            // btnImprimir
            //
            this.btnImprimir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImprimir.BackColor = System.Drawing.Color.White;
            this.btnImprimir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImprimir.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(48)))), ((int)(((byte)(43)))));
            this.btnImprimir.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(238)))));
            this.btnImprimir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImprimir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(48)))), ((int)(((byte)(43)))));
            this.btnImprimir.Location = new System.Drawing.Point(650, 16);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(110, 32);
            this.btnImprimir.TabIndex = 1;
            this.btnImprimir.Text = "Im&primir";
            this.dica.SetToolTip(this.btnImprimir, "Pré-visualizar, imprimir ou salvar em PDF (Ctrl+P)");
            this.btnImprimir.UseVisualStyleBackColor = false;
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            //
            // btnFechar
            //
            this.btnFechar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFechar.BackColor = System.Drawing.Color.White;
            this.btnFechar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFechar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnFechar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnFechar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.btnFechar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFechar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnFechar.Location = new System.Drawing.Point(770, 16);
            this.btnFechar.Name = "btnFechar";
            this.btnFechar.Size = new System.Drawing.Size(90, 32);
            this.btnFechar.TabIndex = 2;
            this.btnFechar.Text = "Fechar";
            this.dica.SetToolTip(this.btnFechar, "Fechar a tela (Esc)");
            this.btnFechar.UseVisualStyleBackColor = false;
            //
            // FrmRelatorioInsumos
            //
            this.AcceptButton = this.btnGerar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnFechar;
            this.ClientSize = new System.Drawing.Size(880, 590);
            this.Controls.Add(this.pnlGrade);
            this.Controls.Add(this.pnlRodape);
            this.Controls.Add(this.tblResumo);
            this.Controls.Add(this.pnlFiltros);
            this.Controls.Add(this.pnlTopo);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(780, 520);
            this.Name = "FrmRelatorioInsumos";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Hot-dog Brasília — Relatório de Insumos";
            this.Load += new System.EventHandler(this.FrmRelatorioInsumos_Load);
            this.pnlTopo.ResumeLayout(false);
            this.pnlTopo.PerformLayout();
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            this.tblResumo.ResumeLayout(false);
            this.pnlGrade.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInsumos)).EndInit();
            this.pnlRodape.ResumeLayout(false);
            this.pnlRodape.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTopo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.Label lblLimite;
        private System.Windows.Forms.TextBox txtLimite;
        private System.Windows.Forms.Label lblUnidades;
        private System.Windows.Forms.Button btnGerar;
        private System.Windows.Forms.Button btnLimpar;
        private System.Windows.Forms.Label lblDicaFiltro;
        private System.Windows.Forms.TableLayoutPanel tblResumo;
        private HotDogBrasilia.UI.CartaoResumo cardInsumos;
        private HotDogBrasilia.UI.CartaoResumo cardBaixo;
        private HotDogBrasilia.UI.CartaoResumo cardEsgotado;
        private System.Windows.Forms.Panel pnlGrade;
        private System.Windows.Forms.DataGridView dgvInsumos;
        private System.Windows.Forms.Label lblVazio;
        private System.Windows.Forms.Panel pnlRodape;
        private System.Windows.Forms.Label lblNota;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnImprimir;
        private System.Windows.Forms.Button btnFechar;
        private System.Windows.Forms.ToolTip dica;
    }
}
