using System.Globalization;

namespace GymManager.Helpers
{
    public static class FormatarTexto
    {
        public static string Nome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return string.Empty;
            }

            TextInfo textInfo =
                CultureInfo.GetCultureInfo("pt-PT").TextInfo;

            string nomeFormatado =
                textInfo.ToTitleCase(nome.Trim().ToLower());

            string[] particulas =
            {
                " Da ",
                " Das ",
                " De ",
                " Do ",
                " Dos ",
                " E "
            };

            foreach (string particula in particulas)
            {
                nomeFormatado = nomeFormatado.Replace(
                    particula,
                    particula.ToLower(),
                    StringComparison.Ordinal);
            }

            return nomeFormatado;
        }
    }
}