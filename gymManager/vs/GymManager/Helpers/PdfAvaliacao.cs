using GymManager.Models;
using System;

namespace GymManager.Helpers
{
    public static class PdfAvaliacao
    {
        public static void Gerar(string caminho, AvaliacaoFisica avaliacao)
        {
            using PdfHelper pdf = new PdfHelper(caminho);

            pdf.Titulo("GYM MANAGER");
            pdf.SubTitulo("AVALIAÇÃO FÍSICA");
            pdf.Linha();

            pdf.Campo("Cliente", avaliacao.NomeCliente);
            pdf.Campo("Personal Trainer", avaliacao.NomePT);
            pdf.Campo("Data", avaliacao.DataAvaliacao.ToString("dd/MM/yyyy"));
            pdf.Campo("Estado", avaliacao.Estado);

            pdf.Espaco();

            pdf.SubTitulo("MEDIDAS");

            pdf.Campo("Peso", avaliacao.PesoFormatado);
            pdf.Campo("Altura", avaliacao.AlturaFormatada);
            pdf.Campo("IMC", avaliacao.IMCFormatado);
            pdf.Campo("Classificação IMC", avaliacao.ClassificacaoIMC);
            pdf.Campo("Massa Gorda", avaliacao.MassaGordaFormatada);
            pdf.Campo("Massa Muscular", avaliacao.MassaMuscularFormatada);

            if (!string.IsNullOrWhiteSpace(avaliacao.Observacoes))
            {
                pdf.Espaco();
                pdf.SubTitulo("OBSERVAÇÕES");
                pdf.Texto(avaliacao.Observacoes);
            }

            pdf.Espaco();
            pdf.Linha();
            pdf.Texto("Emitido em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

            pdf.Fechar();
        }
    }

}