using GymManager.Helpers;
using GymManager.Models;
using GymManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GymManager.View
{
    public partial class PresencasPage : Page
    {
        private readonly PresencaService presencaService = new PresencaService();

        private readonly ClienteService clienteService = new ClienteService();

        private List<Presenca> presencas = new List<Presenca>();

        private List<Presenca> presencasAtivas = new List<Presenca>();

        private List<Cliente> clientes = new List<Cliente>();

        private Cliente? clienteSelecionado;

        private bool atualizarPesquisaCliente;

        public PresencasPage()
        {
            InitializeComponent();

            Loaded += PresencasPage_Loaded;
        }

        private void PresencasPage_Loaded(object sender, RoutedEventArgs e)
        {
            CarregarTudo();
        }

        private void PresencasPage_Unloaded(object sender, RoutedEventArgs e)
        {
            popupClientes.IsOpen = false;
        }

        private void CarregarTudo()
        {
            try
            {
                CarregarClientes();
                CarregarPresencas();
                CarregarPresencasAtivas();
                AtualizarEstatisticas();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível carregar as presenças.\n\n" + ex.Message);
            }
        }

        private void CarregarClientes()
        {
            clientes = clienteService.Listar();

            lstClientes.ItemsSource = clientes.Take(8).ToList();
        }

        private void CarregarPresencas()
        {
            presencas = presencaService.Listar();

            dgPresencas.ItemsSource = presencas;
        }

        private void CarregarPresencasAtivas()
        {
            presencasAtivas = presencaService.ListarAtivas();

            lstPresencasAtivas.ItemsSource = presencasAtivas;
        }

        private void AtualizarEstatisticas()
        {
            txtTotalPresentes.Text = presencasAtivas.Count.ToString();

            txtEntradasHoje.Text = presencas.Count(p =>
                    p.DataEntrada.Date == DateTime.Today).ToString();

            List<Presenca> concluidas = presencas.Where(p =>
                        p.DataSaida.HasValue && p.DataSaida.Value >= p.DataEntrada).ToList();

            if (concluidas.Count == 0)
            {
                txtTempoMedio.Text = "-";

                return;
            }

            double minutos = concluidas.Average(p =>
                    (
                        p.DataSaida!.Value - p.DataEntrada).TotalMinutes);

            txtTempoMedio.Text = FormatarDuracao(TimeSpan.FromMinutes(minutos));
        }

        private static string FormatarDuracao(TimeSpan duracao)
        {
            if (duracao.TotalMinutes < 1)
            {
                return "< 1 min";
            }

            int horas = (int)duracao.TotalHours;

            int minutos = duracao.Minutes;

            if (horas <= 0)
            {
                return $"{minutos} min";
            }

            return minutos > 0 ? $"{horas}h {minutos}min" : $"{horas}h";
        }

        private List<Cliente> FiltrarClientes(string pesquisa)
        {
            IEnumerable<Cliente> resultado = clientes;

            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                resultado =
                    resultado.Where(c =>
                        ContemTexto(c.Nome, pesquisa) || ContemTexto(c.NIF, pesquisa));
            }

            return resultado.Take(8).ToList();
        }

        private static bool ContemTexto(string? texto, string pesquisa)
        {
            return !string.IsNullOrWhiteSpace(texto) && texto.Contains(pesquisa, StringComparison.OrdinalIgnoreCase);
        }

        private void SelecionarClientePorId(int idCliente)
        {
            Cliente? cliente = clientes.FirstOrDefault(c =>
                    c.IdCliente == idCliente);

            if (cliente == null)
            {
                Mensagem.Aviso("Não foi possível localizar o cliente selecionado.");

                return;
            }

            SelecionarCliente(cliente);
        }

        private void SelecionarCliente(Cliente cliente)
        {
            atualizarPesquisaCliente = true;

            clienteSelecionado = cliente;

            txtPesquisarCliente.Text = cliente.Nome;

            txtPesquisarCliente.CaretIndex = txtPesquisarCliente.Text.Length;

            popupClientes.IsOpen = false;

            lstClientes.SelectedIndex = -1;

            atualizarPesquisaCliente = false;
        }

        private void lstPresencasAtivas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstPresencasAtivas.SelectedItem is not Presenca presenca)
            {
                return;
            }

            SelecionarClientePorId(presenca.IdCliente);
        }

        private void dgPresencas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgPresencas.SelectedItem is not Presenca presenca)
            {
                return;
            }

            SelecionarClientePorId(presenca.IdCliente);
        }

        private void txtPesquisarCliente_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (atualizarPesquisaCliente)
            {
                return;
            }

            clienteSelecionado = null;

            List<Cliente> resultados = FiltrarClientes(txtPesquisarCliente.Text.Trim());

            lstClientes.ItemsSource = resultados;

            lstClientes.SelectedIndex = -1;

            popupClientes.IsOpen = txtPesquisarCliente.IsKeyboardFocusWithin && resultados.Count > 0;
        }

        private void txtPesquisarCliente_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AbrirPopupClientes();
        }

        private void AbrirPopupClientes()
        {
            List<Cliente> resultados = FiltrarClientes(txtPesquisarCliente.Text.Trim());

            lstClientes.ItemsSource = resultados;

            if (resultados.Count == 0)
            {
                popupClientes.IsOpen = false;

                return;
            }

            lstClientes.SelectedIndex = -1;

            popupClientes.IsOpen = true;
        }

        private void txtPesquisarCliente_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            List<Cliente> resultados = lstClientes.Items.OfType<Cliente>().ToList();

            if (e.Key == Key.Down)
            {
                if (resultados.Count == 0)
                {
                    AbrirPopupClientes();

                    resultados = lstClientes.Items.OfType<Cliente>().ToList();
                }

                if (resultados.Count == 0)
                {
                    return;
                }

                popupClientes.IsOpen = true;

                if (lstClientes.SelectedIndex < resultados.Count - 1)
                {
                    lstClientes.SelectedIndex++;
                }
                else
                {
                    lstClientes.SelectedIndex =
                        0;
                }

                lstClientes.ScrollIntoView(lstClientes.SelectedItem);

                e.Handled = true;

                return;
            }

            if (e.Key == Key.Up)
            {
                if (!popupClientes.IsOpen || resultados.Count == 0)
                {
                    return;
                }

                if (lstClientes.SelectedIndex > 0)
                {
                    lstClientes.SelectedIndex--;
                }
                else
                {
                    lstClientes.SelectedIndex = resultados.Count - 1;
                }

                lstClientes.ScrollIntoView(lstClientes.SelectedItem);

                e.Handled = true;

                return;
            }

            if (e.Key == Key.Enter && lstClientes.SelectedItem is Cliente cliente)
            {
                SelecionarCliente(cliente);

                e.Handled = true;

                return;
            }

            if (e.Key == Key.Escape)
            {
                FecharPopupClientes();

                e.Handled = true;
            }
        }

        private void lstClientes_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && lstClientes.SelectedItem is Cliente cliente)
            {
                SelecionarCliente(cliente);

                e.Handled = true;

                return;
            }

            if (e.Key == Key.Escape)
            {
                FecharPopupClientes();

                txtPesquisarCliente.Focus();

                e.Handled = true;
            }
        }

        private void lstClientes_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is not DependencyObject origem)
            {
                return;
            }

            ListBoxItem? item = ObterAscendente<ListBoxItem>(origem);

            if (item?.DataContext is not Cliente cliente)
            {
                return;
            }

            SelecionarCliente(cliente);

            e.Handled = true;
        }

        private void LimparClienteSelecionado()
        {
            atualizarPesquisaCliente = true;

            clienteSelecionado = null;

            txtPesquisarCliente.Clear();

            lstClientes.ItemsSource = clientes.Take(8).ToList();

            lstClientes.SelectedIndex = -1;

            lstPresencasAtivas.SelectedItem = null;

            dgPresencas.SelectedItem = null;

            popupClientes.IsOpen = false;

            atualizarPesquisaCliente = false;
        }

        private void FecharPopupClientes()
        {
            popupClientes.IsOpen = false;

            lstClientes.SelectedIndex = -1;
        }

        private void Pagina_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!popupClientes.IsOpen)
            {
                return;
            }

            if (e.OriginalSource is not DependencyObject origem)
            {
                FecharPopupClientes();
                return;
            }

            if (EhDescendenteDe(origem, txtPesquisarCliente))
            {
                return;
            }

            FecharPopupClientes();
        }

        private static bool EhDescendenteDe(DependencyObject origem, DependencyObject controlo)
        {
            DependencyObject? atual = origem;

            while (atual != null)
            {
                if (ReferenceEquals(atual, controlo))
                {
                    return true;
                }

                atual = VisualTreeHelper.GetParent(atual);
            }

            return false;
        }

        private static T? ObterAscendente<T>(DependencyObject origem) where T : DependencyObject
        {
            DependencyObject? atual = origem;

            while (atual != null)
            {
                if (atual is T resultado)
                {
                    return resultado;
                }

                atual = VisualTreeHelper.GetParent(atual);
            }

            return null;
        }

        private void btnRegistarEntrada_Click(object sender, RoutedEventArgs e)
        {
            if (clienteSelecionado == null)
            {
                Mensagem.Aviso("Pesquise ou selecione um cliente.");

                return;
            }

            if (presencasAtivas.Any(p =>
                    p.IdCliente == clienteSelecionado.IdCliente))
            {
                Mensagem.Aviso("Este cliente já se encontra no ginásio.");

                return;
            }

            if (!Mensagem.Confirmar(
                    "Pretende registar a entrada deste cliente?\n\n" +
                    $"Cliente: {clienteSelecionado.Nome}\n" +
                    $"Data: {DateTime.Now:dd/MM/yyyy}\n" +
                    $"Hora: {DateTime.Now:HH:mm}"))
            {
                return;
            }

            try
            {
                presencaService.RegistarEntrada(clienteSelecionado.IdCliente, null);

                Mensagem.Sucesso("Entrada registada com sucesso!");

                LimparClienteSelecionado();
                CarregarTudo();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível registar a entrada.\n\n" + ex.Message);
            }
        }

        private void btnRegistarSaida_Click(object sender, RoutedEventArgs e)
        {
            if (clienteSelecionado == null)
            {
                Mensagem.Aviso("Pesquise ou selecione um cliente.");

                return;
            }

            Presenca? presencaAtiva = presencasAtivas.FirstOrDefault(p =>
                    p.IdCliente == clienteSelecionado.IdCliente);

            if (presencaAtiva == null)
            {
                Mensagem.Aviso("Este cliente não possui nenhuma entrada aberta.");

                return;
            }

            TimeSpan duracao = DateTime.Now - presencaAtiva.DataEntrada;

            if (!Mensagem.Confirmar(
                    "Pretende registar a saída deste cliente?\n\n" +
                    $"Cliente: {clienteSelecionado.Nome}\n" +
                    $"Entrada: {presencaAtiva.DataEntrada:dd/MM/yyyy HH:mm}\n" +
                    $"Saída: {DateTime.Now:dd/MM/yyyy HH:mm}\n" + $"Permanência: {FormatarDuracao(duracao)}"))
            {
                return;
            }

            try
            {
                presencaService.RegistarSaida(clienteSelecionado.IdCliente);

                Mensagem.Sucesso("Saída registada com sucesso!");

                LimparClienteSelecionado();
                CarregarTudo();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível registar a saída.\n\n" + ex.Message);
            }
        }

        private void btnAtualizar_Click(object sender, RoutedEventArgs e)
        {
            CarregarTudo();
        }

        private void txtPesquisarHistorico_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsInitialized)
            {
                return;
            }

            string pesquisa = txtPesquisarHistorico.Text.Trim();

            if (string.IsNullOrWhiteSpace(pesquisa))
            {
                dgPresencas.ItemsSource = presencas;

                return;
            }

            dgPresencas.ItemsSource = presencas.Where(p =>
                        ContemTexto(p.NomeCliente, pesquisa) || ContemTexto(p.NIF, pesquisa) || ContemTexto(p.Observacoes, pesquisa) || ContemTexto(p.Estado, pesquisa) || ContemTexto(p.DataEntradaFormatada, pesquisa) || ContemTexto(p.DataSaidaFormatada, pesquisa)).ToList();
        }
    }
}