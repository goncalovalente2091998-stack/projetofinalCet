using GymManager.Helpers;
using GymManager.Models;
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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txtNomeUtilizador.Text = Sessao.Nome;
            txtPerfilUtilizador.Text = Sessao.Perfil;
            AplicarPermissoes();
            txtTituloPagina.Text = "Inicio";
            MainFrame.Navigate(new DashboardPage());
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
        }

        private void btnClientes_Click(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "Clientes";
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
            txtTituloPagina.Text = "Professores";
            MainFrame.Navigate(new ProfessoresPage());
        }

        private void btnAulas_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AulasPage());
        }

        private void btnPT_Click(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "Personal Trainers";
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
            MainFrame.Navigate(new AvaliacoesFisicasPage());
        }

        private void btnEquipamentos_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EquipamentosPage());
        }

        private void btnManutencoes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManutencoesPage());
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (!Mensagem.Confirmar("Pretende terminar a sessão?"))
                return;

            Sessao.Limpar();

            LoginWindow login = new LoginWindow();
            login.Show();

            Close();
        }

        private void btnAgenda_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AgendaPage());

            txtTituloPagina.Text = "Agenda";
        }


        private void AplicarPermissoes()
        {
            bool administrador = Sessao.Perfil.Equals(Perfis.Administrador, StringComparison.OrdinalIgnoreCase);

            bool rececionista = Sessao.Perfil.Equals(Perfis.Rececionista, StringComparison.OrdinalIgnoreCase);

            bool professor = Sessao.Perfil.Equals(Perfis.Professor, StringComparison.OrdinalIgnoreCase);

            bool personalTrainer = Sessao.Perfil.Equals(Perfis.PersonalTrainer, StringComparison.OrdinalIgnoreCase);

            txtAdmin.Visibility = administrador ? Visibility.Visible : Visibility.Collapsed;

            txtAdmin.Visibility = administrador ? Visibility.Visible : Visibility.Collapsed;

            txtComercial.Visibility = administrador || rececionista ? Visibility.Visible : Visibility.Collapsed;

            spComercial.Visibility = administrador || rececionista ? Visibility.Visible : Visibility.Collapsed;


            txtEquipamentos.Visibility = administrador || rececionista ? Visibility.Visible : Visibility.Collapsed;

            spEquipamentos.Visibility = administrador || rececionista ? Visibility.Visible : Visibility.Collapsed;

            btnClientes.Visibility = administrador || rececionista ? Visibility.Visible : Visibility.Collapsed;

            btnPlanos.Visibility = administrador || rececionista ? Visibility.Visible : Visibility.Collapsed;

            btnInscricoes.Visibility = administrador || rececionista ? Visibility.Visible : Visibility.Collapsed;

            btnPagamentos.Visibility = administrador || rececionista ? Visibility.Visible : Visibility.Collapsed;

            btnProfessores.Visibility = administrador ? Visibility.Visible : Visibility.Collapsed;

            btnPT.Visibility = administrador || rececionista ? Visibility.Visible : Visibility.Collapsed;

            btnAulas.Visibility = administrador || rececionista || professor ? Visibility.Visible : Visibility.Collapsed;

            btnTreinos.Visibility = administrador || personalTrainer || rececionista ? Visibility.Visible : Visibility.Collapsed;

            btnExercicios.Visibility = administrador || personalTrainer || rececionista ? Visibility.Visible : Visibility.Collapsed;

            btnAvaliacoes.Visibility = administrador || personalTrainer || rececionista ? Visibility.Visible : Visibility.Collapsed;

            btnEquipamentos.Visibility = administrador || rececionista ? Visibility.Visible : Visibility.Collapsed;

            btnManutencoes.Visibility = administrador || rececionista ? Visibility.Visible : Visibility.Collapsed;
            btnUtilizadores.Visibility =

                Sessao.Perfil.Equals("Administrador", StringComparison.OrdinalIgnoreCase) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btnUtilizadores_Click(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "Utilizadores";
            MainFrame.Navigate(new UtilizadoresPage());
        }

        private void btnPresencas_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PresencasPage());

            txtTituloPagina.Text = "Presenças";
        }
    }
}
