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
    }
}
