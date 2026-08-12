using GymManager.Helpers;
using GymManager.Models;
using GymManager.Services;
using GymManager.View.Forms;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GymManager.View
{
    public partial class AvaliacoesFisicasPage : Page
    {
        private readonly AvaliacaoFisicaService service = new AvaliacaoFisicaService();

        public AvaliacoesFisicasPage()
        {
            InitializeComponent();

            CarregarAvaliacoes();
        }

        private void CarregarAvaliacoes()
        {
            try
            {
                List<AvaliacaoFisica> lista = service.Listar();

                AtualizarPagina(lista);
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível carregar as avaliações físicas.\n\n" + ex.Message);
            }
        }

        private void AtualizarPagina(List<AvaliacaoFisica> lista)
        {
            dgAvaliacoes.ItemsSource = lista;

            txtTotal.Text = lista.Count.ToString();
        }

        private void btnNovo_Click(object sender, RoutedEventArgs e)
        {
            AvaliacaoFisicaForm form = new AvaliacaoFisicaForm
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarAvaliacoes();
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgAvaliacoes.SelectedItem is not AvaliacaoFisica avaliacao)
            {
                Mensagem.Aviso("Selecione uma avaliação física.");

                return;
            }

            AvaliacaoFisicaForm form = new AvaliacaoFisicaForm(avaliacao)
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarAvaliacoes();
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgAvaliacoes.SelectedItem is not AvaliacaoFisica avaliacao)
            {
                Mensagem.Aviso("Selecione uma avaliação física.");

                return;
            }

            if (string.Equals(avaliacao.Estado, "Concluída", StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso("Uma avaliação concluída não pode ser eliminada.");

                return;
            }

            if (!Mensagem.Confirmar($"Tem a certeza que pretende eliminar a avaliação " +
                    $"de '{avaliacao.NomeCliente}' marcada para " +
                    $"{avaliacao.DataAvaliacao:dd/MM/yyyy}?"))
            {
                return;
            }

            try
            {
                service.Eliminar(avaliacao.IdAvaliacao);

                Mensagem.Sucesso("Avaliação física eliminada com sucesso!");

                CarregarAvaliacoes();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível eliminar a avaliação física.\n\n" + ex.Message);
            }
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

                List<AvaliacaoFisica> lista = string.IsNullOrWhiteSpace(pesquisa) ? service.Listar() : service.Pesquisar(pesquisa);

                AtualizarPagina(lista);
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível pesquisar as avaliações físicas.\n\n" + ex.Message);
            }
        }
        private void BtnExportarPdf_Click(object sender, RoutedEventArgs e)
        {
            if (dgAvaliacoes.SelectedItem is not AvaliacaoFisica avaliacao)
            {
                Mensagem.Aviso("Selecione uma avaliação física.");

                return;
            }

            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "PDF (*.pdf)|*.pdf";

            dlg.FileName = $"Avaliacao_{avaliacao.IdAvaliacao}.pdf";

            if (dlg.ShowDialog() == true)
            {
                PdfAvaliacao.Gerar(dlg.FileName, avaliacao);

                Mensagem.Sucesso("Avaliação exportada com sucesso.");
            }
        }
    }
}