using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Models
{
    public class PersonalTrainer
    {
      
            public int IdPT { get; set; }

            public string Nome { get; set; } = string.Empty;

            public string Especialidade { get; set; } = string.Empty;

            public string Telefone { get; set; } = string.Empty;

            public string Email { get; set; } = string.Empty;

            public decimal ValorHora { get; set; }

            public bool Estado { get; set; }
        }
    }

