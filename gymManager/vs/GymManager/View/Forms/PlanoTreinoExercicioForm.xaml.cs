using GymManager.Helpers;
using GymManager.Models;
using GymManager.Models.GymManager.Models;
using GymManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GymManager.View.Forms
{
    public partial class PlanoTreinoExercicioForm : Window
    {
        private readonly PlanoTreino plano;

        private readonly PlanoTreinoExercicioService service = new PlanoTreinoExercicioService();

        private readonly ExercicioService exercicioService = new ExercicioService();

        private readonly PlanoTreinoExercicio? item;

        private List<Exercicio> exercicios = new List<Exercicio>();

        public PlanoTreinoExercicioForm(PlanoTreino planoSelecionado)
        {
            InitializeComponent();

            plano = planoSelecionado;

            txtSubtitulo.Text = $"Plano: {plano.NomePlano}";

            CarregarExercicios();

            txtSeries.Text = "3";
            txtRepeticoes.Text = "10";
            txtTempoDescanso.Text = "60";

            DefinirProximaOrdem();
        }

        public PlanoTreinoExercicioForm(PlanoTreino planoSelecionado, PlanoTreinoExercicio itemSelecionado)
        {
            InitializeComponent();

            plano = planoSelecionado;

            item = itemSelecionado;

            Title = "Editar Exercício do Plano";

            txtTitulo.Text = "Editar Exercício";

            txtSubtitulo.Text = $"Plano: {plano.NomePlano}";

            CarregarExercicios();

            cmbExercicio.SelectedValue = item.IdExercicio;

            txtSeries.Text = item.Series.ToString();

            txtRepeticoes.Text = item.Repeticoes.ToString();

            txtTempoDescanso.Text = item.TempoDescanso.ToString();

            txtOrdem.Text = item.Ordem.ToString();

            txtObservacoes.Text = item.Observacoes;
        }

        private void CarregarExercicios()
        {
            try
            {
                exercicios = exercicioService.Listar().Where(e =>
                string.Equals(e.Estado, "Ativo", StringComparison.OrdinalIgnoreCase)).ToList();

                cmbExercicio.ItemsSource = exercicios;
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível carregar os exercícios.\n\n" + ex.Message);
            }
        }

        private void DefinirProximaOrdem()
        {
            try
            {
                List<PlanoTreinoExercicio> lista = service.ListarPorPlano(plano.IdPlanoTreino);

                int proximaOrdem = lista.Count == 0 ? 1 : lista.Max(x => x.Ordem) + 1;

                txtOrdem.Text = proximaOrdem.ToString();
            }
            catch
            {
                txtOrdem.Text = "1";
            }
        }

        private void txtNumero_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbExercicio.SelectedValue is not int idExercicio)
            {
                Mensagem.Aviso("Selecione um exercício.");

                cmbExercicio.Focus();
                return;
            }

            if (!int.TryParse(txtSeries.Text.Trim(), out int series) || series <= 0)
            {
                Mensagem.Aviso("Introduza um número de séries válido.");

                txtSeries.Focus();
                return;
            }

            if (!int.TryParse(txtRepeticoes.Text.Trim(), out int repeticoes) || repeticoes <= 0)
            {
                Mensagem.Aviso("Introduza um número de repetições válido.");

                txtRepeticoes.Focus();
                return;
            }

            if (!int.TryParse(txtTempoDescanso.Text.Trim(), out int tempoDescanso) || tempoDescanso < 0)
            {
                Mensagem.Aviso("Introduza um tempo de descanso válido.");

                txtTempoDescanso.Focus();
                return;
            }

            if (!int.TryParse(txtOrdem.Text.Trim(), out int ordem) || ordem <= 0)
            {
                Mensagem.Aviso("Introduza uma ordem válida.");

                txtOrdem.Focus();
                return;
            }

            bool novoItem = item == null;

            string nomeExercicio = cmbExercicio.SelectedItem is Exercicio exercicio ? exercicio.Nome : string.Empty;

            if (!Mensagem.Confirmar(
                    $"Pretende {(novoItem ? "adicionar" : "atualizar")} este exercício?\n\n" +
                    $"Exercício: {nomeExercicio}\n" +
                    $"Séries: {series}\n" +
                    $"Repetições: {repeticoes}\n" +
                    $"Descanso: {tempoDescanso} segundos\n" + $"Ordem: {ordem}"))
            {
                return;
            }

            PlanoTreinoExercicio dados = new PlanoTreinoExercicio
            {
                IdPlanoTreinoExercicio = item?.IdPlanoTreinoExercicio ?? 0,

                IdPlanoTreino = plano.IdPlanoTreino,

                IdExercicio = idExercicio,

                Series = series,

                Repeticoes = repeticoes,

                TempoDescanso = tempoDescanso,

                Ordem = ordem,

                Observacoes = txtObservacoes.Text.Trim()
            };

            try
            {
                if (novoItem)
                {
                    service.Inserir(dados);

                    Mensagem.Sucesso("Exercício adicionado ao plano com sucesso!");
                }
                else
                {
                    service.Atualizar(dados);

                    Mensagem.Sucesso("Exercício do plano atualizado com sucesso!");
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                Mensagem.Erro("Não foi possível guardar o exercício do plano.\n\n" + ex.Message);
            }
        }
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}