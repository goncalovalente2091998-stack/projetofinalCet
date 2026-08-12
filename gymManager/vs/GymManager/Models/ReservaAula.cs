using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Models
{
    using System;

    namespace GymManager.Models
    {
        public class ReservaAula
        {
            public int IdReserva { get; set; }

            public int IdAula { get; set; }

            public string NomeAula { get; set; } = string.Empty;

            public DateTime DataAula { get; set; }

            public TimeSpan HoraInicio { get; set; }

            public string Sala { get; set; } = string.Empty;

            public int IdCliente { get; set; }

            public string NomeCliente { get; set; } =  string.Empty;

            public string NIF { get; set; } = string.Empty;

            public DateTime DataReserva { get; set; }

            public string Estado { get; set; } = string.Empty;

            public string HoraFormatada => HoraInicio.ToString(@"hh\:mm");

            public string AulaFormatada => $"{NomeAula} - {DataAula:dd/MM/yyyy} às {HoraInicio:hh\\:mm}";
        }
    }
}
