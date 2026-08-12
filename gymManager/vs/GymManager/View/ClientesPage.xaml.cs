using GymManager.Helpers;
using GymManager.Models;
using GymManager.Services;
using GymManager.View.Forms;
using Microsoft.Win32;
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
            try
            {
                var lista = service.Listar();

                dgClientes.ItemsSource = lista;
                txtTotalClientes.Text = lista.Count.ToString();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível carregar os clientes.\n\n" + ex.Message);
            }
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
            if (!IsLoaded)
                return;

            try
            {
                string pesquisa = txtPesquisar.Text.Trim();

                var lista = string.IsNullOrWhiteSpace(pesquisa) ? service.Listar() : service.Pesquisar(pesquisa);

                dgClientes.ItemsSource = lista;
                txtTotalClientes.Text = lista.Count.ToString();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível pesquisar os clientes.\n\n" + ex.Message);
            }


        }
        private void BtnExportarPdf_Click(object sender, RoutedEventArgs e)
        {
            if (dgClientes.SelectedItem == null)
            {
                Mensagem.Aviso("Selecione um cliente.");
                return;
            }

            Cliente cliente = (Cliente)dgClientes.SelectedItem;

            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "PDF (*.pdf)|*.pdf";

            dlg.FileName = $"Cliente_{cliente.Nome}.pdf";

            if (dlg.ShowDialog() == true)
            {
                PdfCliente.Gerar(dlg.FileName, cliente);

                Mensagem.Sucesso("PDF criado com sucesso.");
            }
        }

        private void BtnDossier_Click(object sender, RoutedEventArgs e)
        {
            if (dgClientes.SelectedItem == null)
            {
                Mensagem.Aviso("Selecione um cliente.");
                return;
            }

            Cliente cliente = (Cliente)dgClientes.SelectedItem;

            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "PDF (*.pdf)|*.pdf";

            dlg.FileName = $"Dossier_{cliente.Nome}.pdf";

            if (dlg.ShowDialog() == true)
            {
                PdfDossier.Gerar(dlg.FileName, cliente);

                Mensagem.Sucesso("Dossier criado com sucesso.");
            }
        }
    }

}
