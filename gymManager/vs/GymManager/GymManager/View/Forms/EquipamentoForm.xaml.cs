using GymManager.Helpers;
using GymManager.Models;
using GymManager.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GymManager.View.Forms
{
    public partial class EquipamentoForm : Window
    {
        private readonly EquipamentoService service =
            new EquipamentoService();

        private readonly Equipamento? equipamento;

        public EquipamentoForm()
        {
            InitializeComponent();

            dpDataAquisicao.SelectedDate =
                DateTime.Today;

            SelecionarCombo(
                cmbEstado,
                "Operacional");

            AtualizarAvisoEstado();
        }

        public EquipamentoForm(
            Equipamento equipamento)
        {
            InitializeComponent();

            this.equipamento =
                equipamento;

            Title =
                "Editar Equipamento";

            txtTitulo.Text =
                "Editar Equipamento";

            txtSubtitulo.Text =
                "Atualizar os dados do equipamento";

            txtNome.Text =
                equipamento.Nome;

            txtMarca.Text =
                equipamento.Marca;

            txtModelo.Text =
                equipamento.Modelo;

            txtNumeroSerie.Text =
                equipamento.NumeroSerie;

            dpDataAquisicao.SelectedDate =
                equipamento.DataAquisicao;

            txtLocalizacao.Text =
                equipamento.Localizacao;

            txtObservacoes.Text =
                equipamento.Observacoes;

            SelecionarCombo(
                cmbCategoria,
                equipamento.Categoria);

            SelecionarCombo(
                cmbEstado,
                equipamento.Estado);

            AtualizarAvisoEstado();
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

        private void cmbEstado_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!IsInitialized)
            {
                return;
            }

            AtualizarAvisoEstado();
        }

        private void AtualizarAvisoEstado()
        {
            if (borderAvisoManutencao == null)
            {
                return;
            }

            string estadoSelecionado =
                ObterTextoCombo(
                    cmbEstado);

            bool passouParaManutencao =
                equipamento != null
                &&
                string.Equals(
                    estadoSelecionado,
                    "Em manutenção",
                    StringComparison.OrdinalIgnoreCase)
                &&
                !string.Equals(
                    equipamento.Estado,
                    "Em manutenção",
                    StringComparison.OrdinalIgnoreCase);

            borderAvisoManutencao.Visibility =
                passouParaManutencao
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void btnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            /*
             * NOME
             */
            string nome =
                txtNome.Text.Trim();

            if (string.IsNullOrWhiteSpace(
                    nome))
            {
                Mensagem.Aviso(
                    "Introduza o nome do equipamento.");

                txtNome.Focus();
                return;
            }

            /*
             * CATEGORIA
             */
            string categoria =
                ObterTextoCombo(
                    cmbCategoria);

            if (string.IsNullOrWhiteSpace(
                    categoria))
            {
                Mensagem.Aviso(
                    "Selecione a categoria.");

                cmbCategoria.Focus();
                return;
            }

            /*
             * MARCA
             */
            string marca =
                txtMarca.Text.Trim();

            if (string.IsNullOrWhiteSpace(
                    marca))
            {
                Mensagem.Aviso(
                    "Introduza a marca do equipamento.");

                txtMarca.Focus();
                return;
            }

            /*
             * DATA DE AQUISIÇÃO
             */
            if (!dpDataAquisicao
                    .SelectedDate
                    .HasValue)
            {
                Mensagem.Aviso(
                    "Selecione a data de aquisição.");

                dpDataAquisicao.Focus();
                return;
            }

            DateTime dataAquisicao =
                dpDataAquisicao
                    .SelectedDate
                    .Value
                    .Date;

            if (dataAquisicao >
                DateTime.Today)
            {
                Mensagem.Aviso(
                    "A data de aquisição não pode ser futura.");

                dpDataAquisicao.Focus();
                return;
            }

            /*
             * LOCALIZAÇÃO
             */
            string localizacao =
                txtLocalizacao.Text.Trim();

            if (string.IsNullOrWhiteSpace(
                    localizacao))
            {
                Mensagem.Aviso(
                    "Introduza a localização do equipamento.");

                txtLocalizacao.Focus();
                return;
            }

            /*
             * ESTADO
             */
            string estado =
                ObterTextoCombo(
                    cmbEstado);

            if (string.IsNullOrWhiteSpace(
                    estado))
            {
                Mensagem.Aviso(
                    "Selecione o estado do equipamento.");

                cmbEstado.Focus();
                return;
            }

            bool novoEquipamento =
                equipamento == null;

            /*
             * O trigger foi criado para UPDATE.
             * Por isso, um equipamento novo deve ser
             * registado primeiro como Operacional.
             */
            if (novoEquipamento &&
                string.Equals(
                    estado,
                    "Em manutenção",
                    StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso(
                    "Um equipamento novo não pode ser registado diretamente " +
                    "como 'Em manutenção'.\n\n" +
                    "Registe-o primeiro como operacional e depois edite o estado.");

                cmbEstado.Focus();
                return;
            }

            if (novoEquipamento &&
                string.Equals(
                    estado,
                    "Abatido",
                    StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso(
                    "Um equipamento novo não pode ser registado como abatido.");

                cmbEstado.Focus();
                return;
            }

            /*
             * Detetar a alteração que irá ativar o trigger.
             */
            bool passouParaManutencao =
                !novoEquipamento
                &&
                string.Equals(
                    estado,
                    "Em manutenção",
                    StringComparison.OrdinalIgnoreCase)
                &&
                !string.Equals(
                    equipamento?.Estado,
                    "Em manutenção",
                    StringComparison.OrdinalIgnoreCase);

            string mensagemManutencao =
                passouParaManutencao
                    ? "\n\nAo guardar, será criada automaticamente uma " +
                      "manutenção agendada sem responsável atribuído."
                    : string.Empty;

            if (!Mensagem.Confirmar(
                    $"Pretende {(novoEquipamento ? "registar" : "atualizar")} " +
                    $"este equipamento?\n\n" +
                    $"Nome: {nome}\n" +
                    $"Categoria: {categoria}\n" +
                    $"Marca: {marca}\n" +
                    $"Modelo: {txtModelo.Text.Trim()}\n" +
                    $"Aquisição: {dataAquisicao:dd/MM/yyyy}\n" +
                    $"Localização: {localizacao}\n" +
                    $"Estado: {estado}" +
                    mensagemManutencao))
            {
                return;
            }

            Equipamento dados =
                new Equipamento
                {
                    IdEquipamento =
                        equipamento?.IdEquipamento
                        ?? 0,

                    Nome =
                        nome,

                    Categoria =
                        categoria,

                    Marca =
                        marca,

                    Modelo =
                        txtModelo.Text.Trim(),

                    NumeroSerie =
                        txtNumeroSerie.Text.Trim(),

                    DataAquisicao =
                        dataAquisicao,

                    Localizacao =
                        localizacao,

                    Estado =
                        estado,

                    Observacoes =
                        txtObservacoes.Text.Trim()
                };

            try
            {
                if (novoEquipamento)
                {
                    service.Inserir(
                        dados);

                    Mensagem.Sucesso(
                        "Equipamento registado com sucesso!");
                }
                else
                {
                    /*
                     * O trigger será executado automaticamente
                     * dentro deste UPDATE quando o estado mudar
                     * para Em manutenção.
                     */
                    service.Atualizar(
                        dados);

                    if (passouParaManutencao)
                    {
                        Mensagem.Aviso(
                            "O equipamento foi colocado em manutenção.\n\n" +
                            "Foi criada automaticamente uma manutenção agendada.\n\n" +
                            "Aceda ao módulo Manutenções, edite o registo " +
                            "criado e atribua o responsável.");
                    }
                    else
                    {
                        Mensagem.Sucesso(
                            "Equipamento atualizado com sucesso!");
                    }
                }

                DialogResult =
                    true;
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível guardar o equipamento.\n\n" +
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