using GymManager.Data;
using GymManager.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GymManager.Services
{
    public class ManutencaoService
    {
        private readonly DataBase db = new DataBase();

        public List<Manutencao> Listar()
        {
            List<Manutencao> lista = new List<Manutencao>();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Manutencoes_Listar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearManutencao(reader));
            }

            return lista;
        }

        public Manutencao? ObterPorId(int idManutencao)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Manutencoes_ObterPorId", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdManutencao", SqlDbType.Int).Value = idManutencao;

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapearManutencao(reader);
        }

        public void Inserir(Manutencao manutencao)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Manutencoes_Inserir", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            AdicionarParametros(cmd, manutencao);

            cmd.ExecuteNonQuery();
        }

        public void Atualizar(Manutencao manutencao)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Manutencoes_Atualizar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdManutencao", SqlDbType.Int).Value = manutencao.IdManutencao;

            AdicionarParametros(cmd, manutencao);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int idManutencao)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Manutencoes_Eliminar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdManutencao", SqlDbType.Int).Value = idManutencao;

            cmd.ExecuteNonQuery();
        }

        public List<Manutencao> Pesquisar(string pesquisa)
        {
            List<Manutencao> lista = new List<Manutencao>();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Manutencoes_Pesquisar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Pesquisa", SqlDbType.NVarChar, 100).Value = pesquisa;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearManutencao(reader));
            }

            return lista;
        }

        private static void AdicionarParametros(SqlCommand cmd, Manutencao manutencao)
        {
            cmd.Parameters.Add("@IdEquipamento", SqlDbType.Int).Value = manutencao.IdEquipamento;

            cmd.Parameters.Add("@Tipo", SqlDbType.NVarChar, 30).Value = manutencao.Tipo.Trim();

            cmd.Parameters.Add("@DataAgendada", SqlDbType.Date).Value = manutencao.DataAgendada.Date;

            cmd.Parameters.Add("@DataRealizacao", SqlDbType.Date).Value = manutencao.DataRealizacao.HasValue ? manutencao.DataRealizacao.Value.Date : DBNull.Value;

            cmd.Parameters.Add("@Descricao", SqlDbType.NVarChar, 500).Value = manutencao.Descricao.Trim();

            cmd.Parameters.Add("@Responsavel", SqlDbType.NVarChar, 100).Value = string.IsNullOrWhiteSpace(manutencao.Responsavel) ? DBNull.Value : manutencao.Responsavel.Trim();

            SqlParameter custo = cmd.Parameters.Add("@Custo", SqlDbType.Decimal);

            custo.Precision = 10;

            custo.Scale = 2;

            custo.Value = manutencao.Custo.HasValue ? manutencao.Custo.Value : DBNull.Value;

            cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 30).Value = manutencao.Estado.Trim();

            cmd.Parameters.Add("@Observacoes", SqlDbType.NVarChar, 500).Value = string.IsNullOrWhiteSpace(manutencao.Observacoes) ? DBNull.Value : manutencao.Observacoes.Trim();
        }

        private static Manutencao MapearManutencao(SqlDataReader reader)
        {
            return new Manutencao
            {
                IdManutencao = Convert.ToInt32(reader["IdManutencao"]),

                IdEquipamento = Convert.ToInt32(reader["IdEquipamento"]),

                NomeEquipamento = reader["NomeEquipamento"].ToString() ?? string.Empty,

                Marca = reader["Marca"] == DBNull.Value ? string.Empty : reader["Marca"].ToString() ?? string.Empty,

                Modelo = reader["Modelo"] == DBNull.Value ? string.Empty : reader["Modelo"].ToString() ?? string.Empty,

                Tipo = reader["Tipo"].ToString() ?? string.Empty,

                DataAgendada = Convert.ToDateTime(reader["DataAgendada"]),

                DataRealizacao = reader["DataRealizacao"] == DBNull.Value ? null : Convert.ToDateTime(reader["DataRealizacao"]),

                Descricao = reader["Descricao"].ToString() ?? string.Empty,

                Responsavel = reader["Responsavel"] == DBNull.Value ? string.Empty : reader["Responsavel"].ToString() ?? string.Empty,

                Custo = reader["Custo"] == DBNull.Value ? null : Convert.ToDecimal(reader["Custo"]),

                Estado = reader["Estado"].ToString() ?? string.Empty,

                Observacoes = reader["Observacoes"] == DBNull.Value ? string.Empty : reader["Observacoes"].ToString() ?? string.Empty
            };
        }
    }
}