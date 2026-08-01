using GymManager.Helpers;
using GymManager.Models;
using GymManager.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GymManager.View.Forms
{
    public partial class ManutencaoForm : Window
    {
        private readonly ManutencaoService manutencaoService =
            new ManutencaoService();

        private readonly EquipamentoService equipamentoService =
            new EquipamentoService();

        private readonly Manutencao? manutencao;

        private readonly CultureInfo culturaPortugal =
            new CultureInfo("pt-PT");

        private List<Equipamento> equipamentos =
            new List<Equipamento>();

        public ManutencaoForm()
        {
            InitializeComponent();

            CarregarEquipamentos();

            dpDataAgendada.SelectedDate =
                DateTime.Today;

            SelecionarCombo(
                cmbTipo,
                "Preventiva");

            SelecionarCombo(
                cmbEstado,
                "Agendada");

            AtualizarEstadoCampos();
        }

        public ManutencaoForm(
            Manutencao manutencao)
        {
            InitializeComponent();

            this.manutencao =
                manutencao;

            Title =
                "Editar Manutenção";

            txtTitulo.Text =
                "Editar Manutenção";

            CarregarEquipamentos();

            cmbEquipamento.SelectedValue =
                manutencao.IdEquipamento;

            SelecionarCombo(
                cmbTipo,
                manutencao.Tipo);

            SelecionarCombo(
                cmbEstado,
                manutencao.Estado);

            dpDataAgendada.SelectedDate =
                manutencao.DataAgendada;

            dpDataRealizacao.SelectedDate =
                manutencao.DataRealizacao;

            txtResponsavel.Text =
                manutencao.Responsavel;

            txtCusto.Text =
                manutencao.Custo.HasValue
                    ? manutencao.Custo.Value.ToString(
                        "0.##",
                        culturaPortugal)
                    : string.Empty;

            txtDescricao.Text =
                manutencao.Descricao;

            txtObservacoes.Text =
                manutencao.Observacoes;

            AtualizarEstadoCampos();
        }

        private void CarregarEquipamentos()
        {
            try
            {
                equipamentos =
                    equipamentoService.Listar();

                cmbEquipamento.ItemsSource =
                    equipamentos;
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível carregar os equipamentos.\n\n" +
                    ex.Message);
            }
        }

        private static void SelecionarCombo(
            ComboBox comboBox,
            string valor)
        {
            foreach (object item in comboBox.Items)
            {
                if (item is ComboBoxItem comboItem &&
                    string.Equals(
                        comboItem.Content?.ToString(),
                        valor,
                        StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedItem =
                        comboItem;

                    return;
                }
            }

            comboBox.SelectedIndex =
                -1;
        }

        private static string ObterTextoCombo(
            ComboBox comboBox)
        {
            if (comboBox.SelectedItem is
                ComboBoxItem item)
            {
                return item.Content?.ToString()
                       ?? string.Empty;
            }

            return string.Empty;
        }

        private void txtDecimal_PreviewTextInput(
            object sender,
            TextCompositionEventArgs e)
        {
            e.Handled =
                !e.Text.All(c =>
                    char.IsDigit(c) ||
                    c == ',' ||
                    c == '.');
        }

        private bool TentarLerDecimal(
            string? texto,
            out decimal valor)
        {
            valor =
                0;

            if (string.IsNullOrWhiteSpace(
                    texto))
            {
                return false;
            }

            string separador =
                culturaPortugal
                    .NumberFormat
                    .NumberDecimalSeparator;

            string textoNormalizado =
                texto.Trim()
                     .Replace(".", separador)
                     .Replace(",", separador);

            return decimal.TryParse(
                textoNormalizado,
                NumberStyles.Number,
                culturaPortugal,
                out valor);
        }

        private void cmbEstado_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!IsInitialized)
            {
                return;
            }

            AtualizarEstadoCampos();
        }

        private void AtualizarEstadoCampos()
        {
            if (dpDataRealizacao == null ||
                txtResponsavel == null ||
                txtCusto == null ||
                txtNotaEstado == null)
            {
                return;
            }

            string estado =
                ObterTextoCombo(
                    cmbEstado);

            bool concluida =
                string.Equals(
                    estado,
                    "Concluída",
                    StringComparison.OrdinalIgnoreCase);

            bool cancelada =
                string.Equals(
                    estado,
                    "Cancelada",
                    StringComparison.OrdinalIgnoreCase);

            dpDataRealizacao.IsEnabled =
                concluida;

            txtResponsavel.IsEnabled =
                !cancelada;

            txtCusto.IsEnabled =
                concluida;

            if (!concluida)
            {
                dpDataRealizacao.SelectedDate =
                    null;

                txtCusto.Clear();
            }

            if (cancelada)
            {
                txtResponsavel.Clear();

                txtNotaEstado.Text =
                    "A manutenção cancelada devolve o equipamento ao estado operacional, caso não existam outras manutenções ativas.";
            }
            else if (concluida)
            {
                txtNotaEstado.Text =
                    "Ao concluir a manutenção, o equipamento volta ao estado operacional.";
            }
            else
            {
                txtNotaEstado.Text =
                    "Uma manutenção agendada ou em curso coloca o equipamento em manutenção.";
            }
        }

        private void btnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (cmbEquipamento.SelectedValue is not
                int idEquipamento)
            {
                Mensagem.Aviso(
                    "Selecione um equipamento.");

                cmbEquipamento.Focus();
                return;
            }

            string tipo =
                ObterTextoCombo(
                    cmbTipo);

            if (string.IsNullOrWhiteSpace(
                    tipo))
            {
                Mensagem.Aviso(
                    "Selecione o tipo de manutenção.");

                cmbTipo.Focus();
                return;
            }

            if (!dpDataAgendada
                    .SelectedDate
                    .HasValue)
            {
                Mensagem.Aviso(
                    "Selecione a data agendada.");

                dpDataAgendada.Focus();
                return;
            }

            DateTime dataAgendada =
                dpDataAgendada
                    .SelectedDate
                    .Value
                    .Date;

            string estado =
                ObterTextoCombo(
                    cmbEstado);

            if (string.IsNullOrWhiteSpace(
                    estado))
            {
                Mensagem.Aviso(
                    "Selecione o estado da manutenção.");

                cmbEstado.Focus();
                return;
            }

            string descricao =
                txtDescricao.Text.Trim();

            if (string.IsNullOrWhiteSpace(
                    descricao))
            {
                Mensagem.Aviso(
                    "Introduza a descrição da manutenção.");

                txtDescricao.Focus();
                return;
            }

            bool concluida =
                string.Equals(
                    estado,
                    "Concluída",
                    StringComparison.OrdinalIgnoreCase);

            DateTime? dataRealizacao =
                null;

            decimal? custo =
                null;

            if (concluida)
            {
                if (!dpDataRealizacao
                        .SelectedDate
                        .HasValue)
                {
                    Mensagem.Aviso(
                        "Selecione a data de realização.");

                    dpDataRealizacao.Focus();
                    return;
                }

                dataRealizacao =
                    dpDataRealizacao
                        .SelectedDate
                        .Value
                        .Date;

                if (dataRealizacao.Value <
                    dataAgendada)
                {
                    Mensagem.Aviso(
                        "A data de realização não pode ser anterior à data agendada.");

                    dpDataRealizacao.Focus();
                    return;
                }

                if (!string.IsNullOrWhiteSpace(
                        txtCusto.Text))
                {
                    if (!TentarLerDecimal(
                            txtCusto.Text,
                            out decimal custoLido)
                        ||
                        custoLido < 0)
                    {
                        Mensagem.Aviso(
                            "Introduza um custo válido.");

                        txtCusto.Focus();
                        return;
                    }

                    custo =
                        custoLido;
                }
            }

            string responsavel =
                txtResponsavel.Text.Trim();

            Equipamento? equipamento =
                cmbEquipamento.SelectedItem as
                    Equipamento;

            bool novaManutencao =
                manutencao == null;

            string detalhesConclusao =
                concluida
                    ? $"\nData de realização: {dataRealizacao:dd/MM/yyyy}" +
                      $"\nCusto: {(custo.HasValue ? $"{custo.Value:N2} €" : "-")}"
                    : string.Empty;

            if (!Mensagem.Confirmar(
                    $"Pretende {(novaManutencao ? "registar" : "atualizar")} esta manutenção?\n\n" +
                    $"Equipamento: {equipamento?.DescricaoCompleta}\n" +
                    $"Tipo: {tipo}\n" +
                    $"Data agendada: {dataAgendada:dd/MM/yyyy}\n" +
                    $"Estado: {estado}\n" +
                    $"Responsável: {(string.IsNullOrWhiteSpace(responsavel) ? "-" : responsavel)}" +
                    detalhesConclusao))
            {
                return;
            }

            Manutencao dados =
                new Manutencao
                {
                    IdManutencao =
                        manutencao?.IdManutencao
                        ?? 0,

                    IdEquipamento =
                        idEquipamento,

                    Tipo =
                        tipo,

                    DataAgendada =
                        dataAgendada,

                    DataRealizacao =
                        dataRealizacao,

                    Descricao =
                        descricao,

                    Responsavel =
                        responsavel,

                    Custo =
                        custo,

                    Estado =
                        estado,

                    Observacoes =
                        txtObservacoes.Text.Trim()
                };

            try
            {
                if (novaManutencao)
                {
                    manutencaoService.Inserir(
                        dados);

                    Mensagem.Sucesso(
                        "Manutenção registada com sucesso!");
                }
                else
                {
                    manutencaoService.Atualizar(
                        dados);

                    Mensagem.Sucesso(
                        "Manutenção atualizada com sucesso!");
                }

                DialogResult =
                    true;
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível guardar a manutenção.\n\n" +
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