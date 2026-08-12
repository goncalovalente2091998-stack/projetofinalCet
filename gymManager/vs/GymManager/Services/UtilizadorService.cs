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
    public class UtilizadorService
    {
        private readonly DataBase db = new DataBase();

        public Utilizador? ObterPorEmail(string email)
        {
            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd = new SqlCommand("sp_Utilizadores_ObterPorEmail", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return new Utilizador
            {
                IdUtilizador = Convert.ToInt32(reader["IdUtilizador"]),
                Nome = reader["Nome"].ToString() ?? string.Empty,
                Email = reader["Email"].ToString() ?? string.Empty,
                PasswordHash = reader["PasswordHash"].ToString() ?? string.Empty,
                Perfil = reader["Perfil"].ToString() ?? string.Empty
            };
        }

        public List<Utilizador> Listar()
        {
            List<Utilizador> lista = new();

            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd = new SqlCommand("sp_Utilizadores_Listar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Utilizador
                {
                    IdUtilizador = Convert.ToInt32(reader["IdUtilizador"]),
                    Nome = reader["Nome"].ToString() ?? string.Empty,
                    Email = reader["Email"].ToString() ?? string.Empty,
                    Perfil = reader["Perfil"].ToString() ?? string.Empty
                });
            }

            return lista;
        }

        public void Inserir(Utilizador utilizador)
        {
            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd = new SqlCommand("sp_Utilizadores_Inserir", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Nome", utilizador.Nome);
            cmd.Parameters.AddWithValue("@Email", utilizador.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", utilizador.PasswordHash);
            cmd.Parameters.AddWithValue("@Perfil", utilizador.Perfil);

            cmd.ExecuteNonQuery();
        }

        public void Atualizar(Utilizador utilizador)
        {
            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd = new SqlCommand("sp_Utilizadores_Atualizar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdUtilizador", utilizador.IdUtilizador);
            cmd.Parameters.AddWithValue("@Nome", utilizador.Nome);
            cmd.Parameters.AddWithValue("@Email", utilizador.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", utilizador.PasswordHash);
            cmd.Parameters.AddWithValue("@Perfil", utilizador.Perfil);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int idUtilizador)
        {
            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd = new SqlCommand("sp_Utilizadores_Eliminar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdUtilizador", idUtilizador);

            cmd.ExecuteNonQuery();
        }
        public Utilizador? ObterPorId(int idUtilizador)
        {
            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd = new SqlCommand("sp_Utilizadores_ObterPorId", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdUtilizador", idUtilizador);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return new Utilizador
            {
                IdUtilizador = Convert.ToInt32(reader["IdUtilizador"]),

                Nome = reader["Nome"].ToString() ?? string.Empty,

                Email = reader["Email"].ToString() ?? string.Empty,

                PasswordHash = reader["PasswordHash"].ToString() ?? string.Empty,

                Perfil = reader["Perfil"].ToString() ?? string.Empty
            };
        }

        public List<Utilizador> Pesquisar(string pesquisa)
        {
            List<Utilizador> lista = new();

            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd = new SqlCommand("sp_Utilizadores_Pesquisar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Pesquisa", pesquisa);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Utilizador
                {
                    IdUtilizador = Convert.ToInt32(reader["IdUtilizador"]),

                    Nome = reader["Nome"].ToString() ?? string.Empty,

                    Email = reader["Email"].ToString() ?? string.Empty,

                    Perfil = reader["Perfil"].ToString() ?? string.Empty
                });
            }

            return lista;
        }
    }
}


