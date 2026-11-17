using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotDogBrasilia.Domain;
using MySql.Data.MySqlClient;

namespace HotDogBrasilia.Data.Repositories
{
    public class RelatorioRepository
    {
        public static List<LinhaRelatorioVenda> listarVendas(DateTime? ini, DateTime? fim)
        {
            var lista = new List<LinhaRelatorioVenda>();
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_relatorio_vendas", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_data_ini",
                    ini.HasValue ? (object)ini.Value.Date : DBNull.Value);
                cmd.Parameters.AddWithValue("p_data_fim",
                    fim.HasValue ? (object)fim.Value.Date : DBNull.Value);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new LinhaRelatorioVenda
                        {
                            IdVenda = rd.GetInt32("id_venda"),
                            DataHora = rd.GetDateTime("data_hora"),
                            Cliente = rd.GetString("cliente"),
                            Total = rd.GetDecimal("total")
                        });
                    }
                }
            }
            return lista;
        }
        public static ResumoVendas resumoVendas(DateTime? ini, DateTime? fim)
        {
            var resumo = new ResumoVendas();
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_relatorio_vendas_resumo", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_data_ini",
                    ini.HasValue ? (object)ini.Value.Date : DBNull.Value);
                cmd.Parameters.AddWithValue("p_data_fim",
                    fim.HasValue ? (object)fim.Value.Date : DBNull.Value);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        resumo.QtdVendas = Convert.ToInt32(rd["qtd_vendas"]);
                        resumo.TotalVendido = rd.GetDecimal("total_vendido");
                        resumo.TicketMedio = rd.GetDecimal("ticket_medio");
                    }
                }
            }
            return resumo;
        }
        public static List<LinhaRelatorioEstoque> listarEstoque(int? limite)
        {
            var lista = new List<LinhaRelatorioEstoque>();
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_relatorio_estoque", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_limite_baixo",
                    limite.HasValue ? (object)limite.Value : DBNull.Value);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new LinhaRelatorioEstoque
                        {
                            Produto = rd.GetString("produto"),
                            Estoque = rd.GetInt32("estoque"),
                            Fornecedor = rd.GetString("fornecedor"),
                            Situacao = rd.GetString("situacao")
                        });
                    }
                }
            }
            return lista;
        }
        public static ResumoFinanceiro resumoFinanceiro(DateTime? ini, DateTime? fim)
        {
            var resumo = new ResumoFinanceiro();
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_relatorio_financeiro", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_data_ini",
                    ini.HasValue ? (object)ini.Value.Date : DBNull.Value);
                cmd.Parameters.AddWithValue("p_data_fim",
                    fim.HasValue ? (object)fim.Value.Date : DBNull.Value);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        resumo.Recebido = rd.GetDecimal("recebido");
                        resumo.AReceber = rd.GetDecimal("a_receber");
                        resumo.Pago = rd.GetDecimal("pago");
                        resumo.APagar = rd.GetDecimal("a_pagar");
                        resumo.SaldoRealizado = rd.GetDecimal("saldo_realizado");
                        resumo.SaldoPrevisto = rd.GetDecimal("saldo_previsto");
                    }
                }
            }
            return resumo;
        }
    }
}
