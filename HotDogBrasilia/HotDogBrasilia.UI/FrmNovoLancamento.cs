using System;
using System.Globalization;
using System.Windows.Forms;
using HotDogBrasilia.Data.Repositories;

namespace HotDogBrasilia.UI
{
    public partial class FrmNovoLancamento : Form
    {
        private static readonly CultureInfo PtBr = new CultureInfo("pt-BR");

        private static readonly string[] TiposTexto = { "A receber", "A pagar" };
        private static readonly string[] TiposValor = { "RECEBER", "PAGAR" };
        private static readonly string[] SituacoesTexto = { "Pendente", "Pago" };
        private static readonly string[] SituacoesValor = { "PENDENTE", "PAGO" };

        public int IdGerado { get; private set; }
        public string Resumo { get; private set; }

        public FrmNovoLancamento()
        {
            InitializeComponent();
        }

        private void FrmNovoLancamento_Load(object sender, EventArgs e)
        {
            cboTipo.Items.AddRange(TiposTexto);
            cboSituacao.Items.AddRange(SituacoesTexto);
            cboTipo.SelectedIndex = -1;
            cboSituacao.SelectedIndex = 0;
            dtpData.Value = DateTime.Today;
            Tema.Dica(txtDescricao, "ex.: Compra de pão");
            Tema.Dica(txtValor, "0,00");
            ActiveControl = cboTipo;
        }

        private void txtValor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '.') e.KeyChar = ',';
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar)) return;
            if (e.KeyChar == ',' && !txtValor.Text.Contains(",")) return;
            if (e.KeyChar == '-' && txtValor.SelectionStart == 0 && !txtValor.Text.Contains("-")) return;
            e.Handled = true;
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (cboTipo.SelectedIndex < 0)
            {
                MessageBox.Show("Escolha o tipo: a receber ou a pagar.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                cboTipo.Focus();
                return;
            }

            decimal? valor = null;
            string texto = txtValor.Text.Trim();
            if (texto.Length > 0)
            {
                decimal lido;
                if (!decimal.TryParse(texto, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                                      PtBr, out lido))
                {
                    MessageBox.Show("Informe o valor em reais, com vírgula nos centavos (ex.: 20,00).",
                        "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtValor.Focus();
                    txtValor.SelectAll();
                    return;
                }
                valor = lido;
            }

            string tipo = TiposValor[cboTipo.SelectedIndex];
            string status = SituacoesValor[cboSituacao.SelectedIndex];
            string descricao = txtDescricao.Text.Trim();

            btnSalvar.Enabled = false;
            Cursor = Cursors.WaitCursor;
            try
            {
                IdGerado = FinanceiroRepository.inserir(tipo, descricao.Length > 0 ? descricao : null,
                    valor, dtpData.Value.Date, status);

                Resumo = tipo + " " + status + " - "
                    + (descricao.Length > 0 ? descricao + " - " : "")
                    + (valor ?? 0m).ToString("C2", PtBr);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Erros.Mostrar(ex);
                txtValor.Focus();
            }
            finally
            {
                btnSalvar.Enabled = true;
                Cursor = Cursors.Default;
            }
        }
    }
}
