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
    /// <summary>
    /// Interaction logic for ProfessoresForm.xaml
    /// </summary>
    public partial class ProfessoresForm : Window
    {
        private readonly ProfessoresService service = new ProfessoresService();
        private readonly Professor? professor;
        public ProfessoresForm()
        {
            InitializeComponent();
        }


        public ProfessoresForm(Professor professor)
        {
            InitializeComponent();
            this.professor = professor;

            Title = "Editar Professor";
            txtTitulo.Text = "Editar Professor";

            txtNome.Text = professor.Nome;
            txtEspecialidade.Text = professor.Especialidade;
            txtTelefone.Text = professor.Telefone;
            txtEmail.Text = professor.Email;
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

            string operacao = professor == null ? "registar" : "atualizar";

            if (!Mensagem.Confirmar($"Tem a certeza que pretende {operacao} este professor?"))
                return;

            Professor dadosProfessor = new Professor
            {
                IdProfessor = professor?.IdProfessor ?? 0,
                Nome = FormatarTexto.Nome(txtNome.Text),
                Especialidade = txtEspecialidade.Text.Trim(),
                Telefone = txtTelefone.Text.Trim(),
                Email = txtEmail.Text.Trim()
            };

            try
            {
                if (professor == null)
                {
                    service.Inserir(dadosProfessor);
                    Mensagem.Sucesso("Professor registado com sucesso!");
                }
                else
                {
                    service.Atualizar(dadosProfessor);
                    Mensagem.Sucesso("Professor atualizado com sucesso!");
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Ocorreu um erro ao guardar o professor.\n\n" + ex.Message);
            }
        }

     
        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

      
    }
}

