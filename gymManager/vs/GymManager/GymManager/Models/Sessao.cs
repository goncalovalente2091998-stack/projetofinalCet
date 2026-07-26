using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Models
{
    public static class Sessao
    {
        public static int IdUtilizador { get; set; }

        public static string Nome { get; set; } = string.Empty;

        public static string Perfil { get; set; } = string.Empty;

        public static bool Logado => IdUtilizador > 0;

        public static void Limpar()
        {
            IdUtilizador = 0;
            Nome = string.Empty;
            Perfil = string.Empty;
        }
    }
}
