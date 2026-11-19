using System;
using System.Collections.Generic;
using System.Data;
using HotDogBrasilia.Domain;
using MySql.Data.MySqlClient;

namespace HotDogBrasilia.Data.Repositories
{
    public static class FinanceiroRepository
    {
        public static List<LancamentoFinanceiro> listar(string tipo, string status,
                                                        DateTime? ini, DateTime? fim)
        {
            var lista = new List<LancamentoFinanceiro>();
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_financeiro_listar", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_tipo", (object)tipo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_status", (object)status ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_data_ini", ini.HasValue ? (object)ini.Value.Date : DBNull.Value);
                cmd.Parameters.AddWithValue("p_data_fim", fim.HasValue ? (object)fim.Value.Date : DBNull.Value);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    int colDescricao = rd.GetOrdinal("descricao");
                    int colVenda = rd.GetOrdinal("id_venda");
                    while (rd.Read())
                    {
                        lista.Add(new LancamentoFinanceiro
                        {
                            IdFinanceiro = rd.GetInt32("id_financeiro"),
                            Tipo = rd.GetString("tipo"),
                            Descricao = rd.IsDBNull(colDescricao) ? "" : rd.GetString(colDescricao),
                            Valor = rd.GetDecimal("valor"),
                            DataLancamento = rd.GetDateTime("data_lancamento"),
                            StatusPagamento = rd.GetString("status_pagamento"),
                            IdVenda = rd.IsDBNull(colVenda) ? (int?)null : rd.GetInt32(colVenda)
                        });
                    }
                }
            }
            return lista;
        }

        public static int inserir(string tipo, string descricao, decimal? valor,
                                  DateTime dataLancamento, string status)
        {
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_financeiro_inserir", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_tipo", (object)tipo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_descricao", (object)descricao ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_valor", valor.HasValue ? (object)valor.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("p_data_lancamento", dataLancamento.Date);
                cmd.Parameters.AddWithValue("p_status", (object)status ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_id_venda", DBNull.Value);
                var pId = new MySqlParameter("p_id_financeiro", MySqlDbType.Int32)
                    { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(pId);
                conn.Open();
                cmd.ExecuteNonQuery();
                return Convert.ToInt32(pId.Value);
            }
        }

        public static void marcarPago(int idFinanceiro)
        {
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_financeiro_marcar_pago", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_id_financeiro", idFinanceiro);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
