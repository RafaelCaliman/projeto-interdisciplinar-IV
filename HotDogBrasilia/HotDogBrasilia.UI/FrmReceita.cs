using System;
using System.Drawing;
using System.Windows.Forms;
using HotDogBrasilia.Data.Repositories;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    public partial class FrmReceita : Form
    {
        private readonly int idProduto;
        private readonly string nomeProduto;
        private readonly Timer timerStatus = new Timer { Interval = 4000 };

        public bool Alterou { get; private set; }

        public FrmReceita(int idProduto, string nomeProduto)
        {
            InitializeComponent();
            this.idProduto = idProduto;
            this.nomeProduto = nomeProduto;
            Text = "Receita - " + nomeProduto;
            lblProduto.Text = "Receita de \"" + nomeProduto + "\" (quanto de cada insumo vai em 1 unidade):";
            timerStatus.Tick += (s, e) =>
            {
                lblStatus.Text = "";
                timerStatus.Stop();
            };
            dgvReceita.Resize += (s, e) => PosicionarVazio();
            FormClosed += (s, e) => timerStatus.Dispose();
        }

        private void FrmReceita_Load(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.ProdutoGravar))
            {
                BeginInvoke(new Action(Close));
                return;
            }

            try
            {
                cboInsumo.DisplayMember = "Nome";
                cboInsumo.ValueMember = "IdInsumo";
                cboInsumo.DataSource = InsumoRepository.listar(null);
                cboInsumo.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
            }
            CarregarGrade();
        }

        private void CarregarGrade()
        {
            UseWaitCursor = true;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                dgvReceita.DataSource = InsumoRepository.listarReceita(idProduto);
                if (dgvReceita.Columns.Contains("IdInsumo"))
                    dgvReceita.Columns["IdInsumo"].Visible = false;
                if (dgvReceita.Columns.Contains("Estoque"))
                    dgvReceita.Columns["Estoque"].HeaderText = "Estoque do insumo";
                foreach (var nome in new[] { "Quantidade", "Estoque" })
                {
                    if (!dgvReceita.Columns.Contains(nome)) continue;
                    var c = dgvReceita.Columns[nome];
                    c.FillWeight = 45;
                    c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                dgvReceita.ClearSelection();
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
            }
            finally
            {
                UseWaitCursor = false;
                lblVazio.Visible = dgvReceita.Rows.Count == 0;
                PosicionarVazio();
            }
        }

        private void PosicionarVazio()
        {
            lblVazio.Bounds = new Rectangle(dgvReceita.Left + 1,
                dgvReceita.Top + dgvReceita.ColumnHeadersHeight + 1,
                Math.Max(0, dgvReceita.Width - 2), 50);
            lblVazio.BringToFront();
        }

        private void MostrarStatus(string texto)
        {
            lblStatus.Text = texto;
            timerStatus.Stop();
            timerStatus.Start();
        }

        private static string Detalhe(string texto)
        {
            return texto.Length > 255 ? texto.Substring(0, 255) : texto;
        }

        private void dgvReceita_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var item = dgvReceita.Rows[e.RowIndex].DataBoundItem as ItemReceita;
            if (item == null) return;
            cboInsumo.SelectedValue = item.IdInsumo;
            nudQuantidade.Value = Math.Max(1, Math.Min(item.Quantidade, (int)nudQuantidade.Maximum));
        }

        private void btnDefinir_Click(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.ProdutoGravar)) return;

            var insumo = cboInsumo.SelectedItem as Insumo;
            if (insumo == null)
            {
                MessageBox.Show("Selecione um insumo.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                cboInsumo.Focus();
                return;
            }

            int quantidade = (int)nudQuantidade.Value;
            lblStatus.Text = "";
            btnDefinir.Enabled = false;
            try
            {
                InsumoRepository.definirReceita(idProduto, insumo.IdInsumo, quantidade);
                Auditoria.Registrar("RECEITA_DEFINIR", "produto", idProduto,
                    Detalhe("produto=" + nomeProduto + "; insumo=" + insumo.Nome
                        + "; quantidade=" + quantidade));
                Alterou = true;
                MostrarStatus(insumo.Nome + ": " + quantidade + " por unidade.");
                CarregarGrade();
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
            }
            finally
            {
                btnDefinir.Enabled = true;
            }
        }

        private void btnRemover_Click(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.ProdutoGravar)) return;

            var item = dgvReceita.SelectedRows.Count > 0
                ? dgvReceita.SelectedRows[0].DataBoundItem as ItemReceita : null;
            if (item == null)
            {
                MessageBox.Show("Selecione um insumo da receita na lista.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string aviso = dgvReceita.Rows.Count == 1
                ? "\n\nÉ o último insumo da receita: o produto volta a usar o estoque próprio, "
                  + "que pode estar desatualizado. Confira o estoque do produto depois."
                : "";
            var resposta = MessageBox.Show("Remover \"" + item.Insumo + "\" da receita?" + aviso,
                "Confirmar remoção", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (resposta != DialogResult.Yes) return;

            lblStatus.Text = "";
            try
            {
                InsumoRepository.removerReceita(idProduto, item.IdInsumo);
                Auditoria.Registrar("RECEITA_REMOVER", "produto", idProduto,
                    Detalhe("produto=" + nomeProduto + "; insumo=" + item.Insumo));
                Alterou = true;
                MostrarStatus(item.Insumo + " removido da receita.");
                CarregarGrade();
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
            }
        }
    }
}
