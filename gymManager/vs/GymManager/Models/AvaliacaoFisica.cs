using System;

namespace GymManager.Models
{
    public class AvaliacaoFisica
    {
        public int IdAvaliacao { get; set; }

        public int IdCliente { get; set; }

        public string NomeCliente { get; set; } = string.Empty;

        public int IdPT { get; set; }

        public string NomePT { get; set; } = string.Empty;

        public DateTime DataAvaliacao { get; set; }

        public decimal? Peso { get; set; }

        public decimal? Altura { get; set; }

        public decimal? IMC { get; set; }

        public decimal? MassaGorda { get; set; }

        public decimal? MassaMuscular { get; set; }

        public string Observacoes { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public string PesoFormatado => Peso.HasValue ? $"{Peso.Value:N2} kg" : "-";

        public string AlturaFormatada => Altura.HasValue ? $"{Altura.Value:N2} m" : "-";

        public string IMCFormatado => IMC.HasValue ? IMC.Value.ToString("N2") : "-";

        public string MassaGordaFormatada => MassaGorda.HasValue ? $"{MassaGorda.Value:N2} %" : "-";

        public string MassaMuscularFormatada => MassaMuscular.HasValue ? $"{MassaMuscular.Value:N2} kg" : "-";

        public string ClassificacaoIMC
        {
            get
            {
                if (!IMC.HasValue)
                {
                    return "-";
                }

                if (IMC.Value < 18.5m)
                {
                    return "Abaixo do peso";
                }

                if (IMC.Value < 25m)
                {
                    return "Peso normal";
                }

                if (IMC.Value < 30m)
                {
                    return "Excesso de peso ou Massa Muscular Forte ,Comparar Massa Gorda e Muscular";
                }

                return "Obesidade";
            }
        }
    }
}