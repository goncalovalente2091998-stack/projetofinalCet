using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Models
{
    public class Inscricao
    {
        public int IdInscricao { get; set; }

        public int IdCliente { get; set; }

        public string NomeCliente { get; set; } = string.Empty;

        public int IdPlano { get; set; }

        public string NomePlano { get; set; } = string.Empty;

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public string Estado { get; set; } = string.Empty;
    }
}
