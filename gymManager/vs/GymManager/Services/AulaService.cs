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
    public class AulaService
    {
        private readonly DataBase db = new DataBase();
        public List<Aula> Listar()
        {
            AtualizarEstados();
            List<Aula> lista = new();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Aulas_Listar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearAula(reader));
            }

            return lista;
        }

        public Aula? ObterPorId(int idAula)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Aulas_ObterPorId", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdAula", SqlDbType.Int).Value = idAula;

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapearAula(reader);
        }

        public void Inserir(Aula aula)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Aulas_Inserir", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            AdicionarParametros(cmd, aula);

            cmd.ExecuteNonQuery();
        }

        public void Atualizar(Aula aula)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Aulas_Atualizar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdAula", SqlDbType.Int).Value = aula.IdAula;

            AdicionarParametros(cmd, aula);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int idAula)
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Aulas_Eliminar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@IdAula", SqlDbType.Int).Value = idAula;

            cmd.ExecuteNonQuery();
        }

        public List<Aula> Pesquisar(string pesquisa)
        {
            List<Aula> lista = new();

            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Aulas_Pesquisar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Pesquisa", SqlDbType.NVarChar, 100).Value = pesquisa;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearAula(reader));
            }

            return lista;
        }

        private static void AdicionarParametros(SqlCommand cmd, Aula aula)
        {
            cmd.Parameters.Add("@IdProfessor", SqlDbType.Int).Value = aula.IdProfessor;

            cmd.Parameters.Add("@Nome", SqlDbType.NVarChar, 100).Value = aula.Nome.Trim();

            cmd.Parameters.Add("@DataAula", SqlDbType.Date).Value = aula.DataAula.Date;

            cmd.Parameters.Add("@HoraInicio", SqlDbType.Time).Value = aula.HoraInicio;

            cmd.Parameters.Add("@DuracaoMinutos", SqlDbType.Int).Value = aula.DuracaoMinutos;

            cmd.Parameters.Add("@Lotacao", SqlDbType.Int).Value = aula.Lotacao;

            cmd.Parameters.Add("@Sala", SqlDbType.NVarChar, 50).Value = aula.Sala.Trim();

            cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = aula.Estado.Trim();
        }

        private static Aula MapearAula(SqlDataReader reader)
        {
            return new Aula
            {
                IdAula = Convert.ToInt32(reader["IdAula"]),

                IdProfessor = Convert.ToInt32(reader["IdProfessor"]),

                NomeProfessor = reader["NomeProfessor"].ToString() ?? string.Empty,

                Nome = reader["Nome"].ToString() ?? string.Empty,

                DataAula = Convert.ToDateTime(reader["DataAula"]),

                HoraInicio = reader["HoraInicio"] is TimeSpan hora ? hora : TimeSpan.Zero,

                DuracaoMinutos = Convert.ToInt32(reader["DuracaoMinutos"]),

                Lotacao = Convert.ToInt32(reader["Lotacao"]),

                Sala = reader["Sala"].ToString() ?? string.Empty,

                Estado = reader["Estado"].ToString() ?? string.Empty,

                VagasOcupadas = reader["VagasOcupadas"] == DBNull.Value ? 0 : Convert.ToInt32(reader["VagasOcupadas"]),
            };
        }
        public void AtualizarEstados()
        {
            using SqlConnection conn = db.GetConnection();

            conn.Open();

            using SqlCommand cmd = new SqlCommand("dbo.sp_Aulas_AtualizarEstados", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.ExecuteNonQuery();
        }
    }
}
