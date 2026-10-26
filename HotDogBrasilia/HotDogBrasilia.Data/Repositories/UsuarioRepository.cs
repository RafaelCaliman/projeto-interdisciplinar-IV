using HotDogBrasilia.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;

namespace HotDogBrasilia.Data.Repositories
{
    public class UsuarioRepository
    {
        public static Usuario Auth(string login, string senha)
        {

            using (MySqlConnection conn = ConexaoBanco.ObterConexao())
            using (MySqlCommand cmd = new MySqlCommand("sp_usuario_autenticar", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_login", login);
                cmd.Parameters.AddWithValue("p_senha", senha);

                conn.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Usuario
                        {
                            IdUser = reader.GetInt32("id_usuario"),
                            Nome = reader.GetString("nome"),
                            Login = reader.GetString("login"),
                            IdPerfil = reader.GetInt32("id_perfil")
                        };
                    }
                }
            }

            return null;
        }
    }
}
