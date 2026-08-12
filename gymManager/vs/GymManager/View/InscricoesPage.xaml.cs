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
    public partial class InscricoesPage : Page
    {
        private readonly InscricaoService service = new InscricaoService();

        public InscricoesPage()
        {
            InitializeComponent();

            CarregarInscricoes();
        }

        private void CarregarInscricoes()
        {
            try
            {
                List<Inscricao> lista = service.Listar();

                AtualizarPagina(lista);
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível carregar as inscrições.\n\n" + ex.Message);
            }
        }

        private void AtualizarPagina(List<Inscricao> lista)
        {
            dgInscricoes.ItemsSource = lista;

            txtTotal.Text = lista.Count.ToString();
        }

        private void btnNovo_Click(object sender, RoutedEventArgs e)
        {
            InscricaoForm form = new InscricaoForm
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarInscricoes();
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgInscricoes.SelectedItem is not Inscricao inscricao)
            {
                Mensagem.Aviso("Selecione uma inscrição.");

                return;
            }

            if (string.Equals(inscricao.Estado, "Terminada", StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso("Uma inscrição terminada não deve ser editada. " + "Utilize a opção Renovar.");

                return;
            }

            InscricaoForm form = new InscricaoForm(inscricao)
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarInscricoes();
            }
        }

        private void btnRenovar_Click(object sender, RoutedEventArgs e)
        {
            if (dgInscricoes.SelectedItem is not Inscricao inscricao)
            {
                Mensagem.Aviso("Selecione uma inscrição.");

                return;
            }

            bool podeRenovar = string.Equals(inscricao.Estado, "Terminada", StringComparison.OrdinalIgnoreCase) || string.Equals(inscricao.Estado, "Cancelada", StringComparison.OrdinalIgnoreCase);

            if (!podeRenovar)
            {
                Mensagem.Aviso("Apenas inscrições terminadas ou canceladas podem ser renovadas.");

                return;
            }

            if (!Mensagem.Confirmar($"Pretende renovar a inscrição de " + $"'{inscricao.NomeCliente}'?"))
            {
                return;
            }

            InscricaoForm form = new InscricaoForm(inscricao, true)
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarInscricoes();
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgInscricoes.SelectedItem is not Inscricao inscricao)
            {
                Mensagem.Aviso("Selecione uma inscrição.");

                return;
            }

            if (string.Equals(inscricao.Estado, "Ativa", StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso("Não é possível eliminar uma inscrição ativa. " + "Cancele primeiro a inscrição.");

                return;
            }

            if (!Mensagem.Confirmar($"Tem a certeza que pretende eliminar a inscrição de " + $"'{inscricao.NomeCliente}'?"))
            {
                return;
            }

            try
            {
                service.Eliminar(inscricao.IdInscricao);

                Mensagem.Sucesso("Inscrição eliminada com sucesso!");

                CarregarInscricoes();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível eliminar a inscrição.\n\n" + ex.Message);
            }
        }

        private void txtPesquisar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            try
            {
                string pesquisa = txtPesquisar.Text.Trim();

                List<Inscricao> lista = string.IsNullOrWhiteSpace(pesquisa) ? service.Listar() : service.Pesquisar(pesquisa);

                AtualizarPagina(lista);
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível pesquisar as inscrições.\n\n" + ex.Message);
            }
        }

        private void btnGerarPagamento_Click(object sender, RoutedEventArgs e)
        {
            if (dgInscricoes.SelectedItem is not Inscricao inscricao)
            {
                Mensagem.Aviso("Selecione uma inscrição.");

                return;
            }

            if (!string.Equals(inscricao.Estado, "Pendente", StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso("Apenas inscrições pendentes podem gerar um pagamento.");

                return;
            }

            if (!Mensagem.Confirmar($"Pretende gerar um novo pagamento para " + $"'{inscricao.NomeCliente}'?"))
            {
                return;
            }

            try
            {
                service.GerarPagamento(inscricao.IdInscricao);

                Mensagem.Sucesso("Pagamento pendente criado com sucesso.");
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível gerar o pagamento.\n\n" + ex.Message);
            }
        }
    }
}