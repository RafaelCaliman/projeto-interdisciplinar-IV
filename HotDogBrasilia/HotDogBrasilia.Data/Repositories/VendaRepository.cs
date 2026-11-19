using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using HotDogBrasilia.Domain;

namespace HotDogBrasilia.Data.Repositories
{
    public static class VendaRepository
    {
        // Grava a venda inteira de uma vez, dentro de uma transação:
        // cria a venda, inclui os itens (o banco valida e baixa o estoque),
        // conclui (calcula o total e lança no financeiro) e registra o log.
        // Se qualquer passo falhar, o Rollback desfaz tudo, inclusive o estoque.
        public static Venda registrarVenda(int? idCliente, List<ItemVenda> itens,
                                           string statusPagamento, int idUsuario)
        {
            using (var conn = ConexaoBanco.ObterConexao())
            {
                conn.Open();
                MySqlTransaction tx = conn.BeginTransaction();
                try
                {
                    int idVenda = inserirVenda(conn, tx, idCliente);

                    foreach (ItemVenda item in itens)
                        inserirItem(conn, tx, idVenda, item.IdProduto, item.Quantidade);

                    decimal total = concluirVenda(conn, tx, idVenda, statusPagamento);

                    LogRepository.registrar(conn, tx, idUsuario, "INSERIR", "venda", idVenda,
                        itens.Count + " item(ns) - total R$ " + total.ToString("N2", new System.Globalization.CultureInfo("pt-BR")));

                    tx.Commit();
                    return new Venda { IdVenda = idVenda, Total = total };
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        // Cancela a venda (o banco devolve o estoque) e registra o log junto.
        // O detalhe guarda cliente e total, porque a venda é apagada do banco.
        public static void cancelarVenda(int idVenda, int idUsuario, string detalhe)
        {
            using (var conn = ConexaoBanco.ObterConexao())
            {
                conn.Open();
                MySqlTransaction tx = conn.BeginTransaction();
                try
                {
                    using (var cmd = new MySqlCommand("sp_venda_cancelar", conn, tx))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("p_id_venda", idVenda);
                        cmd.ExecuteNonQuery();
                    }

                    LogRepository.registrar(conn, tx, idUsuario, "CANCELAR", "venda", idVenda, detalhe);

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public static List<Venda> listarVendas(DateTime? dataInicio, DateTime? dataFim)
        {
            var lista = new List<Venda>();
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_venda_listar", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_data_ini",
                    dataInicio.HasValue ? (object)dataInicio.Value.Date : DBNull.Value);
                cmd.Parameters.AddWithValue("p_data_fim",
                    dataFim.HasValue ? (object)dataFim.Value.Date : DBNull.Value);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new Venda
                        {
                            IdVenda = rd.GetInt32("id_venda"),
                            DataHora = rd.GetDateTime("data_hora"),
                            NomeCliente = rd.GetString("cliente"),
                            Total = rd.GetDecimal("total"),
                            Situacao = rd.GetString("situacao")
                        });
                    }
                }
            }
            return lista;
        }

        public static List<ItemVenda> listarItens(int idVenda)
        {
            var lista = new List<ItemVenda>();
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_venda_itens", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_id_venda", idVenda);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new ItemVenda
                        {
                            IdItem = rd.GetInt32("id_item"),
                            NomeProduto = rd.GetString("produto"),
                            Quantidade = rd.GetInt32("quantidade"),
                            ValorUnitario = rd.GetDecimal("valor_unitario")
                        });
                    }
                }
            }
            return lista;
        }

        private static int inserirVenda(MySqlConnection conn, MySqlTransaction tx, int? idCliente)
        {
            using (var cmd = new MySqlCommand("sp_venda_inserir", conn, tx))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_id_cliente", (object)idCliente ?? DBNull.Value);
                var pId = new MySqlParameter("p_id_venda", MySqlDbType.Int32)
                    { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(pId);
                cmd.ExecuteNonQuery();
                return Convert.ToInt32(pId.Value);
            }
        }

        private static void inserirItem(MySqlConnection conn, MySqlTransaction tx,
                                        int idVenda, int idProduto, int quantidade)
        {
            using (var cmd = new MySqlCommand("sp_item_venda_inserir", conn, tx))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_id_venda", idVenda);
                cmd.Parameters.AddWithValue("p_id_produto", idProduto);
                cmd.Parameters.AddWithValue("p_quantidade", quantidade);
                var pId = new MySqlParameter("p_id_item", MySqlDbType.Int32)
                    { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(pId);
                cmd.ExecuteNonQuery();
            }
        }

        private static decimal concluirVenda(MySqlConnection conn, MySqlTransaction tx,
                                             int idVenda, string statusPagamento)
        {
            using (var cmd = new MySqlCommand("sp_venda_concluir", conn, tx))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_id_venda", idVenda);
                cmd.Parameters.AddWithValue("p_status_pagamento", statusPagamento);
                var pTotal = new MySqlParameter("p_total", MySqlDbType.Decimal)
                    { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(pTotal);
                cmd.ExecuteNonQuery();
                return Convert.ToDecimal(pTotal.Value);
            }
        }
    }
}
