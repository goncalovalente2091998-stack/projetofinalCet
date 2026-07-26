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

namespace GymManager.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly UtilizadorService service =
             new UtilizadorService();

        public LoginWindow()
        {
      
            InitializeComponent();
        }

        private void btnEntrar_Click(
            object sender,
            RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(email))
            {
                Mensagem.Aviso("Introduza o email.");
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                Mensagem.Aviso("Introduza a palavra-passe.");
                txtPassword.Focus();
                return;
            }

            try
            {
                Utilizador? utilizador =
                    service.ObterPorEmail(email);

                bool credenciaisValidas =
                    utilizador != null &&
                    passwordHelper.Verificar(
                        password,
                        utilizador.PasswordHash);

                if (!credenciaisValidas)
                {
                    Mensagem.Aviso(
                        "Email ou palavra-passe inválidos.");

                    txtPassword.Clear();
                    txtPassword.Focus();
                    return;
                }

                Sessao.IdUtilizador =
                    utilizador!.IdUtilizador;

                Sessao.Nome =
                    utilizador.Nome;

                Sessao.Perfil =
                    utilizador.Perfil;

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                Close();
            }
            catch (Exception ex)
            {
                Mensagem.Erro(
                    "Não foi possível iniciar sessão.\n\n" +
                    ex.Message);
            }
        }
    }
}

