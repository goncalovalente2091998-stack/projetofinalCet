using GymManager.Data;
using GymManager.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Services
{
    public class PagamentoService
    {
        private readonly DataBase db = new DataBase();

        public List<Pagamento> Listar()
        {
            List<Pagamento> lista = new();

            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd =
                new SqlCommand("sp_Pagamentos_Listar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearPagamento(reader));
            }

            return lista;
        }

        public Pagamento? ObterPorId(int idPagamento)
        {
            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd =
                new SqlCommand("sp_Pagamentos_ObterPorId", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@IdPagamento",
                SqlDbType.Int).Value = idPagamento;

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return MapearPagamento(reader);
        }

        public void Inserir(Pagamento pagamento)
        {
            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd =
                new SqlCommand("sp_Pagamentos_Inserir", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            AdicionarParametros(cmd, pagamento);

            cmd.ExecuteNonQuery();
        }

        public void Atualizar(Pagamento pagamento)
        {
            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd =
                new SqlCommand("sp_Pagamentos_Atualizar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@IdPagamento",
                SqlDbType.Int).Value = pagamento.IdPagamento;

            AdicionarParametros(cmd, pagamento);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int idPagamento)
        {
            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd =
                new SqlCommand("sp_Pagamentos_Eliminar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@IdPagamento",
                SqlDbType.Int).Value = idPagamento;

            cmd.ExecuteNonQuery();
        }

        public List<Pagamento> Pesquisar(string pesquisa)
        {
            List<Pagamento> lista = new();

            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd =
                new SqlCommand("sp_Pagamentos_Pesquisar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@Pesquisa",
                SqlDbType.NVarChar,
                100).Value = pesquisa;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearPagamento(reader));
            }

            return lista;
        }

        private static void AdicionarParametros(
        SqlCommand cmd,
        Pagamento pagamento)
        {
            cmd.Parameters.Add(
                "@IdCliente",
                SqlDbType.Int).Value = pagamento.IdCliente;

            cmd.Parameters.Add(
    "@IdInscricao",
    SqlDbType.Int).Value =
    pagamento.IdInscricao.HasValue
        ? pagamento.IdInscricao.Value
        : DBNull.Value;

            cmd.Parameters.Add(
                "@DataPagamento",
                SqlDbType.Date).Value = pagamento.DataPagamento.Date;

            SqlParameter valorParametro = cmd.Parameters.Add(
                "@Valor",
                SqlDbType.Decimal);

            valorParametro.Precision = 10;
            valorParametro.Scale = 2;
            valorParametro.Value = pagamento.Valor;

            cmd.Parameters.Add(
                "@MetodoPagamento",
                SqlDbType.NVarChar,
                50).Value = pagamento.MetodoPagamento;

            cmd.Parameters.Add(
                "@Observacoes",
                SqlDbType.NVarChar,
                255).Value =
                string.IsNullOrWhiteSpace(pagamento.Observacoes)
                    ? DBNull.Value
                    : pagamento.Observacoes;

            cmd.Parameters.Add(
                "@Estado",
                SqlDbType.NVarChar,
                30).Value = pagamento.Estado;

            cmd.Parameters.Add(
                "@ReferenciaExterna",
                SqlDbType.NVarChar,
                150).Value =
                string.IsNullOrWhiteSpace(pagamento.ReferenciaExterna)
                    ? DBNull.Value
                    : pagamento.ReferenciaExterna;

            cmd.Parameters.Add(
                "@IdTransacaoExterna",
                SqlDbType.NVarChar,
                150).Value =
                string.IsNullOrWhiteSpace(pagamento.IdTransacaoExterna)
                    ? DBNull.Value
                    : pagamento.IdTransacaoExterna;

            cmd.Parameters.Add(
                "@DataConfirmacao",
                SqlDbType.DateTime2).Value =
                pagamento.DataConfirmacao.HasValue
                    ? pagamento.DataConfirmacao.Value
                    : DBNull.Value;
        }

        private static Pagamento MapearPagamento(
       SqlDataReader reader)
        {
            return new Pagamento
            {
                IdPagamento =
                    Convert.ToInt32(reader["IdPagamento"]),

                IdCliente =
                    Convert.ToInt32(reader["IdCliente"]),

                NomeCliente =
                    reader["NomeCliente"].ToString()
                    ?? string.Empty,
                IdInscricao =
    reader["IdInscricao"] == DBNull.Value
        ? null
        : Convert.ToInt32(
            reader["IdInscricao"]),

                NomePlano =
    reader["NomePlano"] == DBNull.Value
        ? string.Empty
        : reader["NomePlano"].ToString()
          ?? string.Empty,
                DataPagamento =
                    Convert.ToDateTime(reader["DataPagamento"]),

                Valor =
                    Convert.ToDecimal(reader["Valor"]),

                MetodoPagamento =
                    reader["MetodoPagamento"].ToString()
                    ?? string.Empty,

                Observacoes =
                    reader["Observacoes"] == DBNull.Value
                        ? string.Empty
                        : reader["Observacoes"].ToString()
                          ?? string.Empty,

                Estado =
                    reader["Estado"].ToString()
                    ?? "Pendente",

                ReferenciaExterna =
                    reader["ReferenciaExterna"] == DBNull.Value
                        ? string.Empty
                        : reader["ReferenciaExterna"].ToString()
                          ?? string.Empty,

                IdTransacaoExterna =
                    reader["IdTransacaoExterna"] == DBNull.Value
                        ? string.Empty
                        : reader["IdTransacaoExterna"].ToString()
                          ?? string.Empty,

                DataConfirmacao =
                    reader["DataConfirmacao"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(
                            reader["DataConfirmacao"])
            };
        }
        public void Confirmar(int idPagamento)
        {
            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd =
                new SqlCommand("sp_Pagamentos_Confirmar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@IdPagamento",
                SqlDbType.Int).Value = idPagamento;

            cmd.ExecuteNonQuery();
        }

        public void Reembolsar(int idPagamento)
        {
            using SqlConnection conn = db.GetConnection();
            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(
                    "sp_Pagamentos_Reembolsar",
                    conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@IdPagamento",
                SqlDbType.Int).Value =
                idPagamento;

            cmd.ExecuteNonQuery();
        }
    }
}
