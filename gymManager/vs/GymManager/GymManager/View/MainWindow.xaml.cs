using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GymManager.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Navigate(new DashboardPage());
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
        }

        private void btnClientes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientesPage());
        }

        private void btnPlanos_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PlanosPage());
        }

        private void btnInscricoes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new InscricoesPage());
        }

        private void btnPagamentos_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PagamentosPage());
        }

        private void btnProfessores_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProfessoresPage());
        }

        private void btnAulas_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AulasPage());
        }

        private void btnPT_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PersonalTrainersPage());
        }

        private void btnTreinos_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PlanosTreinoPage());
        }

        private void btnExercicios_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ExerciciosPage());
        }

        private void btnAvaliacoes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AvaliacoesPage());
        }

        private void btnEquipamentos_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EquipamentosPage());
        }

        private void btnManutencoes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManutencoesPage());
        }
    }
}
