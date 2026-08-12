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
    public partial class PersonalTrainerForm : Window
    {
        private readonly PersonalTrainerService service = new PersonalTrainerService();

        private readonly PersonalTrainer? personalTrainer;

        public PersonalTrainerForm()
        {
            InitializeComponent();
            chkEstado.IsChecked = true;
        }

        public PersonalTrainerForm(PersonalTrainer personalTrainer)
        {
            InitializeComponent();

            this.personalTrainer = personalTrainer;

            Title = "Editar Personal Trainer";

            txtNome.Text = personalTrainer.Nome;
            txtEspecialidade.Text = personalTrainer.Especialidade;
            txtTelefone.Text = personalTrainer.Telefone;
            txtEmail.Text = personalTrainer.Email;
            txtValorHora.Text = personalTrainer.ValorHora.ToString("F2");
            chkEstado.IsChecked = personalTrainer.Estado;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!Validacoes.CampoObrigatorio(txtNome.Text))
            {
                Mensagem.Aviso("O nome é obrigatório.");
                txtNome.Focus();
                return;
            }

            if (!Validacoes.CampoObrigatorio(txtEspecialidade.Text))
            {
                Mensagem.Aviso("A especialidade é obrigatória.");
                txtEspecialidade.Focus();
                return;
            }

            if (!Validacoes.Telefone(txtTelefone.Text))
            {
                Mensagem.Aviso("Telefone inválido.");
                txtTelefone.Focus();
                return;
            }

            if (!Validacoes.Email(txtEmail.Text))
            {
                Mensagem.Aviso("Email inválido.");
                txtEmail.Focus();
                return;
            }

            string valorTexto = txtValorHora.Text.Trim().Replace(',', '.');

            if (!decimal.TryParse(valorTexto, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal valorHora) || valorHora <= 0)
            {
                Mensagem.Aviso("Introduza um valor por hora válido.");
                txtValorHora.Focus();
                return;
            }

            int idAtual = personalTrainer?.IdPT ?? 0;

            if (service.ExisteEmail(txtEmail.Text.Trim(), idAtual))
            {
                Mensagem.Aviso("Já existe outro Personal Trainer com este email.");
                txtEmail.Focus();
                return;
            }

            string operacao = personalTrainer == null ? "registar" : "atualizar";

            if (!Mensagem.Confirmar($"Tem a certeza que pretende {operacao} este Personal Trainer?"))
            {
                return;
            }

            PersonalTrainer dados = new PersonalTrainer
            {
                IdPT = idAtual,
                Nome = FormatarTexto.Nome(txtNome.Text),
                Especialidade = txtEspecialidade.Text.Trim(),
                Telefone = txtTelefone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                ValorHora = valorHora,
                Estado = chkEstado.IsChecked ?? false
            };

            try
            {
                if (personalTrainer == null)
                {
                    service.Inserir(dados);
                    Mensagem.Sucesso("Personal Trainer registado com sucesso!");
                }
                else
                {
                    service.Atualizar(dados);
                    Mensagem.Sucesso("Personal Trainer atualizado com sucesso!");
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível guardar o Personal Trainer.\n\n" + ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

