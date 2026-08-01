using GymManager.Helpers;
using GymManager.Models;
using GymManager.Models.GymManager.Models;
using GymManager.Services;
using GymManager.View.Forms;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GymManager.View
{
    public partial class PlanoTreinoExerciciosPage : Page
    {
        private readonly PlanoTreino plano;

        private readonly PlanoTreinoExercicioService service =
            new PlanoTreinoExercicioService();

        public PlanoTreinoExerciciosPage(
            PlanoTreino planoSelecionado)
        {
            InitializeComponent();

            plano =
                planoSelecionado;

            txtTitulo.Text =
                $"Exercícios — {plano.NomePlano}";

            txtSubtitulo.Text =
                $"Cliente: {plano.NomeCliente} | PT: {plano.NomePT}";

            CarregarExercicios();
        }

        private void CarregarExercicios()
        {
            try
            {
                List<PlanoTreinoExercicio> lista =
                    service.ListarPorPlano(
                        plano.IdPlanoTreino);

                dgExerciciosPlano.ItemsSource =
                    lista;

                txtTotal.Text =
                    lista.Count.ToString();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível carregar os exercícios do plano.\n\n" +
                    ex.Message);
            }
        }

        private void btnAdicionar_Click(
            object sender,
            RoutedEventArgs e)
        {
            PlanoTreinoExercicioForm form =
                new PlanoTreinoExercicioForm(
                    plano)
                {
                    Owner = Window.GetWindow(this)
                };

            if (form.ShowDialog() == true)
            {
                CarregarExercicios();
            }
        }

        private void btnEditar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgExerciciosPlano.SelectedItem is not
                PlanoTreinoExercicio item)
            {
                Mensagem.Aviso(
                    "Selecione um exercício do plano.");

                return;
            }

            PlanoTreinoExercicioForm form =
                new PlanoTreinoExercicioForm(
                    plano,
                    item)
                {
                    Owner = Window.GetWindow(this)
                };

            if (form.ShowDialog() == true)
            {
                CarregarExercicios();
            }
        }

        private void btnRemover_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgExerciciosPlano.SelectedItem is not
                PlanoTreinoExercicio item)
            {
                Mensagem.Aviso(
                    "Selecione um exercício do plano.");

                return;
            }

            if (!Mensagem.Confirmar(
                    $"Pretende remover '{item.NomeExercicio}' deste plano?"))
            {
                return;
            }

            try
            {
                service.Eliminar(
                    item.IdPlanoTreinoExercicio);

                Mensagem.Sucesso(
                    "Exercício removido do plano com sucesso.");

                CarregarExercicios();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível remover o exercício do plano.\n\n" +
                    ex.Message);
            }
        }

        private void btnSubir_Click(
            object sender,
            RoutedEventArgs e)
        {
            AlterarOrdem(
                "Subir");
        }

        private void btnDescer_Click(
            object sender,
            RoutedEventArgs e)
        {
            AlterarOrdem(
                "Descer");
        }

        private void AlterarOrdem(
            string direcao)
        {
            if (dgExerciciosPlano.SelectedItem is not
                PlanoTreinoExercicio item)
            {
                Mensagem.Aviso(
                    "Selecione um exercício do plano.");

                return;
            }

            try
            {
                service.TrocarOrdem(
                    item.IdPlanoTreinoExercicio,
                    direcao);

                CarregarExercicios();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível alterar a ordem.\n\n" +
                    ex.Message);
            }
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
                    new PlanosTreinoPage());
            }
        }
    }
}