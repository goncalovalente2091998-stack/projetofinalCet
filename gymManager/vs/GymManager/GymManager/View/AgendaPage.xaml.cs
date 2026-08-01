using GymManager.Helpers;
using GymManager.Models;
using GymManager.Services;
using GymManager.View.Forms;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GymManager.View
{
    public partial class AgendaPage : Page
    {
        private readonly EventoAgendaService eventoService =
            new EventoAgendaService();

        private readonly CultureInfo culturaPortugal =
            new CultureInfo("pt-PT");

        private List<EventoAgenda> eventosCalendario =
            new List<EventoAgenda>();

        private List<EventoAgenda> eventosDia =
    new List<EventoAgenda>();



        private DateTime mesVisualizado;
        private DateTime diaSelecionado;
        private bool paginaCarregada;

        public AgendaPage()
        {
            InitializeComponent();

            DateTime hoje =
                DateTime.Today;

            mesVisualizado =
                new DateTime(
                    hoje.Year,
                    hoje.Month,
                    1);

            diaSelecionado =
                hoje;

            Loaded +=
                AgendaPage_Loaded;
        }

        private void AgendaPage_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            if (paginaCarregada)
            {
                return;
            }

            paginaCarregada =
                true;

            CarregarEventosDoCalendario();
        }

        private string? ObterTipoSelecionado()
        {
            if (cmbFiltroTipo.SelectedItem is not ComboBoxItem item)
            {
                return null;
            }

            string tipo =
                item.Content?.ToString()
                ?? string.Empty;

            return string.Equals(
                       tipo,
                       "Todos",
                       StringComparison.OrdinalIgnoreCase)
                ? null
                : tipo;
        }

        private void CarregarEventosDoCalendario()
        {
            try
            {
                DateTime primeiroDiaMes =
                    new DateTime(
                        mesVisualizado.Year,
                        mesVisualizado.Month,
                        1);

                int deslocamento =
                    ObterIndiceDiaSemana(
                        primeiroDiaMes.DayOfWeek);

                DateTime primeiroDiaGrelha =
                    primeiroDiaMes.AddDays(
                        -deslocamento);

                DateTime fimDaGrelha =
                    primeiroDiaGrelha.AddDays(42);

                eventosCalendario =
                    eventoService.ListarPorPeriodo(
                        primeiroDiaGrelha,
                        fimDaGrelha,
                        null,
                        ObterTipoSelecionado());

                ConstruirCalendario();
                CarregarEventosDoDia();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível carregar a agenda.\n\n" +
                    ex.Message);
            }
        }

        private void ConstruirCalendario()
        {
            gridCalendario.Children.Clear();

            string mesAno =
                mesVisualizado.ToString(
                    "MMMM yyyy",
                    culturaPortugal);

            txtMesAno.Text =
                culturaPortugal.TextInfo
                    .ToTitleCase(
                        mesAno);

            DateTime primeiroDiaMes =
                new DateTime(
                    mesVisualizado.Year,
                    mesVisualizado.Month,
                    1);

            int deslocamento =
                ObterIndiceDiaSemana(
                    primeiroDiaMes.DayOfWeek);

            DateTime primeiroDiaGrelha =
                primeiroDiaMes.AddDays(
                    -deslocamento);

            for (int i = 0; i < 42; i++)
            {
                DateTime data =
                    primeiroDiaGrelha.AddDays(i);

                gridCalendario.Children.Add(
                    CriarBotaoDia(
                        data));
            }
        }

        private static int ObterIndiceDiaSemana(
            DayOfWeek diaSemana)
        {
            return diaSemana switch
            {
                DayOfWeek.Monday => 0,
                DayOfWeek.Tuesday => 1,
                DayOfWeek.Wednesday => 2,
                DayOfWeek.Thursday => 3,
                DayOfWeek.Friday => 4,
                DayOfWeek.Saturday => 5,
                DayOfWeek.Sunday => 6,
                _ => 0
            };
        }

        private Button CriarBotaoDia(
            DateTime data)
        {
            Button botao =
                new Button
                {
                    Style =
                        (Style)FindResource(
                            "BotaoDiaCalendario"),

                    Tag =
                        data
                };

            botao.Click +=
                BotaoDia_Click;

            Grid conteudo =
                new Grid();

            conteudo.RowDefinitions.Add(
                new RowDefinition
                {
                    Height =
                        GridLength.Auto
                });

            conteudo.RowDefinitions.Add(
                new RowDefinition
                {
                    Height =
                        new GridLength(
                            1,
                            GridUnitType.Star)
                });

            Border numeroContainer =
                new Border
                {
                    Width =
                        28,

                    Height =
                        28,

                    CornerRadius =
                        new CornerRadius(14),

                    HorizontalAlignment =
                        HorizontalAlignment.Left,

                    VerticalAlignment =
                        VerticalAlignment.Top
                };

            TextBlock numeroDia =
                new TextBlock
                {
                    Text =
                        data.Day.ToString(),

                    FontSize =
                        13,

                    FontWeight =
                        FontWeights.SemiBold,

                    HorizontalAlignment =
                        HorizontalAlignment.Center,

                    VerticalAlignment =
                        VerticalAlignment.Center
                };

            bool pertenceAoMes =
                data.Month ==
                mesVisualizado.Month
                &&
                data.Year ==
                mesVisualizado.Year;

            bool hoje =
                data.Date ==
                DateTime.Today;

            bool selecionado =
                data.Date ==
                diaSelecionado.Date;

            if (selecionado)
            {
                numeroContainer.Background =
                    new SolidColorBrush(
                        Color.FromRgb(
                            37,
                            99,
                            235));

                numeroDia.Foreground =
                    Brushes.White;

                botao.Background =
                    new SolidColorBrush(
                        Color.FromRgb(
                            239,
                            246,
                            255));

                botao.BorderBrush =
                    new SolidColorBrush(
                        Color.FromRgb(
                            37,
                            99,
                            235));

                botao.BorderThickness =
                    new Thickness(2);
            }
            else if (hoje)
            {
                numeroContainer.Background =
                    new SolidColorBrush(
                        Color.FromRgb(
                            22,
                            163,
                            74));

                numeroDia.Foreground =
                    Brushes.White;

                botao.Background =
                    new SolidColorBrush(
                        Color.FromRgb(
                            240,
                            253,
                            244));

                botao.BorderBrush =
                    new SolidColorBrush(
                        Color.FromRgb(
                            34,
                            197,
                            94));

                botao.BorderThickness =
                    new Thickness(2);
            }
            else
            {
                numeroContainer.Background =
                    Brushes.Transparent;

                numeroDia.Foreground =
                    pertenceAoMes
                        ? new SolidColorBrush(
                            Color.FromRgb(
                                30,
                                41,
                                59))
                        : new SolidColorBrush(
                            Color.FromRgb(
                                148,
                                163,
                                184));
            }

            numeroContainer.Child =
                numeroDia;

            conteudo.Children.Add(
                numeroContainer);

            DateTime inicioDia =
                data.Date;

            DateTime fimDia =
                inicioDia.AddDays(1);

            eventosDia =
     eventoService.ListarPorPeriodo(
         inicioDia,
         fimDia,
         null,
         ObterTipoSelecionado())
     .OrderBy(evento => evento.DataInicio)
     .ThenBy(evento => evento.Titulo)
     .ToList();

            if (eventosDia.Count > 0)
            {
                WrapPanel indicadores =
                    CriarIndicadoresEventos(
                        eventosDia);

                Grid.SetRow(
                    indicadores,
                    1);

                conteudo.Children.Add(
                    indicadores);
            }

            botao.Content =
                conteudo;

            return botao;
        }

        private static WrapPanel CriarIndicadoresEventos(
            List<EventoAgenda> eventos)
        {
            WrapPanel painel =
                new WrapPanel
                {
                    Margin =
                        new Thickness(
                            0,
                            7,
                            0,
                            0),

                    VerticalAlignment =
                        VerticalAlignment.Top
                };

            foreach (EventoAgenda evento in eventos.Take(4))
            {
                Ellipse ponto =
                    new Ellipse
                    {
                        Width =
                            8,

                        Height =
                            8,

                        Margin =
                            new Thickness(
                                0,
                                0,
                                5,
                                0),

                        Fill =
                            ObterCorEvento(
                                evento)
                    };

                painel.Children.Add(
                    ponto);
            }

            if (eventos.Count > 4)
            {
                TextBlock total =
                    new TextBlock
                    {
                        Text =
                            $"+{eventos.Count - 4}",

                        FontSize =
                            10,

                        Foreground =
                            new SolidColorBrush(
                                Color.FromRgb(
                                    100,
                                    116,
                                    139))
                    };

                painel.Children.Add(
                    total);
            }

            return painel;
        }

        private static Brush ObterCorEvento(
            EventoAgenda evento)
        {
            if (evento.EstaCancelado)
            {
                return new SolidColorBrush(
                    Color.FromRgb(
                        239,
                        68,
                        68));
            }

            if (evento.EstaConcluido)
            {
                return new SolidColorBrush(
                    Color.FromRgb(
                        100,
                        116,
                        139));
            }

            if (evento.EhAula)
            {
                return new SolidColorBrush(
                    Color.FromRgb(
                        34,
                        197,
                        94));
            }

            return new SolidColorBrush(
                Color.FromRgb(
                    37,
                    99,
                    235));
        }

        private void BotaoDia_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button botao ||
                botao.Tag is not DateTime data)
            {
                return;
            }

            diaSelecionado =
                data.Date;

            if (diaSelecionado.Month !=
                    mesVisualizado.Month
                ||
                diaSelecionado.Year !=
                    mesVisualizado.Year)
            {
                mesVisualizado =
                    new DateTime(
                        diaSelecionado.Year,
                        diaSelecionado.Month,
                        1);

                CarregarEventosDoCalendario();
                return;
            }

            ConstruirCalendario();
            CarregarEventosDoDia();
        }

        private void CarregarEventosDoDia()
        {
            try
            {
                DateTime inicioDia =
                    diaSelecionado.Date;

                DateTime fimDia =
                    inicioDia.AddDays(1);

                eventosDia =
    eventoService.ListarPorPeriodo(
        inicioDia,
        fimDia,
        null,
        ObterTipoSelecionado())
    .OrderBy(evento => evento.DataInicio)
    .ThenBy(evento => evento.Titulo)
    .ToList();

                lstEventosDia.ItemsSource =
                    eventosDia;

                lstEventosDia.SelectedItem =
                    null;

                if (eventosDia.Count == 0)
                {
                    lstEventosDia.Visibility =
                        Visibility.Collapsed;

                    borderSemEventos.Visibility =
                        Visibility.Visible;

                    txtTotalEventosDia.Text =
                        "Sem eventos";
                }
                else
                {
                    lstEventosDia.Visibility =
                        Visibility.Visible;

                    borderSemEventos.Visibility =
                        Visibility.Collapsed;

                    txtTotalEventosDia.Text =
                        eventosDia.Count == 1
                            ? "1 evento"
                            : $"{eventosDia.Count} eventos";
                }

                AtualizarBotoesAcao();

                string textoDia =
                    diaSelecionado.ToString(
                        "dddd, dd 'de' MMMM 'de' yyyy",
                        culturaPortugal);

                txtDiaSelecionado.Text =
                    culturaPortugal.TextInfo
                        .ToTitleCase(textoDia);
            }
            catch (Exception ex)
            {
                lstEventosDia.ItemsSource =
                    null;

                lstEventosDia.Visibility =
                    Visibility.Collapsed;

                borderSemEventos.Visibility =
                    Visibility.Visible;

                txtTotalEventosDia.Text =
                    "Sem eventos";

                AtualizarBotoesAcao();

                Mensagem.Erro(
                    "Não foi possível carregar os eventos do dia.\n\n" +
                    ex.Message);
            }
        }

        private void lstEventosDia_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            AtualizarBotoesAcao();
        }

        private void AtualizarBotoesAcao()
        {
            EventoAgenda? evento =
                ObterEventoSelecionado();

            /*
             * Sem seleção, não existe nenhuma ação disponível.
             */
            if (evento == null)
            {
                btnEditar.Visibility =
                    Visibility.Collapsed;

                btnConcluir.Visibility =
                    Visibility.Collapsed;

                btnCancelarEvento.Visibility =
                    Visibility.Collapsed;

                return;
            }

            /*
             * As aulas são apresentadas apenas para consulta.
             */
            if (evento.EhAula)
            {
                btnEditar.Visibility =
                    Visibility.Collapsed;

                btnConcluir.Visibility =
                    Visibility.Collapsed;

                btnCancelarEvento.Visibility =
                    Visibility.Collapsed;

                return;
            }

            /*
             * Um agendamento concluído ou cancelado
             * fica apenas disponível para consulta.
             */
            if (evento.EstaConcluido ||
                evento.EstaCancelado)
            {
                btnEditar.Visibility =
                    Visibility.Collapsed;

                btnConcluir.Visibility =
                    Visibility.Collapsed;

                btnCancelarEvento.Visibility =
                    Visibility.Collapsed;

                return;
            }

            /*
             * Apenas uma sessão PT agendada pode ser
             * editada, concluída ou cancelada.
             */
            btnEditar.Visibility =
                Visibility.Visible;

            btnConcluir.Visibility =
                Visibility.Visible;

            btnCancelarEvento.Visibility =
                Visibility.Visible;
        }

        private EventoAgenda? ObterEventoSelecionado()
        {
            return lstEventosDia.SelectedItem
                as EventoAgenda;
        }

        private void btnMesAnterior_Click(
            object sender,
            RoutedEventArgs e)
        {
            mesVisualizado =
                mesVisualizado.AddMonths(-1);

            diaSelecionado =
                new DateTime(
                    mesVisualizado.Year,
                    mesVisualizado.Month,
                    1);

            CarregarEventosDoCalendario();
        }

        private void btnMesSeguinte_Click(
            object sender,
            RoutedEventArgs e)
        {
            mesVisualizado =
                mesVisualizado.AddMonths(1);

            diaSelecionado =
                new DateTime(
                    mesVisualizado.Year,
                    mesVisualizado.Month,
                    1);

            CarregarEventosDoCalendario();
        }

        private void btnHoje_Click(
            object sender,
            RoutedEventArgs e)
        {
            diaSelecionado =
                DateTime.Today;

            mesVisualizado =
                new DateTime(
                    diaSelecionado.Year,
                    diaSelecionado.Month,
                    1);

            CarregarEventosDoCalendario();
        }

        private void cmbFiltroTipo_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!IsInitialized ||
                !paginaCarregada)
            {
                return;
            }

            CarregarEventosDoCalendario();
        }

        private void btnNovoAgendamento_Click(
            object sender,
            RoutedEventArgs e)
        {
            EventoAgendaForm form =
                new EventoAgendaForm(
                    diaSelecionado);

            form.Owner =
                Window.GetWindow(this);

            if (form.ShowDialog() == true)
            {
                CarregarEventosDoCalendario();
            }
        }

        private void btnEditar_Click(
            object sender,
            RoutedEventArgs e)
        {
            EventoAgenda? evento =
                ObterEventoSelecionado();

            if (evento == null)
            {
                Mensagem.Aviso(
                    "Selecione um agendamento para editar.");

                return;
            }

            if (evento.EhAula)
            {
                Mensagem.Aviso(
                    "As aulas são apenas apresentadas na agenda.\n\n" +
                    "Para alterar esta aula, utilize o módulo Aulas.");

                return;
            }

            if (evento.EstaCancelado)
            {
                Mensagem.Aviso(
                    "Não é possível editar um agendamento cancelado.");

                return;
            }

            EventoAgendaForm form =
                new EventoAgendaForm(
                    evento);

            form.Owner =
                Window.GetWindow(this);

            if (form.ShowDialog() == true)
            {
                CarregarEventosDoCalendario();
            }
        }

        private void btnConcluir_Click(
            object sender,
            RoutedEventArgs e)
        {
            EventoAgenda? evento =
                ObterEventoSelecionado();

            if (evento == null)
            {
                Mensagem.Aviso(
                    "Selecione um agendamento para concluir.");

                return;
            }

            if (evento.EhAula)
            {
                Mensagem.Aviso(
                    "As aulas são apenas apresentadas na agenda.\n\n" +
                    "Para concluir esta aula, utilize o módulo Aulas.");

                return;
            }

            if (evento.EstaConcluido)
            {
                Mensagem.Aviso(
                    "Este agendamento já está concluído.");

                return;
            }

            if (evento.EstaCancelado)
            {
                Mensagem.Aviso(
                    "Não é possível concluir um agendamento cancelado.");

                return;
            }

            if (!Mensagem.Confirmar(
                    "Pretende marcar este agendamento como concluído?\n\n" +
                    $"Agendamento: {evento.Titulo}\n" +
                    $"Data: {evento.DataInicio:dd/MM/yyyy}\n" +
                    $"Horário: {evento.HorarioFormatado}\n" +
                    $"Personal trainer: {evento.NomePT}"))
            {
                return;
            }

            try
            {
                eventoService.Concluir(
                    evento.IdEvento);

                Mensagem.Sucesso(
                    "Agendamento concluído com sucesso!");

                CarregarEventosDoCalendario();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível concluir o agendamento.\n\n" +
                    ex.Message);
            }
        }

        private void btnCancelarEvento_Click(
            object sender,
            RoutedEventArgs e)
        {
            EventoAgenda? evento =
                ObterEventoSelecionado();

            if (evento == null)
            {
                Mensagem.Aviso(
                    "Selecione um agendamento para cancelar.");

                return;
            }

            if (evento.EhAula)
            {
                Mensagem.Aviso(
                    "As aulas são apenas apresentadas na agenda.\n\n" +
                    "Para cancelar esta aula, utilize o módulo Aulas.");

                return;
            }

            if (evento.EstaCancelado)
            {
                Mensagem.Aviso(
                    "Este agendamento já está cancelado.");

                return;
            }

            if (evento.EstaConcluido)
            {
                Mensagem.Aviso(
                    "Não é possível cancelar um agendamento concluído.");

                return;
            }

            if (!Mensagem.Confirmar(
                    "Pretende cancelar este agendamento?\n\n" +
                    $"Agendamento: {evento.Titulo}\n" +
                    $"Data: {evento.DataInicio:dd/MM/yyyy}\n" +
                    $"Horário: {evento.HorarioFormatado}\n" +
                    $"Personal trainer: {evento.NomePT}"))
            {
                return;
            }

            try
            {
                eventoService.Cancelar(
                    evento.IdEvento);

                Mensagem.Sucesso(
                    "Agendamento cancelado com sucesso!");

                CarregarEventosDoCalendario();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível cancelar o agendamento.\n\n" +
                    ex.Message);
            }
        }
        private void BtnExportarPdf_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "PDF (*.pdf)|*.pdf",
                FileName = $"Agenda_{diaSelecionado:yyyyMMdd}.pdf"
            };

            if (dlg.ShowDialog() == true)
            {
                PdfAgenda.Gerar(
                    dlg.FileName,
                    diaSelecionado,
                    eventosDia);

                Mensagem.Sucesso(
                    "Agenda exportada.");
            }
        }
    }
}