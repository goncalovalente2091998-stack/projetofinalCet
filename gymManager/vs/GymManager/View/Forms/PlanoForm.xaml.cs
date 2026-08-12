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
    public partial class PlanoForm : Window
    {
        private readonly PlanoService service = new PlanoService();

        private readonly Plano? plano;

        public PlanoForm()
        {
            InitializeComponent();
        }

        public PlanoForm(Plano plano)
        {
            InitializeComponent();

            this.plano = plano;

            Title = "Editar Plano";
            txtTitulo.Text = "Editar Plano";

            txtNome.Text = plano.Nome;
            txtPreco.Text = plano.Preco.ToString("F2");
            txtDuracaoMeses.Text = plano.DuracaoMeses.ToString();
            txtDescricao.Text = plano.Descricao ?? string.Empty;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string nome = txtNome.Text.Trim();
            string descricao = txtDescricao.Text.Trim();

            if (!Validacoes.CampoObrigatorio(nome))
            {
                Mensagem.Aviso("O nome do plano é obrigatório.");
                txtNome.Focus();
                return;
            }

            string precoTexto = txtPreco.Text.Trim().Replace(',', '.');

            if (!decimal.TryParse(precoTexto, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal preco) || preco < 0)
            {
                Mensagem.Aviso("Introduza um preço válido.");
                txtPreco.Focus();
                return;
            }

            if (!int.TryParse(txtDuracaoMeses.Text.Trim(), out int duracaoMeses) || duracaoMeses <= 0)
            {
                Mensagem.Aviso("Introduza uma duração válida em meses.");

                txtDuracaoMeses.Focus();
                return;
            }

            bool novoPlano = plano == null;

            string operacao = novoPlano ? "criar" : "atualizar";

            if (!Mensagem.Confirmar($"Tem a certeza que pretende {operacao} este plano?"))
            {
                return;
            }

            Plano dados = new Plano
            {
                IdPlano = plano?.IdPlano ?? 0,
                Nome = nome,
                Preco = preco,
                DuracaoMeses = duracaoMeses,
                Descricao = descricao
            };

            try
            {
                if (novoPlano)
                {
                    service.Inserir(dados); Mensagem.Sucesso("Plano criado com sucesso!");
                }
                else
                {
                    service.Atualizar(dados);
                    Mensagem.Sucesso("Plano atualizado com sucesso!");
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível guardar o plano.\n\n" + ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

