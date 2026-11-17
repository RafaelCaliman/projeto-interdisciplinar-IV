using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using HotDogBrasilia.Data.Repositories;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    // RF14: estoque por produto, com situação OK, BAIXO ou ESGOTADO.
    public partial class FrmRelatorioEstoque : Form
    {
        private const int LimitePadrao = 10;
        private static readonly CultureInfo PtBr = new CultureInfo("pt-BR");
        private readonly Timer timerStatus = new Timer { Interval = 5000 };
        private readonly Font negrito = new Font("Segoe UI", 10F, FontStyle.Bold);
        private DataTable tabela = CriarTabela();
        private string filtrosGerados = "Estoque baixo até " + LimitePadrao + " unidades";

        public FrmRelatorioEstoque()
        {
            InitializeComponent();
            timerStatus.Tick += (s, e) => { timerStatus.Stop(); lblStatus.Text = ""; };
            Tema.Grade(dgvEstoque);
            dgvEstoque.Resize += (s, e) => PosicionarVazio();
            dgvEstoque.CellFormatting += dgvEstoque_CellFormatting;
            FormClosed += (s, e) => { negrito.Dispose(); timerStatus.Dispose(); };
        }

        private void FrmRelatorioEstoque_Load(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.RelatorioVer))
            {
                BeginInvoke(new Action(Close));
                return;
            }

            Tema.Dica(txtLimite, "padrão: " + LimitePadrao);
            dgvEstoque.DataSource = tabela;
            FormatarGrade();
            ActiveControl = txtLimite;
            Gerar();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.F5: Gerar(); return true;
                case Keys.Control | Keys.E: btnExportar.PerformClick(); return true;
                case Keys.Control | Keys.P: btnImprimir.PerformClick(); return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private static DataTable CriarTabela()
        {
            var t = new DataTable("Estoque");
            t.Columns.Add("Produto", typeof(string));
            t.Columns.Add("Estoque", typeof(int));
            t.Columns.Add("Fornecedor", typeof(string));
            t.Columns.Add("Situação", typeof(string));
            return t;
        }

        private void FormatarGrade()
        {
            var g = dgvEstoque;
            g.Columns["Produto"].FillWeight = 130;
            g.Columns["Estoque"].FillWeight = 50;
            g.Columns["Estoque"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            g.Columns["Estoque"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            g.Columns["Fornecedor"].FillWeight = 110;
            g.Columns["Situação"].FillWeight = 60;
            g.Columns["Situação"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns["Situação"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // A cor só reforça: a situação sempre aparece escrita (acessibilidade).
        private void dgvEstoque_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvEstoque.Columns[e.ColumnIndex].Name != "Situação") return;
            string s = Convert.ToString(e.Value);
            if (s == "BAIXO")
            {
                e.CellStyle.BackColor = Tema.AmbarFundo;
                e.CellStyle.ForeColor = Color.FromArgb(120, 70, 0);
                e.CellStyle.Font = negrito;
            }
            else if (s == "ESGOTADO")
            {
                e.CellStyle.BackColor = Tema.VermelhoFundo;
                e.CellStyle.ForeColor = Tema.Erro;
                e.CellStyle.Font = negrito;
            }
        }

        // Só aceita dígitos (e as teclas de edição, como Backspace).
        private void txtLimite_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void btnGerar_Click(object sender, EventArgs e)
        {
            Gerar();
        }

        private void Gerar()
        {
            int? limite = null;
            string texto = txtLimite.Text.Trim();
            if (texto.Length > 0)
            {
                int valor;
                if (!int.TryParse(texto, out valor) || valor < 0)
                {
                    MessageBox.Show("Informe um número inteiro de unidades (ex.: 10) ou deixe em branco.",
                        "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLimite.Focus();
                    txtLimite.SelectAll();
                    return;
                }
                limite = valor;
            }

            btnGerar.Enabled = false;
            UseWaitCursor = true;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                var linhas = RelatorioRepository.listarEstoque(limite);

                tabela.Rows.Clear();
                foreach (var l in linhas)
                    tabela.Rows.Add(l.Produto, l.Estoque, l.Fornecedor, l.Situacao);

                cardProdutos.Valor = linhas.Count.ToString(PtBr);
                cardBaixo.Valor = linhas.Count(l => l.Situacao == "BAIXO").ToString(PtBr);
                cardEsgotado.Valor = linhas.Count(l => l.Situacao == "ESGOTADO").ToString(PtBr);

                filtrosGerados = "Estoque baixo até " + (limite ?? LimitePadrao) + " unidades";
                AtualizarVazio();
                dgvEstoque.ClearSelection();

                if (linhas.Count > 0)
                    MostrarStatus(linhas.Count + (linhas.Count == 1 ? " produto listado." : " produtos listados."), false);
                else
                    lblStatus.Text = "";
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
            }
            finally
            {
                UseWaitCursor = false;
                btnGerar.Enabled = true;
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            txtLimite.Clear();
            Gerar();
            txtLimite.Focus();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (!TemDados()) return;
            string caminho = AcoesRelatorio.ExportarCsv(this, tabela, "relatorio_estoque",
                "Estoque", filtrosGerados);
            if (caminho != null)
                MostrarStatus("CSV salvo em " + caminho, false);
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (!TemDados()) return;
            AcoesRelatorio.Imprimir(this, tabela, "Hot-dog Brasília — Relatório de Estoque",
                "Estoque", filtrosGerados);
        }

        private bool TemDados()
        {
            if (tabela.Rows.Count > 0) return true;
            MessageBox.Show("Não há dados para exportar ou imprimir.", "Atenção",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        private void AtualizarVazio()
        {
            lblVazio.Visible = tabela.Rows.Count == 0;
            PosicionarVazio();
        }

        private void PosicionarVazio()
        {
            lblVazio.Bounds = new Rectangle(dgvEstoque.Left + 1,
                dgvEstoque.Top + dgvEstoque.ColumnHeadersHeight + 1,
                Math.Max(0, dgvEstoque.Width - 2), 60);
            lblVazio.BringToFront();
        }

        private void MostrarStatus(string msg, bool erro)
        {
            lblStatus.ForeColor = erro ? Tema.Erro : Tema.Sucesso;
            lblStatus.Text = msg;
            timerStatus.Stop();
            timerStatus.Start();
        }
    }
}
