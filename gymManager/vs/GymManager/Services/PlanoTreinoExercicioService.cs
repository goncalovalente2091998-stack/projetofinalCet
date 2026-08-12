using GymManager.Data;
using GymManager.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GymManager.Services
{
    public class PlanoTreinoExercicioService
    {
        private readonly DataBase db = new DataBase();

        public List<PlanoTreinoExercicio> ListarPorPlano(int idPlanoTreino)
        {
            List<PlanoTreinoExercicio> lista = new();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_PlanoTreinoExercicios_ListarPorPlano", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdPlanoTreino", SqlDbType.Int).Value = idPlanoTreino;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(Mapear(reader));
            }

            return lista;
        }

        public void Inserir(PlanoTreinoExercicio item)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_PlanoTreinoExercicios_Inserir", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            AdicionarParametrosInserir(cmd, item);

            cmd.ExecuteNonQuery();
        }

        public void Atualizar(PlanoTreinoExercicio item)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_PlanoTreinoExercicios_Atualizar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdPlanoTreinoExercicio", SqlDbType.Int).Value = item.IdPlanoTreinoExercicio;

            AdicionarParametrosComuns(cmd, item);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int idPlanoTreinoExercicio)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_PlanoTreinoExercicios_Eliminar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdPlanoTreinoExercicio", SqlDbType.Int).Value = idPlanoTreinoExercicio;

            cmd.ExecuteNonQuery();
        }

        private static void AdicionarParametrosInserir(SqlCommand cmd, PlanoTreinoExercicio item)
        {
            cmd.Parameters.Add("@IdPlanoTreino", SqlDbType.Int).Value = item.IdPlanoTreino;

            AdicionarParametrosComuns(cmd, item);
        }

        private static void AdicionarParametrosComuns(SqlCommand cmd, PlanoTreinoExercicio item)
        {
            cmd.Parameters.Add("@IdExercicio", SqlDbType.Int).Value = item.IdExercicio;

            cmd.Parameters.Add("@Series", SqlDbType.Int).Value = item.Series;

            cmd.Parameters.Add("@Repeticoes", SqlDbType.Int).Value = item.Repeticoes;

            cmd.Parameters.Add("@TempoDescanso", SqlDbType.Int).Value = item.TempoDescanso;

            cmd.Parameters.Add("@Ordem", SqlDbType.Int).Value = item.Ordem;

            cmd.Parameters.Add("@Observacoes", SqlDbType.NVarChar, 255).Value = string.IsNullOrWhiteSpace(item.Observacoes) ? DBNull.Value : item.Observacoes.Trim();
        }

        private static PlanoTreinoExercicio Mapear(SqlDataReader reader)
        {
            return new PlanoTreinoExercicio
            {
                IdPlanoTreinoExercicio = Convert.ToInt32(reader["IdPlanoTreinoExercicio"]),

                IdPlanoTreino = Convert.ToInt32(reader["IdPlanoTreino"]),

                IdExercicio = Convert.ToInt32(reader["IdExercicio"]),

                NomeExercicio = reader["NomeExercicio"].ToString() ?? string.Empty,

                GrupoMuscular = reader["GrupoMuscular"].ToString() ?? string.Empty,

                Equipamento = reader["Equipamento"] == DBNull.Value ? string.Empty : reader["Equipamento"].ToString() ?? string.Empty,

                Series = Convert.ToInt32(reader["Series"]),

                Repeticoes = Convert.ToInt32(reader["Repeticoes"]),

                TempoDescanso = Convert.ToInt32(reader["TempoDescanso"]),

                Ordem = Convert.ToInt32(reader["Ordem"]),

                Observacoes = reader["Observacoes"] == DBNull.Value ? string.Empty : reader["Observacoes"].ToString() ?? string.Empty
            };

        }
        public void TrocarOrdem(int idPlanoTreinoExercicio, string direcao)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_PlanoTreinoExercicios_TrocarOrdem", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdPlanoTreinoExercicio", SqlDbType.Int).Value = idPlanoTreinoExercicio;

            cmd.Parameters.Add("@Direcao", SqlDbType.NVarChar, 10).Value = direcao;

            cmd.ExecuteNonQuery();
        }
    }
}