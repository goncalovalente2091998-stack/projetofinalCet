using GymManager.Helpers;
using GymManager.Models;
using GymManager.Services;
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
using System.Windows.Shapes;

namespace GymManager.View.Forms
{
    /// <summary>
    /// Interaction logic for UtilizadorForm.xaml
    /// </summary>
    public partial class UtilizadorForm : Window
    {
        private readonly UtilizadorService service =
             new UtilizadorService();

        private readonly Utilizador? utilizador;

        private string hashAtual = string.Empty;

        public UtilizadorForm()
        {
            InitializeComponent();

            cmbPerfil.SelectedIndex = 1;
        }

        public UtilizadorForm(Utilizador utilizador)
        {
            InitializeComponent();

            this.utilizador = utilizador;

            txtTitulo.Text = "Editar Utilizador";
            Title = "Editar Utilizador";

            txtNome.Text = utilizador.Nome;
            txtEmail.Text = utilizador.Email;

            SelecionarPerfil(utilizador.Perfil);

            txtAjudaPassword.Visibility =
                Visibility.Visible;

            Utilizador? dadosCompletos =
                service.ObterPorId(utilizador.IdUtilizador);

            if (dadosCompletos != null)
            {
                hashAtual =
                    dadosCompletos.PasswordHash;
            }

            if (utilizador.IdUtilizador ==
                Sessao.IdUtilizador)
            {
                cmbPerfil.IsEnabled = false;
            }
        }

        private void SelecionarPerfil(string perfil)
        {
            foreach (object item in cmbPerfil.Items)
            {
                if (item is ComboBoxItem comboItem &&
                    string.Equals(
                        comboItem.Content?.ToString(),
                        perfil,
                        StringComparison.OrdinalIgnoreCase))
                {
                    cmbPerfil.SelectedItem = comboItem;
                    break;
                }
            }
        }

        private void btnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            string nome = txtNome.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;
            string confirmarPassword =
                txtConfirmarPassword.Password;

            if (!Validacoes.CampoObrigatorio(nome))
            {
                Mensagem.Aviso(
                    "O nome é obrigatório.");

                txtNome.Focus();
                return;
            }

            if (!Validacoes.Email(email))
            {
                Mensagem.Aviso(
                    "Introduza um email válido.");

                txtEmail.Focus();
                return;
            }

            if (cmbPerfil.SelectedItem is not
                ComboBoxItem perfilSelecionado)
            {
                Mensagem.Aviso(
                    "Selecione um perfil.");

                cmbPerfil.Focus();
                return;
            }

            bool novoUtilizador =
                utilizador == null;

            if (novoUtilizador &&
                string.IsNullOrWhiteSpace(password))
            {
                Mensagem.Aviso(
                    "A palavra-passe é obrigatória.");

                txtPassword.Focus();
                return;
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                if (password.Length < 8)
                {
                    Mensagem.Aviso(
                        "A palavra-passe deve ter pelo menos 8 caracteres.");

                    txtPassword.Focus();
                    return;
                }

                if (password != confirmarPassword)
                {
                    Mensagem.Aviso(
                        "As palavras-passe não coincidem.");

                    txtConfirmarPassword.Focus();
                    return;
                }
            }

            string operacao =
                novoUtilizador
                    ? "criar"
                    : "atualizar";

            if (!Mensagem.Confirmar(
                $"Tem a certeza que pretende {operacao} este utilizador?"))
            {
                return;
            }

            string passwordHash;

            if (novoUtilizador ||
                !string.IsNullOrWhiteSpace(password))
            {
                passwordHash =
                    passwordHelper.CriarHash(password);
            }
            else
            {
                passwordHash = hashAtual;
            }

            Utilizador dados =
                new Utilizador
                {
                    IdUtilizador =
                        utilizador?.IdUtilizador ?? 0,

                    Nome = nome,
                    Email = email,

                    PasswordHash =
                        passwordHash,

                    Perfil =
                        perfilSelecionado
                            .Content?
                            .ToString()
                        ?? string.Empty
                };

            try
            {
                if (novoUtilizador)
                {
                    service.Inserir(dados);

                    Mensagem.Sucesso(
                        "Utilizador criado com sucesso!");
                }
                else
                {
                    service.Atualizar(dados);

                    if (dados.IdUtilizador ==
                        Sessao.IdUtilizador)
                    {
                        Sessao.Nome = dados.Nome;
                    }

                    Mensagem.Sucesso(
                        "Utilizador atualizado com sucesso!");
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível guardar o utilizador.\n\n" +
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

