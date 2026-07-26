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
    public partial class InscricaoForm : Window
    {
        private readonly InscricaoService inscricaoService =
            new InscricaoService();

        private readonly ClienteService clienteService =
            new ClienteService();

        private readonly PlanoService planoService =
            new PlanoService();

        private readonly Inscricao? inscricao;

        private List<Cliente> clientes = new();

        private List<Plano> planos = new();

        private Cliente? clienteSelecionado;

        private bool atualizandoTextoCliente;

        public InscricaoForm()
        {
            InitializeComponent();

            CarregarDados();

            dpDataInicio.SelectedDate = DateTime.Today;
            cmbEstado.SelectedIndex = 0;
        }

        public InscricaoForm(Inscricao inscricao)
        {
            InitializeComponent();

            this.inscricao = inscricao;

            Title = "Editar Inscrição";
            txtTitulo.Text = "Editar Inscrição";

            CarregarDados();

            Cliente? cliente = clientes
                .FirstOrDefault(c =>
                    c.IdCliente == inscricao.IdCliente);

            if (cliente != null)
            {
                SelecionarCliente(cliente);
            }

            cmbPlano.SelectedValue = inscricao.IdPlano;
            dpDataInicio.SelectedDate = inscricao.DataInicio;
            dpDataFim.SelectedDate = inscricao.DataFim;

            SelecionarEstado(inscricao.Estado);
        }

        private void CarregarDados()
        {
            try
            {
                clientes = clienteService.Listar();
                planos = planoService.Listar();

                lstClientes.ItemsSource = clientes;
                cmbPlano.ItemsSource = planos;
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível carregar clientes e planos.\n\n" +
                    ex.Message);
            }
        }

        private void txtCliente_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            if (atualizandoTextoCliente)
                return;

            clienteSelecionado = null;

            string pesquisa = txtCliente.Text.Trim();

            if (string.IsNullOrWhiteSpace(pesquisa))
            {
                lstClientes.ItemsSource = clientes;
                popupClientes.IsOpen = false;
                return;
            }

            List<Cliente> resultados = clientes
                .Where(c =>
                    c.Nome.Contains(
                        pesquisa,
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    c.NIF.Contains(
                        pesquisa,
                        StringComparison.OrdinalIgnoreCase))
                .Take(10)
                .ToList();

            lstClientes.ItemsSource = resultados;
            lstClientes.SelectedIndex = -1;

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
                lstClientes.SelectedIndex = 0;

                e.Handled = true;
                return;
            }

            if (e.Key == Key.Escape)
            {
                popupClientes.IsOpen = false;
                e.Handled = true;
            }
        }

        private void lstClientes_KeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.Key == Key.Enter &&
                lstClientes.SelectedItem is Cliente cliente)
            {
                SelecionarCliente(cliente);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Escape)
            {
                popupClientes.IsOpen = false;
                txtCliente.Focus();
                e.Handled = true;
            }
        }

        private void lstClientes_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!lstClientes.IsKeyboardFocusWithin)
                return;

            if (lstClientes.SelectedItem is Cliente cliente)
            {
                SelecionarCliente(cliente);
            }
        }

        private void lstClientes_MouseDoubleClick(
            object sender,
            MouseButtonEventArgs e)
        {
            if (lstClientes.SelectedItem is Cliente cliente)
            {
                SelecionarCliente(cliente);
            }
        }

        private void SelecionarCliente(Cliente cliente)
        {
            clienteSelecionado = cliente;

            atualizandoTextoCliente = true;

            txtCliente.Text = cliente.Nome;

            txtCliente.CaretIndex =
                txtCliente.Text.Length;

            atualizandoTextoCliente = false;

            popupClientes.IsOpen = false;
            lstClientes.SelectedIndex = -1;
        }

        private void cmbPlano_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            AtualizarDataFim();
        }

        private void dpDataInicio_SelectedDateChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            AtualizarDataFim();
        }

        private void AtualizarDataFim()
        {
            if (cmbPlano.SelectedItem is not Plano plano ||
                !dpDataInicio.SelectedDate.HasValue)
            {
                return;
            }

            DateTime inicio =
                dpDataInicio.SelectedDate.Value.Date;

            dpDataFim.SelectedDate =
                inicio.AddMonths(plano.DuracaoMeses);

            txtResumoPlano.Text =
                $"{plano.Nome}: " +
                $"{plano.DuracaoMeses} meses, " +
                $"{plano.Preco:F2} €.";
        }

        private void SelecionarEstado(string estado)
        {
            foreach (object item in cmbEstado.Items)
            {
                if (item is ComboBoxItem comboItem &&
                    string.Equals(
                        comboItem.Content?.ToString(),
                        estado,
                        StringComparison.OrdinalIgnoreCase))
                {
                    cmbEstado.SelectedItem = comboItem;
                    return;
                }
            }

            cmbEstado.SelectedIndex = 0;
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

            if (cmbPlano.SelectedValue is not int idPlano)
            {
                Mensagem.Aviso("Selecione um plano.");
                cmbPlano.Focus();
                return;
            }

            if (!dpDataInicio.SelectedDate.HasValue)
            {
                Mensagem.Aviso(
                    "Selecione a data de início.");

                dpDataInicio.Focus();
                return;
            }

            if (!dpDataFim.SelectedDate.HasValue)
            {
                Mensagem.Aviso(
                    "Selecione a data de fim.");

                dpDataFim.Focus();
                return;
            }

            if (dpDataFim.SelectedDate.Value.Date <
                dpDataInicio.SelectedDate.Value.Date)
            {
                Mensagem.Aviso(
                    "A data de fim não pode ser anterior à data de início.");

                dpDataFim.Focus();
                return;
            }

            if (cmbEstado.SelectedItem is not
                ComboBoxItem estadoItem)
            {
                Mensagem.Aviso("Selecione o estado.");
                cmbEstado.Focus();
                return;
            }

            string estado =
                estadoItem.Content?.ToString()
                ?? string.Empty;

            bool novaInscricao =
                inscricao == null;

            string operacao =
                novaInscricao
                    ? "criar"
                    : "atualizar";

            if (!Mensagem.Confirmar(
                $"Tem a certeza que pretende {operacao} esta inscrição?"))
            {
                return;
            }

            Inscricao dados = new Inscricao
            {
                IdInscricao =
                    inscricao?.IdInscricao ?? 0,

                IdCliente =
                    clienteSelecionado.IdCliente,

                IdPlano =
                    idPlano,

                DataInicio =
                    dpDataInicio.SelectedDate.Value.Date,

                DataFim =
                    dpDataFim.SelectedDate.Value.Date,

                Estado =
                    estado
            };

            try
            {
                if (novaInscricao)
                {
                    inscricaoService.Inserir(dados);

                    Mensagem.Sucesso(
                        "Inscrição criada com sucesso!");
                }
                else
                {
                    inscricaoService.Atualizar(dados);

                    Mensagem.Sucesso(
                        "Inscrição atualizada com sucesso!");
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível guardar a inscrição.\n\n" +
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