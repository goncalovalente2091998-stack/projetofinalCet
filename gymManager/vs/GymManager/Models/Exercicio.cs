using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Models
{
    public class Exercicio
    {
        public int IdExercicio { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string GrupoMuscular { get; set; } = string.Empty;

        public string Equipamento { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public string Dificuldade { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;
    }
}
