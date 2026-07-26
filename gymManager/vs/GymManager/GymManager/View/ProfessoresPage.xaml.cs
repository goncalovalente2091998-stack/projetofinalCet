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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GymManager.Services;
using GymManager.View.Forms;
using GymManager.Models;
using GymManager.Helpers;

namespace GymManager.View
{
    /// <summary>
    /// Interaction logic for ProfessoresPage.xaml
    /// </summary>
    public partial class ProfessoresPage : Page
    {
        private readonly ProfessoresService service = new ProfessoresService();
     
        public ProfessoresPage()
        {
            InitializeComponent();

            CarregarProfessores();
        }

        private void CarregarProfessores()
        {
            try
            {
                var lista = service.Listar();

                dgProfessores.ItemsSource = lista;
                txtTotalProfessores.Text = lista.Count.ToString();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível carregar os professores.\n\n" +
                    ex.Message);
            }
        }

        private void txtPesquisar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            try
            {
                string pesquisa = txtPesquisar.Text.Trim();

                var lista = string.IsNullOrWhiteSpace(pesquisa)
                    ? service.Listar()
                    : service.Pesquisar(pesquisa);

                dgProfessores.ItemsSource = lista;
                txtTotalProfessores.Text = lista.Count.ToString();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível pesquisar os professores.\n\n" +
                    ex.Message);
            }
        }

        private void btnNovo_Click(object sender, RoutedEventArgs e)
        {
            ProfessoresForm form = new ProfessoresForm();

            if (form.ShowDialog() == true)
            {
                CarregarProfessores();
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
      
            if (dgProfessores.SelectedItem is not Professor professor)
            {
                Mensagem.Aviso("Selecione um professor.");
                return;
            }

            ProfessoresForm form = new ProfessoresForm(professor)
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarProfessores();
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgProfessores.SelectedItem is not Professor professor)
            {
                Mensagem.Aviso("Selecione um professor.");
                return;
            }

            if (!Mensagem.Confirmar(
                $"Tem a certeza que pretende eliminar o professor '{professor.Nome}'?"))
            {
                return;
            }

            try
            {
                service.Eliminar(professor.IdProfessor);

                Mensagem.Sucesso("Professor eliminado com sucesso.");

                CarregarProfessores();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível eliminar o professor.\n\n" +
                    ex.Message);
            }
        }
    }
    
}
