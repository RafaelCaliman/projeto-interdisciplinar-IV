using System;
using System.Windows.Forms;
using HotDogBrasilia.Data.Repositories;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    public partial class FrmClientes : Form
    {
        private int idSelecionado = 0;

        public FrmClientes()
        {
            InitializeComponent();
        }

        private void FrmClientes_Load(object sender, EventArgs e)
        {
            CarregarGrade();
        }

        private void CarregarGrade()
        {
            dgvClientes.DataSource = ClienteRepository.listarClientes(null);
            if (dgvClientes.Columns.Contains("Id"))
                dgvClientes.Columns["Id"].Visible = false;
            dgvClientes.ClearSelection();
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
            try
            {
                if (idSelecionado == 0)
                    ClienteRepository.inserirCliente(txtNome.Text, mskTelefone.Text);
                else
                    ClienteRepository.atualizarCliente(idSelecionado, txtNome.Text, mskTelefone.Text);

                lblStatus.Text = "Cliente salvo com sucesso.";
                Limpar();
                CarregarGrade();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
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

            try
            {
                ClienteRepository.excluirCliente(idSelecionado);
                lblStatus.Text = "Cliente excluído.";
                Limpar();
                CarregarGrade();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
