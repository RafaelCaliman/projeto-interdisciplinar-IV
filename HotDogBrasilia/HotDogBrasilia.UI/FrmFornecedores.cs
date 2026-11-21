using System;
using System.Drawing;
using System.Windows.Forms;
using HotDogBrasilia.Data.Repositories;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    public partial class FrmFornecedores : Form
    {
        private int idSelecionado = 0;
        private string nomeSelecionado = "";
        private bool podeGravar;
        private DateTime ultimaGravacao = DateTime.MinValue;
        private readonly Timer timerStatus = new Timer { Interval = 4000 };

        public FrmFornecedores()
        {
            InitializeComponent();
            Tema.Grade(dgvFornecedores);
            Tema.BotaoPrimario(btnSalvar);
            Tema.BotaoSecundario(btnNovo);
            Tema.BotaoNeutro(btnExcluir);
            timerStatus.Tick += (s, e) =>
            {
                lblStatus.Text = "";
                timerStatus.Stop();
            };
            dgvFornecedores.Resize += (s, e) => PosicionarVazio();
            FormClosed += (s, e) => timerStatus.Dispose();
        }

        private void FrmFornecedores_Load(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.FornecedorVer))
            {
                BeginInvoke(new Action(Close));
                return;
            }

            Tema.Dica(txtBusca, "parte do nome do fornecedor");
            AplicarPermissoes();
            CarregarGrade(null);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (AtalhosCadastro.Processar(this, keyData, btnNovo, btnSalvar, btnExcluir,
                    () => CarregarGrade(idSelecionado == 0 ? (int?)null : idSelecionado), txtNome))
                return true;
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void FrmFornecedores_Shown(object sender, EventArgs e)
        {
            txtNome.Focus();
        }

        private void AplicarPermissoes()
        {
            podeGravar = Sessao.TemPermissao(Permissoes.FornecedorGravar);
            btnNovo.Enabled = podeGravar;
            btnSalvar.Enabled = podeGravar;
            btnExcluir.Enabled = podeGravar;
            txtNome.ReadOnly = !podeGravar;
        }

        private void CarregarGrade(int? idSelecionar)
        {
            UseWaitCursor = true;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                dgvFornecedores.DataSource = FornecedorRepository.pesquisar(txtBusca.Text);
                if (dgvFornecedores.Columns.Contains("IdFornecedor"))
                    dgvFornecedores.Columns["IdFornecedor"].Visible = false;
                if (dgvFornecedores.Columns.Contains("QtdProdutos"))
                {
                    var c = dgvFornecedores.Columns["QtdProdutos"];
                    c.HeaderText = "Produtos";
                    c.FillWeight = 30;
                    c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                dgvFornecedores.ClearSelection();
                if (idSelecionar.HasValue)
                    Selecionar(idSelecionar.Value);
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
            }
            finally
            {
                UseWaitCursor = false;
                AtualizarVazio();
            }
        }

        private void Selecionar(int idFornecedor)
        {
            foreach (DataGridViewRow r in dgvFornecedores.Rows)
            {
                var f = r.DataBoundItem as Fornecedor;
                if (f == null || f.IdFornecedor != idFornecedor) continue;
                r.Selected = true;
                dgvFornecedores.CurrentCell = r.Cells["Nome"];
                idSelecionado = f.IdFornecedor;
                nomeSelecionado = f.Nome;
                txtNome.Text = f.Nome;
                return;
            }
        }

        private void AtualizarVazio()
        {
            lblVazio.Visible = dgvFornecedores.Rows.Count == 0;
            PosicionarVazio();
        }

        private void PosicionarVazio()
        {
            lblVazio.Bounds = new Rectangle(dgvFornecedores.Left + 1,
                dgvFornecedores.Top + dgvFornecedores.ColumnHeadersHeight + 1,
                Math.Max(0, dgvFornecedores.Width - 2), 50);
            lblVazio.BringToFront();
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
            nomeSelecionado = "";
            txtNome.Clear();
            dgvFornecedores.ClearSelection();
            txtNome.Focus();
        }

        private static string Detalhe(string nome)
        {
            string d = "nome=" + nome;
            return d.Length > 255 ? d.Substring(0, 255) : d;
        }

        private void txtBusca_TextChanged(object sender, EventArgs e)
        {
            idSelecionado = 0;
            nomeSelecionado = "";
            txtNome.Clear();
            CarregarGrade(null);
        }

        private void dgvFornecedores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var f = dgvFornecedores.Rows[e.RowIndex].DataBoundItem as Fornecedor;
            if (f == null) return;
            idSelecionado = f.IdFornecedor;
            nomeSelecionado = f.Nome;
            txtNome.Text = f.Nome;
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.FornecedorGravar)) return;
            Limpar();
            lblStatus.Text = "";
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.FornecedorGravar)) return;
            if ((DateTime.Now - ultimaGravacao).TotalMilliseconds < 800) return;
            lblStatus.Text = "";

            string nome = txtNome.Text;
            btnSalvar.Enabled = false;
            UseWaitCursor = true;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                int id;
                if (idSelecionado == 0)
                {
                    id = FornecedorRepository.inserir(nome);
                    Auditoria.Registrar("INSERIR", "fornecedor", id, Detalhe(nome.Trim()));
                }
                else
                {
                    id = idSelecionado;
                    FornecedorRepository.atualizar(id, nome);
                    Auditoria.Registrar("ATUALIZAR", "fornecedor", id, Detalhe(nome.Trim()));
                }

                idSelecionado = id;
                nomeSelecionado = nome.Trim();
                MostrarStatus("Fornecedor salvo com sucesso.");
                CarregarGrade(id);
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
                txtNome.Focus();
            }
            finally
            {
                ultimaGravacao = DateTime.Now;
                UseWaitCursor = false;
                btnSalvar.Enabled = podeGravar;
            }
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.FornecedorGravar)) return;

            if (idSelecionado == 0)
            {
                MessageBox.Show("Selecione um fornecedor na lista.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var resposta = MessageBox.Show("Deseja realmente excluir este fornecedor?",
                "Confirmar exclusão", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (resposta != DialogResult.Yes) return;

            lblStatus.Text = "";
            int id = idSelecionado;
            string nome = nomeSelecionado;

            try
            {
                FornecedorRepository.excluir(id);
                Auditoria.Registrar("EXCLUIR", "fornecedor", id, Detalhe(nome));
                MostrarStatus("Fornecedor excluído com sucesso.");
                Limpar();
                CarregarGrade(null);
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
            }
        }
    }
}
