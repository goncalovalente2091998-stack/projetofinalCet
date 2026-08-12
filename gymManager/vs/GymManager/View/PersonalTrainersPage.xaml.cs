using GymManager.Helpers;
using GymManager.Services;
using GymManager.View.Forms;
using System;
using System.Collections.Generic;
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
using GymManager.View.Forms;
using GymManager.Models;

namespace GymManager.View
{
    public partial class PersonalTrainersPage : Page
    {
        private readonly PersonalTrainerService service = new PersonalTrainerService();

        private PersonalTrainer? personalTrainerSelecionado;

        public PersonalTrainersPage()
        {
            InitializeComponent();
            CarregarPersonalTrainers();
        }


        private void CarregarPersonalTrainers()
        {
            try
            {
                List<PersonalTrainer> lista = service.Listar();

                icPersonalTrainers.ItemsSource = lista;
                txtTotalPT.Text = lista.Count.ToString();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível carregar os Personal Trainers.\n\n" + ex.Message);
            }
        }

        private void btnNovo_Click(object sender, RoutedEventArgs e)
        {
            PersonalTrainerForm form = new PersonalTrainerForm
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarPersonalTrainers();
            }
        }

        private void btnEditarCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button botao || botao.Tag is not PersonalTrainer personalTrainer)
            {
                Mensagem.Aviso("Não foi possível identificar o Personal Trainer.");
                return;
            }

            PersonalTrainerForm form = new PersonalTrainerForm(personalTrainer)
            {
                Owner = Window.GetWindow(this)
            };

            if (form.ShowDialog() == true)
            {
                CarregarPersonalTrainers();
            }
        }

        private void btnEliminarCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button botao || botao.Tag is not PersonalTrainer personalTrainer)
            {
                Mensagem.Aviso("Não foi possível identificar o Personal Trainer.");
                return;
            }

            if (!Mensagem.Confirmar($"Tem a certeza que pretende eliminar o Personal Trainer " + $"'{personalTrainer.Nome}'?"))
            {
                return;
            }

            try
            {
                service.Eliminar(personalTrainer.IdPT);

                Mensagem.Sucesso("Personal Trainer eliminado com sucesso!");

                CarregarPersonalTrainers();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível eliminar o Personal Trainer.\n\n" + ex.Message);
            }
        }

        private void txtPesquisar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            try
            {
                string pesquisa = txtPesquisar.Text.Trim();

                List<PersonalTrainer> lista = string.IsNullOrWhiteSpace(pesquisa) ? service.Listar() : service.Pesquisar(pesquisa);

                icPersonalTrainers.ItemsSource = lista;
                txtTotalPT.Text = lista.Count.ToString();
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível pesquisar os Personal Trainers.\n\n" + ex.Message);
            }
        }
    }
}

