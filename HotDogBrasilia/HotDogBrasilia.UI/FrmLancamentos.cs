using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using HotDogBrasilia.Data.Repositories;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    public partial class FrmLancamentos : Form
    {
        private static readonly CultureInfo PtBr = new CultureInfo("pt-BR");
        private readonly Timer timerStatus = new Timer { Interval = 5000 };
        private readonly Font negrito = new Font("Segoe UI", 10F, FontStyle.Bold);
        private DataTable tabela = CriarTabela();
        private bool podeGravar;

        private static readonly string[] TiposTexto = { "Todos", "A receber", "A pagar" };
        private static readonly string[] TiposValor = { null, "RECEBER", "PAGAR" };
        private static readonly string[] SituacoesTexto = { "Todas", "Pago", "Pendente" };
        private static readonly string[] SituacoesValor = { null, "PAGO", "PENDENTE" };

        public FrmLancamentos()
        {
            InitializeComponent();
            timerStatus.Tick += (s, e) => { timerStatus.Stop(); lblStatus.Text = ""; };
            Tema.Grade(dgvLancamentos);
            dgvLancamentos.Resize += (s, e) => PosicionarVazio();
            dgvLancamentos.CellFormatting += dgvLancamentos_CellFormatting;
            dgvLancamentos.SelectionChanged += (s, e) => AtualizarBotoes();
            dgvLancamentos.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && podeGravar) btnMarcarPago_Click(s, EventArgs.Empty);
            };
            FormClosed += (s, e) => { negrito.Dispose(); timerStatus.Dispose(); };
        }

        private void FrmLancamentos_Load(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.FinanceiroVer))
            {
                BeginInvoke(new Action(Close));
                return;
            }

            podeGravar = Sessao.TemPermissao(Permissoes.FinanceiroGravar);
            btnNovo.Enabled = podeGravar;

            cboTipo.Items.AddRange(TiposTexto);
            cboSituacao.Items.AddRange(SituacoesTexto);
            cboTipo.SelectedIndex = 0;
            cboSituacao.SelectedIndex = 0;

            dgvLancamentos.DataSource = tabela;
            FormatarGrade();
            Atualizar(null);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.F5: Atualizar(null); return true;
                case Keys.Control | Keys.N: NovoLancamento(); return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private static DataTable CriarTabela()
        {
            var t = new DataTable("Lancamentos");
            t.Columns.Add("Nº", typeof(int));
            t.Columns.Add("Tipo", typeof(string));
            t.Columns.Add("Descrição", typeof(string));
            t.Columns.Add("Valor", typeof(decimal));
            t.Columns.Add("Data", typeof(DateTime));
            t.Columns.Add("Situação", typeof(string));
            t.Columns.Add("Venda", typeof(string));
            return t;
        }

        private void FormatarGrade()
        {
            var g = dgvLancamentos;
            g.Columns["Nº"].FillWeight = 35;
            g.Columns["Nº"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns["Nº"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns["Tipo"].FillWeight = 60;
            g.Columns["Descrição"].FillWeight = 160;
            g.Columns["Valor"].FillWeight = 65;
            Tema.ColunaMoeda(g.Columns["Valor"]);
            g.Columns["Data"].FillWeight = 60;
            g.Columns["Data"].DefaultCellStyle.Format = "dd/MM/yyyy";
            g.Columns["Data"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns["Data"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns["Situação"].FillWeight = 60;
            g.Columns["Situação"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns["Situação"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns["Venda"].HeaderText = "Venda";
            g.Columns["Venda"].ToolTipText = "Venda de origem do lançamento, quando houver";
            g.Columns["Venda"].FillWeight = 60;
            g.Columns["Venda"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns["Venda"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (DataGridViewColumn c in g.Columns)
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void dgvLancamentos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvLancamentos.Columns[e.ColumnIndex].Name != "Situação") return;
            string s = Convert.ToString(e.Value);
            if (s == "Pendente")
            {
                e.CellStyle.BackColor = Tema.AmbarFundo;
                e.CellStyle.ForeColor = Color.FromArgb(120, 70, 0);
                e.CellStyle.Font = negrito;
            }
            else if (s == "Pago")
            {
                e.CellStyle.ForeColor = Tema.Sucesso;
            }
        }

        private void btnAtualizar_Click(object sender, EventArgs e)
        {
            Atualizar(null);
        }

        private void Atualizar(int? idSelecionar)
        {
            if (dtpInicio.Checked && dtpFim.Checked && dtpInicio.Value.Date > dtpFim.Value.Date)
            {
                tabela.Rows.Clear();
                AtualizarVazio();
                MessageBox.Show("A data inicial não pode ser maior que a data final.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpInicio.Focus();
                return;
            }

            string tipo = TiposValor[Math.Max(0, cboTipo.SelectedIndex)];
            string status = SituacoesValor[Math.Max(0, cboSituacao.SelectedIndex)];
            DateTime? ini = dtpInicio.Checked ? dtpInicio.Value.Date : (DateTime?)null;
            DateTime? fim = dtpFim.Checked ? dtpFim.Value.Date : (DateTime?)null;

            btnAtualizar.Enabled = false;
            UseWaitCursor = true;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                var lista = FinanceiroRepository.listar(tipo, status, ini, fim);

                tabela.Rows.Clear();
                foreach (var l in lista)
                    tabela.Rows.Add(l.IdFinanceiro,
                        l.Tipo == "RECEBER" ? "A receber" : "A pagar",
                        l.Descricao,
                        l.Valor,
                        l.DataLancamento,
                        l.StatusPagamento == "PAGO" ? "Pago" : "Pendente",
                        l.IdVenda.HasValue ? "#" + l.IdVenda.Value : "—");

                AtualizarVazio();
                dgvLancamentos.ClearSelection();
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
                btnAtualizar.Enabled = true;
                AtualizarBotoes();
            }
        }

        private void Selecionar(int idFinanceiro)
        {
            foreach (DataGridViewRow r in dgvLancamentos.Rows)
            {
                if (Convert.ToInt32(r.Cells["Nº"].Value) != idFinanceiro) continue;
                r.Selected = true;
                dgvLancamentos.CurrentCell = r.Cells["Descrição"];
                return;
            }
        }

        private DataGridViewRow LinhaSelecionada()
        {
            return dgvLancamentos.SelectedRows.Count > 0 ? dgvLancamentos.SelectedRows[0] : null;
        }

        private void AtualizarBotoes()
        {
            var r = LinhaSelecionada();
            btnMarcarPago.Enabled = podeGravar && r != null
                && Convert.ToString(r.Cells["Situação"].Value) == "Pendente";
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            cboTipo.SelectedIndex = 0;
            cboSituacao.SelectedIndex = 0;
            dtpInicio.Value = DateTime.Today;
            dtpFim.Value = DateTime.Today;
            dtpInicio.Checked = false;
            dtpFim.Checked = false;
            Atualizar(null);
            cboTipo.Focus();
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            NovoLancamento();
        }

        private void NovoLancamento()
        {
            if (!ControleAcesso.Verificar(Permissoes.FinanceiroGravar)) return;

            using (var f = new FrmNovoLancamento())
            {
                if (f.ShowDialog(this) != DialogResult.OK) return;

                Auditoria.Registrar("FINANCEIRO_INSERIR", "financeiro", f.IdGerado, f.Resumo);
                MostrarStatus("Lançamento #" + f.IdGerado + " gravado.", false);
                Atualizar(f.IdGerado);
            }
        }

        private void btnMarcarPago_Click(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.FinanceiroGravar)) return;

            var r = LinhaSelecionada();
            if (r == null)
            {
                MessageBox.Show("Selecione um lançamento na lista.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = Convert.ToInt32(r.Cells["Nº"].Value);
            string descricao = Convert.ToString(r.Cells["Descrição"].Value);
            string resumo = "Lançamento #" + id
                + (descricao.Length > 0 ? " - " + descricao : "")
                + " - " + Convert.ToDecimal(r.Cells["Valor"].Value).ToString("C2", PtBr);

            var resposta = MessageBox.Show("Marcar como pago?\n\n" + resumo,
                "Confirmar pagamento", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (resposta != DialogResult.Yes) return;

            lblStatus.Text = "";
            try
            {
                FinanceiroRepository.marcarPago(id);
                Auditoria.Registrar("FINANCEIRO_PAGAR", "financeiro", id, resumo);
                MostrarStatus("Lançamento #" + id + " marcado como pago.", false);
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
            }
            Atualizar(id);
        }

        private void AtualizarVazio()
        {
            lblVazio.Visible = tabela.Rows.Count == 0;
            PosicionarVazio();
        }

        private void PosicionarVazio()
        {
            lblVazio.Bounds = new Rectangle(dgvLancamentos.Left + 1,
                dgvLancamentos.Top + dgvLancamentos.ColumnHeadersHeight + 1,
                Math.Max(0, dgvLancamentos.Width - 2), 60);
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
