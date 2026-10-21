using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.Data.Repositories
{
    public static class ProdutoRepository
    {
        public static List<Produto> listarProdutos(string busca)
        {
            var lista = new List<Produto>();
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_produto_listar", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_busca",
                    string.IsNullOrWhiteSpace(busca) ? (object)DBNull.Value : busca.Trim());
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new Produto
                        {
                            IdProduto = rd.GetInt32("id_produto"),
                            Nome = rd.GetString("nome"),
                            Valor = rd.GetDecimal("valor"),
                            Estoque = rd.GetInt32("estoque"),
                            IdFornecedor = rd.GetInt32("id_fornecedor"),
                            NomeFornecedor = rd.GetString("fornecedor")
                        });
                    }
                }
            }
            return lista;
        }

        public static int inserirProduto(string nome, decimal valor, int estoque, int? idFornecedor)
        {
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_produto_inserir", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_nome", nome);
                cmd.Parameters.AddWithValue("p_valor", valor);
                cmd.Parameters.AddWithValue("p_estoque", estoque);
                cmd.Parameters.AddWithValue("p_id_fornecedor",
                    (object)idFornecedor ?? DBNull.Value);
                var pId = new MySqlParameter("p_id_produto", MySqlDbType.Int32)
                    { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(pId);
                conn.Open();
                cmd.ExecuteNonQuery();
                return Convert.ToInt32(pId.Value);
            }
        }

        public static void atualizarProduto(int id, string nome, decimal valor,
                                            int estoque, int? idFornecedor)
        {
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_produto_atualizar", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_id_produto", id);
                cmd.Parameters.AddWithValue("p_nome", nome);
                cmd.Parameters.AddWithValue("p_valor", valor);
                cmd.Parameters.AddWithValue("p_estoque", estoque);
                cmd.Parameters.AddWithValue("p_id_fornecedor",
                    (object)idFornecedor ?? DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void excluirProduto(int id)
        {
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_produto_excluir", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_id_produto", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
