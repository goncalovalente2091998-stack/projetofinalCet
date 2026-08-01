using GymManager.Data;
using GymManager.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GymManager.Services
{
    public class EquipamentoService
    {
        private readonly DataBase db =
            new DataBase();

        public List<Equipamento> Listar()
        {
            List<Equipamento> lista =
                new List<Equipamento>();

            using SqlConnection conn =
                db.GetConnection();

            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(
                    "dbo.sp_Equipamentos_Listar",
                    conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            using SqlDataReader reader =
                cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(
                    MapearEquipamento(reader));
            }

            return lista;
        }

        public Equipamento? ObterPorId(
            int idEquipamento)
        {
            using SqlConnection conn =
                db.GetConnection();

            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(
                    "dbo.sp_Equipamentos_ObterPorId",
                    conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@IdEquipamento",
                SqlDbType.Int).Value =
                idEquipamento;

            using SqlDataReader reader =
                cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapearEquipamento(
                reader);
        }

        public void Inserir(
            Equipamento equipamento)
        {
            using SqlConnection conn =
                db.GetConnection();

            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(
                    "dbo.sp_Equipamentos_Inserir",
                    conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            AdicionarParametros(
                cmd,
                equipamento);

            cmd.ExecuteNonQuery();
        }

        public void Atualizar(
            Equipamento equipamento)
        {
            using SqlConnection conn =
                db.GetConnection();

            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(
                    "dbo.sp_Equipamentos_Atualizar",
                    conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@IdEquipamento",
                SqlDbType.Int).Value =
                equipamento.IdEquipamento;

            AdicionarParametros(
                cmd,
                equipamento);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(
            int idEquipamento)
        {
            using SqlConnection conn =
                db.GetConnection();

            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(
                    "dbo.sp_Equipamentos_Eliminar",
                    conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@IdEquipamento",
                SqlDbType.Int).Value =
                idEquipamento;

            cmd.ExecuteNonQuery();
        }

        public List<Equipamento> Pesquisar(
            string pesquisa)
        {
            List<Equipamento> lista =
                new List<Equipamento>();

            using SqlConnection conn =
                db.GetConnection();

            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(
                    "dbo.sp_Equipamentos_Pesquisar",
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
                    MapearEquipamento(reader));
            }

            return lista;
        }

        private static void AdicionarParametros(
            SqlCommand cmd,
            Equipamento equipamento)
        {
            cmd.Parameters.Add(
                "@Nome",
                SqlDbType.NVarChar,
                100).Value =
                equipamento.Nome.Trim();

            cmd.Parameters.Add(
                "@Categoria",
                SqlDbType.NVarChar,
                50).Value =
                equipamento.Categoria.Trim();

            cmd.Parameters.Add(
                "@Marca",
                SqlDbType.NVarChar,
                100).Value =
                equipamento.Marca.Trim();

            cmd.Parameters.Add(
                "@Modelo",
                SqlDbType.NVarChar,
                50).Value =
                string.IsNullOrWhiteSpace(
                    equipamento.Modelo)
                    ? DBNull.Value
                    : equipamento.Modelo.Trim();

            cmd.Parameters.Add(
                "@NumeroSerie",
                SqlDbType.NVarChar,
                100).Value =
                string.IsNullOrWhiteSpace(
                    equipamento.NumeroSerie)
                    ? DBNull.Value
                    : equipamento.NumeroSerie.Trim();

            cmd.Parameters.Add(
                "@DataAquisicao",
                SqlDbType.Date).Value =
                equipamento.DataAquisicao.Date;

            cmd.Parameters.Add(
                "@Localizacao",
                SqlDbType.NVarChar,
                100).Value =
                equipamento.Localizacao.Trim();

            cmd.Parameters.Add(
                "@Estado",
                SqlDbType.NVarChar,
                50).Value =
                equipamento.Estado.Trim();

            cmd.Parameters.Add(
                "@Observacoes",
                SqlDbType.NVarChar,
                500).Value =
                string.IsNullOrWhiteSpace(
                    equipamento.Observacoes)
                    ? DBNull.Value
                    : equipamento.Observacoes.Trim();
        }

        private static Equipamento MapearEquipamento(
            SqlDataReader reader)
        {
            return new Equipamento
            {
                IdEquipamento =
                    Convert.ToInt32(
                        reader["IdEquipamento"]),

                Nome =
                    reader["Nome"].ToString()
                    ?? string.Empty,

                Categoria =
                    reader["Categoria"].ToString()
                    ?? string.Empty,

                Marca =
                    reader["Marca"].ToString()
                    ?? string.Empty,

                Modelo =
                    reader["Modelo"] == DBNull.Value
                        ? string.Empty
                        : reader["Modelo"].ToString()
                          ?? string.Empty,

                NumeroSerie =
                    reader["NumeroSerie"] == DBNull.Value
                        ? string.Empty
                        : reader["NumeroSerie"].ToString()
                          ?? string.Empty,

                DataAquisicao =
                    Convert.ToDateTime(
                        reader["DataAquisicao"]),

                Localizacao =
                    reader["Localizacao"].ToString()
                    ?? string.Empty,

                Estado =
                    reader["Estado"].ToString()
                    ?? string.Empty,

                Observacoes =
                    reader["Observacoes"] == DBNull.Value
                        ? string.Empty
                        : reader["Observacoes"].ToString()
                          ?? string.Empty
            };
        }
    }
}