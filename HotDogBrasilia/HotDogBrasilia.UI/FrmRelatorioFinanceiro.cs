using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using HotDogBrasilia.Data.Repositories;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    public partial class FrmRelatorioFinanceiro : Form
    {
        private static readonly CultureInfo PtBr = new CultureInfo("pt-BR", false);
        private readonly Timer timerStatus = new Timer { Interval = 5000 };
        private DataTable tabela = CriarTabela();
        private string filtrosGerados = "Período: todos";

        public FrmRelatorioFinanceiro()
        {
            InitializeComponent();
            timerStatus.Tick += (s, e) => { timerStatus.Stop(); lblStatus.Text = ""; };
            FormClosed += (s, e) => timerStatus.Dispose();
        }

        private void FrmRelatorioFinanceiro_Load(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.RelatorioVer))
            {
                BeginInvoke(new Action(Close));
                return;
            }

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
            var t = new DataTable("Financeiro");
            t.Columns.Add("Indicador", typeof(string));
            t.Columns.Add("Valor", typeof(decimal));
            return t;
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
                var r = RelatorioRepository.resumoFinanceiro(ini, fim);

                Mostrar(cardRecebido, r.Recebido, false);
                Mostrar(cardAReceber, r.AReceber, false);
                Mostrar(cardPago, r.Pago, false);
                Mostrar(cardAPagar, r.APagar, false);
                Mostrar(cardSaldoRealizado, r.SaldoRealizado, true);
                Mostrar(cardSaldoPrevisto, r.SaldoPrevisto, true);

                tabela.Rows.Clear();
                tabela.Rows.Add("Recebido", r.Recebido);
                tabela.Rows.Add("A receber", r.AReceber);
                tabela.Rows.Add("Pago", r.Pago);
                tabela.Rows.Add("A pagar", r.APagar);
                tabela.Rows.Add("Saldo realizado", r.SaldoRealizado);
                tabela.Rows.Add("Saldo previsto", r.SaldoPrevisto);

                filtrosGerados = AcoesRelatorio.TextoPeriodo(dtpInicio, dtpFim);

                bool vazio = r.Recebido == 0 && r.AReceber == 0 && r.Pago == 0 && r.APagar == 0;
                if (vazio)
                    MostrarStatus("Nenhum lançamento encontrado para o período.", true);
                else
                    MostrarStatus("Relatório gerado.", false);
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

        private static void Mostrar(CartaoResumo card, decimal valor, bool ehSaldo)
        {
            card.Valor = valor.ToString("C2", PtBr);
            if (ehSaldo)
                card.CorValor = valor < 0 ? Tema.Erro : Tema.Sucesso;
        }

        private void Zerar()
        {
            foreach (var card in new[] { cardRecebido, cardAReceber, cardPago, cardAPagar,
                                         cardSaldoRealizado, cardSaldoPrevisto })
                Mostrar(card, 0m, false);
            cardSaldoRealizado.CorValor = Tema.TextoEscuro;
            cardSaldoPrevisto.CorValor = Tema.TextoEscuro;
            tabela.Rows.Clear();
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
            string caminho = AcoesRelatorio.ExportarCsv(this, tabela, "relatorio_financeiro",
                "Financeiro", filtrosGerados);
            if (caminho != null)
                MostrarStatus("CSV salvo em " + caminho, false);
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (!TemDados()) return;
            AcoesRelatorio.Imprimir(this, tabela, "Hot-dog Brasília — Relatório Financeiro",
                "Financeiro", filtrosGerados);
        }

        private bool TemDados()
        {
            if (tabela.Rows.Count > 0) return true;
            MessageBox.Show("Não há dados para exportar ou imprimir.\nGere o relatório com outro período.",
                "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        private void MostrarStatus(string msg, bool aviso)
        {
            lblStatus.ForeColor = aviso ? Tema.Erro : Tema.Sucesso;
            lblStatus.Text = msg;
            timerStatus.Stop();
            timerStatus.Start();
        }
    }
}
