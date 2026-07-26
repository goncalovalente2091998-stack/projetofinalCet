using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Models
{
    public class Plano
    {
        public int IdPlano { get; set; }

        public string Nome { get; set; } = string.Empty;

        public decimal Preco { get; set; }

        public int DuracaoMeses { get; set; }

        public string? Descricao { get; set; }
    }
}
