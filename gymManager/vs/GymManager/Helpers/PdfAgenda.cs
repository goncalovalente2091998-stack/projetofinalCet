using GymManager.Models;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
namespace GymManager.Helpers
{
    public static class PdfAgenda
    {

        public static void Gerar(string caminho, DateTime data, List<EventoAgenda> eventos)
        {
            using PdfHelper pdf = new PdfHelper(caminho);

            pdf.Titulo("GYM MANAGER");
            pdf.SubTitulo("AGENDA");
            pdf.Linha();

            pdf.Campo("Data", data.ToString("dd/MM/yyyy"));
            pdf.Campo("Total de Eventos", eventos.Count.ToString());

            pdf.Espaco();
            pdf.SubTitulo("EVENTOS");

            var tabela = pdf.CriarTabela(2, 4, 3, 3);

            pdf.AdicionarCabecalhoTabela(
                tabela,
                "Hora",
                "Evento",
                "Cliente / Aula",
                "Estado"
                );

            foreach (EventoAgenda evento in eventos.OrderBy(e => e.DataInicio))
            {
                string descricao = evento.EhAula? $"{evento.Titulo} ({evento.Localizacao})": evento.Titulo;

                string clienteOuAula = evento.EhAula? evento.NomeAula: evento.NomeCliente;

                pdf.AdicionarLinhaTabela(
                    tabela,
                    evento.HorarioFormatado,
                    descricao,
                    string.IsNullOrWhiteSpace(clienteOuAula) ? "-": clienteOuAula,
                    evento.Estado
                    );
            }

            pdf.AdicionarTabela(tabela);

            pdf.Espaco();

            pdf.Texto("Emitido em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

            pdf.Fechar();
        }
    }

}