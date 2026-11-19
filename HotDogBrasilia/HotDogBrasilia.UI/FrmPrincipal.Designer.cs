namespace HotDogBrasilia.UI
{
    partial class FrmPrincipal
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuCadastros = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClientes = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuProdutos = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuInsumos = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFornecedores = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuVendas = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNovaVenda = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuConsultarVendas = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSistema = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLogAcoes = new System.Windows.Forms.ToolStripMenuItem();
            this.sepSistema = new System.Windows.Forms.ToolStripSeparator();
            this.mnuTrocarUsuario = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSair = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRelatorios = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRelVendas = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRelEstoque = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRelInsumos = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRelFinanceiro = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFinanceiro = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLancamentos = new System.Windows.Forms.ToolStripMenuItem();
            this.stsPrincipal = new System.Windows.Forms.StatusStrip();
            this.lblUsuarioBarra = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.stsPrincipal.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCadastros,
            this.mnuVendas,
            this.mnuFinanceiro,
            this.mnuRelatorios,
            this.mnuSistema});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuCadastros
            // 
            this.mnuCadastros.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuClientes,
            this.mnuProdutos,
            this.mnuInsumos,
            this.mnuFornecedores});
            this.mnuCadastros.Name = "mnuCadastros";
            this.mnuCadastros.Size = new System.Drawing.Size(71, 20);
            this.mnuCadastros.Text = "Cadastros";
            // 
            // mnuClientes
            // 
            this.mnuClientes.Name = "mnuClientes";
            this.mnuClientes.Size = new System.Drawing.Size(122, 22);
            this.mnuClientes.Text = "Clientes";
            this.mnuClientes.Click += new System.EventHandler(this.mnuClientes_Click);
            // 
            // mnuProdutos
            // 
            this.mnuProdutos.Name = "mnuProdutos";
            this.mnuProdutos.Size = new System.Drawing.Size(122, 22);
            this.mnuProdutos.Text = "Produtos";
            this.mnuProdutos.Click += new System.EventHandler(this.mnuProdutos_Click);
            // 
            // mnuInsumos
            // 
            this.mnuInsumos.Name = "mnuInsumos";
            this.mnuInsumos.Size = new System.Drawing.Size(122, 22);
            this.mnuInsumos.Text = "Insumos";
            this.mnuInsumos.Click += new System.EventHandler(this.mnuInsumos_Click);
            // 
            // mnuFornecedores
            // 
            this.mnuFornecedores.Name = "mnuFornecedores";
            this.mnuFornecedores.Size = new System.Drawing.Size(122, 22);
            this.mnuFornecedores.Text = "Fornecedores";
            this.mnuFornecedores.Click += new System.EventHandler(this.mnuFornecedores_Click);
            // 
            // mnuVendas
            // 
            this.mnuVendas.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuNovaVenda,
            this.mnuConsultarVendas});
            this.mnuVendas.Name = "mnuVendas";
            this.mnuVendas.Size = new System.Drawing.Size(56, 20);
            this.mnuVendas.Text = "Vendas";
            // 
            // mnuNovaVenda
            // 
            this.mnuNovaVenda.Name = "mnuNovaVenda";
            this.mnuNovaVenda.Size = new System.Drawing.Size(165, 22);
            this.mnuNovaVenda.Text = "Nova venda";
            this.mnuNovaVenda.Click += new System.EventHandler(this.mnuNovaVenda_Click);
            // 
            // mnuConsultarVendas
            // 
            this.mnuConsultarVendas.Name = "mnuConsultarVendas";
            this.mnuConsultarVendas.Size = new System.Drawing.Size(165, 22);
            this.mnuConsultarVendas.Text = "Consultar vendas";
            this.mnuConsultarVendas.Click += new System.EventHandler(this.mnuConsultarVendas_Click);
            // 
            // mnuSistema
            // 
            this.mnuSistema.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLogAcoes,
            this.sepSistema,
            this.mnuTrocarUsuario,
            this.mnuSair});
            this.mnuSistema.Name = "mnuSistema";
            this.mnuSistema.Size = new System.Drawing.Size(60, 20);
            this.mnuSistema.Text = "Sistema";
            // 
            // mnuLogAcoes
            // 
            this.mnuLogAcoes.Name = "mnuLogAcoes";
            this.mnuLogAcoes.Size = new System.Drawing.Size(149, 22);
            this.mnuLogAcoes.Text = "Log de ações";
            this.mnuLogAcoes.Click += new System.EventHandler(this.mnuLogAcoes_Click);
            // 
            // sepSistema
            // 
            this.sepSistema.Name = "sepSistema";
            this.sepSistema.Size = new System.Drawing.Size(146, 6);
            // 
            // mnuTrocarUsuario
            // 
            this.mnuTrocarUsuario.Name = "mnuTrocarUsuario";
            this.mnuTrocarUsuario.Size = new System.Drawing.Size(149, 22);
            this.mnuTrocarUsuario.Text = "Trocar usuário";
            this.mnuTrocarUsuario.Click += new System.EventHandler(this.mnuTrocarUsuario_Click);
            // 
            // mnuSair
            // 
            this.mnuSair.Name = "mnuSair";
            this.mnuSair.Size = new System.Drawing.Size(149, 22);
            this.mnuSair.Text = "Sair";
            this.mnuSair.Click += new System.EventHandler(this.mnuSair_Click);
            // 
            // mnuRelatorios
            // 
            this.mnuRelatorios.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRelVendas,
            this.mnuRelEstoque,
            this.mnuRelInsumos,
            this.mnuRelFinanceiro});
            this.mnuRelatorios.Name = "mnuRelatorios";
            this.mnuRelatorios.Size = new System.Drawing.Size(73, 20);
            this.mnuRelatorios.Text = "&Relatórios";
            // 
            // mnuRelVendas
            // 
            this.mnuRelVendas.Name = "mnuRelVendas";
            this.mnuRelVendas.Size = new System.Drawing.Size(200, 22);
            this.mnuRelVendas.Text = "Vendas por período";
            this.mnuRelVendas.Click += new System.EventHandler(this.mnuRelVendas_Click);
            // 
            // mnuRelEstoque
            // 
            this.mnuRelEstoque.Name = "mnuRelEstoque";
            this.mnuRelEstoque.Size = new System.Drawing.Size(200, 22);
            this.mnuRelEstoque.Text = "Estoque";
            this.mnuRelEstoque.Click += new System.EventHandler(this.mnuRelEstoque_Click);
            // 
            // mnuRelInsumos
            // 
            this.mnuRelInsumos.Name = "mnuRelInsumos";
            this.mnuRelInsumos.Size = new System.Drawing.Size(200, 22);
            this.mnuRelInsumos.Text = "Insumos";
            this.mnuRelInsumos.Click += new System.EventHandler(this.mnuRelInsumos_Click);
            // 
            // mnuRelFinanceiro
            // 
            this.mnuRelFinanceiro.Name = "mnuRelFinanceiro";
            this.mnuRelFinanceiro.Size = new System.Drawing.Size(200, 22);
            this.mnuRelFinanceiro.Text = "Financeiro";
            this.mnuRelFinanceiro.Click += new System.EventHandler(this.mnuRelFinanceiro_Click);
            //
            // mnuFinanceiro
            //
            this.mnuFinanceiro.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLancamentos});
            this.mnuFinanceiro.Name = "mnuFinanceiro";
            this.mnuFinanceiro.Size = new System.Drawing.Size(74, 20);
            this.mnuFinanceiro.Text = "&Financeiro";
            //
            // mnuLancamentos
            //
            this.mnuLancamentos.Name = "mnuLancamentos";
            this.mnuLancamentos.Size = new System.Drawing.Size(200, 22);
            this.mnuLancamentos.Text = "Lançamentos";
            this.mnuLancamentos.Click += new System.EventHandler(this.mnuLancamentos_Click);
            //
            // stsPrincipal
            //
            this.stsPrincipal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(246)))), ((int)(((byte)(244)))));
            this.stsPrincipal.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.stsPrincipal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblUsuarioBarra});
            this.stsPrincipal.Location = new System.Drawing.Point(0, 426);
            this.stsPrincipal.Name = "stsPrincipal";
            this.stsPrincipal.Size = new System.Drawing.Size(800, 24);
            this.stsPrincipal.SizingGrip = false;
            this.stsPrincipal.TabIndex = 1;
            //
            // lblUsuarioBarra
            //
            this.lblUsuarioBarra.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblUsuarioBarra.Name = "lblUsuarioBarra";
            this.lblUsuarioBarra.Size = new System.Drawing.Size(785, 19);
            this.lblUsuarioBarra.Spring = true;
            this.lblUsuarioBarra.Text = "Usuário:";
            this.lblUsuarioBarra.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // FrmPrincipal
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.stsPrincipal);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hot-dog Brasília";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmPrincipal_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.stsPrincipal.ResumeLayout(false);
            this.stsPrincipal.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuCadastros;
        private System.Windows.Forms.ToolStripMenuItem mnuClientes;
        private System.Windows.Forms.ToolStripMenuItem mnuProdutos;
        private System.Windows.Forms.ToolStripMenuItem mnuInsumos;
        private System.Windows.Forms.ToolStripMenuItem mnuFornecedores;
        private System.Windows.Forms.ToolStripMenuItem mnuVendas;
        private System.Windows.Forms.ToolStripMenuItem mnuNovaVenda;
        private System.Windows.Forms.ToolStripMenuItem mnuConsultarVendas;
        private System.Windows.Forms.ToolStripMenuItem mnuSistema;
        private System.Windows.Forms.ToolStripMenuItem mnuLogAcoes;
        private System.Windows.Forms.ToolStripSeparator sepSistema;
        private System.Windows.Forms.ToolStripMenuItem mnuTrocarUsuario;
        private System.Windows.Forms.ToolStripMenuItem mnuSair;
        private System.Windows.Forms.ToolStripMenuItem mnuRelatorios;
        private System.Windows.Forms.ToolStripMenuItem mnuRelVendas;
        private System.Windows.Forms.ToolStripMenuItem mnuRelEstoque;
        private System.Windows.Forms.ToolStripMenuItem mnuRelInsumos;
        private System.Windows.Forms.ToolStripMenuItem mnuRelFinanceiro;
        private System.Windows.Forms.ToolStripMenuItem mnuFinanceiro;
        private System.Windows.Forms.ToolStripMenuItem mnuLancamentos;
        private System.Windows.Forms.StatusStrip stsPrincipal;
        private System.Windows.Forms.ToolStripStatusLabel lblUsuarioBarra;
    }
}
