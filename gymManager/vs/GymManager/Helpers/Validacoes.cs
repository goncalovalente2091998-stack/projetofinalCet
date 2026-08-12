using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GymManager.Helpers
{
    public static class Validacoes
    {
        public static bool CampoObrigatorio(string texto)
        {
            return !string.IsNullOrWhiteSpace(texto);
        }

        public static bool Email(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public static bool NIF(string nif)
        {
            return !string.IsNullOrWhiteSpace(nif) && nif.Length == 9 && nif.All(char.IsDigit);
        }

        public static bool Telefone(string telefone)
        {
            return !string.IsNullOrWhiteSpace(telefone) && telefone.Length >= 9;
        }

        public static bool MaiorOuIgual14Anos(DateTime? dataNascimento)
        {
            if (!dataNascimento.HasValue)
                return false;

            DateTime hoje = DateTime.Today;
            DateTime nascimento = dataNascimento.Value;

            int idade = hoje.Year - nascimento.Year;

            if (nascimento > hoje.AddYears(-idade))
                idade--;

            return idade >= 14;
        }
    }
}
