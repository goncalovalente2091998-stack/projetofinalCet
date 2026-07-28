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
    public class DashboardService
    {
        private readonly DataBase db =
            new DataBase();

        public DashboardResumo ObterResumo()
        {
            using SqlConnection conn =
                db.GetConnection();

            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(
                    "sp_Dashboard_Resumo",
                    conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            using SqlDataReader reader =
                cmd.ExecuteReader();

            if (!reader.Read())
            {
                return new DashboardResumo();
            }

            return new DashboardResumo
            {
                ClientesAtivos =
                    Convert.ToInt32(
                        reader["ClientesAtivos"]),

                InscricoesAtivas =
                    Convert.ToInt32(
                        reader["InscricoesAtivas"]),

                PagamentosPendentes =
                    Convert.ToInt32(
                        reader["PagamentosPendentes"]),
                ReceitaMes =
    Convert.ToDecimal(
        reader["ReceitaMes"]),

                ReceitaAno =
    Convert.ToDecimal(
        reader["ReceitaAno"]),

                ReceitaTotal =
    Convert.ToDecimal(
        reader["ReceitaTotal"]),

                InscricoesATerminar =
                    Convert.ToInt32(
                        reader["InscricoesATerminar"]),
                AulasHoje =
    Convert.ToInt32(
        reader["AulasHoje"]),

                ReservasHoje =
    Convert.ToInt32(
        reader["ReservasHoje"]),
            };
        }

        public List<DashboardPagamento> ListarUltimosPagamentos()
        {
            List<DashboardPagamento> lista =
                new List<DashboardPagamento>();

            using SqlConnection conn =
                db.GetConnection();

            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(
                    "sp_Dashboard_UltimosPagamentos",
                    conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            using SqlDataReader reader =
                cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(
                    new DashboardPagamento
                    {
                        IdPagamento =
                            Convert.ToInt32(
                                reader["IdPagamento"]),

                        NomeCliente =
                            reader["NomeCliente"]
                                .ToString()
                            ?? string.Empty,

                        NomePlano =
                            reader["NomePlano"] ==
                            DBNull.Value
                                ? string.Empty
                                : reader["NomePlano"]
                                    .ToString()
                                  ?? string.Empty,

                        DataPagamento =
                            Convert.ToDateTime(
                                reader["DataPagamento"]),

                        Valor =
                            Convert.ToDecimal(
                                reader["Valor"]),

                        MetodoPagamento =
                            reader["MetodoPagamento"]
                                .ToString()
                            ?? string.Empty,

                        Estado =
                            reader["Estado"]
                                .ToString()
                            ?? string.Empty
                    });
            }

            return lista;
        }

        public List<DashboardInscricaoTerminar>
            ListarInscricoesATerminar()
        {
            List<DashboardInscricaoTerminar> lista =
                new List<DashboardInscricaoTerminar>();

            using SqlConnection conn =
                db.GetConnection();

            conn.Open();

            using SqlCommand cmd =
                new SqlCommand(
                    "sp_Dashboard_InscricoesATerminar",
                    conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            using SqlDataReader reader =
                cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(
                    new DashboardInscricaoTerminar
                    {
                        IdInscricao =
                            Convert.ToInt32(
                                reader["IdInscricao"]),

                        NomeCliente =
                            reader["NomeCliente"]
                                .ToString()
                            ?? string.Empty,

                        NomePlano =
                            reader["NomePlano"]
                                .ToString()
                            ?? string.Empty,

                        DataFim =
                            Convert.ToDateTime(
                                reader["DataFim"]),

                        DiasRestantes =
                            Convert.ToInt32(
                                reader["DiasRestantes"])
                    });
            }

            return lista;
        }

       
    }

}
