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
    /// Interaction logic for InscricoesPage.xaml
    /// </summary>
    public partial class InscricoesPage : Page
    {
        private readonly InscricaoService service = new InscricaoService();
        public InscricoesPage()
        {
            InitializeComponent();
            CarregarInscricoes();
        }
    

        private void CarregarInscricoes()
        {
            try
            {
                var lista = service.Listar();

                dgInscricoes.ItemsSource = lista;

                txtTotal.Text = lista.Count.ToString();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(ex.Message);
            }
        }
        private void btnNovo_Click(
    object sender,
    RoutedEventArgs e)
        {
            InscricaoForm form = new InscricaoForm
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarInscricoes();
            }
        }

        private void btnEditar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgInscricoes.SelectedItem is not Inscricao inscricao)
            {
                Mensagem.Aviso("Selecione uma inscrição.");
                return;
            }

            InscricaoForm form = new InscricaoForm(inscricao)
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarInscricoes();
            }
        }

        private void btnEliminar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgInscricoes.SelectedItem is not Inscricao inscricao)
            {
                Mensagem.Aviso("Selecione uma inscrição.");
                return;
            }

            if (!Mensagem.Confirmar(
                $"Tem a certeza que pretende eliminar a inscrição de " +
                $"'{inscricao.NomeCliente}'?"))
            {
                return;
            }

            try
            {
                service.Eliminar(inscricao.IdInscricao);

                Mensagem.Sucesso(
                    "Inscrição eliminada com sucesso!");

                CarregarInscricoes();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível eliminar a inscrição.\n\n" +
                    ex.Message);
            }
        }
        private void txtPesquisar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            string pesquisa = txtPesquisar.Text.Trim();

            var lista =
                string.IsNullOrWhiteSpace(pesquisa)
                    ? service.Listar()
                    : service.Pesquisar(pesquisa);

            dgInscricoes.ItemsSource = lista;

            txtTotal.Text = lista.Count.ToString();
        }
    }
}
