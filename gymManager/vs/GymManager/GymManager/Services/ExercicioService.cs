using GymManager.Data;
using GymManager.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GymManager.Services
{
    public class ExercicioService
    {
        private readonly DataBase db =
            new DataBase();

        public List<Exercicio> Listar()
        {
            List<Exercicio> lista =
                new List<Exercicio>();

            using SqlConnection conn =
                db.GetConnection();

            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(
                    "dbo.sp_Exercicios_Listar",
                    conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            using SqlDataReader reader =
                cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(
                    MapearExercicio(reader));
            }

            return lista;
        }

        public Exercicio? ObterPorId(
            int idExercicio)
        {
            using SqlConnection conn =
                db.GetConnection();

            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(
                    "dbo.sp_Exercicios_ObterPorId",
                    conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@IdExercicio",
                SqlDbType.Int).Value =
                idExercicio;

            using SqlDataReader reader =
                cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapearExercicio(reader);
        }

        public void Inserir(
            Exercicio exercicio)
        {
            using SqlConnection conn =
                db.GetConnection();

            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(
                    "dbo.sp_Exercicios_Inserir",
                    conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            AdicionarParametros(
                cmd,
                exercicio);

            cmd.ExecuteNonQuery();
        }

        public void Atualizar(
            Exercicio exercicio)
        {
            using SqlConnection conn =
                db.GetConnection();

            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(
                    "dbo.sp_Exercicios_Atualizar",
                    conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@IdExercicio",
                SqlDbType.Int).Value =
                exercicio.IdExercicio;

            AdicionarParametros(
                cmd,
                exercicio);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(
            int idExercicio)
        {
            using SqlConnection conn =
                db.GetConnection();

            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(
                    "dbo.sp_Exercicios_Eliminar",
                    conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@IdExercicio",
                SqlDbType.Int).Value =
                idExercicio;

            cmd.ExecuteNonQuery();
        }

        public List<Exercicio> Pesquisar(
            string pesquisa)
        {
            List<Exercicio> lista =
                new List<Exercicio>();

            using SqlConnection conn =
                db.GetConnection();

            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(
                    "dbo.sp_Exercicios_Pesquisar",
                    conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@Pesquisa",
                SqlDbType.NVarChar,
                100).Value =
                pesquisa;

            using SqlDataReader reader =
                cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(
                    MapearExercicio(reader));
            }

            return lista;
        }

        private static void AdicionarParametros(
            SqlCommand cmd,
            Exercicio exercicio)
        {
            cmd.Parameters.Add(
                "@Nome",
                SqlDbType.NVarChar,
                100).Value =
                exercicio.Nome.Trim();

            cmd.Parameters.Add(
                "@GrupoMuscular",
                SqlDbType.NVarChar,
                50).Value =
                exercicio.GrupoMuscular.Trim();

            cmd.Parameters.Add(
                "@Equipamento",
                SqlDbType.NVarChar,
                100).Value =
                string.IsNullOrWhiteSpace(
                    exercicio.Equipamento)
                    ? DBNull.Value
                    : exercicio.Equipamento.Trim();

            cmd.Parameters.Add(
                "@Descricao",
                SqlDbType.NVarChar,
                500).Value =
                string.IsNullOrWhiteSpace(
                    exercicio.Descricao)
                    ? DBNull.Value
                    : exercicio.Descricao.Trim();

            cmd.Parameters.Add(
                "@Dificuldade",
                SqlDbType.NVarChar,
                20).Value =
                exercicio.Dificuldade.Trim();

            cmd.Parameters.Add(
                "@Estado",
                SqlDbType.NVarChar,
                20).Value =
                exercicio.Estado.Trim();
        }

        private static Exercicio MapearExercicio(
            SqlDataReader reader)
        {
            return new Exercicio
            {
                IdExercicio =
                    Convert.ToInt32(
                        reader["IdExercicio"]),

                Nome =
                    reader["Nome"].ToString()
                    ?? string.Empty,

                GrupoMuscular =
                    reader["GrupoMuscular"].ToString()
                    ?? string.Empty,

                Equipamento =
                    reader["Equipamento"] == DBNull.Value
                        ? string.Empty
                        : reader["Equipamento"].ToString()
                          ?? string.Empty,

                Descricao =
                    reader["Descricao"] == DBNull.Value
                        ? string.Empty
                        : reader["Descricao"].ToString()
                          ?? string.Empty,

                Dificuldade =
                    reader["Dificuldade"].ToString()
                    ?? string.Empty,

                Estado =
                    reader["Estado"].ToString()
                    ?? string.Empty
            };
        }
    }
}