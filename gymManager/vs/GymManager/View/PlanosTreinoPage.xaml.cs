using GymManager.Helpers;
using GymManager.Models;
using GymManager.Models.GymManager.Models;
using GymManager.Services;
using GymManager.View.Forms;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GymManager.View
{
    public partial class PlanosTreinoPage : Page
    {
        private readonly PlanoTreinoService service = new PlanoTreinoService();

        public PlanosTreinoPage()
        {
            InitializeComponent();

            CarregarPlanos();
        }

        private void CarregarPlanos()
        {
            try
            {
                List<PlanoTreino> lista = service.Listar();

                AtualizarPagina(lista);
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível carregar os planos de treino.\n\n" + ex.Message);
            }
        }

        private void AtualizarPagina(List<PlanoTreino> lista)
        {
            dgPlanosTreino.ItemsSource = lista;

            txtTotal.Text = lista.Count.ToString();
        }

        private void btnNovo_Click(object sender, RoutedEventArgs e)
        {
            PlanoTreinoForm form = new PlanoTreinoForm
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarPlanos();
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgPlanosTreino.SelectedItem is not PlanoTreino plano)
            {
                Mensagem.Aviso("Selecione um plano de treino.");

                return;
            }

            PlanoTreinoForm form = new PlanoTreinoForm(plano)
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarPlanos();
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgPlanosTreino.SelectedItem is not PlanoTreino plano)
            {
                Mensagem.Aviso("Selecione um plano de treino.");

                return;
            }

            if (!Mensagem.Confirmar($"Tem a certeza que pretende eliminar o plano " + $"'{plano.NomePlano}' do cliente " + $"'{plano.NomeCliente}'?"))
            {
                return;
            }

            try
            {
                service.Eliminar(plano.IdPlanoTreino);

                Mensagem.Sucesso("Plano de treino eliminado com sucesso!");

                CarregarPlanos();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível eliminar o plano de treino.\n\n" + ex.Message);
            }
        }

        private void btnExercicios_Click(object sender, RoutedEventArgs e)
        {
            if (dgPlanosTreino.SelectedItem is not PlanoTreino plano)
            {
                Mensagem.Aviso("Selecione um plano de treino.");

                return;
            }

            NavigationService?.Navigate(new PlanoTreinoExerciciosPage(plano));
        }

        private void txtPesquisar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            try
            {
                string pesquisa = txtPesquisar.Text.Trim();

                List<PlanoTreino> lista = string.IsNullOrWhiteSpace(pesquisa) ? service.Listar() : service.Pesquisar(pesquisa);

                AtualizarPagina(lista);
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível pesquisar os planos de treino.\n\n" + ex.Message);
            }
        }
        private void BtnExportarPdf_Click(object sender, RoutedEventArgs e)
        {
            if (dgPlanosTreino.SelectedItem is not PlanoTreino plano)
            {
                Mensagem.Aviso("Selecione um plano de treino.");

                return;
            }

            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "PDF (*.pdf)|*.pdf";

            dlg.FileName = $"{plano.NomePlano}.pdf";

            if (dlg.ShowDialog() == true)
            {
                PdfPlanoTreino.Gerar(dlg.FileName, plano);

                Mensagem.Sucesso("Plano exportado com sucesso.");
            }
        }
    }
}