using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace HotDogBrasilia.UI
{
    public static class Erros
    {
        private const int ErroDaRegra = 1644;
        private const int SemConexao = 1042;
        private const int AcessoNegado = 1045;
        private const int BancoInexistente = 1049;

        public static void Mostrar(Exception ex)
        {
            MessageBox.Show(Texto(ex), "Atenção",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

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
                        + "Confira o usuário e a senha no arquivo HotDogBrasilia.UI.exe.config.";

                if (erroMySql.Number == BancoInexistente)
                    return "O banco de dados hotdog_brasilia não foi encontrado.\n"
                        + "Rode o script do banco (v6) no MySQL ou confira o nome do banco no arquivo HotDogBrasilia.UI.exe.config.";

                return "Ocorreu um erro no banco de dados. Tente novamente.";
            }

            if (ex != null && ex.GetType() == typeof(Exception))
                return ex.Message;

            return "Ocorreu um erro inesperado. Tente novamente.\n"
                + "Se o problema continuar, avise o administrador.";
        }
    }
}
