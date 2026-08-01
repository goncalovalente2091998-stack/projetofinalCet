using Microsoft.Win32;
using System;

namespace GymManager.Helpers
{
    public static class PdfExportador
    {
        public static string? ObterCaminho(
            string nomeFicheiro)
        {
            SaveFileDialog dlg =
                new SaveFileDialog();

            dlg.Filter =
                "Documento PDF (*.pdf)|*.pdf";

            dlg.DefaultExt =
                ".pdf";

            dlg.FileName =
                nomeFicheiro;

            return dlg.ShowDialog() == true
                ? dlg.FileName
                : null;
        }
    }
}