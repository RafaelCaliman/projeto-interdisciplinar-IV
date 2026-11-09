using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace HotDogBrasilia.UI
{
    // Mostra os erros para o usuário sempre do mesmo jeito.
    public static class Erros
    {
        // Códigos do MySQL que tratamos de forma especial.
        private const int ErroDaRegra = 1644;      // SIGNAL das procedures (mensagem já em português)
        private const int SemConexao = 1042;       // MySQL desligado ou endereço errado
        private const int AcessoNegado = 1045;     // usuário/senha da conexão errados

        public static void Mostrar(Exception ex)
        {
            MessageBox.Show(Texto(ex), "Atenção",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // Escolhe o texto que o usuário vai ler. Texto técnico do .NET
        // (em inglês ou cheio de detalhes) nunca aparece na tela.
        public static string Texto(Exception ex)
        {
            var erroMySql = ex as MySqlException;
            if (erroMySql != null)
            {
                if (erroMySql.Number == ErroDaRegra)
                    return ex.Message;

                if (erroMySql.Number == SemConexao)
                    return "Não foi possível conectar ao banco de dados.\n"
                        + "Verifique se o MySQL está ligado e tente novamente.";

                if (erroMySql.Number == AcessoNegado)
                    return "O banco de dados recusou o acesso.\n"
                        + "Confira o usuário e a senha em ConexaoBanco.cs.";

                return "Ocorreu um erro no banco de dados. Tente novamente.";
            }

            // Mensagens criadas por nós com "throw new Exception(...)"
            // (ex.: valor inválido) já foram escritas para o usuário.
            if (ex != null && ex.GetType() == typeof(Exception))
                return ex.Message;

            return "Ocorreu um erro inesperado. Tente novamente.\n"
                + "Se o problema continuar, avise o administrador.";
        }
    }
}
