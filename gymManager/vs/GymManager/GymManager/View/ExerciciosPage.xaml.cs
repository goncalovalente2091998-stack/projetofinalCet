using GymManager.Helpers;
using GymManager.Models;
using GymManager.Services;
using GymManager.View.Forms;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GymManager.View
{
    public partial class ExerciciosPage : Page
    {
        private readonly ExercicioService service =
            new ExercicioService();

        public ExerciciosPage()
        {
            InitializeComponent();

            CarregarExercicios();
        }

        private void CarregarExercicios()
        {
            try
            {
                List<Exercicio> lista =
                    service.Listar();

                AtualizarPagina(
                    lista);
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível carregar os exercícios.\n\n" +
                    ex.Message);
            }
        }

        private void AtualizarPagina(
            List<Exercicio> lista)
        {
            dgExercicios.ItemsSource =
                lista;

            txtTotal.Text =
                lista.Count.ToString();
        }

        private void btnNovo_Click(
            object sender,
            RoutedEventArgs e)
        {
            ExercicioForm form =
                new ExercicioForm
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
            if (dgExercicios.SelectedItem is not
                Exercicio exercicio)
            {
                Mensagem.Aviso(
                    "Selecione um exercício.");

                return;
            }

            ExercicioForm form =
                new ExercicioForm(exercicio)
                {
                    Owner = Window.GetWindow(this)
                };

            if (form.ShowDialog() == true)
            {
                CarregarExercicios();
            }
        }

        private void btnEliminar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgExercicios.SelectedItem is not
                Exercicio exercicio)
            {
                Mensagem.Aviso(
                    "Selecione um exercício.");

                return;
            }

            if (!Mensagem.Confirmar(
                    $"Tem a certeza que pretende eliminar o exercício " +
                    $"'{exercicio.Nome}'?"))
            {
                return;
            }

            try
            {
                service.Eliminar(
                    exercicio.IdExercicio);

                Mensagem.Sucesso(
                    "Exercício eliminado com sucesso!");

                CarregarExercicios();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível eliminar o exercício.\n\n" +
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

            try
            {
                string pesquisa =
                    txtPesquisar.Text.Trim();

                List<Exercicio> lista =
                    string.IsNullOrWhiteSpace(pesquisa)
                        ? service.Listar()
                        : service.Pesquisar(pesquisa);

                AtualizarPagina(
                    lista);
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível pesquisar os exercícios.\n\n" +
                    ex.Message);
            }
        }
    }
}