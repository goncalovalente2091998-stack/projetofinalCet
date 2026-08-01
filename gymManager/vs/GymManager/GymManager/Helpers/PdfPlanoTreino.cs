using GymManager.Models;
using GymManager.Models.GymManager.Models;
using GymManager.Services;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymManager.Helpers
{
    public static class PdfPlanoTreino
    {
        public static void Gerar(
            string caminho,
            PlanoTreino plano)
        {
            PlanoTreinoExercicioService exercicioService =
                new PlanoTreinoExercicioService();

            List<PlanoTreinoExercicio> exercicios =
                exercicioService
                    .ListarPorPlano(plano.IdPlanoTreino)
                    .OrderBy(e => e.Ordem)
                    .ToList();

            using PdfHelper pdf =
                new PdfHelper(caminho);

            pdf.Titulo("GYM MANAGER");

            pdf.SubTitulo("PLANO DE TREINO");

            pdf.Linha();

            pdf.Campo(
                "Plano",
                plano.NomePlano);

            pdf.Campo(
                "Cliente",
                plano.NomeCliente);

            pdf.Campo(
                "Personal Trainer",
                plano.NomePT);

            pdf.Campo(
                "Objetivo",
                plano.Objetivo);

            pdf.Campo(
                "Período",
                plano.PeriodoFormatado);

            pdf.Campo(
                "Estado",
                plano.Estado);

            pdf.Espaco(15);

            pdf.SubTitulo("EXERCÍCIOS");

            if (exercicios.Count == 0)
            {
                pdf.Texto(
                    "Não existem exercícios associados a este plano.");
            }
            else
            {
                Table tabela =
                    pdf.CriarTabela(
                        4,
                        2,
                        3,
                        2,
                        2,
                        2);

                pdf.AdicionarCabecalhoTabela(
                    tabela,
                    "Exercício",
                    "Grupo",
                    "Equipamento",
                    "Séries",
                    "Reps",
                    "Descanso");

                foreach (PlanoTreinoExercicio item in exercicios)
                {
                    string equipamento =
                        string.IsNullOrWhiteSpace(
                            item.Equipamento)
                            ? "-"
                            : item.Equipamento;

                    pdf.AdicionarLinhaTabela(
                        tabela,
                        item.NomeExercicio,
                        item.GrupoMuscular,
                        equipamento,
                        item.Series.ToString(),
                        item.Repeticoes.ToString(),
                        item.DescansoFormatado);
                }

                pdf.AdicionarTabela(
                    tabela);
            }

            if (!string.IsNullOrWhiteSpace(
                    plano.Observacoes))
            {
                pdf.Espaco(15);

                pdf.SubTitulo(
                    "OBSERVAÇÕES");

                pdf.Texto(
                    plano.Observacoes);
            }

            pdf.Espaco(10);

            pdf.Linha();

            pdf.Texto(
                "Emitido em " +
                DateTime.Now.ToString(
                    "dd/MM/yyyy HH:mm"));

            pdf.Fechar();
        }
    }
}