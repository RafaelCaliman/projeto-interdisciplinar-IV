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
            this.mnuVendas = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNovaVenda = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuConsultarVendas = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSistema = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTrocarUsuario = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSair = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            //
            // menuStrip1
            //
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCadastros,
            this.mnuVendas,
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
            this.mnuProdutos});
            this.mnuCadastros.Name = "mnuCadastros";
            this.mnuCadastros.Size = new System.Drawing.Size(71, 20);
            this.mnuCadastros.Text = "Cadastros";
            //
            // mnuClientes
            //
            this.mnuClientes.Name = "mnuClientes";
            this.mnuClientes.Size = new System.Drawing.Size(180, 22);
            this.mnuClientes.Text = "Clientes";
            this.mnuClientes.Click += new System.EventHandler(this.mnuClientes_Click);
            //
            // mnuProdutos
            //
            this.mnuProdutos.Name = "mnuProdutos";
            this.mnuProdutos.Size = new System.Drawing.Size(180, 22);
            this.mnuProdutos.Text = "Produtos";
            this.mnuProdutos.Click += new System.EventHandler(this.mnuProdutos_Click);
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
            this.mnuNovaVenda.Size = new System.Drawing.Size(180, 22);
            this.mnuNovaVenda.Text = "Nova venda";
            this.mnuNovaVenda.Click += new System.EventHandler(this.mnuNovaVenda_Click);
            //
            // mnuConsultarVendas
            //
            this.mnuConsultarVendas.Name = "mnuConsultarVendas";
            this.mnuConsultarVendas.Size = new System.Drawing.Size(180, 22);
            this.mnuConsultarVendas.Text = "Consultar vendas";
            this.mnuConsultarVendas.Click += new System.EventHandler(this.mnuConsultarVendas_Click);
            //
            // mnuSistema
            //
            this.mnuSistema.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuTrocarUsuario,
            this.mnuSair});
            this.mnuSistema.Name = "mnuSistema";
            this.mnuSistema.Size = new System.Drawing.Size(66, 21);
            this.mnuSistema.Text = "Sistema";
            //
            // mnuTrocarUsuario
            //
            this.mnuTrocarUsuario.Name = "mnuTrocarUsuario";
            this.mnuTrocarUsuario.Size = new System.Drawing.Size(180, 22);
            this.mnuTrocarUsuario.Text = "Trocar usuário";
            this.mnuTrocarUsuario.Click += new System.EventHandler(this.mnuTrocarUsuario_Click);
            //
            // mnuSair
            //
            this.mnuSair.Name = "mnuSair";
            this.mnuSair.Size = new System.Drawing.Size(180, 22);
            this.mnuSair.Text = "Sair";
            this.mnuSair.Click += new System.EventHandler(this.mnuSair_Click);
            //
            // FrmPrincipal
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuCadastros;
        private System.Windows.Forms.ToolStripMenuItem mnuClientes;
        private System.Windows.Forms.ToolStripMenuItem mnuProdutos;
        private System.Windows.Forms.ToolStripMenuItem mnuVendas;
        private System.Windows.Forms.ToolStripMenuItem mnuNovaVenda;
        private System.Windows.Forms.ToolStripMenuItem mnuConsultarVendas;
        private System.Windows.Forms.ToolStripMenuItem mnuSistema;
        private System.Windows.Forms.ToolStripMenuItem mnuTrocarUsuario;
        private System.Windows.Forms.ToolStripMenuItem mnuSair;
    }
}
