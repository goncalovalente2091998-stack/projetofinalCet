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
    public class PlanoService
    {
        private readonly DataBase db = new DataBase();

        public List<Plano> Listar()
        {
            List<Plano> lista = new();

            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd =
                new SqlCommand("sp_Planos_Listar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Plano
                {
                    IdPlano = Convert.ToInt32(reader["IdPlano"]),
                    Nome = reader["Nome"].ToString() ?? string.Empty,
                    Preco = Convert.ToDecimal(reader["Preco"]),
                    DuracaoMeses = Convert.ToInt32(reader["DuracaoMeses"]),
                    Descricao = reader["Descricao"] == DBNull.Value
                        ? string.Empty
                        : reader["Descricao"].ToString()
                });
            }

            return lista;
        }

        public void Inserir(Plano plano)
        {
            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd =
                new SqlCommand("sp_Planos_Inserir", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Nome", plano.Nome);
            cmd.Parameters.AddWithValue("@Preco", plano.Preco);
            cmd.Parameters.AddWithValue("@DuracaoMeses", plano.DuracaoMeses);
            cmd.Parameters.AddWithValue(
                "@Descricao",
                string.IsNullOrWhiteSpace(plano.Descricao)
                    ? DBNull.Value
                    : plano.Descricao);

            cmd.ExecuteNonQuery();
        }

        public void Atualizar(Plano plano)
        {
            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd =
                new SqlCommand("sp_Planos_Atualizar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdPlano", plano.IdPlano);
            cmd.Parameters.AddWithValue("@Nome", plano.Nome);
            cmd.Parameters.AddWithValue("@Preco", plano.Preco);
            cmd.Parameters.AddWithValue("@DuracaoMeses", plano.DuracaoMeses);
            cmd.Parameters.AddWithValue(
                "@Descricao",
                string.IsNullOrWhiteSpace(plano.Descricao)
                    ? DBNull.Value
                    : plano.Descricao);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int idPlano)
        {
            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd =
                new SqlCommand("sp_Planos_Eliminar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdPlano", idPlano);

            cmd.ExecuteNonQuery();
        }
        public List<Plano> Pesquisar(string pesquisa)
        {
            List<Plano> lista = new();

            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd =
                new SqlCommand("sp_Planos_Pesquisar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@Pesquisa",
                SqlDbType.NVarChar,
                100).Value = pesquisa;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Plano
                {
                    IdPlano = Convert.ToInt32(reader["IdPlano"]),
                    Nome = reader["Nome"].ToString() ?? string.Empty,
                    Preco = Convert.ToDecimal(reader["Preco"]),
                    DuracaoMeses = Convert.ToInt32(reader["DuracaoMeses"]),
                    Descricao = reader["Descricao"] == DBNull.Value
                        ? string.Empty
                        : reader["Descricao"].ToString()
                });
            }

            return lista;
        }
    }
}

