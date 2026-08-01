using GymManager.Helpers;
using GymManager.Models;
using GymManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GymManager.View.Forms
{
    public partial class ReservaAulaForm : Window
    {
        private readonly Aula aula;

        private readonly ReservaAulaService reservaService =
            new ReservaAulaService();

        private readonly ClienteService clienteService =
            new ClienteService();

        private List<Cliente> clientes = new();

        private Cliente? clienteSelecionado;

        private bool atualizandoTextoCliente;

        public ReservaAulaForm(
            Aula aulaSelecionada)
        {
            InitializeComponent();

            aula =
                aulaSelecionada;

            PreencherDadosAula();

            CarregarClientes();

            txtCliente.Focus();
        }

        private void PreencherDadosAula()
        {
            txtNomeAula.Text =
                aula.Nome;

            txtDadosAula.Text =
                $"{aula.DataAula:dd/MM/yyyy} às " +
                $"{aula.HoraInicio:hh\\:mm} — " +
                $"{aula.Sala}";
        }

        private void CarregarClientes()
        {
            try
            {
                clientes =
                    clienteService.Listar();

                lstClientes.ItemsSource =
                    clientes;
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível carregar os clientes.\n\n" +
                    ex.Message);
            }
        }

        private void txtCliente_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            if (atualizandoTextoCliente)
            {
                return;
            }

            clienteSelecionado =
                null;

            pnlClienteSelecionado.Visibility =
                Visibility.Collapsed;

            string pesquisa =
                txtCliente.Text.Trim();

            if (string.IsNullOrWhiteSpace(
                    pesquisa))
            {
                lstClientes.ItemsSource =
                    clientes;

                popupClientes.IsOpen =
                    false;

                return;
            }

            List<Cliente> resultados =
                clientes
                    .Where(c =>
                        c.Nome.Contains(
                            pesquisa,
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        c.NIF.Contains(
                            pesquisa,
                            StringComparison.OrdinalIgnoreCase))
                    .Take(15)
                    .ToList();

            lstClientes.ItemsSource =
                resultados;

            lstClientes.SelectedIndex =
                -1;

            popupClientes.IsOpen =
                resultados.Count > 0;
        }

        private void txtCliente_KeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.Key == Key.Down &&
                popupClientes.IsOpen &&
                lstClientes.Items.Count > 0)
            {
                lstClientes.Focus();

                lstClientes.SelectedIndex =
                    0;

                e.Handled =
                    true;

                return;
            }

            if (e.Key == Key.Enter &&
                popupClientes.IsOpen &&
                lstClientes.Items.Count == 1 &&
                lstClientes.Items[0] is Cliente cliente)
            {
                SelecionarCliente(
                    cliente);

                e.Handled =
                    true;

                return;
            }

            if (e.Key == Key.Escape)
            {
                popupClientes.IsOpen =
                    false;

                e.Handled =
                    true;
            }
        }

        private void lstClientes_KeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.Key == Key.Enter &&
                lstClientes.SelectedItem is
                    Cliente cliente)
            {
                SelecionarCliente(
                    cliente);

                e.Handled =
                    true;

                return;
            }

            if (e.Key == Key.Escape)
            {
                popupClientes.IsOpen =
                    false;

                txtCliente.Focus();

                e.Handled =
                    true;
            }
        }

        private void lstClientes_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!lstClientes.IsKeyboardFocusWithin)
            {
                return;
            }

            if (lstClientes.SelectedItem is
                Cliente cliente)
            {
                SelecionarCliente(
                    cliente);
            }
        }

        private void lstClientes_MouseDoubleClick(
            object sender,
            MouseButtonEventArgs e)
        {
            if (lstClientes.SelectedItem is
                Cliente cliente)
            {
                SelecionarCliente(
                    cliente);
            }
        }

        private void SelecionarCliente(
            Cliente cliente)
        {
            clienteSelecionado =
                cliente;

            atualizandoTextoCliente =
                true;

            /*
             * Depois da seleção mostramos apenas o nome,
             * sem colocar NIF ou data de nascimento na TextBox.
             */
            txtCliente.Text =
                cliente.Nome;

            txtCliente.CaretIndex =
                txtCliente.Text.Length;

            atualizandoTextoCliente =
                false;

            popupClientes.IsOpen =
                false;

            lstClientes.SelectedIndex =
                -1;

            txtClienteSelecionado.Text =
                cliente.Nome;

            txtNifSelecionado.Text =
                $"NIF: {cliente.NIF}";

            pnlClienteSelecionado.Visibility =
                Visibility.Visible;
        }

        private void btnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (clienteSelecionado == null)
            {
                Mensagem.Aviso(
                    "Pesquise e selecione um cliente da lista.");

                txtCliente.Focus();
                return;
            }

            if (!string.Equals(
                    aula.Estado,
                    "Agendada",
                    StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso(
                    "Apenas aulas agendadas aceitam reservas.");

                return;
            }

            if (aula.DataAula.Date <
                DateTime.Today)
            {
                Mensagem.Aviso(
                    "Não é possível reservar uma aula que já terminou.");

                return;
            }

            if (!Mensagem.Confirmar(
                    $"Pretende reservar '{aula.Nome}' para " +
                    $"'{clienteSelecionado.Nome}'?"))
            {
                return;
            }

            try
            {
                reservaService.Inserir(
                    aula.IdAula,
                    clienteSelecionado.IdCliente);

                Mensagem.Sucesso(
                    "Reserva criada com sucesso!");

                DialogResult =
                    true;
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível criar a reserva.\n\n" +
                    ex.Message);
            }
        }

        private void btnCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}