using GymManager.Helpers;
using GymManager.Models;
using GymManager.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GymManager.View.Forms
{
    public partial class EventoAgendaForm : Window
    {
        private readonly EventoAgendaService eventoService =
            new EventoAgendaService();

        private readonly PersonalTrainerService ptService =
            new PersonalTrainerService();

        private readonly ClienteService clienteService =
            new ClienteService();

        private readonly EventoAgenda? evento;

        private List<PersonalTrainer> personalTrainers =
            new List<PersonalTrainer>();

        private List<Cliente> clientes =
            new List<Cliente>();

        private readonly CultureInfo culturaPortugal =
            new CultureInfo("pt-PT");

        public EventoAgendaForm(
            DateTime dataInicial)
        {
            InitializeComponent();

            CarregarDadosAuxiliares();

            dpData.SelectedDate =
                dataInicial.Date;

            txtHoraInicio.Text =
                "09:00";

            txtHoraFim.Text =
                "10:00";

            txtTitulo.Text =
                "Sessão de Personal Training";
        }

        public EventoAgendaForm(
            EventoAgenda evento)
        {
            if (evento.EhAula)
            {
                throw new InvalidOperationException(
                    "As aulas não podem ser editadas através da agenda.");
            }

            InitializeComponent();

            this.evento =
                evento;

            Title =
                "Editar agendamento PT";

            txtTituloJanela.Text =
                "Editar agendamento PT";

            CarregarDadosAuxiliares();

            txtTitulo.Text =
                evento.Titulo;

            dpData.SelectedDate =
                evento.DataInicio.Date;

            txtHoraInicio.Text =
                evento.DataInicio.ToString(
                    "HH:mm");

            txtHoraFim.Text =
                evento.DataFim.ToString(
                    "HH:mm");

            cmbPT.SelectedValue =
                evento.IdPT;

            cmbCliente.SelectedValue =
                evento.IdCliente;

            txtLocalizacao.Text =
                evento.Localizacao;

            txtDescricao.Text =
                evento.Descricao;
        }

        private void CarregarDadosAuxiliares()
        {
            try
            {
                personalTrainers =
                    ptService.Listar();

                clientes =
                    clienteService.Listar();

                cmbPT.ItemsSource =
                    personalTrainers;

                cmbCliente.ItemsSource =
                    clientes;
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível carregar os dados do formulário.\n\n" +
                    ex.Message);
            }
        }

        private void txtHora_PreviewTextInput(
            object sender,
            TextCompositionEventArgs e)
        {
            e.Handled =
                !e.Text.All(c =>
                    char.IsDigit(c)
                    ||
                    c == ':');
        }

        private bool TentarLerHora(
            string texto,
            out TimeSpan hora)
        {
            return TimeSpan.TryParseExact(
                texto.Trim(),
                @"hh\:mm",
                culturaPortugal,
                out hora);
        }

        private void btnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            string titulo =
                txtTitulo.Text.Trim();

            if (string.IsNullOrWhiteSpace(
                    titulo))
            {
                Mensagem.Aviso(
                    "Introduza o título do agendamento.");

                txtTitulo.Focus();
                return;
            }

            if (!dpData.SelectedDate.HasValue)
            {
                Mensagem.Aviso(
                    "Selecione a data do agendamento.");

                dpData.Focus();
                return;
            }

            if (!TentarLerHora(
                    txtHoraInicio.Text,
                    out TimeSpan horaInicio))
            {
                Mensagem.Aviso(
                    "Introduza uma hora de início válida.\n\n" +
                    "Exemplo: 09:30.");

                txtHoraInicio.Focus();
                return;
            }

            if (!TentarLerHora(
                    txtHoraFim.Text,
                    out TimeSpan horaFim))
            {
                Mensagem.Aviso(
                    "Introduza uma hora de fim válida.\n\n" +
                    "Exemplo: 10:30.");

                txtHoraFim.Focus();
                return;
            }

            DateTime data =
                dpData.SelectedDate.Value.Date;

            DateTime dataInicio =
                data.Add(
                    horaInicio);

            DateTime dataFim =
                data.Add(
                    horaFim);

            if (dataFim <= dataInicio)
            {
                Mensagem.Aviso(
                    "A hora de fim deve ser posterior à hora de início.");

                txtHoraFim.Focus();
                return;
            }

            if (evento == null &&
                dataInicio <= DateTime.Now)
            {
                Mensagem.Aviso(
                    "A data e hora de início devem ser posteriores ao momento atual.");

                txtHoraInicio.Focus();
                return;
            }

            if (cmbPT.SelectedValue is not int idPT)
            {
                Mensagem.Aviso(
                    "Selecione o personal trainer.");

                cmbPT.Focus();
                return;
            }

            if (cmbCliente.SelectedValue is not int idCliente)
            {
                Mensagem.Aviso(
                    "Selecione o cliente.");

                cmbCliente.Focus();
                return;
            }

            EventoAgenda dados =
                new EventoAgenda
                {
                    IdEvento =
                        evento?.IdEvento
                        ?? 0,

                    Titulo =
                        titulo,

                    Tipo =
                        "Sessão PT",

                    DataInicio =
                        dataInicio,

                    DataFim =
                        dataFim,

                    IdPT =
                        idPT,

                    IdProfessor =
                        null,

                    IdCliente =
                        idCliente,

                    IdAula =
                        null,

                    Localizacao =
                        txtLocalizacao.Text.Trim(),

                    Descricao =
                        txtDescricao.Text.Trim(),

                    Estado =
                        evento?.Estado
                        ?? "Agendado"
                };

            bool novoAgendamento =
                evento == null;

            if (!Mensagem.Confirmar(
                    $"Pretende {(novoAgendamento ? "criar" : "atualizar")} este agendamento?\n\n" +
                    $"Título: {titulo}\n" +
                    $"Data: {dataInicio:dd/MM/yyyy}\n" +
                    $"Horário: {dataInicio:HH:mm} - {dataFim:HH:mm}\n" +
                    $"Personal trainer: {cmbPT.Text}\n" +
                    $"Cliente: {cmbCliente.Text}"))
            {
                return;
            }

            try
            {
                btnGuardar.IsEnabled =
                    false;

                if (novoAgendamento)
                {
                    eventoService.Inserir(
                        dados);

                    Mensagem.Sucesso(
                        "Agendamento criado com sucesso!");
                }
                else
                {
                    eventoService.Atualizar(
                        dados);

                    Mensagem.Sucesso(
                        "Agendamento atualizado com sucesso!");
                }

                DialogResult =
                    true;
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível guardar o agendamento.\n\n" +
                    ex.Message);
            }
            finally
            {
                btnGuardar.IsEnabled =
                    true;
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