using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace GymManager.Helpers
{
   public class passwordHelper
    {
        public static string CriarHash(string password)
        {
            return BC.HashPassword(password, workFactor: 12);
        }

        public static bool Verificar(
            string password,
            string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(passwordHash))
            {
                return false;
            }

            return BC.Verify(password, passwordHash);
        }
    }
}

