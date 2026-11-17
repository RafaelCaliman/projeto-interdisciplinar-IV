namespace HotDogBrasilia.UI
{
    partial class FrmRelatorioFinanceiro
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
            this.dtpFim = new System.Windows.Forms.DateTimePicker();
            this.lblFim = new System.Windows.Forms.Label();
            this.dtpInicio = new System.Windows.Forms.DateTimePicker();
            this.lblInicio = new System.Windows.Forms.Label();
            this.lblDicaFiltro = new System.Windows.Forms.Label();
            this.tblResumo = new System.Windows.Forms.TableLayoutPanel();
            this.cardRecebido = new HotDogBrasilia.UI.CartaoResumo();
            this.cardPago = new HotDogBrasilia.UI.CartaoResumo();
            this.cardSaldoRealizado = new HotDogBrasilia.UI.CartaoResumo();
            this.cardAReceber = new HotDogBrasilia.UI.CartaoResumo();
            this.cardAPagar = new HotDogBrasilia.UI.CartaoResumo();
            this.cardSaldoPrevisto = new HotDogBrasilia.UI.CartaoResumo();
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
            this.lblSubtitulo.Text = "Entradas, saídas e saldo do período (contas a receber e a pagar)";
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
            this.lblTitulo.Text = "Relatório Financeiro";
            //
            // pnlFiltros
            //
            this.pnlFiltros.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(246)))), ((int)(((byte)(244)))));
            this.pnlFiltros.Controls.Add(this.lblDicaFiltro);
            this.pnlFiltros.Controls.Add(this.btnLimpar);
            this.pnlFiltros.Controls.Add(this.btnGerar);
            this.pnlFiltros.Controls.Add(this.dtpFim);
            this.pnlFiltros.Controls.Add(this.lblFim);
            this.pnlFiltros.Controls.Add(this.dtpInicio);
            this.pnlFiltros.Controls.Add(this.lblInicio);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Location = new System.Drawing.Point(0, 72);
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(880, 96);
            this.pnlFiltros.TabIndex = 1;
            //
            // lblInicio
            //
            this.lblInicio.AutoSize = true;
            this.lblInicio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblInicio.Location = new System.Drawing.Point(20, 12);
            this.lblInicio.Name = "lblInicio";
            this.lblInicio.Size = new System.Drawing.Size(80, 19);
            this.lblInicio.TabIndex = 0;
            this.lblInicio.Text = "Data &inicial";
            //
            // dtpInicio
            //
            this.dtpInicio.AccessibleName = "Data inicial";
            this.dtpInicio.Checked = false;
            this.dtpInicio.CustomFormat = "dd/MM/yyyy";
            this.dtpInicio.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpInicio.Location = new System.Drawing.Point(20, 34);
            this.dtpInicio.Name = "dtpInicio";
            this.dtpInicio.ShowCheckBox = true;
            this.dtpInicio.Size = new System.Drawing.Size(150, 25);
            this.dtpInicio.TabIndex = 1;
            //
            // lblFim
            //
            this.lblFim.AutoSize = true;
            this.lblFim.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblFim.Location = new System.Drawing.Point(186, 12);
            this.lblFim.Name = "lblFim";
            this.lblFim.Size = new System.Drawing.Size(70, 19);
            this.lblFim.TabIndex = 2;
            this.lblFim.Text = "Data &final";
            //
            // dtpFim
            //
            this.dtpFim.AccessibleName = "Data final";
            this.dtpFim.Checked = false;
            this.dtpFim.CustomFormat = "dd/MM/yyyy";
            this.dtpFim.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFim.Location = new System.Drawing.Point(186, 34);
            this.dtpFim.Name = "dtpFim";
            this.dtpFim.ShowCheckBox = true;
            this.dtpFim.Size = new System.Drawing.Size(150, 25);
            this.dtpFim.TabIndex = 3;
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
            this.btnGerar.Location = new System.Drawing.Point(356, 31);
            this.btnGerar.Name = "btnGerar";
            this.btnGerar.Size = new System.Drawing.Size(120, 32);
            this.btnGerar.TabIndex = 4;
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
            this.btnLimpar.Location = new System.Drawing.Point(486, 31);
            this.btnLimpar.Name = "btnLimpar";
            this.btnLimpar.Size = new System.Drawing.Size(130, 32);
            this.btnLimpar.TabIndex = 5;
            this.btnLimpar.Text = "&Limpar filtros";
            this.dica.SetToolTip(this.btnLimpar, "Volta os filtros ao estado inicial (todas as datas)");
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
            this.lblDicaFiltro.TabIndex = 6;
            this.lblDicaFiltro.Text = "Marque a caixa da data para usá-la. Desmarcada = sem limite naquele lado do período.";
            //
            // tblResumo
            //
            this.tblResumo.BackColor = System.Drawing.Color.White;
            this.tblResumo.ColumnCount = 3;
            this.tblResumo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tblResumo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tblResumo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tblResumo.Controls.Add(this.cardRecebido, 0, 0);
            this.tblResumo.Controls.Add(this.cardPago, 1, 0);
            this.tblResumo.Controls.Add(this.cardSaldoRealizado, 2, 0);
            this.tblResumo.Controls.Add(this.cardAReceber, 0, 1);
            this.tblResumo.Controls.Add(this.cardAPagar, 1, 1);
            this.tblResumo.Controls.Add(this.cardSaldoPrevisto, 2, 1);
            this.tblResumo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblResumo.Location = new System.Drawing.Point(0, 168);
            this.tblResumo.Name = "tblResumo";
            this.tblResumo.Padding = new System.Windows.Forms.Padding(14, 16, 14, 16);
            this.tblResumo.RowCount = 2;
            this.tblResumo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblResumo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblResumo.Size = new System.Drawing.Size(880, 260);
            this.tblResumo.TabIndex = 2;
            //
            // cardRecebido
            //
            this.cardRecebido.CorDestaque = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(110)))), ((int)(((byte)(46)))));
            this.cardRecebido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardRecebido.Margin = new System.Windows.Forms.Padding(8);
            this.cardRecebido.Name = "cardRecebido";
            this.cardRecebido.Titulo = "Recebido (entradas pagas)";
            this.cardRecebido.Valor = "R$ 0,00";
            //
            // cardPago
            //
            this.cardPago.CorDestaque = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(48)))), ((int)(((byte)(43)))));
            this.cardPago.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardPago.Margin = new System.Windows.Forms.Padding(8);
            this.cardPago.Name = "cardPago";
            this.cardPago.Titulo = "Pago (saídas pagas)";
            this.cardPago.Valor = "R$ 0,00";
            //
            // cardSaldoRealizado
            //
            this.cardSaldoRealizado.CorDestaque = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cardSaldoRealizado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardSaldoRealizado.Margin = new System.Windows.Forms.Padding(8);
            this.cardSaldoRealizado.Name = "cardSaldoRealizado";
            this.cardSaldoRealizado.Titulo = "Saldo realizado";
            this.cardSaldoRealizado.Valor = "R$ 0,00";
            //
            // cardAReceber
            //
            this.cardAReceber.CorDestaque = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(160)))), ((int)(((byte)(20)))));
            this.cardAReceber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardAReceber.Margin = new System.Windows.Forms.Padding(8);
            this.cardAReceber.Name = "cardAReceber";
            this.cardAReceber.Titulo = "A receber (pendente)";
            this.cardAReceber.Valor = "R$ 0,00";
            //
            // cardAPagar
            //
            this.cardAPagar.CorDestaque = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(160)))), ((int)(((byte)(20)))));
            this.cardAPagar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardAPagar.Margin = new System.Windows.Forms.Padding(8);
            this.cardAPagar.Name = "cardAPagar";
            this.cardAPagar.Titulo = "A pagar (pendente)";
            this.cardAPagar.Valor = "R$ 0,00";
            //
            // cardSaldoPrevisto
            //
            this.cardSaldoPrevisto.CorDestaque = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cardSaldoPrevisto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardSaldoPrevisto.Margin = new System.Windows.Forms.Padding(8);
            this.cardSaldoPrevisto.Name = "cardSaldoPrevisto";
            this.cardSaldoPrevisto.Titulo = "Saldo previsto";
            this.cardSaldoPrevisto.Valor = "R$ 0,00";
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
            this.pnlRodape.Location = new System.Drawing.Point(0, 436);
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
            this.lblNota.Size = new System.Drawing.Size(420, 15);
            this.lblNota.TabIndex = 3;
            this.lblNota.Text = "Realizado = lançamentos já pagos · Previsto = inclui também os pendentes.";
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
            // FrmRelatorioFinanceiro
            //
            this.AcceptButton = this.btnGerar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnFechar;
            this.ClientSize = new System.Drawing.Size(880, 500);
            this.Controls.Add(this.tblResumo);
            this.Controls.Add(this.pnlRodape);
            this.Controls.Add(this.pnlFiltros);
            this.Controls.Add(this.pnlTopo);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(780, 460);
            this.Name = "FrmRelatorioFinanceiro";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Hot-dog Brasília — Relatório Financeiro";
            this.Load += new System.EventHandler(this.FrmRelatorioFinanceiro_Load);
            this.pnlTopo.ResumeLayout(false);
            this.pnlTopo.PerformLayout();
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            this.tblResumo.ResumeLayout(false);
            this.pnlRodape.ResumeLayout(false);
            this.pnlRodape.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTopo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.Label lblInicio;
        private System.Windows.Forms.DateTimePicker dtpInicio;
        private System.Windows.Forms.Label lblFim;
        private System.Windows.Forms.DateTimePicker dtpFim;
        private System.Windows.Forms.Button btnGerar;
        private System.Windows.Forms.Button btnLimpar;
        private System.Windows.Forms.Label lblDicaFiltro;
        private System.Windows.Forms.TableLayoutPanel tblResumo;
        private HotDogBrasilia.UI.CartaoResumo cardRecebido;
        private HotDogBrasilia.UI.CartaoResumo cardPago;
        private HotDogBrasilia.UI.CartaoResumo cardSaldoRealizado;
        private HotDogBrasilia.UI.CartaoResumo cardAReceber;
        private HotDogBrasilia.UI.CartaoResumo cardAPagar;
        private HotDogBrasilia.UI.CartaoResumo cardSaldoPrevisto;
        private System.Windows.Forms.Panel pnlRodape;
        private System.Windows.Forms.Label lblNota;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnImprimir;
        private System.Windows.Forms.Button btnFechar;
        private System.Windows.Forms.ToolTip dica;
    }
}
