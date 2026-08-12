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

    public partial class PlanosPage : Page
    {
        private readonly PlanoService service = new PlanoService();

        public PlanosPage()
        {
            InitializeComponent();
            CarregarPlanos();
        }

        private void CarregarPlanos()
        {
            try
            {
                List<Plano> lista = service.Listar();

                icPlanos.ItemsSource = lista;
                txtTotalPlanos.Text = lista.Count.ToString();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível carregar os planos.\n\n" + ex.Message);
            }
        }

        private void btnNovo_Click(object sender, RoutedEventArgs e)
        {
            PlanoForm form = new PlanoForm
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarPlanos();
            }
        }

        private void btnEditarCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button botao || botao.Tag is not Plano plano)
            {
                Mensagem.Aviso("Não foi possível identificar o plano.");
                return;
            }

            PlanoForm form = new PlanoForm(plano)
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarPlanos();
            }
        }

        private void btnEliminarCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button botao || botao.Tag is not Plano plano)
            {
                Mensagem.Aviso("Não foi possível identificar o plano.");
                return;
            }

            if (!Mensagem.Confirmar($"Tem a certeza que pretende eliminar o plano '{plano.Nome}'?"))
            {
                return;
            }

            try
            {
                service.Eliminar(plano.IdPlano);

                Mensagem.Sucesso("Plano eliminado com sucesso!");

                CarregarPlanos();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível eliminar o plano.\n\n" + ex.Message);
            }
        }

        private void txtPesquisar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            try
            {
                string pesquisa = txtPesquisar.Text.Trim();

                List<Plano> lista = string.IsNullOrWhiteSpace(pesquisa) ? service.Listar() : service.Pesquisar(pesquisa);

                icPlanos.ItemsSource = lista;
                txtTotalPlanos.Text = lista.Count.ToString();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível pesquisar os planos.\n\n" + ex.Message);
            }
        }
    }
}
