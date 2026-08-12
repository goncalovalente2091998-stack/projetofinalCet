using System;

namespace GymManager.Models
{
    public class Presenca
    {
        public int IdPresenca { get; set; }

        public int IdCliente { get; set; }

        public string NomeCliente { get; set; } = string.Empty;

        public string NIF { get; set; } =string.Empty;

        public DateTime DataEntrada { get; set; }

        public DateTime? DataSaida { get; set; }

        public string Observacoes { get; set; } = string.Empty;

        public bool EstaAtiva => !DataSaida.HasValue;

        public string Estado => EstaAtiva ? "No ginásio" : "Concluída";

        public string DataEntradaFormatada => DataEntrada.ToString("dd/MM/yyyy HH:mm");

        public string DataSaidaFormatada => DataSaida.HasValue ? DataSaida.Value.ToString( "dd/MM/yyyy HH:mm"): "-";

        public string DuracaoFormatada
        {
            get
            {
                DateTime fim = DataSaida ?? DateTime.Now;

                TimeSpan duracao = fim - DataEntrada;

                if (duracao.TotalMinutes < 1)
                {
                    return "Menos de 1 min";
                }

                if (duracao.TotalHours < 1)
                {
                    return $"{(int)duracao.TotalMinutes} min";
                }

                int horas = (int)duracao.TotalHours;

                int minutos = duracao.Minutes;

                return minutos > 0 ? $"{horas}h {minutos}min" : $"{horas}h";
            }
        }
    }
}