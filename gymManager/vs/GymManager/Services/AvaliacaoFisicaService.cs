using GymManager.Data;
using GymManager.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GymManager.Services
{
    public class AvaliacaoFisicaService
    {
        private readonly DataBase db = new DataBase();

        public List<AvaliacaoFisica> Listar()
        {
            List<AvaliacaoFisica> lista =new List<AvaliacaoFisica>();

            using SqlConnection conn =db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_AvaliacoesFisicas_Listar",conn);

            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader reader =cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add( MapearAvaliacao(reader));
            }

            return lista;
        }

        public AvaliacaoFisica? ObterPorId(int idAvaliacao)
        {
            using SqlConnection conn =db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand( "dbo.sp_AvaliacoesFisicas_ObterPorId", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdAvaliacao",SqlDbType.Int).Value =idAvaliacao;

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapearAvaliacao(reader);
        }

        public void Inserir( AvaliacaoFisica avaliacao)
        {
            using SqlConnection conn =db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_AvaliacoesFisicas_Inserir", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            AdicionarParametros( cmd, avaliacao);

            cmd.ExecuteNonQuery();
        }

        public void Atualizar(AvaliacaoFisica avaliacao)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand( "dbo.sp_AvaliacoesFisicas_Atualizar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(  "@IdAvaliacao", SqlDbType.Int).Value = avaliacao.IdAvaliacao;

            AdicionarParametros(cmd, avaliacao);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar( int idAvaliacao)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd =new SqlCommand("dbo.sp_AvaliacoesFisicas_Eliminar", conn);

            cmd.CommandType =CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdAvaliacao", SqlDbType.Int).Value = idAvaliacao;

            cmd.ExecuteNonQuery();
        }

        public List<AvaliacaoFisica> Pesquisar(string pesquisa)
        {
            List<AvaliacaoFisica> lista =new List<AvaliacaoFisica>();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd =new SqlCommand("dbo.sp_AvaliacoesFisicas_Pesquisar",conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add( "@Pesquisa",SqlDbType.NVarChar,100).Value = pesquisa;

            using SqlDataReader reader =cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearAvaliacao(reader));
            }

            return lista;
        }

        private static void AdicionarParametros( SqlCommand cmd, AvaliacaoFisica avaliacao)
        {
            cmd.Parameters.Add("@IdCliente", SqlDbType.Int).Value = avaliacao.IdCliente;

            cmd.Parameters.Add("@IdPT",SqlDbType.Int).Value =avaliacao.IdPT;

            cmd.Parameters.Add( "@DataAvaliacao", SqlDbType.Date).Value =avaliacao.DataAvaliacao.Date;
            
            SqlParameter peso = cmd.Parameters.Add("@Peso", SqlDbType.Decimal);

            peso.Precision = 5;
            peso.Scale = 2;
           
            peso.Value = avaliacao.Peso.HasValue ? avaliacao.Peso.Value: DBNull.Value;

            SqlParameter altura =cmd.Parameters.Add("@Altura", SqlDbType.Decimal);

            altura.Precision = 4;
            altura.Scale = 2;
           
            altura.Value = avaliacao.Altura.HasValue ? avaliacao.Altura.Value: DBNull.Value;

            SqlParameter massaGorda = cmd.Parameters.Add("@MassaGorda",SqlDbType.Decimal);

            massaGorda.Precision = 5;
            massaGorda.Scale = 2;
           
            massaGorda.Value = avaliacao.MassaGorda.HasValue ? avaliacao.MassaGorda.Value: DBNull.Value;

            SqlParameter massaMuscular = cmd.Parameters.Add("@MassaMuscular", SqlDbType.Decimal);

            massaMuscular.Precision = 5;
            massaMuscular.Scale = 2;
          
            massaMuscular.Value = avaliacao.MassaMuscular.HasValue ? avaliacao.MassaMuscular.Value: DBNull.Value;

            cmd.Parameters.Add("@Observacoes", SqlDbType.NVarChar, 255).Value = string.IsNullOrWhiteSpace(avaliacao.Observacoes)? DBNull.Value : avaliacao.Observacoes.Trim();

            cmd.Parameters.Add("@Estado",SqlDbType.NVarChar, 20).Value = avaliacao.Estado.Trim();
        }

        private static AvaliacaoFisica MapearAvaliacao(SqlDataReader reader)
        {
            return new AvaliacaoFisica
            {
                IdAvaliacao =Convert.ToInt32(reader["IdAvaliacao"]),

                IdCliente =Convert.ToInt32(reader["IdCliente"]),

                NomeCliente =reader["NomeCliente"].ToString()?? string.Empty,

                IdPT = Convert.ToInt32(reader["IdPT"]),

                NomePT = reader["NomePT"].ToString()?? string.Empty,

                DataAvaliacao = Convert.ToDateTime( reader["DataAvaliacao"]),

                Peso = reader["Peso"] == DBNull.Value ? null : Convert.ToDecimal( reader["Peso"]),

                Altura = reader["Altura"] == DBNull.Value ? null : Convert.ToDecimal( reader["Altura"]),

                IMC = reader["IMC"] == DBNull.Value ? null: Convert.ToDecimal( reader["IMC"]),

                MassaGorda = reader["MassaGorda"] == DBNull.Value ? null: Convert.ToDecimal( reader["MassaGorda"]),

                MassaMuscular = reader["MassaMuscular"] == DBNull.Value? null : Convert.ToDecimal( reader["MassaMuscular"]),

                Observacoes =  reader["Observacoes"] == DBNull.Value ? string.Empty : reader["Observacoes"].ToString()?? string.Empty,

                Estado = reader["Estado"].ToString() ?? string.Empty
            };
        }
    }
}