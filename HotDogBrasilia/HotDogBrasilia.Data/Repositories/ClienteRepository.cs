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
    public class ClienteRepository
    {
        public static List<Cliente> listarClientes()
        {
            List<Cliente> clientes = new List<Cliente>();

            MySqlConnection cn = ConexaoBanco.ObterConexao();

            cn.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "Select id_cliente, nome , telefone from cliente";

            MySqlDataReader resultado = cmd.ExecuteReader();

            while (resultado.Read())
            {
                Cliente cliente = new Cliente();

                cliente.Id = Convert.ToInt32(resultado["id_cliente"]);
                cliente.Nome = resultado["nome"].ToString();
                cliente.Telefone = resultado["telefone"].ToString();

                clientes.Add(cliente);
            }

            cn.Close();

            return clientes;


        }

        public static List<Cliente> listarClientes(string busca)
        {
            var lista = new List<Cliente>();
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_cliente_listar", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_busca",
                    string.IsNullOrWhiteSpace(busca) ? (object)DBNull.Value : busca.Trim());
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new Cliente
                        {
                            Id = rd.GetInt32("id_cliente"),
                            Nome = rd.GetString("nome"),
                            Telefone = rd.GetString("telefone")
                        });
                    }
                }
            }
            return lista;
        }

        public static int inserirCliente(string nome, string telefone)
        {
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_cliente_inserir", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_nome", nome);
                cmd.Parameters.AddWithValue("p_telefone", telefone);
                var pId = new MySqlParameter("p_id_cliente", MySqlDbType.Int32)
                    { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(pId);
                conn.Open();
                cmd.ExecuteNonQuery();
                return Convert.ToInt32(pId.Value);
            }
        }

        public static void atualizarCliente(int id, string nome, string telefone)
        {
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_cliente_atualizar", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_id_cliente", id);
                cmd.Parameters.AddWithValue("p_nome", nome);
                cmd.Parameters.AddWithValue("p_telefone", telefone);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void excluirCliente(int id)
        {
            using (var conn = ConexaoBanco.ObterConexao())
            using (var cmd = new MySqlCommand("sp_cliente_excluir", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_id_cliente", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
