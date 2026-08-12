
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
    public class ProfessoresService
    {
        private readonly DataBase db = new DataBase();

        public List<Professor> Listar()
        {
            List<Professor> lista = new List<Professor>();

            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("sp_Professores_Listar", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Professor professor = new Professor
                    {
                        IdProfessor = (int)reader["IdProfessor"],
                        Nome = reader["Nome"].ToString(),
                        Especialidade = reader["Especialidade"].ToString(),
                        Telefone = reader["Telefone"].ToString(),
                        Email = reader["Email"].ToString()
                    };

                    lista.Add(professor);
                }
            }

            return lista;
        }

        public void Inserir(Professor professor)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("sp_Professores_Inserir", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nome", professor.Nome);
                cmd.Parameters.AddWithValue("@Especialidade", professor.Especialidade);
                cmd.Parameters.AddWithValue("@Telefone", professor.Telefone);
                cmd.Parameters.AddWithValue("@Email", professor.Email);

                cmd.ExecuteNonQuery();
            }
        }
        public void Atualizar(Professor professor)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("sp_Professores_Atualizar", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdProfessor", professor.IdProfessor);
                cmd.Parameters.AddWithValue("@Nome", professor.Nome);
                cmd.Parameters.AddWithValue("@Especialidade", professor.Especialidade);
                cmd.Parameters.AddWithValue("@Telefone", professor.Telefone);
                cmd.Parameters.AddWithValue("@Email", professor.Email);

                cmd.ExecuteNonQuery();
            }
        }
        public void Eliminar(int idProfessor)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("sp_Professores_Eliminar", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdProfessor", idProfessor);

                cmd.ExecuteNonQuery();
            }
        }
        public List<Professor> Pesquisar(string pesquisa)
        {
            List<Professor> lista = new List<Professor>();

            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("sp_Professores_Pesquisar", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Pesquisa", pesquisa);

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Professor professor = new Professor
                    {
                        IdProfessor = Convert.ToInt32(reader["IdProfessor"]),
                        Nome = reader["Nome"].ToString() ?? string.Empty,
                        Especialidade = reader["Especialidade"].ToString() ?? string.Empty,
                        Telefone = reader["Telefone"].ToString() ?? string.Empty,
                        Email = reader["Email"].ToString() ?? string.Empty
                    };

                    lista.Add(professor);
                }
            }

            return lista;
        }
    }
}
