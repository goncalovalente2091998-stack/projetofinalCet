using System;

namespace GymManager.Models
{
    public class Equipamento
    {
        public int IdEquipamento { get; set; }

        public string Nome { get; set; } =
            string.Empty;

        public string Categoria { get; set; } =
            string.Empty;

        public string Marca { get; set; } =
            string.Empty;

        public string Modelo { get; set; } =
            string.Empty;

        public string NumeroSerie { get; set; } =
            string.Empty;

        public DateTime DataAquisicao { get; set; }

        public string Localizacao { get; set; } =
            string.Empty;

        public string Estado { get; set; } =
            string.Empty;

        public string Observacoes { get; set; } =
            string.Empty;

        public string DescricaoCompleta
        {
            get
            {
                string marcaModelo =
                    $"{Marca} {Modelo}".Trim();

                return string.IsNullOrWhiteSpace(marcaModelo)
                    ? Nome
                    : $"{Nome} — {marcaModelo}";
            }
        }
    }
}