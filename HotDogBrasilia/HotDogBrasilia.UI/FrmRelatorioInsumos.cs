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
    public partial class FrmRelatorioInsumos : Form
    {
        private const int LimitePadrao = 10;
        private static readonly CultureInfo PtBr = new CultureInfo("pt-BR", false);
        private readonly Timer timerStatus = new Timer { Interval = 5000 };
        private readonly Font negrito = new Font("Segoe UI", 10F, FontStyle.Bold);
        private DataTable tabela = CriarTabela();
        private string filtrosGerados = "Estoque baixo até " + LimitePadrao + " unidades";

        public FrmRelatorioInsumos()
        {
            InitializeComponent();
            timerStatus.Tick += (s, e) => { timerStatus.Stop(); lblStatus.Text = ""; };
            Tema.Grade(dgvInsumos);
            dgvInsumos.Resize += (s, e) => PosicionarVazio();
            dgvInsumos.CellFormatting += dgvInsumos_CellFormatting;
            FormClosed += (s, e) => { negrito.Dispose(); timerStatus.Dispose(); };
        }

        private void FrmRelatorioInsumos_Load(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.RelatorioVer))
            {
                BeginInvoke(new Action(Close));
                return;
            }

            Tema.Dica(txtLimite, "padrão: " + LimitePadrao);
            dgvInsumos.DataSource = tabela;
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
            var t = new DataTable("Insumos");
            t.Columns.Add("Insumo", typeof(string));
            t.Columns.Add("Estoque", typeof(int));
            t.Columns.Add("Situação", typeof(string));
            return t;
        }

        private void FormatarGrade()
        {
            var g = dgvInsumos;
            g.Columns["Insumo"].FillWeight = 150;
            g.Columns["Estoque"].FillWeight = 50;
            g.Columns["Estoque"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            g.Columns["Estoque"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            g.Columns["Situação"].FillWeight = 60;
            g.Columns["Situação"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns["Situação"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void dgvInsumos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvInsumos.Columns[e.ColumnIndex].Name != "Situação") return;
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
                var linhas = RelatorioRepository.listarInsumos(limite);

                tabela.Rows.Clear();
                foreach (var l in linhas)
                    tabela.Rows.Add(l.Insumo, l.Estoque, l.Situacao);

                cardInsumos.Valor = linhas.Count.ToString(PtBr);
                cardBaixo.Valor = linhas.Count(l => l.Situacao == "BAIXO").ToString(PtBr);
                cardEsgotado.Valor = linhas.Count(l => l.Situacao == "ESGOTADO").ToString(PtBr);

                filtrosGerados = "Estoque baixo até " + (limite ?? LimitePadrao) + " unidades";
                AtualizarVazio();
                dgvInsumos.ClearSelection();

                if (linhas.Count > 0)
                    MostrarStatus(linhas.Count + (linhas.Count == 1 ? " insumo listado." : " insumos listados."), false);
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
            string caminho = AcoesRelatorio.ExportarCsv(this, tabela, "relatorio_insumos",
                "Insumos", filtrosGerados);
            if (caminho != null)
                MostrarStatus("CSV salvo em " + caminho, false);
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (!TemDados()) return;
            AcoesRelatorio.Imprimir(this, tabela, "Hot-dog Brasília — Relatório de Insumos",
                "Insumos", filtrosGerados);
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
            lblVazio.Bounds = new Rectangle(dgvInsumos.Left + 1,
                dgvInsumos.Top + dgvInsumos.ColumnHeadersHeight + 1,
                Math.Max(0, dgvInsumos.Width - 2), 60);
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
