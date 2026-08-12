using GymManager.Data;
using GymManager.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GymManager.Services
{
    public class EventoAgendaService
    {
        private readonly DataBase db = new DataBase();

        public List<EventoAgenda> Listar()
        {
            List<EventoAgenda> lista = new List<EventoAgenda>();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_EventosAgenda_Listar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearEvento(reader));
            }

            return lista;
        }

        public EventoAgenda? ObterPorId(int idEvento)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_EventosAgenda_ObterPorId", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdEvento", SqlDbType.Int).Value = idEvento;

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapearEvento(reader);
        }

        public List<EventoAgenda> ListarPorPeriodo(DateTime dataInicio, DateTime dataFim, int? idPT = null, string? tipo = null)
        {
            List<EventoAgenda> lista = new List<EventoAgenda>();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_EventosAgenda_ListarPorPeriodo", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@DataInicio", SqlDbType.DateTime2).Value = dataInicio;

            cmd.Parameters.Add("@DataFim", SqlDbType.DateTime2).Value = dataFim;

            cmd.Parameters.Add("@IdPT", SqlDbType.Int).Value = idPT.HasValue ? idPT.Value : DBNull.Value;

            cmd.Parameters.Add("@Tipo", SqlDbType.NVarChar, 30).Value = string.IsNullOrWhiteSpace(tipo) ? DBNull.Value : tipo.Trim();

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearEvento(reader));
            }

            return lista;
        }

        public void Inserir(EventoAgenda evento)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_EventosAgenda_Inserir", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            AdicionarParametrosEvento(cmd, evento, incluirId: false);

            cmd.ExecuteNonQuery();
        }

        public void Atualizar(EventoAgenda evento)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_EventosAgenda_Atualizar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            AdicionarParametrosEvento(cmd, evento, incluirId: true);

            cmd.ExecuteNonQuery();
        }

        public void Cancelar(int idEvento)
        {
            ExecutarAcaoPorId("dbo.sp_EventosAgenda_Cancelar", idEvento);
        }

        public void Concluir(int idEvento)
        {
            ExecutarAcaoPorId("dbo.sp_EventosAgenda_Concluir", idEvento);
        }

        private void ExecutarAcaoPorId(string storedProcedure, int idEvento)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand(storedProcedure, conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdEvento", SqlDbType.Int).Value = idEvento;

            cmd.ExecuteNonQuery();
        }

        private static void AdicionarParametrosEvento(SqlCommand cmd, EventoAgenda evento, bool incluirId)
        {
            if (incluirId)
            {
                cmd.Parameters.Add("@IdEvento", SqlDbType.Int).Value = evento.IdEvento;
            }

            cmd.Parameters.Add("@Titulo", SqlDbType.NVarChar, 150).Value = evento.Titulo.Trim();

            cmd.Parameters.Add("@Tipo", SqlDbType.NVarChar, 30).Value = evento.Tipo.Trim();

            cmd.Parameters.Add("@DataInicio", SqlDbType.DateTime2).Value = evento.DataInicio;

            cmd.Parameters.Add("@DataFim", SqlDbType.DateTime2).Value = evento.DataFim;

            cmd.Parameters.Add("@IdPT", SqlDbType.Int).Value = evento.IdPT.HasValue ? evento.IdPT.Value : DBNull.Value;

            cmd.Parameters.Add("@IdProfessor", SqlDbType.Int).Value = evento.IdProfessor.HasValue ? evento.IdProfessor.Value : DBNull.Value;

            cmd.Parameters.Add("@IdCliente", SqlDbType.Int).Value = evento.IdCliente.HasValue ? evento.IdCliente.Value : DBNull.Value;

            cmd.Parameters.Add("@IdAula", SqlDbType.Int).Value = evento.IdAula.HasValue ? evento.IdAula.Value : DBNull.Value;

            cmd.Parameters.Add("@Localizacao", SqlDbType.NVarChar, 100).Value = string.IsNullOrWhiteSpace(evento.Localizacao) ? DBNull.Value : evento.Localizacao.Trim();

            cmd.Parameters.Add("@Descricao", SqlDbType.NVarChar, 500).Value = string.IsNullOrWhiteSpace(evento.Descricao) ? DBNull.Value : evento.Descricao.Trim();

            cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = evento.Estado.Trim();
        }

        private static EventoAgenda MapearEvento(SqlDataReader reader)
        {
            return new EventoAgenda
            {
                IdEvento = Convert.ToInt32(reader["IdEvento"]),

                Titulo = reader["Titulo"] == DBNull.Value ? string.Empty : reader["Titulo"].ToString() ?? string.Empty,

                Tipo = reader["Tipo"] == DBNull.Value ? string.Empty : reader["Tipo"].ToString() ?? string.Empty,

                DataInicio = Convert.ToDateTime(reader["DataInicio"]),

                DataFim = Convert.ToDateTime(reader["DataFim"]),

                IdPT = reader["IdPT"] == DBNull.Value ? null : Convert.ToInt32(reader["IdPT"]),

                IdProfessor = reader["IdProfessor"] == DBNull.Value ? null : Convert.ToInt32(reader["IdProfessor"]),

                NomePT = reader["NomePT"] == DBNull.Value ? string.Empty : reader["NomePT"].ToString() ?? string.Empty,

                IdCliente = reader["IdCliente"] == DBNull.Value ? null : Convert.ToInt32(reader["IdCliente"]),

                NomeCliente = reader["NomeCliente"] == DBNull.Value ? string.Empty : reader["NomeCliente"].ToString() ?? string.Empty,

                IdAula = reader["IdAula"] == DBNull.Value ? null : Convert.ToInt32(reader["IdAula"]),

                NomeAula = reader["NomeAula"] == DBNull.Value ? string.Empty : reader["NomeAula"].ToString() ?? string.Empty,

                Localizacao = reader["Localizacao"] == DBNull.Value ? string.Empty : reader["Localizacao"].ToString() ?? string.Empty,

                Descricao = reader["Descricao"] == DBNull.Value ? string.Empty : reader["Descricao"].ToString() ?? string.Empty,

                Estado = reader["Estado"] == DBNull.Value ? string.Empty : reader["Estado"].ToString() ?? string.Empty
            };
        }
    }
}