using System;
using System.Windows.Forms;
using HotDogBrasilia.Data.Repositories;
using HotDogBrasilia.Domain;
using MySql.Data.MySqlClient;


namespace HotDogBrasilia.UI
{
    public partial class FrmLogin : Form
    {
        public  Usuario UsuarioLogado { private set; get; }
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
                if (string.IsNullOrWhiteSpace(txtLogin.Text.Trim()) || string.IsNullOrWhiteSpace(txtSenha.Text))
                {
                    MessageBox.Show("Preencha todos os Campos !");
                    return;
                }
            try
            {
                UsuarioLogado = UsuarioRepository.Auth(txtLogin.Text.Trim(), txtSenha.Text);
                if(UsuarioLogado != null)
                {
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Não foi Possível efetuar o Login.");
                }

            } catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
