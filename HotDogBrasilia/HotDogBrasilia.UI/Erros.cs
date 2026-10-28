using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace HotDogBrasilia.UI
{
    // Mostra os erros para o usuário sempre do mesmo jeito.
    public static class Erros
    {
        public static void Mostrar(Exception ex)
        {
            string mensagem = ex.Message;

            // 1042 = o MySQL não respondeu (desligado ou endereço errado).
            // A mensagem original vem em inglês, então trocamos por uma explicação.
            var erroMySql = ex as MySqlException;
            if (erroMySql != null && erroMySql.Number == 1042)
                mensagem = "Não foi possível conectar ao banco de dados.\n"
                    + "Verifique se o MySQL está ligado e tente novamente.";

            MessageBox.Show(mensagem, "Atenção",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
