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

namespace GymManager.View
{
    public partial class PagamentosPage : Page
    {
        private readonly PagamentoService service = new PagamentoService();

        private readonly CultureInfo culturaPortugal = new CultureInfo("pt-PT");

        public PagamentosPage()
        {
            InitializeComponent();

            CarregarPagamentos();
        }

        private void CarregarPagamentos()
        {
            try
            {
                List<Pagamento> lista = service.Listar();

                AtualizarPagina(lista);
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível carregar os pagamentos.\n\n" + ex.Message);
            }
        }

        private void AtualizarPagina(List<Pagamento> lista)
        {
            dgPagamentos.ItemsSource = lista;

            decimal receitaTotal = lista.Where(p =>
                    string.Equals(p.Estado, "Pago", StringComparison.OrdinalIgnoreCase)).Sum(p =>
                    p.Valor);

            int totalPagos = lista.Count(p =>
                string.Equals(p.Estado, "Pago", StringComparison.OrdinalIgnoreCase));

            int totalPendentes = lista.Count(p =>
                string.Equals(p.Estado, "Pendente", StringComparison.OrdinalIgnoreCase));

            int totalFalhados = lista.Count(p =>
                string.Equals(p.Estado, "Falhado", StringComparison.OrdinalIgnoreCase));

            txtReceitaTotal.Text = receitaTotal.ToString("C2", culturaPortugal);

            txtTotalPagos.Text = totalPagos.ToString();

            txtTotalPendentes.Text = totalPendentes.ToString();

            txtTotalFalhados.Text = totalFalhados.ToString();
        }

        private void btnNovo_Click(object sender, RoutedEventArgs e)
        {
            PagamentoForm form = new PagamentoForm
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarPagamentos();
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgPagamentos.SelectedItem is not Pagamento pagamento)
            {
                Mensagem.Aviso("Selecione um pagamento.");

                return;
            }
            if (string.Equals(pagamento.Estado, "Reembolsado", StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso("Um pagamento reembolsado não pode ser editado.");

                return;
            }
            PagamentoForm form = new PagamentoForm(pagamento)
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarPagamentos();
            }
        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (dgPagamentos.SelectedItem is not Pagamento pagamento)
            {
                Mensagem.Aviso("Selecione um pagamento.");

                return;
            }

            if (!string.Equals(pagamento.Estado, "Pendente", StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso("Apenas pagamentos pendentes podem ser confirmados.");

                return;
            }

            bool metodoConfirmavel = string.Equals(pagamento.MetodoPagamento, "Transferência Bancária", StringComparison.OrdinalIgnoreCase) || string.Equals(pagamento.MetodoPagamento, "Pagamento Posterior", StringComparison.OrdinalIgnoreCase) || string.Equals(pagamento.MetodoPagamento, "PayPal", StringComparison.OrdinalIgnoreCase);

            if (!metodoConfirmavel)
            {
                Mensagem.Aviso("Este método não permite confirmação manual.");

                return;
            }

            string valorFormatado = pagamento.Valor.ToString("C2", culturaPortugal);

            if (!Mensagem.Confirmar($"Confirma a receção do pagamento de " + $"{valorFormatado} de '{pagamento.NomeCliente}'?"))
            {
                return;
            }

            try
            {
                service.Confirmar(pagamento.IdPagamento);

                Mensagem.Sucesso("Pagamento confirmado com sucesso!");

                CarregarPagamentos();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível confirmar o pagamento.\n\n" + ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgPagamentos.SelectedItem is not Pagamento pagamento)
            {
                Mensagem.Aviso("Selecione um pagamento.");

                return;
            }

            if (string.Equals(pagamento.Estado, "Pago", StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso("Um pagamento pago não pode ser eliminado.");

                return;
            }

            if (string.Equals(pagamento.Estado, "Reembolsado", StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso("Um pagamento reembolsado não pode ser eliminado.");

                return;
            }

            if (!Mensagem.Confirmar($"Tem a certeza que pretende eliminar o pagamento " + $"de '{pagamento.NomeCliente}' no valor de " + $"{pagamento.Valor:N2} €?"))
            {
                return;
            }

            try
            {
                service.Eliminar(pagamento.IdPagamento);

                Mensagem.Sucesso("Pagamento eliminado com sucesso.");

                CarregarPagamentos();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível eliminar o pagamento.\n\n" + ex.Message);
            }
        }

        private void txtPesquisar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            try
            {
                string pesquisa = txtPesquisar.Text.Trim();

                List<Pagamento> lista = string.IsNullOrWhiteSpace(pesquisa) ? service.Listar() : service.Pesquisar(pesquisa);

                AtualizarPagina(lista);
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível pesquisar os pagamentos.\n\n" + ex.Message);
            }
        }

        private void btnReembolsar_Click(object sender, RoutedEventArgs e)
        {
            if (dgPagamentos.SelectedItem is not Pagamento pagamento)
            {
                Mensagem.Aviso("Selecione um pagamento.");

                return;
            }

            if (!string.Equals(pagamento.Estado, "Pago", StringComparison.OrdinalIgnoreCase))
            {
                Mensagem.Aviso("Apenas pagamentos pagos podem ser reembolsados.");

                return;
            }

            string valorFormatado = pagamento.Valor.ToString("C2", culturaPortugal);

            if (!Mensagem.Confirmar($"Tem a certeza que pretende reembolsar " + $"{valorFormatado} a '{pagamento.NomeCliente}'?"))
            {
                return;
            }

            try
            {
                service.Reembolsar(pagamento.IdPagamento);

                Mensagem.Sucesso("Pagamento marcado como reembolsado.");

                CarregarPagamentos();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível reembolsar o pagamento.\n\n" + ex.Message);
            }
        }
        private void BtnExportarPdf_Click(object sender, RoutedEventArgs e)
        {
            if (dgPagamentos.SelectedItem is not Pagamento pagamento)
            {
                Mensagem.Aviso("Selecione um pagamento.");

                return;
            }

            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "PDF (*.pdf)|*.pdf";

            dlg.FileName = $"Pagamento_{pagamento.IdPagamento}.pdf";

            if (dlg.ShowDialog() == true)
            {
                PdfPagamento.Gerar(dlg.FileName, pagamento);

                Mensagem.Sucesso("Pagamento exportado com sucesso.");
            }
        }
    }
}