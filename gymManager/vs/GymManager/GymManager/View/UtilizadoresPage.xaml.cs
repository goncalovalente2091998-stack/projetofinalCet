using GymManager.Helpers;
using GymManager.Models;
using GymManager.Services;
using GymManager.View;

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
    /// Interaction logic for UtilizadoresPage.xaml
    /// </summary>
    public partial class UtilizadoresPage : Page
    {
        private readonly UtilizadorService service =
              new UtilizadorService();

        public UtilizadoresPage()
        {
            InitializeComponent();
            CarregarUtilizadores();
        }

        private void CarregarUtilizadores()
        {
            try
            {
                dgUtilizadores.ItemsSource = service.Listar();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível carregar os utilizadores.\n\n" +
                    ex.Message);
            }
        }

        private void btnNovo_Click(
            object sender,
            RoutedEventArgs e)
        {
            UtilizadorForm form = new UtilizadorForm
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarUtilizadores();
            }
        }

        private void btnEditar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button botao ||
                botao.Tag is not Utilizador utilizador)
            {
                Mensagem.Aviso(
                    "Não foi possível identificar o utilizador.");
                return;
            }

            UtilizadorForm form = new UtilizadorForm(utilizador)
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarUtilizadores();
            }
        }

        private void btnEliminar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button botao ||
                botao.Tag is not Utilizador utilizador)
            {
                Mensagem.Aviso(
                    "Não foi possível identificar o utilizador.");
                return;
            }

            if (utilizador.IdUtilizador == Sessao.IdUtilizador)
            {
                Mensagem.Aviso(
                    "Não pode eliminar o utilizador com sessão iniciada.");
                return;
            }

            if (!Mensagem.Confirmar(
                $"Tem a certeza que pretende eliminar '{utilizador.Nome}'?"))
            {
                return;
            }

            try
            {
                service.Eliminar(utilizador.IdUtilizador);

                Mensagem.Sucesso(
                    "Utilizador eliminado com sucesso!");

                CarregarUtilizadores();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível eliminar o utilizador.\n\n" +
                    ex.Message);
            }
        }

        private void txtPesquisar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            try
            {
                string pesquisa =
                    txtPesquisar.Text.Trim();

                dgUtilizadores.ItemsSource =
                    string.IsNullOrWhiteSpace(pesquisa)
                        ? service.Listar()
                        : service.Pesquisar(pesquisa);
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível pesquisar os utilizadores.\n\n" +
                    ex.Message);
            }
        }
    }
}
