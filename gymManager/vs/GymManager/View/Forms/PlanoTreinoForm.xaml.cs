using GymManager.Helpers;
using GymManager.Models;
using GymManager.Models.GymManager.Models;
using GymManager.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace GymManager.View.Forms
{
    public partial class PlanoTreinoForm : Window
    {
        private readonly PlanoTreinoService planoService = new PlanoTreinoService();

        private readonly ClienteService clienteService = new ClienteService();

        private readonly PersonalTrainerService ptService = new PersonalTrainerService();

        private readonly PlanoTreino? plano;

        private List<Cliente> clientes = new List<Cliente>();

        private List<PersonalTrainer> personalTrainers = new List<PersonalTrainer>();

        private bool atualizarPesquisaCliente;


        public PlanoTreinoForm()
        {
            InitializeComponent();

            CarregarClientes();
            CarregarPersonalTrainers();

            dpDataInicio.SelectedDate = DateTime.Today;

            dpDataFim.SelectedDate = DateTime.Today.AddMonths(1);

            SelecionarEstado("Ativo");

            cmbEstado.IsEnabled = false;
        }

        public PlanoTreinoForm(PlanoTreino plano)
        {
            InitializeComponent();

            this.plano = plano;

            Title = "Editar Plano de Treino";

            txtTitulo.Text = "Editar Plano de Treino";

            CarregarClientes();
            CarregarPersonalTrainers();

            txtNomePlano.Text = plano.NomePlano;

            cmbCliente.SelectedValue = plano.IdCliente;

            cmbPT.SelectedValue = plano.IdPT;

            dpDataInicio.SelectedDate = plano.DataInicio;

            dpDataFim.SelectedDate = plano.DataFim;

            txtObjetivo.Text = plano.Objetivo;

            txtObservacoes.Text = plano.Observacoes;

            SelecionarEstado(plano.Estado);

            cmbEstado.IsEnabled = true;
        }

        private void CarregarClientes()
        {
            try
            {
                clientes = clienteService.Listar();

                cmbCliente.DisplayMemberPath = "DescricaoReserva";

                cmbCliente.ItemsSource = clientes;
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível carregar os clientes.\n\n" + ex.Message);
            }
        }

        private void CarregarPersonalTrainers()
        {
            try
            {
                personalTrainers = ptService.Listar();

                cmbPT.ItemsSource = personalTrainers;
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível carregar os personal trainers.\n\n" + ex.Message);
            }
        }

        private static string ObterTextoCombo(ComboBox comboBox)
        {
            if (comboBox.SelectedItem is ComboBoxItem item)
            {
                return item.Content?.ToString() ?? string.Empty;
            }

            return string.Empty;
        }

        private void SelecionarEstado(string estado)
        {
            foreach (object item in cmbEstado.Items)
            {
                if (item is ComboBoxItem comboItem && string.Equals(comboItem.Content?.ToString(), estado, StringComparison.OrdinalIgnoreCase))
                {
                    cmbEstado.SelectedItem = comboItem;

                    return;
                }
            }

            cmbEstado.SelectedIndex = 0;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string nomePlano = txtNomePlano.Text.Trim();

            if (string.IsNullOrWhiteSpace(nomePlano))
            {
                Mensagem.Aviso("Introduza o nome do plano.");

                txtNomePlano.Focus();
                return;
            }

            Cliente? clienteSelecionado = cmbCliente.SelectedItem as Cliente;

            if (clienteSelecionado == null)
            {
                string textoCliente = cmbCliente.Text.Trim();

                clienteSelecionado = clientes.FirstOrDefault(c =>
                        string.Equals(c.DescricaoReserva, textoCliente, StringComparison.OrdinalIgnoreCase) || string.Equals(c.Nome, textoCliente, StringComparison.OrdinalIgnoreCase) || string.Equals(c.NIF, textoCliente, StringComparison.OrdinalIgnoreCase));
            }

            if (clienteSelecionado == null)
            {
                Mensagem.Aviso("Selecione um cliente da lista.");

                cmbCliente.Focus();
                return;
            }

            int idCliente = clienteSelecionado.IdCliente;

            cmbCliente.SelectedItem = clienteSelecionado;

            if (cmbPT.SelectedValue is not int idPT)
            {
                Mensagem.Aviso("Selecione um personal trainer.");

                cmbPT.Focus();
                return;
            }

            if (!dpDataInicio.SelectedDate.HasValue)
            {
                Mensagem.Aviso("Selecione a data de início.");

                dpDataInicio.Focus();
                return;
            }

            if (!dpDataFim.SelectedDate.HasValue)
            {
                Mensagem.Aviso("Selecione a data de fim.");

                dpDataFim.Focus();
                return;
            }

            DateTime dataInicio = dpDataInicio.SelectedDate.Value.Date;

            DateTime dataFim = dpDataFim.SelectedDate.Value.Date;

            if (dataFim < dataInicio)
            {
                Mensagem.Aviso("A data final não pode ser anterior à data inicial.");

                dpDataFim.Focus();
                return;
            }

            string objetivo = txtObjetivo.Text.Trim();

            if (string.IsNullOrWhiteSpace(objetivo))
            {
                Mensagem.Aviso("Introduza o objetivo do plano.");

                txtObjetivo.Focus();
                return;
            }

            string estado = ObterTextoCombo(cmbEstado);

            if (string.IsNullOrWhiteSpace(estado))
            {
                Mensagem.Aviso("Selecione o estado.");

                cmbEstado.Focus();
                return;
            }

            bool novoPlano = plano == null;

            if (novoPlano)
            {
                estado = "Ativo";
            }

            string nomeCliente = clienteSelecionado.Nome;

            string nomePT = cmbPT.SelectedItem is PersonalTrainer pt ? pt.Nome : string.Empty;

            if (!Mensagem.Confirmar(
                    $"Pretende {(novoPlano ? "criar" : "atualizar")} este plano?\n\n" +
                    $"Plano: {nomePlano}\n" +
                    $"Cliente: {nomeCliente}\n" +
                    $"Personal trainer: {nomePT}\n" +
                    $"Período: {dataInicio:dd/MM/yyyy} → {dataFim:dd/MM/yyyy}\n" +
                    $"Estado: {estado}"))
            {
                return;
            }

            PlanoTreino dados = new PlanoTreino
            {
                IdPlanoTreino = plano?.IdPlanoTreino ?? 0,

                IdCliente = idCliente,

                IdPT = idPT,

                NomePlano = nomePlano,

                Objetivo = objetivo,

                DataInicio = dataInicio,

                DataFim = dataFim,

                Observacoes = txtObservacoes.Text.Trim(),

                Estado = estado
            };

            try
            {
                if (novoPlano)
                {
                    planoService.Inserir(dados);

                    Mensagem.Sucesso("Plano de treino criado com sucesso!");
                }
                else
                {
                    planoService.Atualizar(dados);

                    Mensagem.Sucesso("Plano de treino atualizado com sucesso!");
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível guardar o plano de treino.\n\n" + ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void cmbCliente_Loaded(object sender, RoutedEventArgs e)
        {
            if (cmbCliente.Template.FindName("PART_EditableTextBox", cmbCliente) is not TextBox tb)
            {
                return;
            }

            tb.TextChanged -= ClientePesquisa_TextChanged;

            tb.TextChanged += ClientePesquisa_TextChanged;
        }
        private void ClientePesquisa_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (atualizarPesquisaCliente)
            {
                return;
            }

            if (sender is not TextBox tb)
            {
                return;
            }

            string texto = tb.Text.Trim();

            if (cmbCliente.SelectedItem is Cliente selecionado && string.Equals(texto, selecionado.DescricaoReserva, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            List<Cliente> resultado;

            if (string.IsNullOrWhiteSpace(texto))
            {
                resultado = clientes.OrderBy(c => c.Nome).ToList();
            }
            else
            {
                resultado = clientes.Where(c =>
                            c.Nome.Contains(texto, StringComparison.OrdinalIgnoreCase)
                            || c.NIF.Contains(texto, StringComparison.OrdinalIgnoreCase) || c.DescricaoReserva.Contains(texto, StringComparison.OrdinalIgnoreCase)).OrderBy(c =>
                            c.Nome).ToList();
            }

            atualizarPesquisaCliente = true;

            try
            {
                cmbCliente.ItemsSource = resultado;

                cmbCliente.IsDropDownOpen = true;

                tb.Text = texto;

                tb.CaretIndex = tb.Text.Length;
            }
            finally
            {
                atualizarPesquisaCliente = false;
            }
        }
    }
}