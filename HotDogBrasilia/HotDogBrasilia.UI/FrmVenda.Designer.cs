namespace HotDogBrasilia.UI
{
    partial class FrmVenda
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
            this.pnlTopo = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblCliente = new System.Windows.Forms.Label();
            this.cboCliente = new System.Windows.Forms.ComboBox();
            this.lblProduto = new System.Windows.Forms.Label();
            this.cboProduto = new System.Windows.Forms.ComboBox();
            this.lblQuantidade = new System.Windows.Forms.Label();
            this.nudQuantidade = new System.Windows.Forms.NumericUpDown();
            this.btnAdicionar = new System.Windows.Forms.Button();
            this.lblDisponivel = new System.Windows.Forms.Label();
            this.dgvItens = new System.Windows.Forms.DataGridView();
            this.btnRemover = new System.Windows.Forms.Button();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblPagamento = new System.Windows.Forms.Label();
            this.cboPagamento = new System.Windows.Forms.ComboBox();
            this.btnFinalizar = new System.Windows.Forms.Button();
            this.btnDescartar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudQuantidade)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItens)).BeginInit();
            this.pnlTopo.SuspendLayout();
            this.SuspendLayout();
            //
            // lblCliente
            //
            this.lblCliente.AutoSize = true;
            this.lblCliente.Location = new System.Drawing.Point(20, 87);
            this.lblCliente.Name = "lblCliente";
            this.lblCliente.Size = new System.Drawing.Size(53, 19);
            this.lblCliente.TabIndex = 0;
            this.lblCliente.Text = "Cliente:";
            //
            // cboCliente
            //
            this.cboCliente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCliente.FormattingEnabled = true;
            this.cboCliente.Location = new System.Drawing.Point(20, 109);
            this.cboCliente.Name = "cboCliente";
            this.cboCliente.Size = new System.Drawing.Size(300, 23);
            this.cboCliente.TabIndex = 1;
            //
            // lblProduto
            //
            this.lblProduto.AutoSize = true;
            this.lblProduto.Location = new System.Drawing.Point(20, 147);
            this.lblProduto.Name = "lblProduto";
            this.lblProduto.Size = new System.Drawing.Size(60, 19);
            this.lblProduto.TabIndex = 2;
            this.lblProduto.Text = "Produto:";
            //
            // cboProduto
            //
            this.cboProduto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProduto.FormattingEnabled = true;
            this.cboProduto.Location = new System.Drawing.Point(20, 169);
            this.cboProduto.Name = "cboProduto";
            this.cboProduto.Size = new System.Drawing.Size(300, 23);
            this.cboProduto.TabIndex = 3;
            this.cboProduto.SelectedIndexChanged += new System.EventHandler(this.cboProduto_SelectedIndexChanged);
            //
            // lblQuantidade
            //
            this.lblQuantidade.AutoSize = true;
            this.lblQuantidade.Location = new System.Drawing.Point(335, 147);
            this.lblQuantidade.Name = "lblQuantidade";
            this.lblQuantidade.Size = new System.Drawing.Size(80, 19);
            this.lblQuantidade.TabIndex = 4;
            this.lblQuantidade.Text = "Quantidade:";
            //
            // nudQuantidade
            //
            this.nudQuantidade.Location = new System.Drawing.Point(335, 169);
            this.nudQuantidade.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudQuantidade.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudQuantidade.Name = "nudQuantidade";
            this.nudQuantidade.Size = new System.Drawing.Size(80, 23);
            this.nudQuantidade.TabIndex = 5;
            this.nudQuantidade.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            //
            // btnAdicionar
            //
            this.btnAdicionar.Location = new System.Drawing.Point(430, 166);
            this.btnAdicionar.Name = "btnAdicionar";
            this.btnAdicionar.Size = new System.Drawing.Size(100, 30);
            this.btnAdicionar.TabIndex = 6;
            this.btnAdicionar.Text = "Adicionar";
            this.btnAdicionar.UseVisualStyleBackColor = true;
            this.btnAdicionar.Click += new System.EventHandler(this.btnAdicionar_Click);
            //
            // lblDisponivel
            //
            this.lblDisponivel.AutoSize = true;
            this.lblDisponivel.ForeColor = System.Drawing.Color.DimGray;
            this.lblDisponivel.Location = new System.Drawing.Point(20, 197);
            this.lblDisponivel.Name = "lblDisponivel";
            this.lblDisponivel.Size = new System.Drawing.Size(0, 19);
            this.lblDisponivel.TabIndex = 7;
            //
            // dgvItens
            //
            this.dgvItens.AllowUserToAddRows = false;
            this.dgvItens.AllowUserToDeleteRows = false;
            this.dgvItens.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvItens.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItens.Location = new System.Drawing.Point(20, 224);
            this.dgvItens.MultiSelect = false;
            this.dgvItens.Name = "dgvItens";
            this.dgvItens.ReadOnly = true;
            this.dgvItens.RowHeadersVisible = false;
            this.dgvItens.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvItens.Size = new System.Drawing.Size(720, 220);
            this.dgvItens.TabIndex = 8;
            //
            // btnRemover
            //
            this.btnRemover.Location = new System.Drawing.Point(20, 454);
            this.btnRemover.Name = "btnRemover";
            this.btnRemover.Size = new System.Drawing.Size(120, 30);
            this.btnRemover.TabIndex = 9;
            this.btnRemover.Text = "Remover item";
            this.btnRemover.UseVisualStyleBackColor = true;
            this.btnRemover.Click += new System.EventHandler(this.btnRemover_Click);
            //
            // lblTotal
            //
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(30)))), ((int)(((byte)(28)))));
            this.lblTotal.Location = new System.Drawing.Point(440, 456);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(300, 25);
            this.lblTotal.TabIndex = 10;
            this.lblTotal.Text = "Total: R$ 0,00";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblPagamento
            //
            this.lblPagamento.AutoSize = true;
            this.lblPagamento.Location = new System.Drawing.Point(20, 504);
            this.lblPagamento.Name = "lblPagamento";
            this.lblPagamento.Size = new System.Drawing.Size(78, 19);
            this.lblPagamento.TabIndex = 11;
            this.lblPagamento.Text = "Pagamento:";
            //
            // cboPagamento
            //
            this.cboPagamento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPagamento.FormattingEnabled = true;
            this.cboPagamento.Location = new System.Drawing.Point(105, 501);
            this.cboPagamento.Name = "cboPagamento";
            this.cboPagamento.Size = new System.Drawing.Size(120, 23);
            this.cboPagamento.TabIndex = 12;
            //
            // btnFinalizar
            //
            this.btnFinalizar.Location = new System.Drawing.Point(490, 497);
            this.btnFinalizar.Name = "btnFinalizar";
            this.btnFinalizar.Size = new System.Drawing.Size(130, 32);
            this.btnFinalizar.TabIndex = 13;
            this.btnFinalizar.Text = "Finalizar venda";
            this.btnFinalizar.UseVisualStyleBackColor = true;
            this.btnFinalizar.Click += new System.EventHandler(this.btnFinalizar_Click);
            //
            // btnDescartar
            //
            this.btnDescartar.Location = new System.Drawing.Point(630, 497);
            this.btnDescartar.Name = "btnDescartar";
            this.btnDescartar.Size = new System.Drawing.Size(110, 32);
            this.btnDescartar.TabIndex = 14;
            this.btnDescartar.Text = "Descartar";
            this.btnDescartar.UseVisualStyleBackColor = true;
            this.btnDescartar.Click += new System.EventHandler(this.btnDescartar_Click);
            //
            // pnlTopo
            //
            this.pnlTopo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(48)))), ((int)(((byte)(43)))));
            this.pnlTopo.Controls.Add(this.lblSubtitulo);
            this.pnlTopo.Controls.Add(this.lblTitulo);
            this.pnlTopo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopo.Location = new System.Drawing.Point(0, 0);
            this.pnlTopo.Name = "pnlTopo";
            this.pnlTopo.Size = new System.Drawing.Size(760, 72);
            this.pnlTopo.TabIndex = 50;
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(18, 10);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Nova venda";
            //
            // lblSubtitulo
            //
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(178)))));
            this.lblSubtitulo.Location = new System.Drawing.Point(22, 44);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Escolha o cliente, adicione os itens e finalize com a forma de pagamento";
            //
            // FrmVenda
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(246)))), ((int)(((byte)(244)))));
            this.ClientSize = new System.Drawing.Size(760, 547);
            this.Controls.Add(this.btnDescartar);
            this.Controls.Add(this.btnFinalizar);
            this.Controls.Add(this.cboPagamento);
            this.Controls.Add(this.lblPagamento);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.btnRemover);
            this.Controls.Add(this.dgvItens);
            this.Controls.Add(this.lblDisponivel);
            this.Controls.Add(this.btnAdicionar);
            this.Controls.Add(this.nudQuantidade);
            this.Controls.Add(this.lblQuantidade);
            this.Controls.Add(this.cboProduto);
            this.Controls.Add(this.lblProduto);
            this.Controls.Add(this.cboCliente);
            this.Controls.Add(this.lblCliente);
            this.Controls.Add(this.pnlTopo);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmVenda";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hot-dog Brasília - Nova venda";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmVenda_FormClosing);
            this.Load += new System.EventHandler(this.FrmVenda_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudQuantidade)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItens)).EndInit();
            this.pnlTopo.ResumeLayout(false);
            this.pnlTopo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlTopo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.ComboBox cboCliente;
        private System.Windows.Forms.Label lblProduto;
        private System.Windows.Forms.ComboBox cboProduto;
        private System.Windows.Forms.Label lblQuantidade;
        private System.Windows.Forms.NumericUpDown nudQuantidade;
        private System.Windows.Forms.Button btnAdicionar;
        private System.Windows.Forms.Label lblDisponivel;
        private System.Windows.Forms.DataGridView dgvItens;
        private System.Windows.Forms.Button btnRemover;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblPagamento;
        private System.Windows.Forms.ComboBox cboPagamento;
        private System.Windows.Forms.Button btnFinalizar;
        private System.Windows.Forms.Button btnDescartar;
    }
}
