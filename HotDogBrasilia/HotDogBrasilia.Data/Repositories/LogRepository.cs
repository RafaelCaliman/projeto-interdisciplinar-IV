using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace HotDogBrasilia.Data.Repositories
{
    // Grava no log_acao quem fez o quê (a data/hora o banco preenche sozinho).
    public static class LogRepository
    {
        // Usa a conexão e a transação de quem chamou: assim o log é salvo
        // junto com a ação, ou desfeito junto com ela se algo der errado.
        public static void registrar(MySqlConnection conn, MySqlTransaction tx, int idUsuario,
                                     string acao, string entidade, int? idRegistro, string detalhe)
        {
            using (var cmd = new MySqlCommand("sp_log_registrar", conn, tx))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_id_usuario", idUsuario);
                cmd.Parameters.AddWithValue("p_acao", acao);
                cmd.Parameters.AddWithValue("p_entidade", entidade);
                cmd.Parameters.AddWithValue("p_id_registro", (object)idRegistro ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_detalhe", detalhe);
                cmd.ExecuteNonQuery();
            }
        }

        // Versão simples, para quando não existe transação aberta.
        public static void registrar(int idUsuario, string acao, string entidade,
                                     int? idRegistro, string detalhe)
        {
            using (var conn = ConexaoBanco.ObterConexao())
            {
                conn.Open();
                registrar(conn, null, idUsuario, acao, entidade, idRegistro, detalhe);
            }
        }
    }
}
