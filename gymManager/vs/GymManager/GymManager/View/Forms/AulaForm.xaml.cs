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
    public partial class AulaForm : Window
    {
        private readonly AulaService aulaService =
            new AulaService();

        private readonly ProfessoresService professorService =
            new ProfessoresService();

        private readonly Aula? aula;

        private List<Professor> professores =
            new List<Professor>();

        public AulaForm()
        {
            InitializeComponent();

            CarregarProfessores();

            dpDataAula.SelectedDate =
                DateTime.Today;

            txtHoraInicio.Text =
                "09:00";

            txtDuracao.Text =
                "60";

            txtLotacao.Text =
                "20";

            cmbSala.SelectedIndex =
                0;

            SelecionarEstado(
                "Agendada");

            cmbEstado.IsEnabled =
                false;

            AtualizarResumo();
        }

        public AulaForm(
            Aula aula)
        {
            InitializeComponent();

            this.aula =
                aula;

            Title =
                "Editar Aula";

            txtTitulo.Text =
                "Editar Aula";

            CarregarProfessores();

            txtNome.Text =
                aula.Nome;

            cmbProfessor.SelectedValue =
                aula.IdProfessor;

            dpDataAula.SelectedDate =
                aula.DataAula;

            txtHoraInicio.Text =
                aula.HoraInicio.ToString(
                    @"hh\:mm");

            txtDuracao.Text =
                aula.DuracaoMinutos.ToString();

            txtLotacao.Text =
                aula.Lotacao.ToString();

            SelecionarSala(
                aula.Sala);

            SelecionarEstado(
                aula.Estado);

            cmbEstado.IsEnabled =
                true;

            AtualizarResumo();
        }

        private void CarregarProfessores()
        {
            try
            {
                professores =
                    professorService.Listar();

                cmbProfessor.ItemsSource =
                    professores;

                cmbProfessor.DisplayMemberPath =
                    "Nome";

                cmbProfessor.SelectedValuePath =
                    "IdProfessor";
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível carregar os professores.\n\n" +
                    ex.Message);
            }
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

        private static string ObterTextoCombo(
            ComboBox comboBox)
        {
            if (comboBox.SelectedItem is
                ComboBoxItem item)
            {
                return item.Content?.ToString()
                       ?? string.Empty;
            }

            return comboBox.SelectedItem?.ToString()
                   ?? string.Empty;
        }

        private void txtNumero_PreviewTextInput(
            object sender,
            TextCompositionEventArgs e)
        {
            e.Handled =
                !e.Text.All(
                    char.IsDigit);
        }

        private void txtHoraInicio_PreviewTextInput(
            object sender,
            TextCompositionEventArgs e)
        {
            e.Handled =
                !e.Text.All(c =>
                    char.IsDigit(c) ||
                    c == ':');
        }

        /*
         * Liga este evento no XAML aos campos:
         *
         * txtNome.TextChanged
         * cmbProfessor.SelectionChanged
         * dpDataAula.SelectedDateChanged
         * txtHoraInicio.TextChanged
         * txtDuracao.TextChanged
         * txtSala.TextChanged
         */
        private void CamposAlterados(
            object sender,
            RoutedEventArgs e)
        {
            /*
             * Durante o InitializeComponent, alguns
             * controlos ainda podem não estar prontos.
             */
            if (!IsInitialized)
            {
                return;
            }

            AtualizarResumo();
        }

        private void AtualizarResumo()
        {
            if (txtResumo == null)
            {
                return;
            }

            string nome =
                string.IsNullOrWhiteSpace(
                    txtNome?.Text)
                    ? "Aula"
                    : txtNome.Text.Trim();

            string professor =
        cmbProfessor?.SelectedItem is
            Professor professorSelecionado
            ? professorSelecionado.Nome
            : "";

            string textoProfessor =
                string.IsNullOrWhiteSpace(professor)
                    ? ""
                    : $" com {professor}";

            string data =
                dpDataAula?.SelectedDate.HasValue == true
                    ? dpDataAula.SelectedDate.Value
                        .ToString(
                            "dd/MM/yyyy")
                    : "sem data";

            string hora =
                string.IsNullOrWhiteSpace(
                    txtHoraInicio?.Text)
                    ? "--:--"
                    : txtHoraInicio.Text.Trim();

            string duracao =
                string.IsNullOrWhiteSpace(
                    txtDuracao?.Text)
                    ? "sem duração"
                    : $"{txtDuracao.Text.Trim()} minutos";

            string sala =
     cmbSala?.SelectedItem is
         ComboBoxItem salaItem
         ? salaItem.Content?.ToString()
           ?? "sem sala"
         : "sem sala";

            txtResumo.Text =
    $"{nome}{textoProfessor}, em {data} às {hora}, " +
    $"{duracao}, {sala}.";
        }

        private void btnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            string nome =
                txtNome.Text.Trim();

            if (string.IsNullOrWhiteSpace(
                    nome))
            {
                Mensagem.Aviso(
                    "Introduza o nome da aula.");

                txtNome.Focus();
                return;
            }

            if (cmbProfessor.SelectedValue is not
                int idProfessor)
            {
                Mensagem.Aviso(
                    "Selecione um professor.");

                cmbProfessor.Focus();
                return;
            }

            if (!dpDataAula.SelectedDate.HasValue)
            {
                Mensagem.Aviso(
                    "Selecione a data da aula.");

                dpDataAula.Focus();
                return;
            }

            string textoHora =
                txtHoraInicio.Text.Trim();

            if (!TimeSpan.TryParseExact(
                    textoHora,
                    @"hh\:mm",
                    CultureInfo.InvariantCulture,
                    out TimeSpan horaInicio))
            {
                Mensagem.Aviso(
                    "Introduza uma hora válida no formato HH:mm.\n\n" +
                    "Exemplo: 18:30.");

                txtHoraInicio.Focus();
                return;
            }

            if (horaInicio < TimeSpan.Zero ||
                horaInicio >= TimeSpan.FromDays(1))
            {
                Mensagem.Aviso(
                    "A hora de início não é válida.");

                txtHoraInicio.Focus();
                return;
            }

            if (!int.TryParse(
                    txtDuracao.Text.Trim(),
                    out int duracaoMinutos)
                ||
                duracaoMinutos <= 0)
            {
                Mensagem.Aviso(
                    "Introduza uma duração válida, superior a zero.");

                txtDuracao.Focus();
                return;
            }

            if (duracaoMinutos > 720)
            {
                Mensagem.Aviso(
                    "A duração da aula não pode ser superior a 12 horas.");

                txtDuracao.Focus();
                return;
            }

            TimeSpan horaFim =
                horaInicio.Add(
                    TimeSpan.FromMinutes(
                        duracaoMinutos));

            if (horaFim > TimeSpan.FromDays(1))
            {
                Mensagem.Aviso(
                    "A aula não pode terminar depois da meia-noite.");

                txtDuracao.Focus();
                return;
            }

            if (!int.TryParse(
                    txtLotacao.Text.Trim(),
                    out int lotacao)
                ||
                lotacao <= 0)
            {
                Mensagem.Aviso(
                    "Introduza uma lotação válida, superior a zero.");

                txtLotacao.Focus();
                return;
            }

            if (lotacao > 1000)
            {
                Mensagem.Aviso(
                    "A lotação indicada é demasiado elevada.");

                txtLotacao.Focus();
                return;
            }

            if (cmbSala.SelectedItem is not
        ComboBoxItem salaItem)
            {
                Mensagem.Aviso(
                    "Selecione uma sala.");

                cmbSala.Focus();
                return;
            }

            string sala =
                salaItem.Content?.ToString()
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(
                    sala))
            {
                Mensagem.Aviso(
                    "Selecione uma sala válida.");

                cmbSala.Focus();
                return;
            }

            string estado =
                ObterTextoCombo(
                    cmbEstado);

            if (string.IsNullOrWhiteSpace(
                    estado))
            {
                Mensagem.Aviso(
                    "Selecione um estado válido.");

                cmbEstado.Focus();
                return;
            }

            DateTime dataAula =
                dpDataAula
                    .SelectedDate
                    .Value
                    .Date;

            bool novaAula =
                aula == null;

            /*
             * Uma aula nova começa obrigatoriamente
             * no estado Agendada.
             */
            if (novaAula)
            {
                estado =
                    "Agendada";
            }

            /*
             * Não permite criar uma aula nova
             * numa data anterior à atual.
             */
            if (novaAula &&
                dataAula < DateTime.Today)
            {
                Mensagem.Aviso(
                    "Não é possível criar uma aula numa data passada.");

                dpDataAula.Focus();
                return;
            }

            /*
             * Se a aula for hoje, não permite criar
             * uma aula cujo fim já tenha passado.
             */
            DateTime dataHoraInicio =
      dataAula.Date.Add(horaInicio);

            if (novaAula &&
                dataHoraInicio <= DateTime.Now)
            {
                Mensagem.Aviso(
                    "A data e hora de início da aula devem ser posteriores ao momento atual.");

                txtHoraInicio.Focus();
                return;
            }

            /*
             * Não permite alterar uma aula concluída.
             * A validação deve existir também na AulasPage.
             */
            if (!novaAula &&
                string.Equals(
                    aula!.Estado,
                    "Concluída",
                    StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso(
                    "Uma aula concluída não pode ser alterada.");

                return;
            }

            string operacao =
                novaAula
                    ? "criar"
                    : "atualizar";

            string nomeProfessor =
                cmbProfessor.SelectedItem is
                    Professor professorSelecionado
                    ? professorSelecionado.Nome
                    : string.Empty;

            if (!Mensagem.Confirmar(
                    $"Tem a certeza que pretende {operacao} esta aula?\n\n" +
                    $"Aula: {nome}\n" +
                    $"Professor: {nomeProfessor}\n" +
                    $"Data: {dataAula:dd/MM/yyyy}\n" +
                    $"Hora: {horaInicio:hh\\:mm}\n" +
                    $"Fim: {horaFim:hh\\:mm}\n" +
                    $"Duração: {duracaoMinutos} minutos\n" +
                    $"Sala: {sala}\n" +
                    $"Lotação: {lotacao}\n" +
                    $"Estado: {estado}"))
            {
                return;
            }

            Aula dados =
                new Aula
                {
                    IdAula =
                        aula?.IdAula
                        ?? 0,

                    IdProfessor =
                        idProfessor,

                    Nome =
                        nome,

                    DataAula =
                        dataAula,

                    HoraInicio =
                        horaInicio,

                    DuracaoMinutos =
                        duracaoMinutos,

                    Lotacao =
                        lotacao,

                    Sala =
                        sala,

                    Estado =
                        estado
                };

            try
            {
                if (novaAula)
                {
                    aulaService.Inserir(
                        dados);

                    Mensagem.Sucesso(
                        "Aula criada com sucesso!");
                }
                else
                {
                    aulaService.Atualizar(
                        dados);

                    Mensagem.Sucesso(
                        "Aula atualizada com sucesso!");
                }

                DialogResult =
                    true;
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    novaAula
                        ? "Não foi possível criar a aula.\n\n" +
                          ex.Message
                        : "Não foi possível atualizar a aula.\n\n" +
                          ex.Message);
            }
        }
        private void SelecionarSala(
    string sala)
        {
            foreach (object item in cmbSala.Items)
            {
                if (item is ComboBoxItem comboItem &&
                    string.Equals(
                        comboItem.Content?.ToString(),
                        sala,
                        StringComparison.OrdinalIgnoreCase))
                {
                    cmbSala.SelectedItem =
                        comboItem;

                    return;
                }
            }

            cmbSala.SelectedIndex =
                -1;
        }
        private void btnCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}