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
    /// Interaction logic for ClientesPage.xaml
    /// </summary>
    public partial class ClientesPage : Page
    {
        private readonly ClienteService service = new ClienteService();

        public ClientesPage()
        {
            InitializeComponent();

            CarregarClientes();
        }

        private void CarregarClientes()
        {
            dgClientes.ItemsSource = service.Listar();
        }

        private void btnNovo_Click(object sender, RoutedEventArgs e)
        {
            ClienteForm form = new ClienteForm();

            if (form.ShowDialog() == true)
            {
                CarregarClientes();
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
         
            if (dgClientes.SelectedItem == null)
            {
                Mensagem.Aviso("Selecione um cliente.");
                return;
            }

            Cliente cliente = (Cliente)dgClientes.SelectedItem;

            ClienteForm form = new ClienteForm(cliente);

            if (form.ShowDialog() == true)
            {
                CarregarClientes();
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgClientes.SelectedItem == null)
            {
                Mensagem.Aviso("Selecione um cliente.");
                return;
            }

            Cliente cliente = (Cliente)dgClientes.SelectedItem;

            if (!Mensagem.Confirmar($"Tem a certeza que pretende eliminar o cliente '{cliente.Nome}'?"))
                return;

            try
            {
                service.Eliminar(cliente.IdCliente);

                Mensagem.Sucesso("Cliente eliminado com sucesso.");

                CarregarClientes();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível eliminar o cliente.\n\n" + ex.Message);
            }
        }

        private void txtPesquisar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPesquisar.Text))
            {
                CarregarClientes();
                return;
            }

            dgClientes.ItemsSource = service.Pesquisar(txtPesquisar.Text);
        }
    }
    
}
