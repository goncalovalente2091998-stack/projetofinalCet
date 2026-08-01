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
    public partial class ManutencoesPage : Page
    {
        private readonly ManutencaoService service =
            new ManutencaoService();

        public ManutencoesPage()
        {
            InitializeComponent();

            CarregarManutencoes();
        }

        private void CarregarManutencoes()
        {
            try
            {
                List<Manutencao> lista =
                    service.Listar();

                AtualizarPagina(lista);
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível carregar as manutenções.\n\n" +
                    ex.Message);
            }
        }

        private void AtualizarPagina(
            List<Manutencao> lista)
        {
            dgManutencoes.ItemsSource =
                lista;

            txtTotal.Text =
                lista.Count.ToString();
        }

        private void btnNovo_Click(
            object sender,
            RoutedEventArgs e)
        {
            ManutencaoForm form =
                new ManutencaoForm
                {
                    Owner = Window.GetWindow(this)
                };

            if (form.ShowDialog() == true)
            {
                CarregarManutencoes();
            }
        }

        private void btnEditar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgManutencoes.SelectedItem is not
                Manutencao manutencao)
            {
                Mensagem.Aviso(
                    "Selecione uma manutenção.");

                return;
            }

            ManutencaoForm form =
                new ManutencaoForm(manutencao)
                {
                    Owner = Window.GetWindow(this)
                };

            if (form.ShowDialog() == true)
            {
                CarregarManutencoes();
            }
        }

        private void btnEliminar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dgManutencoes.SelectedItem is not
                Manutencao manutencao)
            {
                Mensagem.Aviso(
                    "Selecione uma manutenção.");

                return;
            }

            if (string.Equals(
                    manutencao.Estado,
                    "Concluída",
                    StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso(
                    "Uma manutenção concluída não pode ser eliminada.");

                return;
            }

            if (!Mensagem.Confirmar(
                    $"Tem a certeza que pretende eliminar a manutenção " +
                    $"do equipamento '{manutencao.NomeEquipamento}'?"))
            {
                return;
            }

            try
            {
                service.Eliminar(
                    manutencao.IdManutencao);

                Mensagem.Sucesso(
                    "Manutenção eliminada com sucesso!");

                CarregarManutencoes();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível eliminar a manutenção.\n\n" +
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

                List<Manutencao> lista =
                    string.IsNullOrWhiteSpace(pesquisa)
                        ? service.Listar()
                        : service.Pesquisar(pesquisa);

                AtualizarPagina(lista);
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível pesquisar as manutenções.\n\n" +
                    ex.Message);
            }
        }
    }
}