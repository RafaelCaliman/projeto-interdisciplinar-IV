using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace HotDogBrasilia.Data.Repositories
{
    public class PermissaoRepository
    {
        public static List<string> listarCodigos(int idUsuario)
        {
            var lista = new List<string>();
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_permissoes_do_usuario", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_id_usuario", idUsuario);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(rd.GetString("codigo"));
                    }
                }
            }
            return lista;
        }
    }
}
