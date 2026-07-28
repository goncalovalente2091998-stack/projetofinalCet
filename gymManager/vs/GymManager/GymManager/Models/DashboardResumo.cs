using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Models
{
    public class DashboardResumo
    {

        public int ClientesAtivos { get; set; }

        public int InscricoesAtivas { get; set; }

        public int PagamentosPendentes { get; set; }

        public decimal ReceitaMes { get; set; }

        public decimal ReceitaAno { get; set; }

        public decimal ReceitaTotal { get; set; }

        public int InscricoesATerminar { get; set; }
    }
}
