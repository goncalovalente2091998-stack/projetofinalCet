using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Models
{
    public class Utilizador
    {
            public int IdUtilizador { get; set; }

            public string Nome { get; set; } = string.Empty;

            public string Email { get; set; } = string.Empty;

            public string PasswordHash { get; set; } = string.Empty;

            public string Perfil { get; set; } = string.Empty;
        }
    }

