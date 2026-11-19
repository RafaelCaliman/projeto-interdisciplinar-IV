using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.Data.Repositories
{
    public static class InsumoRepository
    {
        public static List<Insumo> listar(string busca)
        {
            var lista = new List<Insumo>();
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_insumo_listar", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_busca",
                    string.IsNullOrWhiteSpace(busca) ? (object)DBNull.Value : busca.Trim());
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new Insumo
                        {
                            IdInsumo = rd.GetInt32("id_insumo"),
                            Nome = rd.GetString("nome"),
                            Estoque = Convert.ToInt32(rd["estoque"])
                        });
                    }
                }
            }
            return lista;
        }

        public static int inserir(string nome, int? estoque)
        {
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_insumo_inserir", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_nome", nome);
                cmd.Parameters.AddWithValue("p_estoque", (object)estoque ?? DBNull.Value);
                var pId = new MySqlParameter("p_id_insumo", MySqlDbType.Int32)
                    { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(pId);
                conn.Open();
                cmd.ExecuteNonQuery();
                return Convert.ToInt32(pId.Value);
            }
        }

        public static void atualizar(int id, string nome)
        {
            Executar("sp_insumo_atualizar", cmd =>
            {
                cmd.Parameters.AddWithValue("p_id_insumo", id);
                cmd.Parameters.AddWithValue("p_nome", nome);
            });
        }

        public static void entrada(int id, int? quantidade)
        {
            Executar("sp_insumo_entrada", cmd =>
            {
                cmd.Parameters.AddWithValue("p_id_insumo", id);
                cmd.Parameters.AddWithValue("p_quantidade", (object)quantidade ?? DBNull.Value);
            });
        }

        public static void ajustar(int id, int? novoEstoque)
        {
            Executar("sp_insumo_ajustar", cmd =>
            {
                cmd.Parameters.AddWithValue("p_id_insumo", id);
                cmd.Parameters.AddWithValue("p_novo_estoque", (object)novoEstoque ?? DBNull.Value);
            });
        }

        public static void excluir(int id)
        {
            Executar("sp_insumo_excluir", cmd =>
            {
                cmd.Parameters.AddWithValue("p_id_insumo", id);
            });
        }

        public static List<ItemReceita> listarReceita(int idProduto)
        {
            var lista = new List<ItemReceita>();
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_receita_listar", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_id_produto", idProduto);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new ItemReceita
                        {
                            IdInsumo = rd.GetInt32("id_insumo"),
                            Insumo = rd.GetString("insumo"),
                            Quantidade = Convert.ToInt32(rd["quantidade"]),
                            Estoque = Convert.ToInt32(rd["estoque"])
                        });
                    }
                }
            }
            return lista;
        }

        public static void definirReceita(int idProduto, int? idInsumo, int? quantidade)
        {
            Executar("sp_receita_definir", cmd =>
            {
                cmd.Parameters.AddWithValue("p_id_produto", idProduto);
                cmd.Parameters.AddWithValue("p_id_insumo", (object)idInsumo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_quantidade", (object)quantidade ?? DBNull.Value);
            });
        }

        public static void removerReceita(int idProduto, int idInsumo)
        {
            Executar("sp_receita_remover", cmd =>
            {
                cmd.Parameters.AddWithValue("p_id_produto", idProduto);
                cmd.Parameters.AddWithValue("p_id_insumo", idInsumo);
            });
        }

        private static void Executar(string procedure, Action<MySqlCommand> parametros)
        {
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand(procedure, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                parametros(cmd);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
