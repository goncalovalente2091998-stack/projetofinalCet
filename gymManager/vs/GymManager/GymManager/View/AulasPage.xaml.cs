using GymManager.Helpers;
using GymManager.Models;
using GymManager.Services;
using GymManager.View.Forms;
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

namespace GymManager.View
{
    /// <summary>
    /// Interaction logic for AulasPage.xaml
    /// </summary>
    public partial class AulasPage : Page
    {
        private readonly AulaService service =
             new AulaService();

        public AulasPage()
        {
            InitializeComponent();

            Loaded += AulasPage_Loaded;
        }
        private void AulasPage_Loaded(
    object sender,
    RoutedEventArgs e)
        {
            CarregarAulas();
        }
        private void CarregarAulas()
        {
            try
            {
                List<Aula> lista =
                    service.Listar();

                AtualizarPagina(lista);
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível carregar as aulas.\n\n" +
                    ex.Message);
            }
        }

        private void AtualizarPagina(
            List<Aula> lista)
        {
            dgAulas.ItemsSource =
                lista;

            txtTotal.Text =
                lista.Count.ToString();
        }

        private void btnNova_Click(
            object sender,
            RoutedEventArgs e)
        {
            AulaForm form = new AulaForm
                {
                    Owner = Window.GetWindow(this)
                };

            if (form.ShowDialog() == true)
            {
                CarregarAulas();
            }
        }

        private void btnEditar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgAulas.SelectedItem is not Aula aula)
            {
                Mensagem.Aviso(
                    "Selecione uma aula.");

                return;
            }

            if (string.Equals(
                    aula.Estado,
                    "Concluída",
                    StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso(
                    "Uma aula concluída não pode ser editada.");

                return;
            }

            AulaForm form = new AulaForm(aula)
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarAulas();
            }
        }

        private void btnEliminar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgAulas.SelectedItem is not Aula aula)
            {
                Mensagem.Aviso(
                    "Selecione uma aula.");

                return;
            }

            if (string.Equals(
                    aula.Estado,
                    "Concluída",
                    StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso(
                    "Uma aula concluída deve ser mantida para preservar o histórico.");

                return;
            }

            if (!Mensagem.Confirmar(
                    $"Tem a certeza que pretende eliminar a aula " +
                    $"'{aula.Nome}' de {aula.DataAula:dd/MM/yyyy} " +
                    $"às {aula.HoraInicio:hh\\:mm}?"))
            {
                return;
            }

            try
            {
                service.Eliminar(
                    aula.IdAula);

                Mensagem.Sucesso(
                    "Aula eliminada com sucesso!");

                CarregarAulas();
            }
            catch (Exception ex)
            {
                Mensagem.Aviso(
                    "A aula não pode ser eliminada.\n\n" +
                    ex.Message);
            }
        }

        private void txtPesquisar_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            try
            {
                string pesquisa =
                    txtPesquisar.Text.Trim();

                List<Aula> lista =
                    string.IsNullOrWhiteSpace(pesquisa)
                        ? service.Listar()
                        : service.Pesquisar(pesquisa);

                AtualizarPagina(lista);
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível pesquisar as aulas.\n\n" +
                    ex.Message);
            }
        }

        private void btnReservas_Click(object sender, RoutedEventArgs e)
        {
            if (dgAulas.SelectedItem is not Aula aula)
            {
                Mensagem.Aviso(
                    "Selecione uma aula.");

                return;
            }

            NavigationService?.Navigate(
                new ReservasAulasPage(aula));
        }
    }
}
