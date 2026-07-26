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
    /// Interaction logic for ClienteForm.xaml
    /// </summary>
    public partial class ClienteForm : Window
    {

        private Cliente cliente;

public ClienteForm(Cliente cliente)
{
    InitializeComponent();

    this.cliente = cliente;

            if (cliente != null)
            {
                txtNome.Text = cliente.Nome;
                txtNIF.Text = cliente.NIF;
                dpNascimento.SelectedDate = cliente.DataNascimento;
                txtTelefone.Text = cliente.Telefone;
                txtEmail.Text = cliente.Email;
                txtMorada.Text = cliente.Morada;
                dpInscricao.SelectedDate = cliente.DataInscricao;
                chkEstado.IsChecked = cliente.Estado;
            }
}

        private readonly ClienteService service = new ClienteService();

        public ClienteForm() : this(null)
        {
            InitializeComponent();

            dpInscricao.SelectedDate = DateTime.Today;
            chkEstado.IsChecked = true;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!Validacoes.CampoObrigatorio(txtNome.Text))
            {
                Mensagem.Aviso("O nome é obrigatório.");
                txtNome.Focus();
                return;
            }

            if (!Validacoes.NIF(txtNIF.Text))
            {
                Mensagem.Aviso("O NIF deve conter 9 dígitos.");
                txtNIF.Focus();
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

            if (!Mensagem.Confirmar("Tem a certeza que pretende gravar este cliente?"))
                return;

            int id = cliente == null ? 0 : cliente.IdCliente;

            if (service.ExisteNIF(txtNIF.Text, id))
            {
                Mensagem.Aviso("Já existe um cliente com esse NIF.");
                txtNIF.Focus();
                return;
            }


            if (!Validacoes.MaiorOuIgual14Anos(dpNascimento.SelectedDate))
            {
                Mensagem.Aviso("O cliente deve ter pelo menos 14 anos.");
                dpNascimento.Focus();
                return;
            }

            Cliente novoCliente = new Cliente
            {
                Nome = txtNome.Text,
                NIF = txtNIF.Text,
                DataNascimento = dpNascimento.SelectedDate ?? DateTime.Today,
                Telefone = txtTelefone.Text,
                Email = txtEmail.Text,
                Morada = txtMorada.Text,
                DataInscricao = dpInscricao.SelectedDate ?? DateTime.Today,
                Estado = chkEstado.IsChecked ?? false
            };

            if (cliente == null)
            {
                service.Inserir(novoCliente);
                Mensagem.Sucesso("Cliente registado com sucesso!");
            }
            else
            {
                novoCliente.IdCliente = cliente.IdCliente;
                service.Atualizar(novoCliente);
                Mensagem.Sucesso("Cliente atualizado com sucesso!");
            }

            DialogResult = true;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
