using GymManager.Models;
using iText.Layout.Element;
using System;

namespace GymManager.Helpers
{
    public static class PdfPagamento
    {
        public
static void Gerar(string caminho, Pagamento pagamento)
        {
            using
PdfHelper pdf = new PdfHelper(caminho);

            pdf.Titulo("GYM MANAGER");
            pdf.SubTitulo("RECIBO DE PAGAMENTO");
            pdf.Linha();

            pdf.Campo("Cliente", pagamento.NomeCliente);
            pdf.Campo("Plano", pagamento.ReferenciaInscricao);
            pdf.Campo("Data", pagamento.DataPagamento.ToString("dd/MM/yyyy"));
            pdf.Campo("Valor", pagamento.Valor.ToString("C2"));
            pdf.Campo("Método", pagamento.MetodoPagamento);
            pdf.Campo("Estado", pagamento.Estado);

            if (!string.IsNullOrWhiteSpace(pagamento.ReferenciaExterna))
                pdf.Campo("Referência", pagamento.ReferenciaExterna);

            if (!string.IsNullOrWhiteSpace(pagamento.IdTransacaoExterna))
                pdf.Campo("Transação", pagamento.IdTransacaoExterna);

            if (pagamento.DataConfirmacao.HasValue)
                pdf.Campo(
                    "Confirmado em",
                    pagamento.DataConfirmacao.Value.ToString("dd/MM/yyyy HH:mm"));

            if (!string.IsNullOrWhiteSpace(pagamento.Observacoes))
            {
                pdf.Espaco();
                pdf.SubTitulo("OBSERVAÇÕES");
                pdf.Texto(pagamento.Observacoes);
            }

            pdf.Espaco();
            pdf.Linha();
            pdf.Texto("Emitido em " +
                DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

            pdf.Fechar();
        }
    }

}