using GymManager.Data;
using GymManager.Models;
using GymManager.Models.GymManager.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GymManager.Services
{
    public class ReservaAulaService
    {
        private readonly DataBase db = new DataBase();

        public List<ReservaAula> Listar()
        {
            List<ReservaAula> lista = new();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_ReservasAulas_Listar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearReserva(reader));
            }

            return lista;
        }

        public List<ReservaAula> ListarPorAula(int idAula)
        {
            List<ReservaAula> lista = new();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_ReservasAulas_ListarPorAula", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdAula", SqlDbType.Int).Value = idAula;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearReserva(reader));
            }

            return lista;
        }

        public List<ReservaAula> Pesquisar(string pesquisa)
        {
            List<ReservaAula> lista = new();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_ReservasAulas_Pesquisar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Pesquisa", SqlDbType.NVarChar, 100).Value = pesquisa;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearReserva(reader));
            }

            return lista;
        }

        public void Inserir(int idAula, int idCliente)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_ReservasAulas_Inserir", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdAula", SqlDbType.Int).Value = idAula;

            cmd.Parameters.Add("@IdCliente", SqlDbType.Int).Value = idCliente;

            cmd.ExecuteNonQuery();
        }

        public void Cancelar(int idReserva)
        {
            ExecutarAcao("dbo.sp_ReservasAulas_Cancelar", idReserva);
        }

        public void MarcarPresente(int idReserva)
        {
            ExecutarAcao("dbo.sp_ReservasAulas_MarcarPresente", idReserva);
        }

        public void MarcarFalta(int idReserva)
        {
            ExecutarAcao("dbo.sp_ReservasAulas_MarcarFalta", idReserva);
        }

        private void ExecutarAcao(string procedure, int idReserva)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand(procedure, conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdReserva", SqlDbType.Int).Value = idReserva;

            cmd.ExecuteNonQuery();
        }

        private static ReservaAula MapearReserva(SqlDataReader reader)
        {
            return new ReservaAula
            {
                IdReserva = Convert.ToInt32(reader["IdReserva"]),

                IdAula = Convert.ToInt32(reader["IdAula"]),

                NomeAula = reader["NomeAula"].ToString() ?? string.Empty,

                DataAula = Convert.ToDateTime(reader["DataAula"]),

                HoraInicio = reader["HoraInicio"] is TimeSpan hora ? hora : TimeSpan.Zero,

                Sala = reader["Sala"].ToString() ?? string.Empty,

                IdCliente = Convert.ToInt32(reader["IdCliente"]),

                NomeCliente = reader["NomeCliente"].ToString() ?? string.Empty,

                NIF = reader["NIF"].ToString() ?? string.Empty,

                DataReserva = Convert.ToDateTime(reader["DataReserva"]),

                Estado = reader["Estado"].ToString() ?? string.Empty
            };
        }
    }
}