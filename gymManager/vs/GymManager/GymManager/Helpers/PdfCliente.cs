using GymManager.Models;
using System;

namespace GymManager.Helpers
{
    public static class PdfCliente
    {
        public static void Gerar(
            string caminho,
            Cliente cliente)
        {
            using PdfHelper pdf =
                new PdfHelper(caminho);

            pdf.Titulo("GYM MANAGER");

            pdf.SubTitulo("FICHA DO CLIENTE");

            pdf.Linha();

            pdf.Campo(
                "Nome",
                cliente.Nome);

            pdf.Campo(
                "NIF",
                cliente.NIF);

            pdf.Campo(
                "Data de Nascimento",
                cliente.DataNascimento.ToString("dd/MM/yyyy"));

            pdf.Campo(
                "Telefone",
                cliente.Telefone);

            pdf.Campo(
                "Email",
                cliente.Email);

            pdf.Campo(
                "Morada",
                cliente.Morada);

            pdf.Campo(
                "Data de Inscrição",
                cliente.DataInscricao.ToString("dd/MM/yyyy"));

            pdf.Campo(
                "Estado",
                cliente.Estado
                    ? "Ativo"
                    : "Inativo");

            pdf.Espaco(20);

            pdf.Linha();

            pdf.Texto(
                "Documento emitido em " +
                DateTime.Now.ToString(
                    "dd/MM/yyyy HH:mm"));

            pdf.Fechar();
        }
    }
}