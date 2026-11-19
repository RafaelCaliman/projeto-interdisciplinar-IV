using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.Data.Repositories
{
    public static class FornecedorRepository
    {
        public static List<Fornecedor> listarFornecedores()
        {
            var lista = new List<Fornecedor>();
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_fornecedor_listar", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new Fornecedor
                        {
                            IdFornecedor = rd.GetInt32("id_fornecedor"),
                            Nome = rd.GetString("nome")
                        });
                    }
                }
            }
            return lista;
        }

        public static List<Fornecedor> pesquisar(string busca)
        {
            var lista = new List<Fornecedor>();
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_fornecedor_pesquisar", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_busca",
                    string.IsNullOrWhiteSpace(busca) ? (object)DBNull.Value : busca.Trim());
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new Fornecedor
                        {
                            IdFornecedor = rd.GetInt32("id_fornecedor"),
                            Nome = rd.GetString("nome"),
                            QtdProdutos = Convert.ToInt32(rd["qtd_produtos"])
                        });
                    }
                }
            }
            return lista;
        }

        public static int inserir(string nome)
        {
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_fornecedor_inserir", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_nome", nome);
                var pId = new MySqlParameter("p_id_fornecedor", MySqlDbType.Int32)
                    { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(pId);
                conn.Open();
                cmd.ExecuteNonQuery();
                return Convert.ToInt32(pId.Value);
            }
        }

        public static void atualizar(int id, string nome)
        {
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_fornecedor_atualizar", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_id_fornecedor", id);
                cmd.Parameters.AddWithValue("p_nome", nome);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void excluir(int id)
        {
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_fornecedor_excluir", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_id_fornecedor", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
