using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace HotDogBrasilia.Data
{
    public class ConexaoBanco
    {
        private const string NomeConexao = "hotdog";

        private const string MensagemConfiguracao =
            "A configuração de conexão com o banco de dados está ausente ou inválida.\n"
            + "Confira a cadeia de conexão \"hotdog\" no arquivo HotDogBrasilia.UI.exe.config.";

        public static MySqlConnection ObterConexao()
        {
            string cadeia;
            try
            {
                var item = ConfigurationManager.ConnectionStrings[NomeConexao];
                cadeia = item == null ? null : item.ConnectionString;
            }
            catch (ConfigurationErrorsException)
            {
                throw new Exception(MensagemConfiguracao);
            }

            if (string.IsNullOrWhiteSpace(cadeia))
                throw new Exception(MensagemConfiguracao);

            try
            {
                return new MySqlConnection(cadeia);
            }
            catch (ArgumentException)
            {
                throw new Exception(MensagemConfiguracao);
            }
        }
    }
}
