namespace HotDogBrasilia.UI
{
    partial class FrmLancamentos
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
            this.lblTipo = new System.Windows.Forms.Label();
            this.cboTipo = new System.Windows.Forms.ComboBox();
            this.lblSituacao = new System.Windows.Forms.Label();
            this.cboSituacao = new System.Windows.Forms.ComboBox();
            this.lblInicio = new System.Windows.Forms.Label();
            this.dtpInicio = new System.Windows.Forms.DateTimePicker();
            this.lblFim = new System.Windows.Forms.Label();
            this.dtpFim = new System.Windows.Forms.DateTimePicker();
            this.btnAtualizar = new System.Windows.Forms.Button();
            this.btnLimpar = new System.Windows.Forms.Button();
            this.lblDicaFiltro = new System.Windows.Forms.Label();
            this.pnlGrade = new System.Windows.Forms.Panel();
            this.dgvLancamentos = new System.Windows.Forms.DataGridView();
            this.lblVazio = new System.Windows.Forms.Label();
            this.pnlRodape = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnNovo = new System.Windows.Forms.Button();
            this.btnMarcarPago = new System.Windows.Forms.Button();
            this.btnFechar = new System.Windows.Forms.Button();
            this.dica = new System.Windows.Forms.ToolTip(this.components);
            this.pnlTopo.SuspendLayout();
            this.pnlFiltros.SuspendLayout();
            this.pnlGrade.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLancamentos)).BeginInit();
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
            this.pnlTopo.Size = new System.Drawing.Size(920, 72);
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
            this.lblSubtitulo.Text = "Contas a receber e a pagar: consultar, lançar e marcar como pago";
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(18, 10);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(270, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Lançamentos financeiros";
            //
            // pnlFiltros
            //
            this.pnlFiltros.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(246)))), ((int)(((byte)(244)))));
            this.pnlFiltros.Controls.Add(this.lblDicaFiltro);
            this.pnlFiltros.Controls.Add(this.btnLimpar);
            this.pnlFiltros.Controls.Add(this.btnAtualizar);
            this.pnlFiltros.Controls.Add(this.dtpFim);
            this.pnlFiltros.Controls.Add(this.lblFim);
            this.pnlFiltros.Controls.Add(this.dtpInicio);
            this.pnlFiltros.Controls.Add(this.lblInicio);
            this.pnlFiltros.Controls.Add(this.cboSituacao);
            this.pnlFiltros.Controls.Add(this.lblSituacao);
            this.pnlFiltros.Controls.Add(this.cboTipo);
            this.pnlFiltros.Controls.Add(this.lblTipo);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Location = new System.Drawing.Point(0, 72);
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(920, 96);
            this.pnlFiltros.TabIndex = 1;
            //
            // lblTipo
            //
            this.lblTipo.AutoSize = true;
            this.lblTipo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblTipo.Location = new System.Drawing.Point(20, 12);
            this.lblTipo.Name = "lblTipo";
            this.lblTipo.Size = new System.Drawing.Size(36, 19);
            this.lblTipo.TabIndex = 0;
            this.lblTipo.Text = "&Tipo";
            //
            // cboTipo
            //
            this.cboTipo.AccessibleName = "Tipo";
            this.cboTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipo.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cboTipo.Location = new System.Drawing.Point(20, 34);
            this.cboTipo.Name = "cboTipo";
            this.cboTipo.Size = new System.Drawing.Size(130, 25);
            this.cboTipo.TabIndex = 1;
            //
            // lblSituacao
            //
            this.lblSituacao.AutoSize = true;
            this.lblSituacao.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblSituacao.Location = new System.Drawing.Point(164, 12);
            this.lblSituacao.Name = "lblSituacao";
            this.lblSituacao.Size = new System.Drawing.Size(60, 19);
            this.lblSituacao.TabIndex = 2;
            this.lblSituacao.Text = "&Situação";
            //
            // cboSituacao
            //
            this.cboSituacao.AccessibleName = "Situação";
            this.cboSituacao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSituacao.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cboSituacao.Location = new System.Drawing.Point(164, 34);
            this.cboSituacao.Name = "cboSituacao";
            this.cboSituacao.Size = new System.Drawing.Size(120, 25);
            this.cboSituacao.TabIndex = 3;
            //
            // lblInicio
            //
            this.lblInicio.AutoSize = true;
            this.lblInicio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblInicio.Location = new System.Drawing.Point(298, 12);
            this.lblInicio.Name = "lblInicio";
            this.lblInicio.Size = new System.Drawing.Size(80, 19);
            this.lblInicio.TabIndex = 4;
            this.lblInicio.Text = "Data &inicial";
            //
            // dtpInicio
            //
            this.dtpInicio.AccessibleName = "Data inicial";
            this.dtpInicio.Checked = false;
            this.dtpInicio.CustomFormat = "dd/MM/yyyy";
            this.dtpInicio.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpInicio.Location = new System.Drawing.Point(298, 34);
            this.dtpInicio.Name = "dtpInicio";
            this.dtpInicio.ShowCheckBox = true;
            this.dtpInicio.Size = new System.Drawing.Size(150, 25);
            this.dtpInicio.TabIndex = 5;
            //
            // lblFim
            //
            this.lblFim.AutoSize = true;
            this.lblFim.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblFim.Location = new System.Drawing.Point(462, 12);
            this.lblFim.Name = "lblFim";
            this.lblFim.Size = new System.Drawing.Size(70, 19);
            this.lblFim.TabIndex = 6;
            this.lblFim.Text = "Data &final";
            //
            // dtpFim
            //
            this.dtpFim.AccessibleName = "Data final";
            this.dtpFim.Checked = false;
            this.dtpFim.CustomFormat = "dd/MM/yyyy";
            this.dtpFim.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFim.Location = new System.Drawing.Point(462, 34);
            this.dtpFim.Name = "dtpFim";
            this.dtpFim.ShowCheckBox = true;
            this.dtpFim.Size = new System.Drawing.Size(150, 25);
            this.dtpFim.TabIndex = 7;
            //
            // btnAtualizar
            //
            this.btnAtualizar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(48)))), ((int)(((byte)(43)))));
            this.btnAtualizar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAtualizar.FlatAppearance.BorderSize = 0;
            this.btnAtualizar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(30)))), ((int)(((byte)(28)))));
            this.btnAtualizar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(38)))), ((int)(((byte)(34)))));
            this.btnAtualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAtualizar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAtualizar.ForeColor = System.Drawing.Color.White;
            this.btnAtualizar.Location = new System.Drawing.Point(630, 31);
            this.btnAtualizar.Name = "btnAtualizar";
            this.btnAtualizar.Size = new System.Drawing.Size(120, 32);
            this.btnAtualizar.TabIndex = 8;
            this.btnAtualizar.Text = "&Atualizar";
            this.dica.SetToolTip(this.btnAtualizar, "Buscar os lançamentos com os filtros (Enter ou F5)");
            this.btnAtualizar.UseVisualStyleBackColor = false;
            this.btnAtualizar.Click += new System.EventHandler(this.btnAtualizar_Click);
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
            this.btnLimpar.Location = new System.Drawing.Point(760, 31);
            this.btnLimpar.Name = "btnLimpar";
            this.btnLimpar.Size = new System.Drawing.Size(130, 32);
            this.btnLimpar.TabIndex = 9;
            this.btnLimpar.Text = "&Limpar filtros";
            this.dica.SetToolTip(this.btnLimpar, "Volta os filtros ao estado inicial (todos)");
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
            this.lblDicaFiltro.TabIndex = 10;
            this.lblDicaFiltro.Text = "Marque a caixa da data para usá-la. Desmarcada = sem limite naquele lado do período.";
            //
            // pnlGrade
            //
            this.pnlGrade.BackColor = System.Drawing.Color.White;
            this.pnlGrade.Controls.Add(this.lblVazio);
            this.pnlGrade.Controls.Add(this.dgvLancamentos);
            this.pnlGrade.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGrade.Location = new System.Drawing.Point(0, 168);
            this.pnlGrade.Name = "pnlGrade";
            this.pnlGrade.Padding = new System.Windows.Forms.Padding(20, 12, 20, 8);
            this.pnlGrade.Size = new System.Drawing.Size(920, 358);
            this.pnlGrade.TabIndex = 2;
            //
            // dgvLancamentos
            //
            this.dgvLancamentos.AccessibleName = "Lançamentos financeiros";
            this.dgvLancamentos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLancamentos.Location = new System.Drawing.Point(20, 12);
            this.dgvLancamentos.Name = "dgvLancamentos";
            this.dgvLancamentos.Size = new System.Drawing.Size(880, 338);
            this.dgvLancamentos.TabIndex = 0;
            //
            // lblVazio
            //
            this.lblVazio.BackColor = System.Drawing.Color.White;
            this.lblVazio.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic);
            this.lblVazio.ForeColor = System.Drawing.Color.DimGray;
            this.lblVazio.Location = new System.Drawing.Point(21, 50);
            this.lblVazio.Name = "lblVazio";
            this.lblVazio.Size = new System.Drawing.Size(878, 60);
            this.lblVazio.TabIndex = 1;
            this.lblVazio.Text = "Nenhum registro encontrado.";
            this.lblVazio.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblVazio.Visible = false;
            //
            // pnlRodape
            //
            this.pnlRodape.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(246)))), ((int)(((byte)(244)))));
            this.pnlRodape.Controls.Add(this.lblStatus);
            this.pnlRodape.Controls.Add(this.btnNovo);
            this.pnlRodape.Controls.Add(this.btnMarcarPago);
            this.pnlRodape.Controls.Add(this.btnFechar);
            this.pnlRodape.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRodape.Location = new System.Drawing.Point(0, 526);
            this.pnlRodape.Name = "pnlRodape";
            this.pnlRodape.Size = new System.Drawing.Size(920, 64);
            this.pnlRodape.TabIndex = 3;
            //
            // lblStatus
            //
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.AutoEllipsis = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(110)))), ((int)(((byte)(46)))));
            this.lblStatus.Location = new System.Drawing.Point(20, 22);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(450, 20);
            this.lblStatus.TabIndex = 3;
            //
            // btnNovo
            //
            this.btnNovo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNovo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(48)))), ((int)(((byte)(43)))));
            this.btnNovo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNovo.FlatAppearance.BorderSize = 0;
            this.btnNovo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(30)))), ((int)(((byte)(28)))));
            this.btnNovo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(38)))), ((int)(((byte)(34)))));
            this.btnNovo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNovo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnNovo.ForeColor = System.Drawing.Color.White;
            this.btnNovo.Location = new System.Drawing.Point(490, 16);
            this.btnNovo.Name = "btnNovo";
            this.btnNovo.Size = new System.Drawing.Size(150, 32);
            this.btnNovo.TabIndex = 0;
            this.btnNovo.Text = "&Novo lançamento";
            this.dica.SetToolTip(this.btnNovo, "Lançar uma conta a receber ou a pagar (Ctrl+N)");
            this.btnNovo.UseVisualStyleBackColor = false;
            this.btnNovo.Click += new System.EventHandler(this.btnNovo_Click);
            //
            // btnMarcarPago
            //
            this.btnMarcarPago.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMarcarPago.BackColor = System.Drawing.Color.White;
            this.btnMarcarPago.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMarcarPago.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(48)))), ((int)(((byte)(43)))));
            this.btnMarcarPago.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(240)))), ((int)(((byte)(238)))));
            this.btnMarcarPago.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMarcarPago.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(48)))), ((int)(((byte)(43)))));
            this.btnMarcarPago.Location = new System.Drawing.Point(650, 16);
            this.btnMarcarPago.Name = "btnMarcarPago";
            this.btnMarcarPago.Size = new System.Drawing.Size(150, 32);
            this.btnMarcarPago.TabIndex = 1;
            this.btnMarcarPago.Text = "&Marcar como pago";
            this.dica.SetToolTip(this.btnMarcarPago, "Marcar o lançamento selecionado como pago");
            this.btnMarcarPago.UseVisualStyleBackColor = false;
            this.btnMarcarPago.Click += new System.EventHandler(this.btnMarcarPago_Click);
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
            this.btnFechar.Location = new System.Drawing.Point(810, 16);
            this.btnFechar.Name = "btnFechar";
            this.btnFechar.Size = new System.Drawing.Size(90, 32);
            this.btnFechar.TabIndex = 2;
            this.btnFechar.Text = "Fechar";
            this.dica.SetToolTip(this.btnFechar, "Fechar a tela (Esc)");
            this.btnFechar.UseVisualStyleBackColor = false;
            //
            // FrmLancamentos
            //
            this.AcceptButton = this.btnAtualizar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnFechar;
            this.ClientSize = new System.Drawing.Size(920, 590);
            this.Controls.Add(this.pnlGrade);
            this.Controls.Add(this.pnlRodape);
            this.Controls.Add(this.pnlFiltros);
            this.Controls.Add(this.pnlTopo);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(936, 520);
            this.Name = "FrmLancamentos";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Hot-dog Brasília — Lançamentos financeiros";
            this.Load += new System.EventHandler(this.FrmLancamentos_Load);
            this.pnlTopo.ResumeLayout(false);
            this.pnlTopo.PerformLayout();
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            this.pnlGrade.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLancamentos)).EndInit();
            this.pnlRodape.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTopo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.Label lblTipo;
        private System.Windows.Forms.ComboBox cboTipo;
        private System.Windows.Forms.Label lblSituacao;
        private System.Windows.Forms.ComboBox cboSituacao;
        private System.Windows.Forms.Label lblInicio;
        private System.Windows.Forms.DateTimePicker dtpInicio;
        private System.Windows.Forms.Label lblFim;
        private System.Windows.Forms.DateTimePicker dtpFim;
        private System.Windows.Forms.Button btnAtualizar;
        private System.Windows.Forms.Button btnLimpar;
        private System.Windows.Forms.Label lblDicaFiltro;
        private System.Windows.Forms.Panel pnlGrade;
        private System.Windows.Forms.DataGridView dgvLancamentos;
        private System.Windows.Forms.Label lblVazio;
        private System.Windows.Forms.Panel pnlRodape;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnNovo;
        private System.Windows.Forms.Button btnMarcarPago;
        private System.Windows.Forms.Button btnFechar;
        private System.Windows.Forms.ToolTip dica;
    }
}
