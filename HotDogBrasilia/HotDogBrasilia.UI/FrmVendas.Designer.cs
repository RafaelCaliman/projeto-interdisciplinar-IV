namespace HotDogBrasilia.UI
{
    partial class FrmVendas
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
            this.lblInicio = new System.Windows.Forms.Label();
            this.dtpInicio = new System.Windows.Forms.DateTimePicker();
            this.lblFim = new System.Windows.Forms.Label();
            this.dtpFim = new System.Windows.Forms.DateTimePicker();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.dgvVendas = new System.Windows.Forms.DataGridView();
            this.lblItens = new System.Windows.Forms.Label();
            this.dgvItens = new System.Windows.Forms.DataGridView();
            this.btnCancelarVenda = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVendas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItens)).BeginInit();
            this.SuspendLayout();
            //
            // lblInicio
            //
            this.lblInicio.AutoSize = true;
            this.lblInicio.Location = new System.Drawing.Point(20, 15);
            this.lblInicio.Name = "lblInicio";
            this.lblInicio.Size = new System.Drawing.Size(27, 19);
            this.lblInicio.TabIndex = 0;
            this.lblInicio.Text = "De:";
            //
            // dtpInicio
            //
            this.dtpInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpInicio.Location = new System.Drawing.Point(20, 37);
            this.dtpInicio.Name = "dtpInicio";
            this.dtpInicio.Size = new System.Drawing.Size(130, 23);
            this.dtpInicio.TabIndex = 1;
            //
            // lblFim
            //
            this.lblFim.AutoSize = true;
            this.lblFim.Location = new System.Drawing.Point(165, 15);
            this.lblFim.Name = "lblFim";
            this.lblFim.Size = new System.Drawing.Size(32, 19);
            this.lblFim.TabIndex = 2;
            this.lblFim.Text = "Até:";
            //
            // dtpFim
            //
            this.dtpFim.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFim.Location = new System.Drawing.Point(165, 37);
            this.dtpFim.Name = "dtpFim";
            this.dtpFim.Size = new System.Drawing.Size(130, 23);
            this.dtpFim.TabIndex = 3;
            //
            // btnBuscar
            //
            this.btnBuscar.Location = new System.Drawing.Point(310, 34);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(90, 30);
            this.btnBuscar.TabIndex = 4;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            //
            // dgvVendas
            //
            this.dgvVendas.AllowUserToAddRows = false;
            this.dgvVendas.AllowUserToDeleteRows = false;
            this.dgvVendas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvVendas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVendas.Location = new System.Drawing.Point(20, 80);
            this.dgvVendas.MultiSelect = false;
            this.dgvVendas.Name = "dgvVendas";
            this.dgvVendas.ReadOnly = true;
            this.dgvVendas.RowHeadersVisible = false;
            this.dgvVendas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVendas.Size = new System.Drawing.Size(720, 190);
            this.dgvVendas.TabIndex = 5;
            this.dgvVendas.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVendas_CellClick);
            //
            // lblItens
            //
            this.lblItens.AutoSize = true;
            this.lblItens.Location = new System.Drawing.Point(20, 282);
            this.lblItens.Name = "lblItens";
            this.lblItens.Size = new System.Drawing.Size(178, 19);
            this.lblItens.TabIndex = 6;
            this.lblItens.Text = "Itens da venda selecionada:";
            //
            // dgvItens
            //
            this.dgvItens.AllowUserToAddRows = false;
            this.dgvItens.AllowUserToDeleteRows = false;
            this.dgvItens.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvItens.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItens.Location = new System.Drawing.Point(20, 304);
            this.dgvItens.MultiSelect = false;
            this.dgvItens.Name = "dgvItens";
            this.dgvItens.ReadOnly = true;
            this.dgvItens.RowHeadersVisible = false;
            this.dgvItens.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvItens.Size = new System.Drawing.Size(720, 130);
            this.dgvItens.TabIndex = 7;
            //
            // btnCancelarVenda
            //
            this.btnCancelarVenda.Location = new System.Drawing.Point(20, 445);
            this.btnCancelarVenda.Name = "btnCancelarVenda";
            this.btnCancelarVenda.Size = new System.Drawing.Size(140, 32);
            this.btnCancelarVenda.TabIndex = 8;
            this.btnCancelarVenda.Text = "Cancelar venda";
            this.btnCancelarVenda.UseVisualStyleBackColor = true;
            this.btnCancelarVenda.Click += new System.EventHandler(this.btnCancelarVenda_Click);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.ForestGreen;
            this.lblStatus.Location = new System.Drawing.Point(175, 452);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 19);
            this.lblStatus.TabIndex = 9;
            //
            // FrmVendas
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 495);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnCancelarVenda);
            this.Controls.Add(this.dgvItens);
            this.Controls.Add(this.lblItens);
            this.Controls.Add(this.dgvVendas);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.dtpFim);
            this.Controls.Add(this.lblFim);
            this.Controls.Add(this.dtpInicio);
            this.Controls.Add(this.lblInicio);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmVendas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hot-dog Brasília - Consultar vendas";
            this.Load += new System.EventHandler(this.FrmVendas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVendas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItens)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInicio;
        private System.Windows.Forms.DateTimePicker dtpInicio;
        private System.Windows.Forms.Label lblFim;
        private System.Windows.Forms.DateTimePicker dtpFim;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.DataGridView dgvVendas;
        private System.Windows.Forms.Label lblItens;
        private System.Windows.Forms.DataGridView dgvItens;
        private System.Windows.Forms.Button btnCancelarVenda;
        private System.Windows.Forms.Label lblStatus;
    }
}
