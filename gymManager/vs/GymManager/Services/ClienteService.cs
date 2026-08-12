using GymManager.Data;
using GymManager.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GymManager.Services
{
    public class ClienteService
    {
        private readonly DataBase db = new DataBase();

        public List<Cliente> Listar()
        {

            List<Cliente> lista = new List<Cliente>();


            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("sp_Clientes_Listar", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Cliente cliente = new Cliente
                    {
                        IdCliente = (int)reader["IdCliente"],
                        Nome = reader["Nome"].ToString(),
                        NIF = reader["NIF"].ToString(),
                        DataNascimento = (DateTime)reader["DataNascimento"],
                        Telefone = reader["Telefone"].ToString(),
                        Email = reader["Email"].ToString(),
                        Morada = reader["Morada"].ToString(),
                        DataInscricao = (DateTime)reader["DataInscricao"],
                        Estado = (bool)reader["Estado"]
                    };

                    lista.Add(cliente);
                }
            }

            return lista;
        }

        public void Inserir(Cliente cliente)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("sp_Clientes_Inserir", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
                cmd.Parameters.AddWithValue("@NIF", cliente.NIF);
                cmd.Parameters.AddWithValue("@DataNascimento", cliente.DataNascimento);
                cmd.Parameters.AddWithValue("@Telefone", cliente.Telefone);
                cmd.Parameters.AddWithValue("@Email", cliente.Email);
                cmd.Parameters.AddWithValue("@Morada", cliente.Morada);
                cmd.Parameters.AddWithValue("@DataInscricao", cliente.DataInscricao);
                cmd.Parameters.AddWithValue("@Estado", cliente.Estado);

                cmd.ExecuteNonQuery();
            }
        }

        public void Atualizar(Cliente cliente)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("sp_Clientes_Atualizar", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);
                cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
                cmd.Parameters.AddWithValue("@NIF", cliente.NIF);
                cmd.Parameters.AddWithValue("@DataNascimento", cliente.DataNascimento);
                cmd.Parameters.AddWithValue("@Telefone", cliente.Telefone);
                cmd.Parameters.AddWithValue("@Email", cliente.Email);
                cmd.Parameters.AddWithValue("@Morada", cliente.Morada);
                cmd.Parameters.AddWithValue("@DataInscricao", cliente.DataInscricao);
                cmd.Parameters.AddWithValue("@Estado", cliente.Estado);

                cmd.ExecuteNonQuery();
            }
        }

        public void Eliminar(int idCliente)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("sp_Clientes_Eliminar", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdCliente", idCliente);

                cmd.ExecuteNonQuery();
            }
        }

        public List<Cliente> Pesquisar(string pesquisa)
        {
            List<Cliente> lista = new List<Cliente>();

            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("sp_Clientes_Pesquisar", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Pesquisa", pesquisa);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Cliente cliente = new Cliente
                    {
                        IdCliente = (int)reader["IdCliente"],
                        Nome = reader["Nome"].ToString(),
                        NIF = reader["NIF"].ToString(),
                        DataNascimento = (DateTime)reader["DataNascimento"],
                        Telefone = reader["Telefone"].ToString(),
                        Email = reader["Email"].ToString(),
                        Morada = reader["Morada"].ToString(),
                        DataInscricao = (DateTime)reader["DataInscricao"],
                        Estado = Convert.ToBoolean(reader["Estado"])
                    };

                    lista.Add(cliente);
                }
            }

            return lista;
        }

        public bool ExisteNIF(string nif, int idCliente = 0)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    @"SELECT COUNT(*)
                    FROM Clientes
                    WHERE NIF = @NIF
                    AND IdCliente <> @IdCliente", conn);

                cmd.Parameters.AddWithValue("@NIF", nif);
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);

                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }
}
