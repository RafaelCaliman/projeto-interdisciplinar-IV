using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace HotDogBrasilia.Data
{
    public class ConexaoBanco
    {
        private const string Server = "localhost";
        private const string Database = "hotdog_brasilia";
        private const string User = "root";
        private const string Password = "";

        private static string ConnectionString =
           $"Server={Server};Database={Database};User={User};Password={Password};Charset=utf8mb4;";

        public static MySqlConnection ObterConexao()
        {
            return new MySqlConnection(ConnectionString);
        }

    }
}
