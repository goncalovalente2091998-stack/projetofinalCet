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
        public class PlanoTreino
        {
            public int IdPlanoTreino { get; set; }

            public int IdCliente { get; set; }

            public string NomeCliente { get; set; } =
                string.Empty;

            public int IdPT { get; set; }

            public string NomePT { get; set; } =
                string.Empty;

            public string NomePlano { get; set; } =
                string.Empty;

            public string Objetivo { get; set; } =
                string.Empty;

            public DateTime DataInicio { get; set; }

            public DateTime DataFim { get; set; }

            public string Observacoes { get; set; } =
                string.Empty;

            public string Estado { get; set; } =
                string.Empty;

            public string PeriodoFormatado =>
                $"{DataInicio:dd/MM/yyyy} → {DataFim:dd/MM/yyyy}";
        }
    }
}
