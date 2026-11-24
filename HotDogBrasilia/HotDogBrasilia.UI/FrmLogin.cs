using System;
using System.Windows.Forms;
using HotDogBrasilia.Data.Repositories;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.UI
{
    public partial class FrmLogin : Form
    {
        public Usuario UsuarioLogado { private set; get; }

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void FrmLogin_Shown(object sender, EventArgs e)
        {
            txtLogin.Focus();
        }

        private void chkMostrarSenha_CheckedChanged(object sender, EventArgs e)
        {
            txtSenha.UseSystemPasswordChar = !chkMostrarSenha.Checked;
        }

        private void Campos_TextChanged(object sender, EventArgs e)
        {
            lblMensagem.Text = "";
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            lblMensagem.Text = "";

            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                lblMensagem.Text = "Informe o usuário.";
                txtLogin.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSenha.Text))
            {
                lblMensagem.Text = "Informe a senha.";
                txtSenha.Focus();
                return;
            }

            btnEntrar.Enabled = false;
            btnEntrar.Text = "Entrando...";
            Cursor = Cursors.WaitCursor;
            btnEntrar.Refresh();

            try
            {
                UsuarioLogado = UsuarioRepository.Auth(txtLogin.Text.Trim(), txtSenha.Text);
                if (UsuarioLogado != null)
                {
                    var codigos = PermissaoRepository.listarCodigos(UsuarioLogado.IdUser);
                    Sessao.Iniciar(UsuarioLogado, codigos);
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    lblMensagem.Text = "Login ou senha inválidos.";
                    LimparSenha();
                }
            }
            catch (Exception ex)
            {
                lblMensagem.Text = Erros.Texto(ex);
                LimparSenha();
            }
            finally
            {
                btnEntrar.Enabled = true;
                btnEntrar.Text = "Entrar";
                Cursor = Cursors.Default;
            }
        }

        private void LimparSenha()
        {
            txtSenha.TextChanged -= Campos_TextChanged;
            txtSenha.Clear();
            txtSenha.TextChanged += Campos_TextChanged;
            txtSenha.Focus();
        }
    }
}
