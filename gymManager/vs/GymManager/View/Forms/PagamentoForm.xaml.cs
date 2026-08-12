using GymManager.Helpers;
using GymManager.Models;
using GymManager.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GymManager.View.Forms
{

    public partial class PagamentoForm : Window
    {
        private readonly PagamentoService pagamentoService = new PagamentoService();

        private readonly ClienteService clienteService = new ClienteService();

        private readonly InscricaoService inscricaoService = new InscricaoService();

        private readonly Pagamento? pagamento;

        private List<Cliente> clientes = new();

        private List<InscricaoPagamento> inscricoesAtivas = new();

        private Cliente? clienteSelecionado;

        private bool atualizandoTextoCliente;

        private string referenciaTransferencia = string.Empty;

        private string referenciaPayPal = string.Empty;
        public PagamentoForm()
        {
            InitializeComponent();

            CarregarClientes();

            dpDataPagamento.SelectedDate = DateTime.Today;

            cmbMetodoPagamento.SelectedIndex = 0;
            cmbEstado.SelectedIndex = 1;

            dpDataConfirmacao.SelectedDate = DateTime.Today;

            cmbInscricao.IsEnabled = false;
            txtValor.IsReadOnly = true;

            AtualizarCamposMetodo();
        }

        public PagamentoForm(Pagamento pagamento)
        {
            InitializeComponent();

            this.pagamento = pagamento;

            Title = "Editar Pagamento";
            txtTitulo.Text = "Editar Pagamento";

            CarregarClientes();

            Cliente? cliente = clientes
                .FirstOrDefault(c =>
                    c.IdCliente == pagamento.IdCliente);

            if (cliente != null)
            {
                SelecionarCliente(cliente);
                if (pagamento.IdInscricao.HasValue)
                {
                    InscricaoPagamento? inscricaoAtual = inscricoesAtivas.FirstOrDefault(i =>
                            i.IdInscricao == pagamento.IdInscricao.Value);

                    if (inscricaoAtual == null)
                    {
                        inscricaoAtual = new InscricaoPagamento
                        {
                            IdInscricao = pagamento.IdInscricao.Value,
                            IdCliente = pagamento.IdCliente,
                            NomePlano = pagamento.NomePlano,
                            Preco = pagamento.Valor,
                            DataInicio = pagamento.DataPagamento,
                            DataFim = pagamento.DataPagamento
                        };

                        inscricoesAtivas.Add(inscricaoAtual);

                        cmbInscricao.ItemsSource = null;
                        cmbInscricao.ItemsSource = inscricoesAtivas;
                        cmbInscricao.IsEnabled = true;
                    }

                    cmbInscricao.SelectedValue = pagamento.IdInscricao.Value;
                }
            }

            if (pagamento.IdInscricao.HasValue)
            {
                cmbInscricao.SelectedValue = pagamento.IdInscricao.Value;
            }

            dpDataPagamento.SelectedDate = pagamento.DataPagamento;

            txtValor.Text = pagamento.Valor.ToString("F2");

            SelecionarCombo(cmbMetodoPagamento, pagamento.MetodoPagamento);

            SelecionarCombo(cmbEstado, pagamento.Estado);

            txtReferenciaExterna.Text = pagamento.ReferenciaExterna;

            referenciaTransferencia = pagamento.ReferenciaExterna;

            txtReferenciaTransferencia.Text = referenciaTransferencia;

            txtIdTransacao.Text = pagamento.IdTransacaoExterna;

            dpDataConfirmacao.SelectedDate = pagamento.DataConfirmacao;

            txtObservacoes.Text = pagamento.Observacoes;

            txtValor.IsReadOnly = true;

            if (ObterTextoCombo(cmbMetodoPagamento) == "PayPal")
            {
                GerarReferenciaPayPal();
            }

            AtualizarCamposMetodo();

            AplicarRegrasEstadoEdicao();
        }

        private void CarregarClientes()
        {
            try
            {
                clientes = clienteService.Listar();

                lstClientes.ItemsSource = clientes;
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível carregar os clientes.\n\n" + ex.Message);
            }
        }

        private void txtCliente_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (atualizandoTextoCliente)
                return;

            clienteSelecionado = null;

            inscricoesAtivas.Clear();

            cmbInscricao.ItemsSource = null;
            cmbInscricao.IsEnabled = false;

            txtValor.Clear();

            string pesquisa = txtCliente.Text.Trim();

            if (string.IsNullOrWhiteSpace(pesquisa))
            {
                lstClientes.ItemsSource = clientes;

                popupClientes.IsOpen = false;
                return;
            }

            List<Cliente> resultados = clientes
                .Where(c => c.Nome.Contains(pesquisa, StringComparison.OrdinalIgnoreCase) || c.NIF.Contains(pesquisa, StringComparison.OrdinalIgnoreCase)).ToList();

            lstClientes.ItemsSource = resultados;

            lstClientes.SelectedIndex = -1;

            popupClientes.IsOpen = resultados.Count > 0;
        }

        private void txtCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && popupClientes.IsOpen && lstClientes.Items.Count > 0)
            {
                lstClientes.Focus();
                lstClientes.SelectedIndex = 0;

                e.Handled = true;
                return;
            }

            if (e.Key == Key.Escape)
            {
                popupClientes.IsOpen = false;
                e.Handled = true;
            }
        }

        private void lstClientes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && lstClientes.SelectedItem is Cliente cliente)
            {
                SelecionarCliente(cliente);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Escape)
            {
                popupClientes.IsOpen = false;

                txtCliente.Focus();

                e.Handled = true;
            }
        }

        private void lstClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!lstClientes.IsKeyboardFocusWithin)
                return;

            if (lstClientes.SelectedItem is Cliente cliente)
            {
                SelecionarCliente(cliente);
            }
        }

        private void lstClientes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstClientes.SelectedItem is Cliente cliente)
            {
                SelecionarCliente(cliente);
            }
        }

        private void SelecionarCliente(Cliente cliente)
        {
            clienteSelecionado = cliente;

            atualizandoTextoCliente = true;

            txtCliente.Text = cliente.DescricaoReserva;

            txtCliente.CaretIndex = txtCliente.Text.Length;

            atualizandoTextoCliente = false;

            popupClientes.IsOpen = false;

            lstClientes.SelectedIndex = -1;

            CarregarInscricoesAtivas(cliente.IdCliente);

            if (ObterTextoCombo(cmbMetodoPagamento) == "Transferência Bancária")
            {
                GerarReferenciaTransferencia();
            }
        }

        private void CarregarInscricoesAtivas(int idCliente)
        {
            try
            {
                inscricoesAtivas = inscricaoService.ListarDisponiveisParaPagamento(idCliente);

                cmbInscricao.ItemsSource = inscricoesAtivas;

                cmbInscricao.IsEnabled = inscricoesAtivas.Count > 0;

                txtValor.Clear();

                if (inscricoesAtivas.Count == 0)
                {
                    cmbInscricao.SelectedIndex = -1;
                    txtAvisoMetodo.Text = "O cliente não possui inscrições disponíveis para pagamento.";

                    return;
                }

                if (inscricoesAtivas.Count == 1)
                {
                    cmbInscricao.SelectedIndex = 0;
                }
                else
                {
                    cmbInscricao.SelectedIndex = -1;

                    txtAvisoMetodo.Text = "Selecione a inscrição que pretende pagar.";
                }
            }
            catch (Exception ex)
            {
                inscricoesAtivas.Clear();

                cmbInscricao.ItemsSource = null;
                cmbInscricao.IsEnabled = false;

                txtValor.Clear();

                Mensagem.Erro("Não foi possível carregar as inscrições do cliente.\n\n" + ex.Message);
            }
        }

        private void cmbInscricao_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbInscricao.SelectedItem is not InscricaoPagamento inscricaoSelecionada)
            {
                txtValor.Clear();
                return;
            }

            txtValor.Text = inscricaoSelecionada.Preco.ToString("F2");

            txtValor.IsReadOnly = true;

            txtAvisoMetodo.Text = $"Plano: {inscricaoSelecionada.NomePlano} — " +
                $"{inscricaoSelecionada.Preco:F2} € — " +
                $"válido até {inscricaoSelecionada.DataFim:dd/MM/yyyy}.";
        }

        private void cmbMetodoPagamento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            AtualizarCamposMetodo();
        }

        private void AtualizarCamposMetodo()
        {
            string metodo = ObterTextoCombo(cmbMetodoPagamento);
            bool ofertaSelecionada = cmbInscricao.SelectedItem is InscricaoPagamento inscricao && inscricao.Preco == 0;

            bool transferencia = metodo == "Transferência Bancária";

            bool pagamentoPosterior = metodo == "Pagamento Posterior";

            bool paypal = metodo == "PayPal";

            bool mbway = metodo == "MB WAY";

            if (!transferencia)
            {
                referenciaTransferencia = string.Empty;

                txtReferenciaTransferencia.Text = string.Empty;
            }

            if (!paypal)
            {
                referenciaPayPal = string.Empty;

                txtReferenciaPayPal.Text = string.Empty;
            }

            if (!transferencia && !paypal)
            {
                txtReferenciaExterna.Clear();
                txtIdTransacao.Clear();
            }

            bool pagamentoJaPago = pagamento != null && string.Equals(pagamento.Estado, "Pago", StringComparison.OrdinalIgnoreCase);

            bool pagamentoReembolsado = pagamento != null && string.Equals(pagamento.Estado, "Reembolsado", StringComparison.OrdinalIgnoreCase);

            pnlTransferencia.Visibility = transferencia ? Visibility.Visible : Visibility.Collapsed;

            btnPagarPayPal.Visibility = paypal ? Visibility.Visible : Visibility.Collapsed;

            pnlPayPal.Visibility = paypal ? Visibility.Visible : Visibility.Collapsed;

            txtReferenciaExterna.IsEnabled = paypal || mbway;

            txtIdTransacao.IsEnabled = paypal || mbway;
            if (ofertaSelecionada)
            {
                SelecionarCombo(cmbMetodoPagamento, "Oferta");

                SelecionarCombo(cmbEstado, "Pago");

                cmbMetodoPagamento.IsEnabled = false;

                cmbEstado.IsEnabled = false;

                dpDataConfirmacao.IsEnabled = false;

                txtAvisoMetodo.Text = "Este plano será registado como oferta.";

                return;
            }
            if (pagamentoJaPago)
            {
                SelecionarCombo(cmbEstado, "Pago");

                cmbEstado.IsEnabled = false;
                dpDataConfirmacao.IsEnabled = false;

                txtAvisoMetodo.Text = "Este pagamento já foi confirmado e não pode voltar a pendente.";

                return;
            }
            if (pagamento == null)
            {
                if (!transferencia)
                {
                    referenciaTransferencia = string.Empty;

                    txtReferenciaTransferencia.Text = string.Empty;
                }

                if (!paypal)
                {
                    referenciaPayPal = string.Empty;

                    txtReferenciaPayPal.Text = string.Empty;

                }

                if (!transferencia && !paypal)
                {
                    txtReferenciaExterna.Clear();
                    txtIdTransacao.Clear();
                }
            }

            if (pagamentoReembolsado)
            {
                SelecionarCombo(cmbEstado, "Reembolsado");

                cmbEstado.IsEnabled = false;
                dpDataConfirmacao.IsEnabled = false;

                txtAvisoMetodo.Text = "Este pagamento foi reembolsado e não pode ser alterado.";

                return;
            }

            if (metodo == "Dinheiro")
            {
                SelecionarCombo(cmbEstado, "Pago");

                cmbEstado.IsEnabled = false;

                dpDataConfirmacao.IsEnabled = true;
                dpDataConfirmacao.SelectedDate = pagamento?.DataConfirmacao ?? DateTime.Today;

                txtAvisoMetodo.Text = "O pagamento em dinheiro será registado imediatamente como pago.";

                return;
            }

            if (transferencia)
            {
                txtTitularBanco.Text = DadosBancarios.NomeTitular;

                txtIBAN.Text = DadosBancarios.IBAN;

                if (string.IsNullOrWhiteSpace(referenciaTransferencia))
                {
                    GerarReferenciaTransferencia();
                }

                SelecionarCombo(cmbEstado, "Pendente");

                cmbEstado.IsEnabled = false;

                dpDataConfirmacao.SelectedDate = null;
                dpDataConfirmacao.IsEnabled = false;

                txtAvisoMetodo.Text = "A transferência ficará pendente até ser confirmada pelo administrador.";

                return;
            }

            if (pagamentoPosterior)
            {
                SelecionarCombo(cmbEstado, "Pendente");

                cmbEstado.IsEnabled = false;

                dpDataConfirmacao.SelectedDate = null;
                dpDataConfirmacao.IsEnabled = false;

                txtAvisoMetodo.Text = "O pagamento ficará pendente e poderá ser confirmado posteriormente.";

                return;
            }

            if (mbway)
            {
                SelecionarCombo(cmbEstado, "Pendente");

                cmbEstado.IsEnabled = false;

                dpDataConfirmacao.SelectedDate = null;
                dpDataConfirmacao.IsEnabled = false;


                return;
            }

            cmbEstado.IsEnabled = true;
            dpDataConfirmacao.IsEnabled = true;

            if (paypal)
            {
                txtNomePayPal.Text = DadosPayPal.NomeConta;

                txtEmailPayPal.Text = DadosPayPal.Email;

                if (string.IsNullOrWhiteSpace(referenciaPayPal))
                {
                    GerarReferenciaPayPal();
                }

                SelecionarCombo(cmbEstado, "Pendente");

                cmbEstado.IsEnabled = false;

                dpDataConfirmacao.SelectedDate = null;
                dpDataConfirmacao.IsEnabled = false;

                txtAvisoMetodo.Text = "O pagamento PayPal ficará pendente até ser confirmado pelo administrador.";

                return;
            }
        }

        private void GerarReferenciaTransferencia()
        {
            if (clienteSelecionado == null) return;

            referenciaTransferencia = $"GYM-{clienteSelecionado.IdCliente}-" + $"{DateTime.Now:yyyyMMddHHmmss}";

            txtReferenciaTransferencia.Text = referenciaTransferencia;

            txtReferenciaExterna.Text = referenciaTransferencia;
        }

        private void btnCopiarIBAN_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(DadosBancarios.IBAN.Replace(" ", string.Empty));

            Mensagem.Sucesso("IBAN copiado para a área de transferência.");
        }

        private static string ObterTextoCombo(ComboBox comboBox)
        {
            return comboBox.SelectedItem is ComboBoxItem item ? item.Content?.ToString() ?? string.Empty : string.Empty;
        }

        private static void SelecionarCombo(ComboBox comboBox, string valor)
        {
            foreach (object item in comboBox.Items)
            {
                if (item is ComboBoxItem comboItem && string.Equals(comboItem.Content?.ToString(), valor, StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedItem = comboItem;

                    return;
                }
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (clienteSelecionado == null)
            {
                Mensagem.Aviso("Pesquise e selecione um cliente.");

                txtCliente.Focus();
                return;
            }

            if (cmbInscricao.SelectedItem is not InscricaoPagamento inscricaoSelecionada)
            {
                Mensagem.Aviso("Selecione a inscrição que pretende pagar.");

                cmbInscricao.Focus();
                return;
            }

            if (!dpDataPagamento.SelectedDate.HasValue)
            {
                Mensagem.Aviso("Selecione a data do pagamento.");

                dpDataPagamento.Focus();
                return;
            }

            string metodo = ObterTextoCombo(cmbMetodoPagamento);

            if (string.IsNullOrWhiteSpace(metodo))
            {
                Mensagem.Aviso("Selecione o método de pagamento.");

                cmbMetodoPagamento.Focus();
                return;
            }

            string estado = ObterTextoCombo(cmbEstado);

            if (string.IsNullOrWhiteSpace(estado))
            {
                Mensagem.Aviso("Selecione o estado.");

                cmbEstado.Focus();
                return;
            }

            bool novoPagamento = pagamento == null;

            bool pagamentoJaPago = pagamento != null && string.Equals(pagamento.Estado, "Pago", StringComparison.OrdinalIgnoreCase);

            bool pagamentoReembolsado = pagamento != null && string.Equals(pagamento.Estado, "Reembolsado", StringComparison.OrdinalIgnoreCase);

            bool oferta = inscricaoSelecionada.Preco == 0;


            if (oferta)
            {
                metodo = "Oferta";
                estado = "Pago";
            }
            else if (pagamentoJaPago)
            {
                estado = "Pago";
            }
            else if (pagamentoReembolsado)
            {
                estado = "Reembolsado";
            }
            else if (metodo == "Dinheiro")
            {
                estado = "Pago";
            }
            else if (metodo == "Transferência Bancária")
            {
                estado = "Pendente";

                if (string.IsNullOrWhiteSpace(referenciaTransferencia))
                {
                    GerarReferenciaTransferencia();
                }
            }
            else if (metodo == "Pagamento Posterior" || metodo == "MB WAY" || metodo == "PayPal")
            {
                estado = "Pendente";
            }

            DateTime? dataConfirmacao = estado == "Pago" ? dpDataConfirmacao.SelectedDate : null;

            if (estado == "Pago" && !dataConfirmacao.HasValue)
            {
                dataConfirmacao = pagamento?.DataConfirmacao ?? DateTime.Now;
            }


            if (oferta)
            {
                if (!Mensagem.Confirmar("Este plano tem o valor de 0,00 €.\n\n" +
                        $"Cliente: {clienteSelecionado.Nome}\n" +
                        $"Plano: {inscricaoSelecionada.NomePlano}\n\n" +
                        "Tem a certeza que pretende dar esta oferta?"))
                {
                    return;
                }
            }
            else
            {
                string operacao = novoPagamento ? "registar" : "atualizar";

                if (!Mensagem.Confirmar($"Tem a certeza que pretende {operacao} este pagamento?"))
                {
                    return;
                }
            }

            Pagamento dados = new Pagamento
            {
                IdPagamento = pagamento?.IdPagamento ?? 0,

                IdCliente = clienteSelecionado.IdCliente,

                IdInscricao = inscricaoSelecionada.IdInscricao,

                DataPagamento = dpDataPagamento.SelectedDate.Value.Date,

                Valor = inscricaoSelecionada.Preco,

                MetodoPagamento = metodo,

                Estado = estado,

                ReferenciaExterna = metodo == "Transferência Bancária" ? referenciaTransferencia : metodo == "PayPal" ? referenciaPayPal : string.Empty,

                IdTransacaoExterna = metodo == "PayPal" ? txtIdTransacao.Text.Trim() : string.Empty,

                DataConfirmacao = dataConfirmacao,

                Observacoes = txtObservacoes.Text.Trim()
            };

            try
            {
                if (novoPagamento)
                {
                    pagamentoService.Inserir(dados);

                    if (oferta)
                    {
                        Mensagem.Sucesso("Oferta registada com sucesso!");
                    }
                    else
                    {
                        Mensagem.Sucesso("Pagamento registado com sucesso!");
                    }
                }
                else
                {
                    pagamentoService.Atualizar(dados);

                    Mensagem.Sucesso("Pagamento atualizado com sucesso!");
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível guardar o pagamento.\n\n" + ex.Message);
            }
        }

        private void btnPagarPayPal_Click(object sender, RoutedEventArgs e)
        {

            Clipboard.SetText(DadosPayPal.Email);

            Mensagem.Sucesso("Email PayPal copiado para a área de transferência.");
        }


        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AplicarRegrasEstadoEdicao()
        {
            if (pagamento == null)
                return;
            if (pagamento.Valor == 0)
            {
                SelecionarCombo(cmbMetodoPagamento, "Oferta");

                SelecionarCombo(cmbEstado, "Pago");

                cmbMetodoPagamento.IsEnabled = false;

                cmbEstado.IsEnabled = false;

                txtAvisoMetodo.Text = "Este plano foi atribuído como oferta.";

                return;
            }
            if (string.Equals(pagamento.Estado, "Pago", StringComparison.OrdinalIgnoreCase))
            {

                SelecionarCombo(cmbEstado, "Pago");

                cmbEstado.IsEnabled = false;

                txtAvisoMetodo.Text = "Este pagamento já foi confirmado e não pode voltar a pendente.";

                return;
            }

            if (string.Equals(pagamento.Estado, "Reembolsado", StringComparison.OrdinalIgnoreCase))
            {
                SelecionarCombo(cmbEstado, "Reembolsado");

                cmbEstado.IsEnabled = false;

                btnGuardar.IsEnabled = false;

                txtAvisoMetodo.Text = "Este pagamento foi reembolsado e já não pode ser alterado.";
            }
        }
        private void GerarReferenciaPayPal()
        {
            if (clienteSelecionado == null)
                return;

            referenciaPayPal = $"PAYPAL-{clienteSelecionado.IdCliente}-" + $"{DateTime.Now:yyyyMMddHHmmss}";

            txtReferenciaPayPal.Text = referenciaPayPal;

            txtReferenciaExterna.Text = referenciaPayPal;
        }

    }


}

