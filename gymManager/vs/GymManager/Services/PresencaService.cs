using GymManager.Data;
using GymManager.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GymManager.Services
{
    public class PresencaService
    {
        private readonly DataBase db = new DataBase();

        public List<Presenca> Listar()
        {
            List<Presenca> lista = new List<Presenca>();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Presencas_Listar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearPresenca(reader));
            }

            return lista;
        }

        public List<Presenca> ListarAtivas()
        {
            List<Presenca> lista = new List<Presenca>();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Presencas_ListarAtivas", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearPresenca(reader));
            }

            return lista;
        }

        public Presenca? ObterPorId(int idPresenca)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Presencas_ObterPorId", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdPresenca", SqlDbType.Int).Value = idPresenca;

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapearPresenca(reader);
        }

        public void RegistarEntrada(int idCliente, string? observacoes)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Presencas_RegistarEntrada", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdCliente", SqlDbType.Int).Value = idCliente;

            cmd.Parameters.Add("@Observacoes", SqlDbType.NVarChar, 255).Value = string.IsNullOrWhiteSpace(observacoes) ? DBNull.Value : observacoes.Trim();

            cmd.ExecuteNonQuery();
        }

        public void RegistarSaida(int idCliente)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Presencas_RegistarSaida", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdCliente", SqlDbType.Int).Value = idCliente;

            cmd.ExecuteNonQuery();
        }

        public List<Presenca> Pesquisar(string pesquisa)
        {
            List<Presenca> lista = new List<Presenca>();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Presencas_Pesquisar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Pesquisa", SqlDbType.NVarChar, 100).Value = pesquisa.Trim();

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearPresenca(reader));
            }

            return lista;
        }

        public void Eliminar(int idPresenca)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Presencas_Eliminar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdPresenca", SqlDbType.Int).Value = idPresenca;

            cmd.ExecuteNonQuery();
        }

        private static Presenca MapearPresenca(SqlDataReader reader)
        {
            return new Presenca
            {
                IdPresenca = Convert.ToInt32(reader["IdPresenca"]),

                IdCliente = Convert.ToInt32(reader["IdCliente"]),

                NomeCliente = reader["NomeCliente"].ToString() ?? string.Empty,

                NIF = reader["NIF"] == DBNull.Value ? string.Empty : reader["NIF"].ToString() ?? string.Empty,

                DataEntrada = Convert.ToDateTime(reader["DataEntrada"]),

                DataSaida = reader["DataSaida"] == DBNull.Value ? null : Convert.ToDateTime(reader["DataSaida"]),

                Observacoes = reader["Observacoes"] == DBNull.Value ? string.Empty : reader["Observacoes"].ToString() ?? string.Empty
            };
        }
    }
}