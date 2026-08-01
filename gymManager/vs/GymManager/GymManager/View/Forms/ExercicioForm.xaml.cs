using GymManager.Helpers;
using GymManager.Models;
using GymManager.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GymManager.View.Forms
{
    public partial class ExercicioForm : Window
    {
        private readonly ExercicioService service =
            new ExercicioService();

        private readonly Exercicio? exercicio;

        public ExercicioForm()
        {
            InitializeComponent();

            cmbDificuldade.SelectedIndex = 1;
            cmbEstado.SelectedIndex = 0;
        }

        public ExercicioForm(
            Exercicio exercicio)
        {
            InitializeComponent();

            this.exercicio =
                exercicio;

            Title =
                "Editar Exercício";

            txtTitulo.Text =
                "Editar Exercício";

            txtNome.Text =
                exercicio.Nome;

            txtEquipamento.Text =
                exercicio.Equipamento;

            txtDescricao.Text =
                exercicio.Descricao;

            SelecionarCombo(
                cmbGrupoMuscular,
                exercicio.GrupoMuscular);

            SelecionarCombo(
                cmbDificuldade,
                exercicio.Dificuldade);

            SelecionarCombo(
                cmbEstado,
                exercicio.Estado);
        }

        private static void SelecionarCombo(
            ComboBox comboBox,
            string valor)
        {
            foreach (object item in comboBox.Items)
            {
                if (item is ComboBoxItem comboItem &&
                    string.Equals(
                        comboItem.Content?.ToString(),
                        valor,
                        StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedItem =
                        comboItem;

                    return;
                }
            }

            comboBox.SelectedIndex =
                -1;
        }

        private static string ObterTextoCombo(
            ComboBox comboBox)
        {
            if (comboBox.SelectedItem is
                ComboBoxItem item)
            {
                return item.Content?.ToString()
                       ?? string.Empty;
            }

            return string.Empty;
        }

        private void btnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            string nome =
                txtNome.Text.Trim();

            if (string.IsNullOrWhiteSpace(nome))
            {
                Mensagem.Aviso(
                    "Introduza o nome do exercício.");

                txtNome.Focus();
                return;
            }

            string grupoMuscular =
                ObterTextoCombo(
                    cmbGrupoMuscular);

            if (string.IsNullOrWhiteSpace(
                    grupoMuscular))
            {
                Mensagem.Aviso(
                    "Selecione o grupo muscular.");

                cmbGrupoMuscular.Focus();
                return;
            }

            string dificuldade =
                ObterTextoCombo(
                    cmbDificuldade);

            if (string.IsNullOrWhiteSpace(
                    dificuldade))
            {
                Mensagem.Aviso(
                    "Selecione a dificuldade.");

                cmbDificuldade.Focus();
                return;
            }

            string estado =
                ObterTextoCombo(
                    cmbEstado);

            if (string.IsNullOrWhiteSpace(
                    estado))
            {
                Mensagem.Aviso(
                    "Selecione o estado.");

                cmbEstado.Focus();
                return;
            }

            string equipamento =
                txtEquipamento.Text.Trim();

            string descricao =
                txtDescricao.Text.Trim();

            bool novoExercicio =
                exercicio == null;

            string operacao =
                novoExercicio
                    ? "registar"
                    : "atualizar";

            if (!Mensagem.Confirmar(
                    $"Tem a certeza que pretende {operacao} este exercício?\n\n" +
                    $"Nome: {nome}\n" +
                    $"Grupo muscular: {grupoMuscular}\n" +
                    $"Dificuldade: {dificuldade}\n" +
                    $"Estado: {estado}"))
            {
                return;
            }

            Exercicio dados =
                new Exercicio
                {
                    IdExercicio =
                        exercicio?.IdExercicio ?? 0,

                    Nome =
                        nome,

                    GrupoMuscular =
                        grupoMuscular,

                    Equipamento =
                        equipamento,

                    Descricao =
                        descricao,

                    Dificuldade =
                        dificuldade,

                    Estado =
                        estado
                };

            try
            {
                if (novoExercicio)
                {
                    service.Inserir(
                        dados);

                    Mensagem.Sucesso(
                        "Exercício registado com sucesso!");
                }
                else
                {
                    service.Atualizar(
                        dados);

                    Mensagem.Sucesso(
                        "Exercício atualizado com sucesso!");
                }

                DialogResult =
                    true;
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível guardar o exercício.\n\n" +
                    ex.Message);
            }
        }

        private void btnCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}