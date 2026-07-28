CREATE OR ALTER PROCEDURE dbo.sp_Dashboard_Resumo
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        (
            SELECT COUNT(*)
            FROM dbo.Clientes
            WHERE Estado = 1
        ) AS ClientesAtivos,

        (
            SELECT COUNT(*)
            FROM dbo.Inscricoes
            WHERE Estado = N'Ativa'
              AND DataFim >= CAST(GETDATE() AS DATE)
        ) AS InscricoesAtivas,

        (
            SELECT COUNT(*)
            FROM dbo.Pagamentos
            WHERE Estado = N'Pendente'
        ) AS PagamentosPendentes,

        (
            SELECT ISNULL(SUM(Valor), 0)
            FROM dbo.Pagamentos
            WHERE Estado = N'Pago'
              AND YEAR(ISNULL(DataConfirmacao, DataPagamento))
                    = YEAR(GETDATE())
              AND MONTH(ISNULL(DataConfirmacao, DataPagamento))
                    = MONTH(GETDATE())
        ) AS ReceitaMes,

        (
            SELECT ISNULL(SUM(Valor), 0)
            FROM dbo.Pagamentos
            WHERE Estado = N'Pago'
              AND YEAR(ISNULL(DataConfirmacao, DataPagamento))
                    = YEAR(GETDATE())
        ) AS ReceitaAno,

        (
            SELECT ISNULL(SUM(Valor), 0)
            FROM dbo.Pagamentos
            WHERE Estado = N'Pago'
        ) AS ReceitaTotal,

        (
            SELECT COUNT(*)
            FROM dbo.Inscricoes
            WHERE Estado = N'Ativa'
              AND DataFim >= CAST(GETDATE() AS DATE)
              AND DataFim <= DATEADD(
                    DAY,
                    7,
                    CAST(GETDATE() AS DATE)
                  )
        ) AS InscricoesATerminar,

        (
            SELECT COUNT(*)
            FROM dbo.Aulas
            WHERE DataAula = CAST(GETDATE() AS DATE)
              AND Estado <> N'Cancelada'
        ) AS AulasHoje,

        (
            SELECT COUNT(*)
            FROM dbo.ReservasAulas AS R
            INNER JOIN dbo.Aulas AS A
                ON A.IdAula = R.IdAula
            WHERE A.DataAula = CAST(GETDATE() AS DATE)
              AND R.Estado IN
              (
                  N'Confirmada',
                  N'Presente'
              )
        ) AS ReservasHoje;
END;
GO
CREATE OR ALTER PROCEDURE sp_Dashboard_UltimosPagamentos
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 5
        PG.IdPagamento,
        C.Nome AS NomeCliente,
        PL.Nome AS NomePlano,
        PG.DataPagamento,
        PG.Valor,
        PG.MetodoPagamento,
        PG.Estado
    FROM Pagamentos AS PG

    INNER JOIN Clientes AS C
        ON C.IdCliente = PG.IdCliente

    LEFT JOIN Inscricoes AS I
        ON I.IdInscricao = PG.IdInscricao

    LEFT JOIN Planos AS PL
        ON PL.IdPlano = I.IdPlano

    ORDER BY
        PG.DataPagamento DESC,
        PG.IdPagamento DESC;
END;
GO
CREATE OR ALTER PROCEDURE sp_Dashboard_InscricoesATerminar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        I.IdInscricao,
        C.Nome AS NomeCliente,
        P.Nome AS NomePlano,
        I.DataFim,
        DATEDIFF(
            DAY,
            CAST(GETDATE() AS DATE),
            I.DataFim
        ) AS DiasRestantes
    FROM Inscricoes AS I

    INNER JOIN Clientes AS C
        ON C.IdCliente = I.IdCliente

    INNER JOIN Planos AS P
        ON P.IdPlano = I.IdPlano

    WHERE I.Estado = 'Ativa'
      AND I.DataFim >= CAST(GETDATE() AS DATE)
      AND I.DataFim <= DATEADD(
            DAY,
            7,
            CAST(GETDATE() AS DATE)
          )

    ORDER BY I.DataFim ASC;
END;
GO
CREATE OR ALTER PROCEDURE sp_Dashboard_ReceitaMensal
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH Meses AS
    (
        SELECT 0 AS Numero
        UNION ALL
        SELECT Numero + 1
        FROM Meses
        WHERE Numero < 11
    )
    SELECT
        YEAR(DATEADD(MONTH, -M.Numero, GETDATE())) AS Ano,
        MONTH(DATEADD(MONTH, -M.Numero, GETDATE())) AS Mes,
        DATENAME(
            MONTH,
            DATEADD(MONTH, -M.Numero, GETDATE())
        ) AS NomeMes,
        ISNULL
        (
            (
                SELECT SUM(P.Valor)
                FROM Pagamentos AS P
                WHERE P.Estado = 'Pago'
                  AND YEAR(P.DataConfirmacao) =
                      YEAR(DATEADD(MONTH, -M.Numero, GETDATE()))
                  AND MONTH(P.DataConfirmacao) =
                      MONTH(DATEADD(MONTH, -M.Numero, GETDATE()))
            ),
            0
        ) AS Receita
    FROM Meses AS M
    ORDER BY
        Ano,
        Mes
    OPTION (MAXRECURSION 12);
END;
GO