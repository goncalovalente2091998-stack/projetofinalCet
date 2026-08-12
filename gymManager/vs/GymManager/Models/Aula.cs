using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Models
{
    public class Aula
    {
        public int IdAula { get; set; }

        public int IdProfessor { get; set; }

        public string NomeProfessor { get; set; } = string.Empty;

        public string Nome { get; set; } = string.Empty;

        public DateTime DataAula { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public int DuracaoMinutos { get; set; }

        public int Lotacao { get; set; }

        public string Sala { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public string HoraFormatada => HoraInicio.ToString(@"hh\:mm");

        public string DuracaoFormatada => $"{DuracaoMinutos} min";

        public string Descricao => $"{Nome} - {DataAula:dd/MM/yyyy} às {HoraInicio:hh\\:mm}";
        public int VagasOcupadas { get; set; }

        public int VagasDisponiveis => Math.Max(0, Lotacao - VagasOcupadas);

        public string OcupacaoFormatada => $"{VagasOcupadas} / {Lotacao}";
    }
}

