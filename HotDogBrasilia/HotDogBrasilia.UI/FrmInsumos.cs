using System;
using System.Drawing;
using System.Windows.Forms;
using HotDogBrasilia.Data.Repositories;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    public partial class FrmInsumos : Form
    {
        private int idSelecionado = 0;
        private string nomeSelecionado = "";
        private int estoqueSelecionado = 0;
        private bool podeGravar;
        private bool podeExcluir;
        private DateTime ultimaGravacao = DateTime.MinValue;
        private readonly Timer timerStatus = new Timer { Interval = 4000 };

        public FrmInsumos()
        {
            InitializeComponent();
            Tema.Grade(dgvInsumos);
            Tema.BotaoPrimario(btnSalvar);
            Tema.BotaoSecundario(btnNovo);
            Tema.BotaoSecundario(btnEntrada);
            Tema.BotaoSecundario(btnAjustar);
            Tema.BotaoNeutro(btnExcluir);
            timerStatus.Tick += (s, e) =>
            {
                lblStatus.Text = "";
                timerStatus.Stop();
            };
            dgvInsumos.Resize += (s, e) => PosicionarVazio();
            FormClosed += (s, e) => timerStatus.Dispose();
        }

        private void FrmInsumos_Load(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.ProdutoVer))
            {
                BeginInvoke(new Action(Close));
                return;
            }

            Tema.Dica(txtBusca, "parte do nome do insumo");
            AplicarPermissoes();
            CarregarGrade(null);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (AtalhosCadastro.Processar(this, keyData, btnNovo, btnSalvar, btnExcluir,
                    () => CarregarGrade(idSelecionado == 0 ? (int?)null : idSelecionado), txtNome, nudEstoque))
                return true;
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void FrmInsumos_Shown(object sender, EventArgs e)
        {
            txtNome.Focus();
        }

        private void AplicarPermissoes()
        {
            podeGravar = Sessao.TemPermissao(Permissoes.ProdutoGravar);
            podeExcluir = Sessao.TemPermissao(Permissoes.ProdutoExcluir);
            btnNovo.Enabled = podeGravar;
            btnSalvar.Enabled = podeGravar;
            btnEntrada.Enabled = podeGravar;
            btnAjustar.Enabled = podeGravar;
            btnExcluir.Enabled = podeExcluir;
            txtNome.ReadOnly = !podeGravar;
            AtualizarCampoEstoque();
        }

        private void AtualizarCampoEstoque()
        {
            nudEstoque.Enabled = podeGravar && idSelecionado == 0;
            lblEstoque.Text = idSelecionado == 0 ? "&Estoque inicial:" : "&Estoque atual:";
        }

        private void CarregarGrade(int? idSelecionar)
        {
            UseWaitCursor = true;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                dgvInsumos.DataSource = InsumoRepository.listar(txtBusca.Text);
                if (dgvInsumos.Columns.Contains("IdInsumo"))
                    dgvInsumos.Columns["IdInsumo"].Visible = false;
                if (dgvInsumos.Columns.Contains("Estoque"))
                {
                    var c = dgvInsumos.Columns["Estoque"];
                    c.FillWeight = 35;
                    c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                dgvInsumos.ClearSelection();
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

        private void Preencher(Insumo i)
        {
            idSelecionado = i.IdInsumo;
            nomeSelecionado = i.Nome;
            estoqueSelecionado = i.Estoque;
            txtNome.Text = i.Nome;
            nudEstoque.Value = Math.Min(i.Estoque, (int)nudEstoque.Maximum);
            AtualizarCampoEstoque();
        }

        private void Selecionar(int idInsumo)
        {
            foreach (DataGridViewRow r in dgvInsumos.Rows)
            {
                var i = r.DataBoundItem as Insumo;
                if (i == null || i.IdInsumo != idInsumo) continue;
                r.Selected = true;
                dgvInsumos.CurrentCell = r.Cells["Nome"];
                Preencher(i);
                return;
            }
        }

        private void AtualizarVazio()
        {
            lblVazio.Visible = dgvInsumos.Rows.Count == 0;
            PosicionarVazio();
        }

        private void PosicionarVazio()
        {
            lblVazio.Bounds = new Rectangle(dgvInsumos.Left + 1,
                dgvInsumos.Top + dgvInsumos.ColumnHeadersHeight + 1,
                Math.Max(0, dgvInsumos.Width - 2), 50);
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
            estoqueSelecionado = 0;
            txtNome.Clear();
            nudEstoque.Value = 0;
            dgvInsumos.ClearSelection();
            AtualizarCampoEstoque();
            txtNome.Focus();
        }

        private static string Detalhe(string texto)
        {
            return texto.Length > 255 ? texto.Substring(0, 255) : texto;
        }

        private bool TemSelecionado()
        {
            if (idSelecionado != 0) return true;
            MessageBox.Show("Selecione um insumo na lista.", "Atenção",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        private void txtBusca_TextChanged(object sender, EventArgs e)
        {
            Limpar();
            txtBusca.Focus();
            CarregarGrade(null);
        }

        private void dgvInsumos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var i = dgvInsumos.Rows[e.RowIndex].DataBoundItem as Insumo;
            if (i == null) return;
            Preencher(i);
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.ProdutoGravar)) return;
            Limpar();
            lblStatus.Text = "";
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.ProdutoGravar)) return;
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
                    int estoque = (int)nudEstoque.Value;
                    id = InsumoRepository.inserir(nome, estoque);
                    Auditoria.Registrar("INSUMO_INSERIR", "insumo", id,
                        Detalhe("nome=" + nome.Trim() + "; estoque=" + estoque));
                }
                else
                {
                    id = idSelecionado;
                    InsumoRepository.atualizar(id, nome);
                    Auditoria.Registrar("INSUMO_ATUALIZAR", "insumo", id,
                        Detalhe("nome=" + nome.Trim()));
                }

                idSelecionado = id;
                MostrarStatus("Insumo salvo com sucesso.");
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

        private void btnEntrada_Click(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.ProdutoGravar)) return;
            if (!TemSelecionado()) return;

            int quantidade;
            using (var f = new FrmQuantidade("Registrar entrada",
                "Quantidade que entrou de \"" + nomeSelecionado + "\" (estoque atual: "
                + estoqueSelecionado + "):", 1, 1))
            {
                if (f.ShowDialog(this) != DialogResult.OK) return;
                quantidade = f.Valor;
            }

            int id = idSelecionado;
            Gravar(btnEntrada, () =>
            {
                InsumoRepository.entrada(id, quantidade);
                Auditoria.Registrar("INSUMO_ENTRADA", "insumo", id,
                    Detalhe("nome=" + nomeSelecionado + "; quantidade=" + quantidade
                        + "; antes=" + estoqueSelecionado));
                MostrarStatus("Entrada de " + quantidade + " registrada em " + nomeSelecionado + ".");
            }, id);
        }

        private void btnAjustar_Click(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.ProdutoGravar)) return;
            if (!TemSelecionado()) return;

            int novoEstoque;
            using (var f = new FrmQuantidade("Ajustar contagem",
                "Estoque contado de \"" + nomeSelecionado + "\" (no sistema: "
                + estoqueSelecionado + "):", 0, estoqueSelecionado))
            {
                if (f.ShowDialog(this) != DialogResult.OK) return;
                novoEstoque = f.Valor;
            }

            int id = idSelecionado;
            Gravar(btnAjustar, () =>
            {
                InsumoRepository.ajustar(id, novoEstoque);
                Auditoria.Registrar("INSUMO_AJUSTAR", "insumo", id,
                    Detalhe("nome=" + nomeSelecionado + "; de=" + estoqueSelecionado
                        + "; para=" + novoEstoque));
                MostrarStatus("Estoque de " + nomeSelecionado + " ajustado para " + novoEstoque + ".");
            }, id);
        }

        private void Gravar(Button botao, Action acao, int idSelecionar)
        {
            lblStatus.Text = "";
            botao.Enabled = false;
            UseWaitCursor = true;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                acao();
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
            }
            finally
            {
                UseWaitCursor = false;
                botao.Enabled = podeGravar;
            }
            CarregarGrade(idSelecionar);
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.ProdutoExcluir)) return;
            if (!TemSelecionado()) return;

            var resposta = MessageBox.Show("Deseja realmente excluir este insumo?",
                "Confirmar exclusão", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (resposta != DialogResult.Yes) return;

            lblStatus.Text = "";
            int id = idSelecionado;
            string nome = nomeSelecionado;

            try
            {
                InsumoRepository.excluir(id);
                Auditoria.Registrar("INSUMO_EXCLUIR", "insumo", id, Detalhe("nome=" + nome));
                MostrarStatus("Insumo excluído com sucesso.");
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
