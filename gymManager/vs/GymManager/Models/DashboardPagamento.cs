using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Models
{
    public class DashboardPagamento
    {
        public int IdPagamento { get; set; }

        public string NomeCliente { get; set; } = string.Empty;

        public string NomePlano { get; set; } =string.Empty;

        public DateTime DataPagamento { get; set; }

        public decimal Valor { get; set; }

        public string MetodoPagamento { get; set; } =string.Empty;

        public string Estado { get; set; } = string.Empty;
    }
}
