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
    public partial class EquipamentosPage : Page
    {
        private readonly EquipamentoService service = new EquipamentoService();

        public EquipamentosPage()
        {
            InitializeComponent();

            CarregarEquipamentos();
        }

        private void CarregarEquipamentos()
        {
            try
            {
                List<Equipamento> lista = service.Listar();

                AtualizarPagina(lista);
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível carregar os equipamentos.\n\n" + ex.Message);
            }
        }

        private void AtualizarPagina(List<Equipamento> lista)
        {
            dgEquipamentos.ItemsSource = lista;

            txtTotal.Text = lista.Count.ToString();
        }

        private void btnNovo_Click(object sender, RoutedEventArgs e)
        {
            EquipamentoForm form = new EquipamentoForm
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarEquipamentos();
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgEquipamentos.SelectedItem is not Equipamento equipamento)
            {
                Mensagem.Aviso("Selecione um equipamento.");

                return;
            }

            EquipamentoForm form = new EquipamentoForm(equipamento)
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarEquipamentos();
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgEquipamentos.SelectedItem is not Equipamento equipamento)
            {
                Mensagem.Aviso("Selecione um equipamento.");

                return;
            }

            if (string.Equals(equipamento.Estado, "Em manutenção", StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso("Um equipamento em manutenção não pode ser eliminado.");

                return;
            }

            if (string.Equals(equipamento.Estado, "Abatido", StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso("Um equipamento abatido não pode ser eliminado.");

                return;
            }

            if (!Mensagem.Confirmar($"Tem a certeza que pretende eliminar o equipamento " + $"'{equipamento.Nome}'?"))
            {
                return;
            }

            try
            {
                service.Eliminar(equipamento.IdEquipamento);

                Mensagem.Sucesso("Equipamento eliminado com sucesso!");

                CarregarEquipamentos();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível eliminar o equipamento.\n\n" + ex.Message);
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

                List<Equipamento> lista = string.IsNullOrWhiteSpace(pesquisa) ? service.Listar() : service.Pesquisar(pesquisa);

                AtualizarPagina(lista);
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível pesquisar os equipamentos.\n\n" + ex.Message);
            }
        }
    }
}