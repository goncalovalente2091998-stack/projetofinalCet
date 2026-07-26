using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GymManager.Helpers
{
    public static class Mensagem
    {
        public static bool Confirmar(string texto)
        {
            return MessageBox.Show(
                texto,
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public static void Sucesso(string texto)
        {
            MessageBox.Show(
                texto,
                "Sucesso",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        public static void Erro(string texto)
        {
            MessageBox.Show(
                texto,
                "Erro",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public static void Aviso(string texto)
        {
            MessageBox.Show(
                texto,
                "Aviso",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        public static void Informacao(string texto)
        {
            MessageBox.Show(
                texto,
                "Informação",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
