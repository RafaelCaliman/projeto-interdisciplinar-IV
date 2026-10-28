using System;
using System.Windows.Forms;
using HotDogBrasilia.Data.Repositories;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    public partial class FrmClientes : Form
    {
        private int idSelecionado = 0;
        private readonly Timer timerStatus = new Timer { Interval = 4000 };

        public FrmClientes()
        {
            InitializeComponent();
            timerStatus.Tick += (s, e) =>
            {
                lblStatus.Text = "";
                timerStatus.Stop();
            };
        }

        private void FrmClientes_Load(object sender, EventArgs e)
        {
            AplicarPermissoes();

            try
            {
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
            bool podeGravar = Sessao.TemPermissao(Permissoes.ClienteGravar);
            btnNovo.Enabled = podeGravar;
            btnSalvar.Enabled = podeGravar;
            btnExcluir.Enabled = Sessao.TemPermissao(Permissoes.ClienteExcluir);
        }

        private void CarregarGrade()
        {
            dgvClientes.DataSource = ClienteRepository.listarClientes(null);
            if (dgvClientes.Columns.Contains("Id"))
                dgvClientes.Columns["Id"].Visible = false;
            dgvClientes.ClearSelection();
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
            mskTelefone.Clear();
            txtNome.Focus();
        }

        private void dgvClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var c = dgvClientes.Rows[e.RowIndex].DataBoundItem as Cliente;
            if (c == null) return;
            idSelecionado = c.Id;
            txtNome.Text = c.Nome;
            mskTelefone.Text = c.Telefone;
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            Limpar();
            lblStatus.Text = "";
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (!ControleAcesso.Verificar(Permissoes.ClienteGravar)) return;
            lblStatus.Text = "";

            try
            {
                if (idSelecionado == 0)
                    ClienteRepository.inserirCliente(txtNome.Text, mskTelefone.Text);
                else
                    ClienteRepository.atualizarCliente(idSelecionado, txtNome.Text, mskTelefone.Text);

                MostrarStatus("Cliente salvo com sucesso.");
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
            if (!ControleAcesso.Verificar(Permissoes.ClienteExcluir)) return;

            if (idSelecionado == 0)
            {
                MessageBox.Show("Selecione um cliente na lista.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var resposta = MessageBox.Show("Deseja realmente excluir este cliente?",
                "Confirmar exclusão", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (resposta != DialogResult.Yes) return;

            lblStatus.Text = "";

            try
            {
                ClienteRepository.excluirCliente(idSelecionado);
                MostrarStatus("Cliente excluído.");
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
