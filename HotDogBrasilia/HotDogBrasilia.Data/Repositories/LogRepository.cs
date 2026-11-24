using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.Data.Repositories
{
    public static class LogRepository
    {
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

        public static void registrar(int idUsuario, string acao, string entidade,
                                     int? idRegistro, string detalhe)
        {
            using (var conn = ConexaoBanco.ObterConexao())
            {
                conn.Open();
                registrar(conn, null, idUsuario, acao, entidade, idRegistro, detalhe);
            }
        }

        public static List<RegistroLog> listar(DateTime? dataIni, DateTime? dataFim, int? idUsuario)
        {
            var lista = new List<RegistroLog>();
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_log_listar", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_data_ini",
                    dataIni.HasValue ? (object)dataIni.Value.Date : DBNull.Value);
                cmd.Parameters.AddWithValue("p_data_fim",
                    dataFim.HasValue ? (object)dataFim.Value.Date : DBNull.Value);
                cmd.Parameters.AddWithValue("p_id_usuario",
                    idUsuario.HasValue ? (object)idUsuario.Value : DBNull.Value);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new RegistroLog
                        {
                            IdLog = rd.GetInt32("id_log"),
                            DataHora = rd.GetDateTime("data_hora"),
                            Usuario = rd.GetString("usuario"),
                            Acao = rd.GetString("acao"),
                            Entidade = rd.IsDBNull(rd.GetOrdinal("entidade")) ? "" : rd.GetString("entidade"),
                            IdRegistro = rd.IsDBNull(rd.GetOrdinal("id_registro"))
                                ? (int?)null : rd.GetInt32("id_registro"),
                            Detalhe = rd.IsDBNull(rd.GetOrdinal("detalhe")) ? "" : rd.GetString("detalhe")
                        });
                    }
                }
            }
            return lista;
        }
    }
}
