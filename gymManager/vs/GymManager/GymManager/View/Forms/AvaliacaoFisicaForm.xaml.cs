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
using System.Windows.Media;

namespace GymManager.View.Forms
{
    public partial class AvaliacaoFisicaForm : Window
    {
        private readonly AvaliacaoFisicaService avaliacaoService =
            new AvaliacaoFisicaService();

        private readonly ClienteService clienteService =
            new ClienteService();

        private readonly PersonalTrainerService ptService =
            new PersonalTrainerService();

        private readonly AvaliacaoFisica? avaliacao;

        private readonly CultureInfo culturaPortugal =
            new CultureInfo("pt-PT");

        private List<Cliente> clientes =
            new List<Cliente>();

        private List<PersonalTrainer> personalTrainers =
            new List<PersonalTrainer>();

        private Cliente? clienteSelecionado;

        private bool atualizarPesquisaCliente;
        public AvaliacaoFisicaForm()
        {
            InitializeComponent();

            CarregarClientes();
            CarregarPersonalTrainers();

            dpDataAvaliacao.SelectedDate =
                DateTime.Today;

            SelecionarEstado(
                "Agendada");

            AtualizarEstadoCampos();
        }

        public AvaliacaoFisicaForm(
            AvaliacaoFisica avaliacao)
        {
            InitializeComponent();

            this.avaliacao =
                avaliacao;

            Title =
                "Editar Avaliação Física";

            txtTitulo.Text =
                "Editar Avaliação Física";

            CarregarClientes();
            CarregarPersonalTrainers();

            Cliente? cliente =
                clientes.FirstOrDefault(c =>
                    c.IdCliente ==
                    avaliacao.IdCliente);

            if (cliente != null)
            {
                SelecionarCliente(
                    cliente);
            }

            cmbPT.SelectedValue =
                avaliacao.IdPT;

            dpDataAvaliacao.SelectedDate =
                avaliacao.DataAvaliacao;

            txtPeso.Text =
                avaliacao.Peso.HasValue
                    ? avaliacao.Peso.Value.ToString(
                        "0.##",
                        culturaPortugal)
                    : string.Empty;

            txtAltura.Text =
                avaliacao.Altura.HasValue
                    ? avaliacao.Altura.Value.ToString(
                        "0.##",
                        culturaPortugal)
                    : string.Empty;

            txtMassaGorda.Text =
                avaliacao.MassaGorda.HasValue
                    ? avaliacao.MassaGorda.Value.ToString(
                        "0.##",
                        culturaPortugal)
                    : string.Empty;

            txtMassaMuscular.Text =
                avaliacao.MassaMuscular.HasValue
                    ? avaliacao.MassaMuscular.Value.ToString(
                        "0.##",
                        culturaPortugal)
                    : string.Empty;

            txtObservacoes.Text =
                avaliacao.Observacoes;

            SelecionarEstado(
                avaliacao.Estado);

            AtualizarEstadoCampos();

            if (string.Equals(
                    avaliacao.Estado,
                    "Concluída",
                    StringComparison.OrdinalIgnoreCase))
            {
                AtualizarIMC();
            }
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

        private void CarregarPersonalTrainers()
        {
            try
            {
                personalTrainers =
                    ptService.Listar();

                cmbPT.ItemsSource =
                    personalTrainers;
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível carregar os personal trainers.\n\n" +
                    ex.Message);
            }
        }


        private void SelecionarCliente(
            Cliente cliente)
        {
            /*
             * Impede o TextChanged de apagar
             * o cliente durante a seleção.
             */
            atualizarPesquisaCliente =
                true;

            clienteSelecionado =
                cliente;

            txtPesquisarCliente.Text =
                cliente.Nome;

            txtPesquisarCliente.CaretIndex =
                txtPesquisarCliente.Text.Length;

            popupClientes.IsOpen =
                false;

            lstClientes.SelectedIndex =
                -1;

            atualizarPesquisaCliente =
                false;

            txtPesquisarCliente.Focus();

            txtPesquisarCliente.CaretIndex =
                txtPesquisarCliente.Text.Length;
        }

        private void txtPesquisarCliente_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            if (atualizarPesquisaCliente)
            {
                return;
            }

            clienteSelecionado =
                null;

            List<Cliente> resultados =
                FiltrarClientes(
                    txtPesquisarCliente.Text.Trim());

            lstClientes.ItemsSource =
                resultados;

            lstClientes.SelectedIndex =
                -1;

            popupClientes.IsOpen =
                txtPesquisarCliente.IsKeyboardFocusWithin
                &&
                resultados.Count > 0;
        }

        private void txtPesquisarCliente_PreviewMouseLeftButtonDown(
            object sender,
            MouseButtonEventArgs e)
        {
            /*
             * Abre diretamente no primeiro clique.
             * Como o Popup usa StaysOpen=True, não fecha
             * no final do mesmo clique.
             */
            AbrirPopupClientes();
        }

        private void AbrirPopupClientes()
        {
            List<Cliente> resultados =
                FiltrarClientes(
                    txtPesquisarCliente.Text.Trim());

            lstClientes.ItemsSource =
                resultados;

            if (resultados.Count == 0)
            {
                lstClientes.SelectedIndex =
                    -1;

                popupClientes.IsOpen =
                    false;

                return;
            }

            /*
             * Quando se abre pelo rato, não pré-seleciona.
             * A primeira seta para baixo seleciona o primeiro.
             */
            if (!popupClientes.IsOpen)
            {
                lstClientes.SelectedIndex =
                    -1;
            }

            popupClientes.IsOpen =
                true;
        }

        private List<Cliente> FiltrarClientes(
            string pesquisa)
        {
            if (string.IsNullOrWhiteSpace(
                    pesquisa))
            {
                return clientes.ToList();
            }

            return clientes
                .Where(c =>
                    ContemTexto(
                        c.Nome,
                        pesquisa)
                    ||
                    ContemTexto(
                        c.NIF,
                        pesquisa))
                .ToList();
        }

        private static bool ContemTexto(
            string? texto,
            string pesquisa)
        {
            return !string.IsNullOrWhiteSpace(
                       texto)
                   &&
                   texto.Contains(
                       pesquisa,
                       StringComparison.OrdinalIgnoreCase);
        }

        private void lstClientes_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            /*
             * Apenas destaca o resultado.
             * A confirmação é feita com clique ou Enter.
             */
        }

        private void txtPesquisarCliente_PreviewKeyDown(
            object sender,
            KeyEventArgs e)
        {
            List<Cliente> resultados =
                lstClientes.Items
                    .OfType<Cliente>()
                    .ToList();

            if (e.Key == Key.Down)
            {
                if (resultados.Count == 0)
                {
                    resultados =
                        FiltrarClientes(
                            txtPesquisarCliente.Text.Trim());

                    lstClientes.ItemsSource =
                        resultados;
                }

                if (resultados.Count == 0)
                {
                    return;
                }

                popupClientes.IsOpen =
                    true;

                if (lstClientes.SelectedIndex < 0)
                {
                    lstClientes.SelectedIndex =
                        0;
                }
                else if (lstClientes.SelectedIndex <
                         resultados.Count - 1)
                {
                    lstClientes.SelectedIndex++;
                }

                lstClientes.ScrollIntoView(
                    lstClientes.SelectedItem);

                txtPesquisarCliente.Focus();
                txtPesquisarCliente.CaretIndex =
                    txtPesquisarCliente.Text.Length;

                e.Handled =
                    true;

                return;
            }

            if (e.Key == Key.Up)
            {
                if (!popupClientes.IsOpen ||
                    resultados.Count == 0)
                {
                    return;
                }

                if (lstClientes.SelectedIndex < 0)
                {
                    lstClientes.SelectedIndex =
                        resultados.Count - 1;
                }
                else if (lstClientes.SelectedIndex > 0)
                {
                    lstClientes.SelectedIndex--;
                }

                lstClientes.ScrollIntoView(
                    lstClientes.SelectedItem);

                txtPesquisarCliente.Focus();
                txtPesquisarCliente.CaretIndex =
                    txtPesquisarCliente.Text.Length;

                e.Handled =
                    true;

                return;
            }

            if (e.Key == Key.Enter)
            {
                Cliente? cliente =
                    lstClientes.SelectedItem as Cliente
                    ??
                    resultados.FirstOrDefault();

                if (cliente != null)
                {
                    SelecionarCliente(
                        cliente);

                    e.Handled =
                        true;
                }

                return;
            }

            if (e.Key == Key.Escape)
            {
                FecharPopupClientes();

                e.Handled =
                    true;
            }
        }

        private void lstClientes_PreviewKeyDown(
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
                FecharPopupClientes();

                txtPesquisarCliente.Focus();

                txtPesquisarCliente.CaretIndex =
                    txtPesquisarCliente.Text.Length;

                e.Handled =
                    true;

                return;
            }

            if (e.Key == Key.Up &&
                lstClientes.SelectedIndex == 0)
            {
                txtPesquisarCliente.Focus();

                txtPesquisarCliente.CaretIndex =
                    txtPesquisarCliente.Text.Length;

                e.Handled =
                    true;
            }
        }

        private void lstClientes_PreviewMouseLeftButtonDown(
            object sender,
            MouseButtonEventArgs e)
        {
            if (e.OriginalSource is not DependencyObject origem)
            {
                return;
            }

            ListBoxItem? item =
                ObterAscendente<ListBoxItem>(
                    origem);

            if (item?.DataContext is not Cliente cliente)
            {
                return;
            }

            SelecionarCliente(
                cliente);

            e.Handled =
                true;
        }

        private void lstClientes_PreviewMouseWheel(
            object sender,
            MouseWheelEventArgs e)
        {
            /*
             * Impede a roda do rato de percorrer a lista.
             * A seleção é feita com pesquisa, setas ou clique.
             */
            e.Handled =
                true;
        }

        private void FecharPopupClientes()
        {
            popupClientes.IsOpen =
                false;

            lstClientes.SelectedIndex =
                -1;
        }

        private static T? ObterAscendente<T>(
            DependencyObject? origem)
            where T : DependencyObject
        {
            DependencyObject? atual =
                origem;

            while (atual != null)
            {
                if (atual is T resultado)
                {
                    return resultado;
                }

                atual =
                    VisualTreeHelper.GetParent(
                        atual);
            }

            return null;
        }

        private void AvaliacaoFisicaForm_PreviewMouseDown(
            object sender,
            MouseButtonEventArgs e)
        {
            if (!popupClientes.IsOpen)
            {
                return;
            }

            if (e.OriginalSource is not DependencyObject origem)
            {
                popupClientes.IsOpen =
                    false;

                return;
            }

            /*
             * Não fecha quando o clique foi no campo de pesquisa.
             * Os cliques dentro do próprio Popup pertencem a outra
             * árvore visual e não chegam a este evento da janela.
             */
            if (EhDescendenteDe(
                    origem,
                    txtPesquisarCliente))
            {
                return;
            }

            popupClientes.IsOpen =
                false;

            lstClientes.SelectedIndex =
                -1;
        }

        private void AvaliacaoFisicaForm_Deactivated(
            object? sender,
            EventArgs e)
        {
            popupClientes.IsOpen =
                false;

            lstClientes.SelectedIndex =
                -1;
        }

        private static bool EhDescendenteDe(
            DependencyObject origem,
            DependencyObject controlo)
        {
            DependencyObject? atual =
                origem;

            while (atual != null)
            {
                if (ReferenceEquals(
                        atual,
                        controlo))
                {
                    return true;
                }

                atual =
                    VisualTreeHelper.GetParent(
                        atual);
            }

            return false;
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

        private void SelecionarEstado(
            string estado)
        {
            foreach (object item in
                     cmbEstado.Items)
            {
                if (item is ComboBoxItem comboItem &&
                    string.Equals(
                        comboItem.Content?.ToString(),
                        estado,
                        StringComparison.OrdinalIgnoreCase))
                {
                    cmbEstado.SelectedItem =
                        comboItem;

                    return;
                }
            }

            cmbEstado.SelectedIndex =
                0;
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

        private void Medidas_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            if (!IsInitialized)
            {
                return;
            }

            string estado =
                ObterTextoCombo(
                    cmbEstado);

            if (!string.Equals(
                    estado,
                    "Concluída",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            AtualizarIMC();
        }

        private void AtualizarIMC()
        {
            if (txtIMC == null ||
                txtClassificacaoIMC == null)
            {
                return;
            }

            if (!TentarLerDecimal(
                    txtPeso?.Text,
                    out decimal peso)
                ||
                !TentarLerDecimal(
                    txtAltura?.Text,
                    out decimal altura)
                ||
                peso <= 0
                ||
                altura <= 0)
            {
                txtIMC.Text =
                    "--";

                txtClassificacaoIMC.Text =
                    "Introduza peso e altura";

                return;
            }

            decimal imc =
                Math.Round(
                    peso /
                    (altura * altura),
                    2);

            txtIMC.Text =
                imc.ToString(
                    "N2",
                    culturaPortugal);

            txtClassificacaoIMC.Text =
                ClassificarIMC(
                    imc);
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
                     .Replace(
                         ".",
                         separador)
                     .Replace(
                         ",",
                         separador);

            return decimal.TryParse(
                textoNormalizado,
                NumberStyles.Number,
                culturaPortugal,
                out valor);
        }

        private static string ClassificarIMC(
            decimal imc)
        {
            if (imc < 18.5m)
            {
                return "Abaixo do peso";
            }

            if (imc < 25m)
            {
                return "Peso normal";
            }

            if (imc < 30m)
            {
                return "Excesso de peso";
            }

            return "Obesidade";
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
            if (txtPeso == null ||
                txtAltura == null ||
                txtMassaGorda == null ||
                txtMassaMuscular == null ||
                txtIMC == null ||
                txtClassificacaoIMC == null)
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

            txtPeso.IsEnabled =
                concluida;

            txtAltura.IsEnabled =
                concluida;

            txtMassaGorda.IsEnabled =
                concluida;

            txtMassaMuscular.IsEnabled =
                concluida;

            if (!concluida)
            {
                txtPeso.Clear();
                txtAltura.Clear();
                txtMassaGorda.Clear();
                txtMassaMuscular.Clear();

                txtIMC.Text =
                    "--";

                txtClassificacaoIMC.Text =
                    string.Equals(
                        estado,
                        "Cancelada",
                        StringComparison.OrdinalIgnoreCase)
                        ? "Avaliação cancelada"
                        : "Avaliação ainda não concluída";
            }
            else
            {
                AtualizarIMC();
            }
        }

        private void btnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (clienteSelecionado == null)
            {
                Mensagem.Aviso(
                    "Pesquise e selecione um cliente.");

                txtPesquisarCliente.Focus();
                return;
            }

            int idCliente =
                clienteSelecionado.IdCliente;

            if (cmbPT.SelectedValue is not
                int idPT)
            {
                Mensagem.Aviso(
                    "Selecione um personal trainer.");

                cmbPT.Focus();
                return;
            }

            if (!dpDataAvaliacao
                    .SelectedDate
                    .HasValue)
            {
                Mensagem.Aviso(
                    "Selecione a data da avaliação.");

                dpDataAvaliacao.Focus();
                return;
            }

            DateTime dataAvaliacao =
                dpDataAvaliacao
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
                    "Selecione o estado da avaliação.");

                cmbEstado.Focus();
                return;
            }

            bool concluida =
                string.Equals(
                    estado,
                    "Concluída",
                    StringComparison.OrdinalIgnoreCase);

            decimal? peso =
                null;

            decimal? altura =
                null;

            decimal? imc =
                null;

            decimal? massaGorda =
                null;

            decimal? massaMuscular =
                null;

            if (concluida)
            {
                if (!TentarLerDecimal(
                        txtPeso.Text,
                        out decimal pesoLido)
                    ||
                    pesoLido <= 0)
                {
                    Mensagem.Aviso(
                        "Introduza um peso válido.");

                    txtPeso.Focus();
                    return;
                }

                if (pesoLido > 999.99m)
                {
                    Mensagem.Aviso(
                        "O peso indicado é demasiado elevado.");

                    txtPeso.Focus();
                    return;
                }

                if (!TentarLerDecimal(
                        txtAltura.Text,
                        out decimal alturaLida)
                    ||
                    alturaLida <= 0)
                {
                    Mensagem.Aviso(
                        "Introduza uma altura válida em metros.\n\n" +
                        "Exemplo: 1,75.");

                    txtAltura.Focus();
                    return;
                }

                if (alturaLida > 9.99m)
                {
                    Mensagem.Aviso(
                        "A altura indicada não é válida.");

                    txtAltura.Focus();
                    return;
                }

                if (!TentarLerDecimal(
                        txtMassaGorda.Text,
                        out decimal massaGordaLida)
                    ||
                    massaGordaLida < 0
                    ||
                    massaGordaLida > 100)
                {
                    Mensagem.Aviso(
                        "A massa gorda deve estar entre 0 e 100.");

                    txtMassaGorda.Focus();
                    return;
                }

                if (!TentarLerDecimal(
                        txtMassaMuscular.Text,
                        out decimal massaMuscularLida)
                    ||
                    massaMuscularLida <= 0)
                {
                    Mensagem.Aviso(
                        "Introduza uma massa muscular válida.");

                    txtMassaMuscular.Focus();
                    return;
                }

                if (massaMuscularLida > 999.99m)
                {
                    Mensagem.Aviso(
                        "A massa muscular indicada é demasiado elevada.");

                    txtMassaMuscular.Focus();
                    return;
                }

                peso =
                    pesoLido;

                altura =
                    alturaLida;

                massaGorda =
                    massaGordaLida;

                massaMuscular =
                    massaMuscularLida;

                imc =
                    Math.Round(
                        pesoLido /
                        (alturaLida * alturaLida),
                        2);
            }

            bool novaAvaliacao =
                avaliacao == null;

            string nomeCliente =
                clienteSelecionado.Nome;

            string nomePT =
                cmbPT.SelectedItem is
                    PersonalTrainer pt
                    ? pt.Nome
                    : string.Empty;

            string detalhesMedicoes =
                concluida
                    ? $"\nPeso: {peso!.Value:N2} kg" +
                      $"\nAltura: {altura!.Value:N2} m" +
                      $"\nIMC: {imc!.Value:N2}" +
                      $"\nClassificação: {ClassificarIMC(imc.Value)}" +
                      $"\nMassa gorda: {massaGorda!.Value:N2} %" +
                      $"\nMassa muscular: {massaMuscular!.Value:N2} kg"
                    : "\nMedições: ainda não registadas";

            if (!Mensagem.Confirmar(
                    $"Pretende {(novaAvaliacao ? "registar" : "atualizar")} " +
                    $"esta avaliação?\n\n" +
                    $"Cliente: {nomeCliente}\n" +
                    $"Personal trainer: {nomePT}\n" +
                    $"Data: {dataAvaliacao:dd/MM/yyyy}\n" +
                    $"Estado: {estado}" +
                    detalhesMedicoes))
            {
                return;
            }

            AvaliacaoFisica dados =
                new AvaliacaoFisica
                {
                    IdAvaliacao =
                        avaliacao?.IdAvaliacao
                        ?? 0,

                    IdCliente =
                        idCliente,

                    IdPT =
                        idPT,

                    DataAvaliacao =
                        dataAvaliacao,

                    Peso =
                        peso,

                    Altura =
                        altura,

                    IMC =
                        imc,

                    MassaGorda =
                        massaGorda,

                    MassaMuscular =
                        massaMuscular,

                    Observacoes =
                        txtObservacoes.Text.Trim(),

                    Estado =
                        estado
                };

            try
            {
                if (novaAvaliacao)
                {
                    avaliacaoService.Inserir(
                        dados);

                    Mensagem.Sucesso(
                        estado == "Agendada"
                            ? "Avaliação física agendada com sucesso!"
                            : estado == "Cancelada"
                                ? "Avaliação física registada como cancelada."
                                : "Avaliação física registada com sucesso!");
                }
                else
                {
                    avaliacaoService.Atualizar(
                        dados);

                    Mensagem.Sucesso(
                        "Avaliação física atualizada com sucesso!");
                }

                DialogResult =
                    true;
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível guardar a avaliação física.\n\n" +
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