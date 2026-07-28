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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace GymManager.View
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        private readonly DashboardService service =
            new DashboardService();

        private readonly CultureInfo culturaPortugal =
            new CultureInfo("pt-PT");

        public DashboardPage()
        {
            InitializeComponent();

            CarregarDashboard();
        }

        private void CarregarDashboard()
        {
            try
            {
                DashboardResumo resumo =
                    service.ObterResumo();

                txtClientesAtivos.Text =
                    resumo.ClientesAtivos.ToString();

                txtInscricoesAtivas.Text =
                    resumo.InscricoesAtivas.ToString();

                txtPagamentosPendentes.Text =
                    resumo.PagamentosPendentes.ToString();

                txtInscricoesATerminar.Text =
                    resumo.InscricoesATerminar.ToString();

                txtReceitaMes.Text =
     resumo.ReceitaMes.ToString(
         "C2",
         culturaPortugal);

                txtReceitaAno.Text =
                    resumo.ReceitaAno.ToString(
                        "C2",
                        culturaPortugal);

                txtReceitaTotal.Text =
                    resumo.ReceitaTotal.ToString(
                        "C2",
                        culturaPortugal);

                dgUltimosPagamentos.ItemsSource =
                    service.ListarUltimosPagamentos();

                dgInscricoesATerminar.ItemsSource =
                    service.ListarInscricoesATerminar();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível carregar o dashboard.\n\n" +
                    ex.Message);
            }
        }

    }
}
