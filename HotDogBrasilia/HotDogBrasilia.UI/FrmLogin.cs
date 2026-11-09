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

        // Já abre com o cursor no campo de usuário.
        private void FrmLogin_Shown(object sender, EventArgs e)
        {
            txtLogin.Focus();
        }

        private void chkMostrarSenha_CheckedChanged(object sender, EventArgs e)
        {
            txtSenha.UseSystemPasswordChar = !chkMostrarSenha.Checked;
        }

        // Some com o aviso assim que o usuário volta a digitar.
        private void Campos_TextChanged(object sender, EventArgs e)
        {
            lblMensagem.Text = "";
        }

        // O Enter também chama este botão (AcceptButton do formulário).
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

            // Mostra que está trabalhando e evita clique duplo.
            btnEntrar.Enabled = false;
            btnEntrar.Text = "Entrando...";
            Cursor = Cursors.WaitCursor;
            btnEntrar.Refresh();

            try
            {
                // Quem confere a senha é o banco (sp_usuario_autenticar).
                UsuarioLogado = UsuarioRepository.Auth(txtLogin.Text.Trim(), txtSenha.Text);
                if (UsuarioLogado != null)
                {
                    // Carrega as permissões do perfil e guarda na sessão antes de abrir o sistema.
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
                // Ex.: "Login ou senha inválidos." ou "Usuário inativo..." vindos do banco.
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
            txtSenha.TextChanged -= Campos_TextChanged;   // não apagar o aviso agora
            txtSenha.Clear();
            txtSenha.TextChanged += Campos_TextChanged;
            txtSenha.Focus();
        }
    }
}
