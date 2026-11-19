namespace HotDogBrasilia.UI
{
    partial class FrmLogAcoes
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
            this.lblInicio = new System.Windows.Forms.Label();
            this.dtpInicio = new System.Windows.Forms.DateTimePicker();
            this.lblFim = new System.Windows.Forms.Label();
            this.dtpFim = new System.Windows.Forms.DateTimePicker();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.cboUsuario = new System.Windows.Forms.ComboBox();
            this.btnConsultar = new System.Windows.Forms.Button();
            this.btnLimpar = new System.Windows.Forms.Button();
            this.lblDicaFiltro = new System.Windows.Forms.Label();
            this.pnlGrade = new System.Windows.Forms.Panel();
            this.dgvLog = new System.Windows.Forms.DataGridView();
            this.lblVazio = new System.Windows.Forms.Label();
            this.pnlRodape = new System.Windows.Forms.Panel();
            this.lblResumo = new System.Windows.Forms.Label();
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnImprimir = new System.Windows.Forms.Button();
            this.btnFechar = new System.Windows.Forms.Button();
            this.dica = new System.Windows.Forms.ToolTip(this.components);
            this.pnlTopo.SuspendLayout();
            this.pnlFiltros.SuspendLayout();
            this.pnlGrade.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLog)).BeginInit();
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
            this.lblSubtitulo.Text = "Quem fez o quê no sistema, por período e por usuário (somente consulta)";
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(18, 10);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(160, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Log de ações";
            //
            // pnlFiltros
            //
            this.pnlFiltros.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(246)))), ((int)(((byte)(244)))));
            this.pnlFiltros.Controls.Add(this.lblDicaFiltro);
            this.pnlFiltros.Controls.Add(this.btnLimpar);
            this.pnlFiltros.Controls.Add(this.btnConsultar);
            this.pnlFiltros.Controls.Add(this.cboUsuario);
            this.pnlFiltros.Controls.Add(this.lblUsuario);
            this.pnlFiltros.Controls.Add(this.dtpFim);
            this.pnlFiltros.Controls.Add(this.lblFim);
            this.pnlFiltros.Controls.Add(this.dtpInicio);
            this.pnlFiltros.Controls.Add(this.lblInicio);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Location = new System.Drawing.Point(0, 72);
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(920, 96);
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
            this.lblFim.Location = new System.Drawing.Point(184, 12);
            this.lblFim.Name = "lblFim";
            this.lblFim.Size = new System.Drawing.Size(70, 19);
            this.lblFim.TabIndex = 2;
            this.lblFim.Text = "Data &final";
            //
            // dtpFim
            //
            this.dtpFim.AccessibleName = "Data final";
            this.dtpFim.CustomFormat = "dd/MM/yyyy";
            this.dtpFim.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFim.Location = new System.Drawing.Point(184, 34);
            this.dtpFim.Name = "dtpFim";
            this.dtpFim.ShowCheckBox = true;
            this.dtpFim.Size = new System.Drawing.Size(150, 25);
            this.dtpFim.TabIndex = 3;
            //
            // lblUsuario
            //
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblUsuario.Location = new System.Drawing.Point(348, 12);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(56, 19);
            this.lblUsuario.TabIndex = 4;
            this.lblUsuario.Text = "&Usuário";
            //
            // cboUsuario
            //
            this.cboUsuario.AccessibleName = "Usuário";
            this.cboUsuario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboUsuario.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cboUsuario.Location = new System.Drawing.Point(348, 34);
            this.cboUsuario.Name = "cboUsuario";
            this.cboUsuario.Size = new System.Drawing.Size(200, 25);
            this.cboUsuario.TabIndex = 5;
            //
            // btnConsultar
            //
            this.btnConsultar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(48)))), ((int)(((byte)(43)))));
            this.btnConsultar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConsultar.FlatAppearance.BorderSize = 0;
            this.btnConsultar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(30)))), ((int)(((byte)(28)))));
            this.btnConsultar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(38)))), ((int)(((byte)(34)))));
            this.btnConsultar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConsultar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnConsultar.ForeColor = System.Drawing.Color.White;
            this.btnConsultar.Location = new System.Drawing.Point(566, 31);
            this.btnConsultar.Name = "btnConsultar";
            this.btnConsultar.Size = new System.Drawing.Size(120, 32);
            this.btnConsultar.TabIndex = 6;
            this.btnConsultar.Text = "&Consultar";
            this.dica.SetToolTip(this.btnConsultar, "Buscar o log com os filtros (Enter ou F5)");
            this.btnConsultar.UseVisualStyleBackColor = false;
            this.btnConsultar.Click += new System.EventHandler(this.btnConsultar_Click);
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
            this.btnLimpar.Location = new System.Drawing.Point(696, 31);
            this.btnLimpar.Name = "btnLimpar";
            this.btnLimpar.Size = new System.Drawing.Size(130, 32);
            this.btnLimpar.TabIndex = 7;
            this.btnLimpar.Text = "&Limpar filtros";
            this.dica.SetToolTip(this.btnLimpar, "Volta aos últimos 7 dias e a todos os usuários");
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
            this.lblDicaFiltro.TabIndex = 8;
            this.lblDicaFiltro.Text = "Abre com os últimos 7 dias. Desmarque a caixa da data para não limitar aquele lado do período.";
            //
            // pnlGrade
            //
            this.pnlGrade.BackColor = System.Drawing.Color.White;
            this.pnlGrade.Controls.Add(this.lblVazio);
            this.pnlGrade.Controls.Add(this.dgvLog);
            this.pnlGrade.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGrade.Location = new System.Drawing.Point(0, 168);
            this.pnlGrade.Name = "pnlGrade";
            this.pnlGrade.Padding = new System.Windows.Forms.Padding(20, 12, 20, 8);
            this.pnlGrade.Size = new System.Drawing.Size(920, 358);
            this.pnlGrade.TabIndex = 2;
            //
            // dgvLog
            //
            this.dgvLog.AccessibleName = "Log de ações";
            this.dgvLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLog.Location = new System.Drawing.Point(20, 12);
            this.dgvLog.Name = "dgvLog";
            this.dgvLog.Size = new System.Drawing.Size(880, 338);
            this.dgvLog.TabIndex = 0;
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
            this.pnlRodape.Controls.Add(this.lblResumo);
            this.pnlRodape.Controls.Add(this.btnExportar);
            this.pnlRodape.Controls.Add(this.btnImprimir);
            this.pnlRodape.Controls.Add(this.btnFechar);
            this.pnlRodape.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRodape.Location = new System.Drawing.Point(0, 526);
            this.pnlRodape.Name = "pnlRodape";
            this.pnlRodape.Size = new System.Drawing.Size(920, 64);
            this.pnlRodape.TabIndex = 3;
            //
            // lblResumo
            //
            this.lblResumo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResumo.AutoEllipsis = true;
            this.lblResumo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblResumo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblResumo.Location = new System.Drawing.Point(20, 22);
            this.lblResumo.Name = "lblResumo";
            this.lblResumo.Size = new System.Drawing.Size(450, 20);
            this.lblResumo.TabIndex = 3;
            this.lblResumo.Text = "0 registros";
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
            this.btnExportar.Location = new System.Drawing.Point(560, 16);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(130, 32);
            this.btnExportar.TabIndex = 0;
            this.btnExportar.Text = "&Exportar CSV";
            this.dica.SetToolTip(this.btnExportar, "Salvar o que está na grade em CSV (Ctrl+E)");
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
            this.btnImprimir.Location = new System.Drawing.Point(700, 16);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(100, 32);
            this.btnImprimir.TabIndex = 1;
            this.btnImprimir.Text = "Im&primir";
            this.dica.SetToolTip(this.btnImprimir, "Imprimir o que está na grade (Ctrl+P)");
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
            this.btnFechar.Location = new System.Drawing.Point(810, 16);
            this.btnFechar.Name = "btnFechar";
            this.btnFechar.Size = new System.Drawing.Size(90, 32);
            this.btnFechar.TabIndex = 2;
            this.btnFechar.Text = "Fechar";
            this.dica.SetToolTip(this.btnFechar, "Fechar a tela (Esc)");
            this.btnFechar.UseVisualStyleBackColor = false;
            //
            // FrmLogAcoes
            //
            this.AcceptButton = this.btnConsultar;
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
            this.Name = "FrmLogAcoes";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Hot-dog Brasília — Log de ações";
            this.Load += new System.EventHandler(this.FrmLogAcoes_Load);
            this.pnlTopo.ResumeLayout(false);
            this.pnlTopo.PerformLayout();
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            this.pnlGrade.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLog)).EndInit();
            this.pnlRodape.ResumeLayout(false);
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
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.ComboBox cboUsuario;
        private System.Windows.Forms.Button btnConsultar;
        private System.Windows.Forms.Button btnLimpar;
        private System.Windows.Forms.Label lblDicaFiltro;
        private System.Windows.Forms.Panel pnlGrade;
        private System.Windows.Forms.DataGridView dgvLog;
        private System.Windows.Forms.Label lblVazio;
        private System.Windows.Forms.Panel pnlRodape;
        private System.Windows.Forms.Label lblResumo;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnImprimir;
        private System.Windows.Forms.Button btnFechar;
        private System.Windows.Forms.ToolTip dica;
    }
}
