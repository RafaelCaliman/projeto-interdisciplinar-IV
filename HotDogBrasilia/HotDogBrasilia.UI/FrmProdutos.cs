using System;
using System.Globalization;
using System.Windows.Forms;
using HotDogBrasilia.Data.Repositories;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    public partial class FrmProdutos : Form
    {
        private int idSelecionado = 0;
        private static readonly CultureInfo PtBr = new CultureInfo("pt-BR");
        private readonly Timer timerStatus = new Timer { Interval = 4000 };

        public FrmProdutos()
        {
            InitializeComponent();
            timerStatus.Tick += (s, e) =>
            {
                lblStatus.Text = "";
                timerStatus.Stop();
            };
        }

        private void FrmProdutos_Load(object sender, EventArgs e)
        {
            AplicarPermissoes();

            try
            {
                cboFornecedor.DisplayMember = "Nome";
                cboFornecedor.ValueMember = "IdFornecedor";
                cboFornecedor.DataSource = FornecedorRepository.listarFornecedores();
                cboFornecedor.SelectedIndex = -1;
                CarregarGrade();
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
            }
        }

        // Desliga os botões que o usuário logado não pode usar.
        private void AplicarPermissoes()
        {
            bool podeGravar = Sessao.TemPermissao(Permissoes.ProdutoGravar);
            btnNovo.Enabled = podeGravar;
            btnSalvar.Enabled = podeGravar;
            btnExcluir.Enabled = Sessao.TemPermissao(Permissoes.ProdutoExcluir);
        }

        private void CarregarGrade()
        {
            dgvProdutos.DataSource = ProdutoRepository.listarProdutos(txtBusca.Text);
            if (dgvProdutos.Columns.Contains("IdProduto"))
                dgvProdutos.Columns["IdProduto"].Visible = false;
            if (dgvProdutos.Columns.Contains("IdFornecedor"))
                dgvProdutos.Columns["IdFornecedor"].Visible = false;
            if (dgvProdutos.Columns.Contains("NomeFornecedor"))
                dgvProdutos.Columns["NomeFornecedor"].HeaderText = "Fornecedor";
            if (dgvProdutos.Columns.Contains("Valor"))
                dgvProdutos.Columns["Valor"].DefaultCellStyle.Format = "C2";
            dgvProdutos.ClearSelection();
        }

        private void MostrarStatus(string texto)
        {
            lblStatus.Text = texto;
            timerStatus.Stop();
            timerStatus.Start();
        }

        private void Limpar()
        {
            idSelecionado = 0;
            txtNome.Clear();
            txtValor.Clear();
            txtEstoque.Clear();
            cboFornecedor.SelectedIndex = -1;
            txtNome.Focus();
        }

        private decimal LerValor()
        {
            decimal v;
            if (!decimal.TryParse(txtValor.Text,
                    NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, PtBr, out v))
                throw new Exception(
                    "Valor unitário inválido: deve ser informado e não pode ser negativo.");
            return v;
        }

        private int LerEstoque()
        {
            int n;
            if (!int.TryParse(txtEstoque.Text, out n))
                throw new Exception(
                    "Estoque inválido: deve ser informado e não pode ser negativo.");
            return n;
        }

        private void txtBusca_TextChanged(object sender, EventArgs e)
        {
            try
            {
                CarregarGrade();
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
            }
        }

        private void dgvProdutos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var p = dgvProdutos.Rows[e.RowIndex].DataBoundItem as Produto;
            if (p == null) return;
            idSelecionado = p.IdProduto;
            txtNome.Text = p.Nome;
            txtValor.Text = p.Valor.ToString("N2", PtBr);
            txtEstoque.Text = p.Estoque.ToString();
            cboFornecedor.SelectedValue = p.IdFornecedor;
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            Limpar();
            lblStatus.Text = "";
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.ProdutoGravar)) return;
            lblStatus.Text = "";

            try
            {
                decimal valor = LerValor();
                int estoque = LerEstoque();
                int? idFornecedor = cboFornecedor.SelectedValue == null
                    ? (int?)null : (int)cboFornecedor.SelectedValue;

                if (idSelecionado == 0)
                    ProdutoRepository.inserirProduto(txtNome.Text, valor, estoque, idFornecedor);
                else
                    ProdutoRepository.atualizarProduto(idSelecionado, txtNome.Text,
                        valor, estoque, idFornecedor);

                MostrarStatus("Produto salvo com sucesso.");
                Limpar();
                CarregarGrade();
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
            }
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.ProdutoExcluir)) return;

            if (idSelecionado == 0)
            {
                MessageBox.Show("Selecione um produto na lista.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var resposta = MessageBox.Show("Deseja realmente excluir este produto?",
                "Confirmar exclusão", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (resposta != DialogResult.Yes) return;

            lblStatus.Text = "";

            try
            {
                ProdutoRepository.excluirProduto(idSelecionado);
                MostrarStatus("Produto excluído.");
                Limpar();
                CarregarGrade();
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
            }
        }
    }
}
