using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using HotDogBrasilia.Data.Repositories;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    public partial class FrmRelatorioVendas : Form
    {
        private static readonly CultureInfo PtBr = new CultureInfo("pt-BR", false);
        private readonly Timer timerStatus = new Timer { Interval = 5000 };
        private DataTable tabela = CriarTabela();
        private string filtrosGerados = "Período: todos";

        public FrmRelatorioVendas()
        {
            InitializeComponent();
            timerStatus.Tick += (s, e) => { timerStatus.Stop(); lblStatus.Text = ""; };
            Tema.Grade(dgvVendas);
            dgvVendas.Resize += (s, e) => PosicionarVazio();
            FormClosed += (s, e) => timerStatus.Dispose();
        }

        private void FrmRelatorioVendas_Load(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.RelatorioVer))
            {
                BeginInvoke(new Action(Close));
                return;
            }

            dgvVendas.DataSource = tabela;
            FormatarGrade();
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
            var t = new DataTable("Vendas");
            t.Columns.Add("Nº da venda", typeof(int));
            t.Columns.Add("Data/hora", typeof(DateTime));
            t.Columns.Add("Cliente", typeof(string));
            t.Columns.Add("Total", typeof(decimal));
            return t;
        }

        private void FormatarGrade()
        {
            var g = dgvVendas;
            g.Columns["Nº da venda"].FillWeight = 40;
            g.Columns["Nº da venda"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns["Nº da venda"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns["Data/hora"].FillWeight = 70;
            g.Columns["Data/hora"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            g.Columns["Cliente"].FillWeight = 140;
            g.Columns["Total"].FillWeight = 60;
            Tema.ColunaMoeda(g.Columns["Total"]);
        }

        private void btnGerar_Click(object sender, EventArgs e)
        {
            Gerar();
        }

        private void Gerar()
        {
            if (dtpInicio.Checked && dtpFim.Checked && dtpInicio.Value.Date > dtpFim.Value.Date)
            {
                Zerar();
                MessageBox.Show("A data inicial não pode ser maior que a data final.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpInicio.Focus();
                return;
            }

            DateTime? ini = dtpInicio.Checked ? dtpInicio.Value.Date : (DateTime?)null;
            DateTime? fim = dtpFim.Checked ? dtpFim.Value.Date : (DateTime?)null;

            btnGerar.Enabled = false;
            UseWaitCursor = true;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                var linhas = RelatorioRepository.listarVendas(ini, fim);
                var resumo = RelatorioRepository.resumoVendas(ini, fim);

                tabela.Rows.Clear();
                foreach (var l in linhas)
                    tabela.Rows.Add(l.IdVenda, l.DataHora, l.Cliente, l.Total);

                cardQtd.Valor = resumo.QtdVendas.ToString(PtBr);
                cardTotal.Valor = resumo.TotalVendido.ToString("C2", PtBr);
                cardTicket.Valor = resumo.TicketMedio.ToString("C2", PtBr);

                filtrosGerados = AcoesRelatorio.TextoPeriodo(dtpInicio, dtpFim);
                AtualizarVazio();
                dgvVendas.ClearSelection();

                if (linhas.Count > 0)
                    MostrarStatus(linhas.Count + (linhas.Count == 1 ? " venda encontrada." : " vendas encontradas."), false);
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

        private void Zerar()
        {
            tabela.Rows.Clear();
            cardQtd.Valor = 0.ToString(PtBr);
            cardTotal.Valor = 0m.ToString("C2", PtBr);
            cardTicket.Valor = 0m.ToString("C2", PtBr);
            AtualizarVazio();
            lblStatus.Text = "";
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            dtpInicio.Value = DateTime.Today;
            dtpFim.Value = DateTime.Today;
            dtpInicio.Checked = false;
            dtpFim.Checked = false;
            Gerar();
            dtpInicio.Focus();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (!TemDados()) return;
            string caminho = AcoesRelatorio.ExportarCsv(this, tabela, "relatorio_vendas",
                "Vendas", filtrosGerados);
            if (caminho != null)
                MostrarStatus("CSV salvo em " + caminho, false);
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (!TemDados()) return;
            AcoesRelatorio.Imprimir(this, tabela, "Hot-dog Brasília — Relatório de Vendas",
                "Vendas", filtrosGerados);
        }

        private bool TemDados()
        {
            if (tabela.Rows.Count > 0) return true;
            MessageBox.Show("Não há dados para exportar ou imprimir.\nGere o relatório com outro período.",
                "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        private void AtualizarVazio()
        {
            lblVazio.Visible = tabela.Rows.Count == 0;
            PosicionarVazio();
        }

        private void PosicionarVazio()
        {
            lblVazio.Bounds = new Rectangle(dgvVendas.Left + 1,
                dgvVendas.Top + dgvVendas.ColumnHeadersHeight + 1,
                Math.Max(0, dgvVendas.Width - 2), 60);
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
