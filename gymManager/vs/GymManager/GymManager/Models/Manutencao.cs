using System;

namespace GymManager.Models
{
    public class Manutencao
    {
        public int IdManutencao { get; set; }

        public int IdEquipamento { get; set; }

        public string NomeEquipamento { get; set; } =
            string.Empty;

        public string Marca { get; set; } =
            string.Empty;

        public string Modelo { get; set; } =
            string.Empty;

        public string Tipo { get; set; } =
            string.Empty;

        public DateTime DataAgendada { get; set; }

        public DateTime? DataRealizacao { get; set; }

        public string Descricao { get; set; } =
            string.Empty;

        public string Responsavel { get; set; } =
            string.Empty;

        public decimal? Custo { get; set; }

        public string Estado { get; set; } =
            string.Empty;

        public string Observacoes { get; set; } =
            string.Empty;

        public string EquipamentoCompleto
        {
            get
            {
                string marcaModelo =
                    $"{Marca} {Modelo}".Trim();

                return string.IsNullOrWhiteSpace(marcaModelo)
                    ? NomeEquipamento
                    : $"{NomeEquipamento} — {marcaModelo}";
            }
        }

        public string DataRealizacaoFormatada =>
            DataRealizacao.HasValue
                ? DataRealizacao.Value.ToString("dd/MM/yyyy")
                : "-";

        public string CustoFormatado =>
            Custo.HasValue
                ? $"{Custo.Value:N2} €"
                : "-";
    }
}