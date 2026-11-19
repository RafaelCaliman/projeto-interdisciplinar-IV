using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using HotDogBrasilia.Data.Repositories;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    public partial class FrmLogAcoes : Form
    {
        private const int DiasPadrao = 7;
        private DataTable tabela = CriarTabela();
        private string filtrosGerados = "Período: todos";

        private class ItemUsuario
        {
            public int? Id;
            public string Texto;
            public override string ToString() { return Texto; }
        }

        public FrmLogAcoes()
        {
            InitializeComponent();
            Tema.Grade(dgvLog);
            dgvLog.ShowCellToolTips = true;
            dgvLog.Resize += (s, e) => PosicionarVazio();
        }

        private void FrmLogAcoes_Load(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.LogVer))
            {
                BeginInvoke(new Action(Close));
                return;
            }

            dgvLog.DataSource = tabela;
            FormatarGrade();
            CarregarUsuarios();
            PeriodoPadrao();
            Consultar();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.F5: Consultar(); return true;
                case Keys.Control | Keys.E: btnExportar.PerformClick(); return true;
                case Keys.Control | Keys.P: btnImprimir.PerformClick(); return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private static DataTable CriarTabela()
        {
            var t = new DataTable("Log");
            t.Columns.Add("Data e hora", typeof(string));
            t.Columns.Add("Usuário", typeof(string));
            t.Columns.Add("Ação", typeof(string));
            t.Columns.Add("Entidade", typeof(string));
            t.Columns.Add("Registro", typeof(string));
            t.Columns.Add("Detalhe", typeof(string));
            return t;
        }

        private void FormatarGrade()
        {
            var g = dgvLog;
            g.Columns["Data e hora"].FillWeight = 70;
            g.Columns["Usuário"].FillWeight = 70;
            g.Columns["Ação"].FillWeight = 80;
            g.Columns["Entidade"].FillWeight = 50;
            g.Columns["Registro"].FillWeight = 35;
            g.Columns["Registro"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns["Registro"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns["Detalhe"].FillWeight = 200;
            g.Columns["Detalhe"].ToolTipText = "Passe o mouse sobre o detalhe para ver o texto completo";
            foreach (DataGridViewColumn c in g.Columns)
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void CarregarUsuarios()
        {
            cboUsuario.Items.Clear();
            cboUsuario.Items.Add(new ItemUsuario { Id = null, Texto = "Todos" });
            try
            {
                foreach (var u in UsuarioRepository.listarUsuarios())
                    cboUsuario.Items.Add(new ItemUsuario
                    {
                        Id = u.IdUser,
                        Texto = u.Nome + (u.Ativo ? "" : " (inativo)")
                    });
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
            }
            cboUsuario.SelectedIndex = 0;
        }

        private void PeriodoPadrao()
        {
            dtpInicio.Value = DateTime.Today.AddDays(-(DiasPadrao - 1));
            dtpFim.Value = DateTime.Today;
            dtpInicio.Checked = true;
            dtpFim.Checked = true;
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            Consultar();
        }

        private void Consultar()
        {
            if (dtpInicio.Checked && dtpFim.Checked && dtpInicio.Value.Date > dtpFim.Value.Date)
            {
                tabela.Rows.Clear();
                AtualizarResumo();
                MessageBox.Show("A data inicial não pode ser maior que a data final.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpInicio.Focus();
                return;
            }

            DateTime? ini = dtpInicio.Checked ? dtpInicio.Value.Date : (DateTime?)null;
            DateTime? fim = dtpFim.Checked ? dtpFim.Value.Date : (DateTime?)null;
            var item = cboUsuario.SelectedItem as ItemUsuario;
            int? idUsuario = item == null ? null : item.Id;

            btnConsultar.Enabled = false;
            UseWaitCursor = true;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                var lista = LogRepository.listar(ini, fim, idUsuario);

                tabela.Rows.Clear();
                foreach (var l in lista)
                    tabela.Rows.Add(l.DataHora.ToString("dd/MM/yyyy HH:mm:ss"), l.Usuario, l.Acao, l.Entidade,
                        l.IdRegistro.HasValue ? l.IdRegistro.Value.ToString() : "",
                        l.Detalhe);

                filtrosGerados = AcoesRelatorio.TextoPeriodo(dtpInicio, dtpFim)
                    + " · Usuário: " + (item == null ? "Todos" : item.Texto);
                dgvLog.ClearSelection();
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
            }
            finally
            {
                UseWaitCursor = false;
                btnConsultar.Enabled = true;
                AtualizarResumo();
            }
        }

        private void AtualizarResumo()
        {
            int n = tabela.Rows.Count;
            lblResumo.Text = n + (n == 1 ? " registro" : " registros");
            lblVazio.Visible = n == 0;
            PosicionarVazio();
        }

        private void PosicionarVazio()
        {
            lblVazio.Bounds = new Rectangle(dgvLog.Left + 1,
                dgvLog.Top + dgvLog.ColumnHeadersHeight + 1,
                Math.Max(0, dgvLog.Width - 2), 60);
            lblVazio.BringToFront();
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            cboUsuario.SelectedIndex = 0;
            PeriodoPadrao();
            Consultar();
            dtpInicio.Focus();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (!TemDados()) return;
            AcoesRelatorio.ExportarCsv(this, tabela, "log_acoes", "Log de ações", filtrosGerados);
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (!TemDados()) return;
            AcoesRelatorio.Imprimir(this, tabela, "Hot-dog Brasília — Log de ações",
                "Log de ações", filtrosGerados);
        }

        private bool TemDados()
        {
            if (tabela.Rows.Count > 0) return true;
            MessageBox.Show("Não há dados para exportar ou imprimir.\nConsulte com outro filtro.",
                "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }
    }
}
