using GymManager.Helpers;
using GymManager.Models;
using GymManager.Models.GymManager.Models;
using GymManager.Services;
using GymManager.View.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace GymManager.View
{
    public partial class ReservasAulasPage : Page
    {
        private readonly Aula aula;

        private readonly ReservaAulaService service =
            new ReservaAulaService();

        private List<ReservaAula> reservas = new();

        public ReservasAulasPage(
            Aula aulaSelecionada)
        {
            InitializeComponent();

            aula =
                aulaSelecionada;

            PreencherDadosAula();

            CarregarReservas();
        }

        private void PreencherDadosAula()
        {
            txtTitulo.Text =
                $"Reservas — {aula.Nome}";

            txtNomeAula.Text =
                aula.Nome;

            txtProfessor.Text =
                $"Professor: {aula.NomeProfessor}";

            txtDataHora.Text =
                $"{aula.DataAula:dd/MM/yyyy} às " +
                $"{aula.HoraInicio:hh\\:mm}";

            txtSala.Text =
                $"Sala: {aula.Sala}";
        }

        private void CarregarReservas()
        {
            try
            {
                reservas =
                    service.ListarPorAula(
                        aula.IdAula);

                AtualizarPagina(
                    reservas);
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível carregar as reservas.\n\n" +
                    ex.Message);
            }
        }

        private void AtualizarPagina(
            List<ReservaAula> lista)
        {
            dgReservas.ItemsSource =
                lista;

            txtTotalReservas.Text =
                lista.Count.ToString();

            int ocupadas =
                reservas.Count(r =>
                    string.Equals(
                        r.Estado,
                        "Confirmada",
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    string.Equals(
                        r.Estado,
                        "Presente",
                        StringComparison.OrdinalIgnoreCase));

            int livres =
                Math.Max(
                    0,
                    aula.Lotacao - ocupadas);

            txtReservadas.Text =
                ocupadas.ToString();

            txtLivres.Text =
                livres.ToString();
        }

        private void btnNovaReserva_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!string.Equals(
                    aula.Estado,
                    "Agendada",
                    StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso(
                    "Apenas aulas agendadas aceitam reservas.");

                return;
            }

            ReservaAulaForm form =
                new ReservaAulaForm(aula)
                {
                    Owner = Window.GetWindow(this)
                };

            if (form.ShowDialog() == true)
            {
                CarregarReservas();
            }
        }

        private void btnCancelarReserva_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgReservas.SelectedItem is not
                ReservaAula reserva)
            {
                Mensagem.Aviso(
                    "Selecione uma reserva.");

                return;
            }

            if (!string.Equals(
                    reserva.Estado,
                    "Confirmada",
                    StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso(
                    "Apenas reservas confirmadas podem ser canceladas.");

                return;
            }

            if (!Mensagem.Confirmar(
                    $"Pretende cancelar a reserva de " +
                    $"'{reserva.NomeCliente}'?"))
            {
                return;
            }

            try
            {
                service.Cancelar(
                    reserva.IdReserva);

                Mensagem.Sucesso(
                    "Reserva cancelada com sucesso.");

                CarregarReservas();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível cancelar a reserva.\n\n" +
                    ex.Message);
            }
        }

        private void btnPresente_Click(
            object sender,
            RoutedEventArgs e)
        {
            AlterarPresenca(
                marcarPresente: true);
        }

        private void btnFaltou_Click(
            object sender,
            RoutedEventArgs e)
        {
            AlterarPresenca(
                marcarPresente: false);
        }

        private void AlterarPresenca(
            bool marcarPresente)
        {
            if (dgReservas.SelectedItem is not
                ReservaAula reserva)
            {
                Mensagem.Aviso(
                    "Selecione uma reserva.");

                return;
            }

            if (string.Equals(
                    reserva.Estado,
                    "Cancelada",
                    StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso(
                    "Uma reserva cancelada não permite registar presença.");

                return;
            }

            string novoEstado =
                marcarPresente
                    ? "Presente"
                    : "Faltou";

            if (!Mensagem.Confirmar(
                    $"Pretende marcar '{reserva.NomeCliente}' " +
                    $"como {novoEstado}?"))
            {
                return;
            }

            try
            {
                if (marcarPresente)
                {
                    service.MarcarPresente(
                        reserva.IdReserva);
                }
                else
                {
                    service.MarcarFalta(
                        reserva.IdReserva);
                }

                Mensagem.Sucesso(
                    $"Reserva marcada como {novoEstado}.");

                CarregarReservas();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível alterar a presença.\n\n" +
                    ex.Message);
            }
        }

        private void txtPesquisar_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            string pesquisa =
                txtPesquisar.Text.Trim();

            List<ReservaAula> resultado;

            if (string.IsNullOrWhiteSpace(
                    pesquisa))
            {
                resultado =
                    reservas;
            }
            else
            {
                resultado =
                    reservas
                        .Where(r =>
                            r.NomeCliente.Contains(
                                pesquisa,
                                StringComparison.OrdinalIgnoreCase)
                            ||
                            r.NIF.Contains(
                                pesquisa,
                                StringComparison.OrdinalIgnoreCase)
                            ||
                            r.Estado.Contains(
                                pesquisa,
                                StringComparison.OrdinalIgnoreCase))
                        .ToList();
            }

            AtualizarPagina(
                resultado);
        }

        private void btnVoltar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService?.Navigate(
                    new AulasPage());
            }
        }
    }
}