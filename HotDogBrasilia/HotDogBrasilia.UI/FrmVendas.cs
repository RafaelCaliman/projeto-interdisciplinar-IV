using System;
using System.Globalization;
using System.Windows.Forms;
using HotDogBrasilia.Data.Repositories;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    public partial class FrmVendas : Form
    {
        private static readonly CultureInfo PtBr = new CultureInfo("pt-BR");
        private readonly Timer timerStatus = new Timer { Interval = 4000 };
        private Venda vendaSelecionada = null;

        public FrmVendas()
        {
            InitializeComponent();
            timerStatus.Tick += (s, e) =>
            {
                lblStatus.Text = "";
                timerStatus.Stop();
            };
        }

        private void FrmVendas_Load(object sender, EventArgs e)
        {
            dtpInicio.Value = DateTime.Today.AddDays(-30);
            dtpFim.Value = DateTime.Today;
            btnCancelarVenda.Enabled = Sessao.TemPermissao(Permissoes.VendaCancelar);
            CarregarVendas();
        }

        private void CarregarVendas()
        {
            vendaSelecionada = null;
            dgvItens.DataSource = null;

            try
            {
                dgvVendas.DataSource = VendaRepository.listarVendas(dtpInicio.Value, dtpFim.Value);
                if (dgvVendas.Columns.Contains("IdVenda"))
                    dgvVendas.Columns["IdVenda"].HeaderText = "Nº";
                if (dgvVendas.Columns.Contains("DataHora"))
                {
                    dgvVendas.Columns["DataHora"].HeaderText = "Data/hora";
                    dgvVendas.Columns["DataHora"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
                if (dgvVendas.Columns.Contains("NomeCliente"))
                    dgvVendas.Columns["NomeCliente"].HeaderText = "Cliente";
                if (dgvVendas.Columns.Contains("Total"))
                    dgvVendas.Columns["Total"].DefaultCellStyle.Format = "C2";
                dgvVendas.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CarregarItens(int idVenda)
        {
            dgvItens.DataSource = VendaRepository.listarItens(idVenda);
            if (dgvItens.Columns.Contains("IdItem"))
                dgvItens.Columns["IdItem"].Visible = false;
            if (dgvItens.Columns.Contains("IdProduto"))
                dgvItens.Columns["IdProduto"].Visible = false;
            if (dgvItens.Columns.Contains("NomeProduto"))
                dgvItens.Columns["NomeProduto"].HeaderText = "Produto";
            if (dgvItens.Columns.Contains("ValorUnitario"))
            {
                dgvItens.Columns["ValorUnitario"].HeaderText = "Valor unitário";
                dgvItens.Columns["ValorUnitario"].DefaultCellStyle.Format = "C2";
            }
            if (dgvItens.Columns.Contains("Subtotal"))
                dgvItens.Columns["Subtotal"].DefaultCellStyle.Format = "C2";
            dgvItens.ClearSelection();
        }

        private void MostrarStatus(string texto)
        {
            lblStatus.Text = texto;
            timerStatus.Stop();
            timerStatus.Start();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";
            CarregarVendas();
        }

        private void dgvVendas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var v = dgvVendas.Rows[e.RowIndex].DataBoundItem as Venda;
            if (v == null) return;
            vendaSelecionada = v;

            try
            {
                CarregarItens(v.IdVenda);
            }
            catch (Exception ex)
            {
                // Ex.: outra pessoa cancelou esta venda enquanto a tela estava aberta.
                MessageBox.Show(ex.Message, "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CarregarVendas();
            }
        }

        private void btnCancelarVenda_Click(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.VendaCancelar)) return;

            if (vendaSelecionada == null)
            {
                MessageBox.Show("Selecione uma venda na lista.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string resumo = "Venda #" + vendaSelecionada.IdVenda + " - "
                + vendaSelecionada.NomeCliente + " - "
                + vendaSelecionada.Total.ToString("C2", PtBr);

            var resposta = MessageBox.Show("Cancelar a " + resumo + "?\n\n"
                + "Os itens voltam para o estoque e o lançamento financeiro é removido.",
                "Confirmar cancelamento", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (resposta != DialogResult.Yes) return;

            lblStatus.Text = "";

            try
            {
                // O banco recusa venda inexistente (ex.: já cancelada por outra pessoa).
                VendaRepository.cancelarVenda(vendaSelecionada.IdVenda,
                    Sessao.Usuario.IdUser, resumo);
                MostrarStatus("Venda #" + vendaSelecionada.IdVenda + " cancelada.");
                CarregarVendas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CarregarVendas();
            }
        }
    }
}
