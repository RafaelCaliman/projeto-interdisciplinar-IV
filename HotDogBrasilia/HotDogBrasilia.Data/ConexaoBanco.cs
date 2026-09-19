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
        private const string ConnectionString = "Server=localhost;Database=hotdog;User=root;Password=";

        public static MySqlConnection ObterConexao()
        {
            return new MySqlConnection(ConnectionString);
        }

    }
}
