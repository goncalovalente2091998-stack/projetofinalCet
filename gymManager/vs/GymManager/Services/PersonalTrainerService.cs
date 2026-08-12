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
    public class PersonalTrainerService
    {
        private readonly DataBase db = new DataBase();
        public void Inserir(PersonalTrainer pt)
        {
            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd =
                new SqlCommand("sp_PersonalTrainers_Inserir", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Nome", pt.Nome);
            cmd.Parameters.AddWithValue("@Especialidade", pt.Especialidade);
            cmd.Parameters.AddWithValue("@Telefone", pt.Telefone);
            cmd.Parameters.AddWithValue("@Email", pt.Email);
            cmd.Parameters.AddWithValue("@ValorHora", pt.ValorHora);
            cmd.Parameters.AddWithValue("@Estado", pt.Estado);

            cmd.ExecuteNonQuery();
        }
        public List<PersonalTrainer> Listar()
        {
            List<PersonalTrainer> lista = new();

            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd = new("sp_PersonalTrainers_Listar", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new PersonalTrainer
                {
                    IdPT = Convert.ToInt32(reader["IdPT"]),
                    Nome = reader["Nome"].ToString() ?? "",
                    Especialidade = reader["Especialidade"].ToString() ?? "",
                    Telefone = reader["Telefone"].ToString() ?? "",
                    Email = reader["Email"].ToString() ?? "",
                    ValorHora = Convert.ToDecimal(reader["ValorHora"]),
                    Estado = Convert.ToBoolean(reader["Estado"])
                });
            }

            return lista;
        }

        public void Atualizar(PersonalTrainer pt)
        {
            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd = new("sp_PersonalTrainers_Atualizar", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdPT", pt.IdPT);
            cmd.Parameters.AddWithValue("@Nome", pt.Nome);
            cmd.Parameters.AddWithValue("@Especialidade", pt.Especialidade);
            cmd.Parameters.AddWithValue("@Telefone", pt.Telefone);
            cmd.Parameters.AddWithValue("@Email", pt.Email);
            cmd.Parameters.AddWithValue("@ValorHora", pt.ValorHora);
            cmd.Parameters.AddWithValue("@Estado", pt.Estado);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd = new("sp_PersonalTrainers_Eliminar", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdPT", id);

            cmd.ExecuteNonQuery();
        }

        public List<PersonalTrainer> Pesquisar(string pesquisa)
        {
            List<PersonalTrainer> lista = new();

            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd = new("sp_PersonalTrainers_Pesquisar", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Pesquisa", pesquisa);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new PersonalTrainer
                {
                    IdPT = Convert.ToInt32(reader["IdPT"]),
                    Nome = reader["Nome"].ToString() ?? "",
                    Especialidade = reader["Especialidade"].ToString() ?? "",
                    Telefone = reader["Telefone"].ToString() ?? "",
                    Email = reader["Email"].ToString() ?? "",
                    ValorHora = Convert.ToDecimal(reader["ValorHora"]),
                    Estado = Convert.ToBoolean(reader["Estado"])
                });
            }

            return lista;
        }
        public bool ExisteEmail(string email, int idPT = 0)
        {
            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd = new(
                @"SELECT COUNT(*)
                  FROM PersonalTrainers
                  WHERE Email=@Email
                  AND IdPT<>@IdPT", conn);

            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@IdPT", idPT);

            return (int)cmd.ExecuteScalar() > 0;
        }
    }
}
