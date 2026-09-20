using System;
using System.Collections.Generic;
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
    }
}
