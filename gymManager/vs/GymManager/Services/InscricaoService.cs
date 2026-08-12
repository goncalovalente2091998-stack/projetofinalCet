using GymManager.Data;
using GymManager.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GymManager.Services
{
    public class InscricaoService
    {
        private readonly DataBase db = new DataBase();

        public List<InscricaoPagamento> ListarAtivasPorCliente(int idCliente)
        {
            List<InscricaoPagamento> lista = new();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("sp_Inscricoes_ListarAtivasPorCliente", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdCliente", SqlDbType.Int).Value = idCliente;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new InscricaoPagamento
                {
                    IdInscricao = Convert.ToInt32(reader["IdInscricao"]),

                    IdCliente = Convert.ToInt32(reader["IdCliente"]),

                    IdPlano = Convert.ToInt32(reader["IdPlano"]),

                    NomePlano = reader["NomePlano"].ToString() ?? string.Empty,

                    Preco = Convert.ToDecimal(reader["Preco"]),

                    DuracaoMeses = Convert.ToInt32(reader["DuracaoMeses"]),

                    DataInicio = Convert.ToDateTime(reader["DataInicio"]),

                    DataFim = Convert.ToDateTime(reader["DataFim"]),

                    Estado = reader["Estado"].ToString() ?? string.Empty
                });
            }

            return lista;
        }

        public List<Inscricao> Listar()
        {
            AtualizarEstadosExpirados();

            List<Inscricao> lista = new();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("sp_Inscricoes_Listar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearInscricao(reader));
            }

            return lista;
        }

        public Inscricao? ObterPorId(int idInscricao)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("sp_Inscricoes_ObterPorId", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdInscricao", SqlDbType.Int).Value = idInscricao;

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapearInscricao(reader);
        }

        public void Inserir(Inscricao inscricao)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("sp_Inscricoes_Inserir", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            AdicionarParametrosInserir(cmd, inscricao);

            cmd.ExecuteNonQuery();
        }

        public void Atualizar(Inscricao inscricao)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("sp_Inscricoes_Atualizar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdInscricao", SqlDbType.Int).Value = inscricao.IdInscricao;

            AdicionarParametrosAtualizar(cmd, inscricao);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int idInscricao)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("sp_Inscricoes_Eliminar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdInscricao", SqlDbType.Int).Value = idInscricao;

            cmd.ExecuteNonQuery();
        }

        public List<Inscricao> Pesquisar(string pesquisa)
        {
            AtualizarEstadosExpirados();

            List<Inscricao> lista = new();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("sp_Inscricoes_Pesquisar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Pesquisa", SqlDbType.NVarChar, 100).Value = pesquisa;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearInscricao(reader));
            }

            return lista;
        }

        private static void AdicionarParametrosInserir(SqlCommand cmd, Inscricao inscricao)
        {
            cmd.Parameters.Add("@IdCliente", SqlDbType.Int).Value = inscricao.IdCliente;

            cmd.Parameters.Add("@IdPlano", SqlDbType.Int).Value = inscricao.IdPlano;

            cmd.Parameters.Add("@DataInicio", SqlDbType.Date).Value = inscricao.DataInicio.Date;

            cmd.Parameters.Add("@DataFim", SqlDbType.Date).Value = inscricao.DataFim.Date;
        }

        private static void AdicionarParametrosAtualizar(SqlCommand cmd, Inscricao inscricao)
        {
            cmd.Parameters.Add("@IdCliente", SqlDbType.Int).Value = inscricao.IdCliente;

            cmd.Parameters.Add("@IdPlano", SqlDbType.Int).Value = inscricao.IdPlano;

            cmd.Parameters.Add("@DataInicio", SqlDbType.Date).Value = inscricao.DataInicio.Date;

            cmd.Parameters.Add("@DataFim", SqlDbType.Date).Value = inscricao.DataFim.Date;

            cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 50).Value = inscricao.Estado;
        }

        private static Inscricao MapearInscricao(SqlDataReader reader)
        {
            return new Inscricao
            {
                IdInscricao = Convert.ToInt32(reader["IdInscricao"]),

                IdCliente = Convert.ToInt32(reader["IdCliente"]),

                NomeCliente = reader["NomeCliente"].ToString() ?? string.Empty,

                IdPlano = Convert.ToInt32(reader["IdPlano"]),

                NomePlano = reader["NomePlano"].ToString() ?? string.Empty,

                DataInicio = Convert.ToDateTime(reader["DataInicio"]),

                DataFim = Convert.ToDateTime(reader["DataFim"]),

                Estado = reader["Estado"].ToString() ?? string.Empty
            };
        }
        public void AtualizarEstadosExpirados()
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("sp_Inscricoes_AtualizarEstadosExpirados", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.ExecuteNonQuery();
        }
        public void Renovar(int idInscricao)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("sp_Inscricoes_Renovar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdInscricao", SqlDbType.Int).Value = idInscricao;

            cmd.ExecuteNonQuery();
        }

        public void GerarPagamento(int idInscricao)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Inscricoes_GerarPagamento", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdInscricao", SqlDbType.Int).Value = idInscricao;

            cmd.ExecuteNonQuery();
        }

        public List<InscricaoPagamento> ListarDisponiveisParaPagamento(int idCliente)
        {
            List<InscricaoPagamento> lista = new();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("sp_Inscricoes_ListarDisponiveisParaPagamento", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdCliente", SqlDbType.Int).Value = idCliente;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new InscricaoPagamento
                {
                    IdInscricao = Convert.ToInt32(reader["IdInscricao"]),

                    IdCliente = Convert.ToInt32(reader["IdCliente"]),

                    IdPlano = Convert.ToInt32(reader["IdPlano"]),

                    NomePlano = reader["NomePlano"].ToString() ?? string.Empty,

                    Preco = Convert.ToDecimal(reader["Preco"]),

                    DuracaoMeses = Convert.ToInt32(reader["DuracaoMeses"]),

                    DataInicio = Convert.ToDateTime(reader["DataInicio"]),

                    DataFim = Convert.ToDateTime(reader["DataFim"]),

                    Estado = reader["Estado"].ToString() ?? string.Empty
                }
                );
            }

            return lista;
        }
    }


}