using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Models
{
  public  class Cliente
    {

        public int IdCliente { get; set; }

        public string Nome { get; set; }

        public string NIF { get; set; }

        public DateTime DataNascimento { get; set; }

        public string Telefone { get; set; }

        public string Email { get; set; }

        public string Morada { get; set; }

        public DateTime DataInscricao { get; set; }

        public bool Estado { get; set; }

        public string DescricaoReserva =>
           $"{Nome} — NIF: {NIF}";
    }
}
