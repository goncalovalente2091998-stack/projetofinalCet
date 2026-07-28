using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Models
{
    public class InscricaoPagamento
    {
        public int IdInscricao { get; set; }

        public int IdCliente { get; set; }

        public int IdPlano { get; set; }

        public string NomePlano { get; set; } = string.Empty;

        public decimal Preco { get; set; }

        public int DuracaoMeses { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public string Estado { get; set; } = string.Empty;

        public string Descricao =>
            $"{NomePlano} — {Preco:F2} € — termina em {DataFim:dd/MM/yyyy}";
    }
}
