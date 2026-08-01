namespace GymManager.Models
{
    public class PlanoTreinoExercicio
    {
        public int IdPlanoTreinoExercicio { get; set; }

        public int IdPlanoTreino { get; set; }

        public int IdExercicio { get; set; }

        public string NomeExercicio { get; set; } =
            string.Empty;

        public string GrupoMuscular { get; set; } =
            string.Empty;

        public string Equipamento { get; set; } =
            string.Empty;

        public int Series { get; set; }

        public int Repeticoes { get; set; }

        public int TempoDescanso { get; set; }

        public int Ordem { get; set; }

        public string Observacoes { get; set; } =
            string.Empty;

        public string Configuracao =>
            $"{Series} x {Repeticoes}";

        public string DescansoFormatado =>
            $"{TempoDescanso} s";
    }
}