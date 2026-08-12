using System;

namespace GymManager.Models
{
    public class EventoAgenda
    {
        public int IdEvento { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Tipo { get; set; } = string.Empty;

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public int? IdPT { get; set; }
        public int? IdProfessor { get; set; }

        public string NomePT { get; set; } = string.Empty;

        public int? IdCliente { get; set; }

        public string NomeCliente { get; set; } = string.Empty;

        public int? IdAula { get; set; }

        public string NomeAula { get; set; } = string.Empty;

        public string Localizacao { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public string DataFormatada => DataInicio.ToString("dd/MM/yyyy");

        public string HoraInicioFormatada =>DataInicio.ToString("HH:mm");

        public string HoraFimFormatada => DataFim.ToString("HH:mm");

        public string HorarioFormatado => $"{HoraInicioFormatada} - {HoraFimFormatada}";

        public string PeriodoFormatado => $"{DataInicio:dd/MM/yyyy HH:mm} - {DataFim:dd/MM/yyyy HH:mm}";

        public TimeSpan Duracao => DataFim - DataInicio;

        public string DuracaoFormatada
        {
            get
            {
                int horas = (int)Duracao.TotalHours;

                int minutos = Duracao.Minutes;

                if (horas <= 0)
                {
                    return $"{minutos} min";
                }

                return minutos > 0 ? $"{horas}h {minutos}min": $"{horas}h";
            }
        }

        public bool EstaAgendado => string.Equals(Estado, "Agendado", StringComparison.OrdinalIgnoreCase);

        public bool EstaConcluido =>string.Equals(Estado,"Concluído",StringComparison.OrdinalIgnoreCase);

        public bool EstaCancelado =>string.Equals(Estado,"Cancelado",StringComparison.OrdinalIgnoreCase);

        public bool EhSessaoPT =>string.Equals(Tipo,"Sessão PT",StringComparison.OrdinalIgnoreCase);

        public bool EhAula =>string.Equals(Tipo,"Aula",StringComparison.OrdinalIgnoreCase);

        public string Participante
        {
            get
            {
                if (EhSessaoPT)
                {
                    return string.IsNullOrWhiteSpace( NomeCliente)? "-": NomeCliente;
                }

                if (EhAula)
                {
                    return string.IsNullOrWhiteSpace(NomeAula)? "-": NomeAula;
                }

                return "-";
            }
        }
    }
}