using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Models
{
    public class DashboardInscricaoTerminar
    {
        public int IdInscricao { get; set; }

        public string NomeCliente { get; set; } = string.Empty;

        public string NomePlano { get; set; } = string.Empty;

        public DateTime DataFim { get; set; }

        public int DiasRestantes { get; set; }
    }
}
