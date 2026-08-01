using GymManager.Models;
using GymManager.Models.GymManager.Models;
using GymManager.Services;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GymManager.Helpers
{
    public static class PdfDossier
    {
        public static void Gerar(
            string caminho,
            Cliente cliente)
        {
            CultureInfo culturaPortugal =
                new CultureInfo("pt-PT");

            InscricaoService inscricaoService =
                new InscricaoService();

            PagamentoService pagamentoService =
                new PagamentoService();

            AvaliacaoFisicaService avaliacaoService =
                new AvaliacaoFisicaService();

            PlanoTreinoService planoService =
                new PlanoTreinoService();

            PlanoTreinoExercicioService exercicioService =
                new PlanoTreinoExercicioService();

            EventoAgendaService eventoService =
                new EventoAgendaService();

            List<Inscricao> inscricoes =
                inscricaoService
                    .Listar()
                    .Where(i =>
                        i.IdCliente ==
                        cliente.IdCliente)
                    .OrderByDescending(i =>
                        i.DataInicio)
                    .ToList();

            List<Pagamento> pagamentos =
                pagamentoService
                    .Listar()
                    .Where(p =>
                        p.IdCliente ==
                        cliente.IdCliente)
                    .OrderByDescending(p =>
                        p.DataPagamento)
                    .ToList();

            List<AvaliacaoFisica> avaliacoes =
                avaliacaoService
                    .Listar()
                    .Where(a =>
                        a.IdCliente ==
                        cliente.IdCliente)
                    .OrderByDescending(a =>
                        a.DataAvaliacao)
                    .ToList();

            List<PlanoTreino> planos =
                planoService
                    .Listar()
                    .Where(p =>
                        p.IdCliente ==
                        cliente.IdCliente)
                    .OrderByDescending(p =>
                        p.DataInicio)
                    .ToList();

            List<EventoAgenda> eventos =
    eventoService
        .ListarPorPeriodo(
            new DateTime(2000, 1, 1),
            new DateTime(2100, 1, 1),
            null,
            null)
        .Where(e =>
            e.IdCliente.HasValue
            &&
            e.IdCliente.Value ==
            cliente.IdCliente)
        .OrderByDescending(e =>
            e.DataInicio)
        .ToList();

            using PdfHelper pdf =
                new PdfHelper(caminho);

            AdicionarCabecalho(
                pdf,
                cliente);

            AdicionarDadosCliente(
                pdf,
                cliente);

            AdicionarResumo(
                pdf,
                cliente,
                inscricoes,
                pagamentos,
                avaliacoes,
                planos,
                eventos,
                culturaPortugal);

            AdicionarInscricoes(
                pdf,
                inscricoes,
                culturaPortugal);

            AdicionarPagamentos(
                pdf,
                pagamentos,
                culturaPortugal);

            AdicionarAvaliacoes(
                pdf,
                avaliacoes);

            AdicionarPlanosTreino(
                pdf,
                planos,
                exercicioService);

            AdicionarAgenda(
                pdf,
                eventos);

            pdf.Espaco(15);

            pdf.Linha();

            pdf.Texto(
                "Dossier emitido em " +
                DateTime.Now.ToString(
                    "dd/MM/yyyy HH:mm"));

            pdf.Fechar();
        }

        private static void AdicionarCabecalho(
            PdfHelper pdf,
            Cliente cliente)
        {
            pdf.Titulo(
                "GYM MANAGER");

            pdf.SubTitulo(
                "DOSSIER COMPLETO DO CLIENTE");

            pdf.Linha();

            pdf.Campo(
                "Cliente",
                ValorOuTraco(cliente.Nome));

            pdf.Campo(
                "NIF",
                ValorOuTraco(cliente.NIF));

            pdf.Campo(
                "Estado",
                cliente.Estado
                    ? "Ativo"
                    : "Inativo");

            pdf.Campo(
                "Data de emissão",
                DateTime.Now.ToString(
                    "dd/MM/yyyy HH:mm"));

            pdf.Espaco(15);
        }

        private static void AdicionarDadosCliente(
            PdfHelper pdf,
            Cliente cliente)
        {
            pdf.SubTitulo(
                "DADOS DO CLIENTE");

            pdf.Campo(
                "Nome",
                ValorOuTraco(cliente.Nome));

            pdf.Campo(
                "NIF",
                ValorOuTraco(cliente.NIF));

            pdf.Campo(
                "Data de nascimento",
                cliente.DataNascimento.ToString(
                    "dd/MM/yyyy"));

            pdf.Campo(
                "Telefone",
                ValorOuTraco(cliente.Telefone));

            pdf.Campo(
                "Email",
                ValorOuTraco(cliente.Email));

            pdf.Campo(
                "Morada",
                ValorOuTraco(cliente.Morada));

            pdf.Campo(
                "Data de inscrição",
                cliente.DataInscricao.ToString(
                    "dd/MM/yyyy"));

            pdf.Campo(
                "Estado",
                cliente.Estado
                    ? "Ativo"
                    : "Inativo");

            pdf.Espaco(15);
        }

        private static void AdicionarResumo(
            PdfHelper pdf,
            Cliente cliente,
            List<Inscricao> inscricoes,
            List<Pagamento> pagamentos,
            List<AvaliacaoFisica> avaliacoes,
            List<PlanoTreino> planos,
            List<EventoAgenda> eventos,
            CultureInfo cultura)
        {
            pdf.SubTitulo(
                "RESUMO");

            decimal totalPago =
                pagamentos
                    .Where(p =>
                        string.Equals(
                            p.Estado,
                            "Pago",
                            StringComparison.OrdinalIgnoreCase))
                    .Sum(p =>
                        p.Valor);

            int inscricoesAtivas =
                inscricoes.Count(i =>
                    string.Equals(
                        i.Estado,
                        "Ativa",
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    string.Equals(
                        i.Estado,
                        "Ativo",
                        StringComparison.OrdinalIgnoreCase));

            int planosAtivos =
                planos.Count(p =>
                    string.Equals(
                        p.Estado,
                        "Ativo",
                        StringComparison.OrdinalIgnoreCase));

            pdf.Campo(
                "Cliente desde",
                cliente.DataInscricao.ToString(
                    "dd/MM/yyyy"));

            pdf.Campo(
                "Total de inscrições",
                inscricoes.Count.ToString());

            pdf.Campo(
                "Inscrições ativas",
                inscricoesAtivas.ToString());

            pdf.Campo(
                "Total de pagamentos",
                pagamentos.Count.ToString());

            pdf.Campo(
                "Valor total pago",
                totalPago.ToString(
                    "C2",
                    cultura));

            pdf.Campo(
                "Total de avaliações",
                avaliacoes.Count.ToString());

            pdf.Campo(
                "Total de planos de treino",
                planos.Count.ToString());

            pdf.Campo(
                "Planos ativos",
                planosAtivos.ToString());

            pdf.Campo(
                "Sessões na agenda",
                eventos.Count.ToString());

            pdf.Espaco(15);
        }

        private static void AdicionarInscricoes(
            PdfHelper pdf,
            List<Inscricao> inscricoes,
            CultureInfo cultura)
        {
            pdf.SubTitulo(
                "INSCRIÇÕES");

            if (inscricoes.Count == 0)
            {
                pdf.Texto(
                    "Não existem inscrições associadas a este cliente.");

                pdf.Espaco(15);

                return;
            }

            Table tabela =
                pdf.CriarTabela(
                    3,
                    2,
                    2,
                    2);

            pdf.AdicionarCabecalhoTabela(
                tabela,
                "Plano",
                "Início",
                "Fim",
                "Estado");

            foreach (Inscricao inscricao in inscricoes)
            {
                pdf.AdicionarLinhaTabela(
                    tabela,
                    ValorOuTraco(
                        inscricao.NomePlano),

                    inscricao.DataInicio.ToString(
                        "dd/MM/yyyy",
                        cultura),

                    inscricao.DataFim.ToString(
                        "dd/MM/yyyy",
                        cultura),

                    ValorOuTraco(
                        inscricao.Estado));
            }

            pdf.AdicionarTabela(
                tabela);

            pdf.Espaco(15);
        }

        private static void AdicionarPagamentos(
            PdfHelper pdf,
            List<Pagamento> pagamentos,
            CultureInfo cultura)
        {
            pdf.SubTitulo(
                "PAGAMENTOS");

            if (pagamentos.Count == 0)
            {
                pdf.Texto(
                    "Não existem pagamentos associados a este cliente.");

                pdf.Espaco(15);

                return;
            }

            Table tabela =
                pdf.CriarTabela(
                    2,
                    3,
                    2,
                    2,
                    2);

            pdf.AdicionarCabecalhoTabela(
                tabela,
                "Data",
                "Plano",
                "Valor",
                "Método",
                "Estado");

            foreach (Pagamento pagamento in pagamentos)
            {
                pdf.AdicionarLinhaTabela(
                    tabela,

                    pagamento.DataPagamento.ToString(
                        "dd/MM/yyyy"),

                    ValorOuTraco(
                        pagamento.NomePlano),

                    pagamento.Valor.ToString(
                        "C2",
                        cultura),

                    ValorOuTraco(
                        pagamento.MetodoPagamento),

                    ValorOuTraco(
                        pagamento.Estado));
            }

            pdf.AdicionarTabela(
                tabela);

            pdf.Espaco(15);
        }

        private static void AdicionarAvaliacoes(
            PdfHelper pdf,
            List<AvaliacaoFisica> avaliacoes)
        {
            pdf.SubTitulo(
                "AVALIAÇÕES FÍSICAS");

            if (avaliacoes.Count == 0)
            {
                pdf.Texto(
                    "Não existem avaliações físicas associadas a este cliente.");

                pdf.Espaco(15);

                return;
            }

            foreach (AvaliacaoFisica avaliacao in avaliacoes)
            {
                pdf.Campo(
                    "Data",
                    avaliacao.DataAvaliacao.ToString(
                        "dd/MM/yyyy"));

                pdf.Campo(
                    "Personal Trainer",
                    ValorOuTraco(
                        avaliacao.NomePT));

                pdf.Campo(
                    "Peso",
                    avaliacao.PesoFormatado);

                pdf.Campo(
                    "Altura",
                    avaliacao.AlturaFormatada);

                pdf.Campo(
                    "IMC",
                    avaliacao.IMCFormatado);

                pdf.Campo(
                    "Classificação",
                    avaliacao.ClassificacaoIMC);

                pdf.Campo(
                    "Massa gorda",
                    avaliacao.MassaGordaFormatada);

                pdf.Campo(
                    "Massa muscular",
                    avaliacao.MassaMuscularFormatada);

                pdf.Campo(
                    "Estado",
                    ValorOuTraco(
                        avaliacao.Estado));

                if (!string.IsNullOrWhiteSpace(
                        avaliacao.Observacoes))
                {
                    pdf.Campo(
                        "Observações",
                        avaliacao.Observacoes);
                }

                pdf.Linha();

                pdf.Espaco(10);
            }

            pdf.Espaco(5);
        }

        private static void AdicionarPlanosTreino(
            PdfHelper pdf,
            List<PlanoTreino> planos,
            PlanoTreinoExercicioService exercicioService)
        {
            pdf.SubTitulo(
                "PLANOS DE TREINO");

            if (planos.Count == 0)
            {
                pdf.Texto(
                    "Não existem planos de treino associados a este cliente.");

                pdf.Espaco(15);

                return;
            }

            foreach (PlanoTreino plano in planos)
            {
                pdf.Campo(
                    "Plano",
                    ValorOuTraco(
                        plano.NomePlano));

                pdf.Campo(
                    "Personal Trainer",
                    ValorOuTraco(
                        plano.NomePT));

                pdf.Campo(
                    "Objetivo",
                    ValorOuTraco(
                        plano.Objetivo));

                pdf.Campo(
                    "Período",
                    plano.PeriodoFormatado);

                pdf.Campo(
                    "Estado",
                    ValorOuTraco(
                        plano.Estado));

                if (!string.IsNullOrWhiteSpace(
                        plano.Observacoes))
                {
                    pdf.Campo(
                        "Observações",
                        plano.Observacoes);
                }

                List<PlanoTreinoExercicio> exercicios =
                    exercicioService
                        .ListarPorPlano(
                            plano.IdPlanoTreino)
                        .OrderBy(e =>
                            e.Ordem)
                        .ToList();

                pdf.Espaco(8);

                if (exercicios.Count == 0)
                {
                    pdf.Texto(
                        "Este plano não possui exercícios.");

                    pdf.Linha();

                    pdf.Espaco(12);

                    continue;
                }

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

                foreach (
                    PlanoTreinoExercicio exercicio
                    in exercicios)
                {
                    pdf.AdicionarLinhaTabela(
                        tabela,

                        ValorOuTraco(
                            exercicio.NomeExercicio),

                        ValorOuTraco(
                            exercicio.GrupoMuscular),

                        ValorOuTraco(
                            exercicio.Equipamento),

                        exercicio.Series.ToString(),

                        exercicio.Repeticoes.ToString(),

                        exercicio.DescansoFormatado);
                }

                pdf.AdicionarTabela(
                    tabela);

                pdf.Espaco(10);

                pdf.Linha();

                pdf.Espaco(12);
            }
        }

        private static void AdicionarAgenda(
            PdfHelper pdf,
            List<EventoAgenda> eventos)
        {
            pdf.SubTitulo(
                "AGENDA DO CLIENTE");

            if (eventos.Count == 0)
            {
                pdf.Texto(
                    "Não existem sessões de agenda associadas a este cliente.");

                pdf.Espaco(15);

                return;
            }

            Table tabela =
                pdf.CriarTabela(
                    2,
                    2,
                    3,
                    2,
                    2);

            pdf.AdicionarCabecalhoTabela(
                tabela,
                "Data",
                "Hora",
                "Evento",
                "Responsável",
                "Estado");

            foreach (EventoAgenda evento in eventos)
            {
                pdf.AdicionarLinhaTabela(
                    tabela,

                    evento.DataInicio.ToString(
                        "dd/MM/yyyy"),

                    evento.HorarioFormatado,

                    ValorOuTraco(
                        evento.Titulo),

                    ValorOuTraco(
                        evento.NomePT),

                    ValorOuTraco(
                        evento.Estado));
            }

            pdf.AdicionarTabela(
                tabela);

            pdf.Espaco(15);
        }

        private static string ValorOuTraco(
            string? valor)
        {
            return string.IsNullOrWhiteSpace(
                    valor)
                ? "-"
                : valor.Trim();
        }
    }
}