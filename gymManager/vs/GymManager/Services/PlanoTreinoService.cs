
using global::GymManager.Data;
using global::GymManager.Models.GymManager.Models;
using GymManager.Data;
using GymManager.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GymManager.Services
{
    public class PlanoTreinoService
    {
        private readonly DataBase db = new DataBase();

        public List<PlanoTreino> Listar()
        {
            List<PlanoTreino> lista = new List<PlanoTreino>();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_PlanosTreino_Listar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearPlanoTreino(reader));
            }

            return lista;
        }

        public PlanoTreino? ObterPorId(int idPlanoTreino)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_PlanosTreino_ObterPorId", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdPlanoTreino", SqlDbType.Int).Value = idPlanoTreino;

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapearPlanoTreino(reader);
        }

        public void Inserir(PlanoTreino plano)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_PlanosTreino_Inserir", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            AdicionarParametros(cmd, plano);

            cmd.ExecuteNonQuery();
        }

        public void Atualizar(PlanoTreino plano)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_PlanosTreino_Atualizar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdPlanoTreino", SqlDbType.Int).Value = plano.IdPlanoTreino;

            AdicionarParametros(cmd, plano);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int idPlanoTreino)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_PlanosTreino_Eliminar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdPlanoTreino", SqlDbType.Int).Value = idPlanoTreino;

            cmd.ExecuteNonQuery();
        }

        public List<PlanoTreino> Pesquisar(string pesquisa)
        {
            List<PlanoTreino> lista = new List<PlanoTreino>();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_PlanosTreino_Pesquisar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Pesquisa", SqlDbType.NVarChar, 100).Value = pesquisa;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearPlanoTreino(reader));
            }

            return lista;
        }

        private static void AdicionarParametros(SqlCommand cmd, PlanoTreino plano)
        {
            cmd.Parameters.Add("@IdCliente", SqlDbType.Int).Value = plano.IdCliente;

            cmd.Parameters.Add("@IdPT", SqlDbType.Int).Value = plano.IdPT;

            cmd.Parameters.Add("@NomePlano", SqlDbType.NVarChar, 100).Value = plano.NomePlano.Trim();

            cmd.Parameters.Add("@Objetivo", SqlDbType.NVarChar, 255).Value = plano.Objetivo.Trim();

            cmd.Parameters.Add("@DataInicio", SqlDbType.Date).Value = plano.DataInicio.Date;

            cmd.Parameters.Add("@DataFim", SqlDbType.Date).Value = plano.DataFim.Date;

            cmd.Parameters.Add("@Observacoes", SqlDbType.NVarChar, 255).Value = string.IsNullOrWhiteSpace(plano.Observacoes) ? DBNull.Value : plano.Observacoes.Trim();

            cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = plano.Estado.Trim();
        }

        private static PlanoTreino MapearPlanoTreino(SqlDataReader reader)
        {
            return new PlanoTreino
            {
                IdPlanoTreino = Convert.ToInt32(reader["IdPlanoTreino"]),

                IdCliente = Convert.ToInt32(reader["IdCliente"]),

                NomeCliente = reader["NomeCliente"].ToString() ?? string.Empty,

                IdPT = Convert.ToInt32(reader["IdPT"]),

                NomePT = reader["NomePT"].ToString() ?? string.Empty,

                NomePlano = reader["NomePlano"].ToString() ?? string.Empty,

                Objetivo = reader["Objetivo"].ToString() ?? string.Empty,

                DataInicio = Convert.ToDateTime(reader["DataInicio"]),

                DataFim = Convert.ToDateTime(reader["DataFim"]),

                Observacoes = reader["Observacoes"] == DBNull.Value ? string.Empty : reader["Observacoes"].ToString() ?? string.Empty,

                Estado = reader["Estado"].ToString() ?? string.Empty
            };
        }
    }
}

