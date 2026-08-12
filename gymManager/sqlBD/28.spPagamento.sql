CREATE OR ALTER PROCEDURE dbo.sp_Pagamentos_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PG.IdPagamento,
        PG.IdCliente,
        C.Nome AS NomeCliente,

        PG.IdInscricao,
        PL.Nome AS NomePlano,

        I.DataInicio,
        I.DataFim,

        PG.DataPagamento,
        PG.Valor,
        PG.MetodoPagamento,
        PG.Observacoes,
        PG.Estado,
        PG.ReferenciaExterna,
        PG.IdTransacaoExterna,
        PG.DataConfirmacao

    FROM dbo.Pagamentos AS PG

    INNER JOIN dbo.Clientes AS C
        ON C.IdCliente = PG.IdCliente

    LEFT JOIN dbo.Inscricoes AS I
        ON I.IdInscricao = PG.IdInscricao

    LEFT JOIN dbo.Planos AS PL
        ON PL.IdPlano = I.IdPlano

    ORDER BY
        PG.DataPagamento DESC,
        PG.IdPagamento DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_Pagamentos_Inserir
(
    @IdCliente INT,
    @IdInscricao INT,
    @DataPagamento DATE,
    @Valor DECIMAL(10,2),
    @MetodoPagamento NVARCHAR(50),
    @Observacoes NVARCHAR(255),
    @Estado NVARCHAR(30),
    @ReferenciaExterna NVARCHAR(150),
    @IdTransacaoExterna NVARCHAR(150),
    @DataConfirmacao DATETIME2
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Clientes
        WHERE IdCliente = @IdCliente
    )
    BEGIN
        RAISERROR(
            'O cliente selecionado não existe.',
            16,
            1
        );

        RETURN;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Inscricoes
        WHERE IdInscricao = @IdInscricao
          AND IdCliente = @IdCliente
    )
    BEGIN
        RAISERROR(
            'A inscrição selecionada não pertence ao cliente.',
            16,
            1
        );

        RETURN;
    END;
IF @Valor < 0
BEGIN
    RAISERROR(
        'O valor não pode ser negativo.',
        16,
        1
    );

    RETURN;
END;

    IF @Estado NOT IN
    (
        'Pendente',
        'Pago',
        'Falhado',
        'Reembolsado'
    )
    BEGIN
        RAISERROR(
            'O estado do pagamento não é válido.',
            16,
            1
        );

        RETURN;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM Pagamentos
        WHERE IdInscricao = @IdInscricao
          AND Estado IN ('Pago', 'Pendente')
    )
    BEGIN
        RAISERROR(
            'Já existe um pagamento pago ou pendente para esta inscrição.',
            16,
            1
        );

        RETURN;
    END;

    IF @Estado = 'Pago'
       AND @DataConfirmacao IS NULL
    BEGIN
        SET @DataConfirmacao = SYSDATETIME();
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO Pagamentos
        (
            IdCliente,
            IdInscricao,
            DataPagamento,
            Valor,
            MetodoPagamento,
            Observacoes,
            Estado,
            ReferenciaExterna,
            IdTransacaoExterna,
            DataConfirmacao
        )
        VALUES
        (
            @IdCliente,
            @IdInscricao,
            @DataPagamento,
            @Valor,
            @MetodoPagamento,
            NULLIF(@Observacoes, ''),
            @Estado,
            NULLIF(@ReferenciaExterna, ''),
            NULLIF(@IdTransacaoExterna, ''),
            @DataConfirmacao
        );

        IF @Estado = 'Pago'
        BEGIN
            UPDATE Inscricoes
            SET Estado = 'Ativa'
            WHERE IdInscricao = @IdInscricao
              AND Estado = 'Pendente';
        END;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE sp_Pagamentos_Atualizar
(
    @IdPagamento INT,
    @IdCliente INT,
    @IdInscricao INT,
    @DataPagamento DATE,
    @Valor DECIMAL(10,2),
    @MetodoPagamento NVARCHAR(50),
    @Observacoes NVARCHAR(255),
    @Estado NVARCHAR(30),
    @ReferenciaExterna NVARCHAR(150),
    @IdTransacaoExterna NVARCHAR(150),
    @DataConfirmacao DATETIME2
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoAtual NVARCHAR(30);

    -- Obter o estado atual
    SELECT
        @EstadoAtual = Estado
    FROM Pagamentos
    WHERE IdPagamento = @IdPagamento;

    -- Validar existência do pagamento
    IF @EstadoAtual IS NULL
    BEGIN
        RAISERROR(
            'O pagamento indicado não existe.',
            16,
            1
        );

        RETURN;
    END;

    -- Validar cliente
    IF NOT EXISTS
    (
        SELECT 1
        FROM Clientes
        WHERE IdCliente = @IdCliente
    )
    BEGIN
        RAISERROR(
            'O cliente selecionado não existe.',
            16,
            1
        );

        RETURN;
    END;

    -- Validar inscrição e cliente
    IF NOT EXISTS
    (
        SELECT 1
        FROM Inscricoes
        WHERE IdInscricao = @IdInscricao
          AND IdCliente = @IdCliente
    )
    BEGIN
        RAISERROR(
            'A inscrição selecionada não pertence ao cliente.',
            16,
            1
        );

        RETURN;
    END;

    -- Validar valor
    IF @Valor < 0
    BEGIN
        RAISERROR(
            'O valor não pode ser negativo',
            16,
            1
        );

        RETURN;
    END;

    -- Validar estado recebido
    IF @Estado NOT IN
    (
        'Pendente',
        'Pago',
        'Falhado',
        'Reembolsado'
    )
    BEGIN
        RAISERROR(
            'O estado do pagamento não é válido.',
            16,
            1
        );

        RETURN;
    END;

    /*
        TRANSIÇÕES PERMITIDAS

        Pendente    -> Pendente, Pago ou Falhado
        Falhado     -> Falhado, Pendente ou Pago
        Pago        -> Pago ou Reembolsado
        Reembolsado -> Reembolsado
    */

    -- Pago não pode voltar a pendente ou falhado
    IF @EstadoAtual = 'Pago'
       AND @Estado NOT IN ('Pago', 'Reembolsado')
    BEGIN
        RAISERROR(
            'Um pagamento pago apenas pode permanecer pago ou ser reembolsado.',
            16,
            1
        );

        RETURN;
    END;

    -- Reembolsado é um estado final
    IF @EstadoAtual = 'Reembolsado'
       AND @Estado <> 'Reembolsado'
    BEGIN
        RAISERROR(
            'Um pagamento reembolsado não pode mudar de estado.',
            16,
            1
        );

        RETURN;
    END;

    -- Impedir outro pagamento pago ou pendente para a inscrição
    IF @Estado IN ('Pago', 'Pendente')
       AND EXISTS
       (
           SELECT 1
           FROM Pagamentos
           WHERE IdInscricao = @IdInscricao
             AND IdPagamento <> @IdPagamento
             AND Estado IN ('Pago', 'Pendente')
       )
    BEGIN
        RAISERROR(
            'Já existe outro pagamento pago ou pendente para esta inscrição.',
            16,
            1
        );

        RETURN;
    END;

    -- Se passar para Pago e não tiver data, usar a data atual
    IF @Estado = 'Pago'
       AND @DataConfirmacao IS NULL
    BEGIN
        SET @DataConfirmacao = SYSDATETIME();
    END;

    -- Estados não pagos não devem ter data de confirmação
    IF @Estado IN ('Pendente', 'Falhado')
    BEGIN
        SET @DataConfirmacao = NULL;
    END;

    UPDATE Pagamentos
    SET
        IdCliente = @IdCliente,
        IdInscricao = @IdInscricao,
        DataPagamento = @DataPagamento,
        Valor = @Valor,
        MetodoPagamento = @MetodoPagamento,
        Observacoes = NULLIF(@Observacoes, ''),
        Estado = @Estado,
        ReferenciaExterna =
            NULLIF(@ReferenciaExterna, ''),
        IdTransacaoExterna =
            NULLIF(@IdTransacaoExterna, ''),
        DataConfirmacao = @DataConfirmacao
    WHERE IdPagamento = @IdPagamento;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_Pagamentos_Eliminar
(
    @IdPagamento INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Pagamentos
        WHERE IdPagamento = @IdPagamento
    )
    BEGIN
        THROW 50001,
              'O pagamento indicado não existe.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Pagamentos
        WHERE IdPagamento = @IdPagamento
          AND Estado IN
          (
              N'Pago',
              N'Reembolsado'
          )
    )
    BEGIN
        THROW 50002,
              'Pagamentos pagos ou reembolsados não podem ser eliminados.',
              1;
    END;

    DELETE FROM dbo.Pagamentos
    WHERE IdPagamento = @IdPagamento;
END;
GO


CREATE OR ALTER PROCEDURE dbo.sp_Pagamentos_Pesquisar
(
    @Pesquisa NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Pesquisa =
        LTRIM(RTRIM(@Pesquisa));

    SELECT
        PG.IdPagamento,
        PG.IdCliente,
        C.Nome AS NomeCliente,

        PG.IdInscricao,
        PL.Nome AS NomePlano,

        I.DataInicio,
        I.DataFim,

        PG.DataPagamento,
        PG.Valor,
        PG.MetodoPagamento,
        PG.Observacoes,
        PG.Estado,
        PG.ReferenciaExterna,
        PG.IdTransacaoExterna,
        PG.DataConfirmacao

    FROM dbo.Pagamentos AS PG

    INNER JOIN dbo.Clientes AS C
        ON C.IdCliente = PG.IdCliente

    LEFT JOIN dbo.Inscricoes AS I
        ON I.IdInscricao = PG.IdInscricao

    LEFT JOIN dbo.Planos AS PL
        ON PL.IdPlano = I.IdPlano

    WHERE
        C.Nome LIKE
            N'%' + @Pesquisa + N'%'

        OR C.NIF LIKE
            N'%' + @Pesquisa + N'%'

        OR PL.Nome LIKE
            N'%' + @Pesquisa + N'%'

        OR PG.MetodoPagamento LIKE
            N'%' + @Pesquisa + N'%'

        OR PG.Estado LIKE
            N'%' + @Pesquisa + N'%'

        OR PG.ReferenciaExterna LIKE
            N'%' + @Pesquisa + N'%'

        OR PG.IdTransacaoExterna LIKE
            N'%' + @Pesquisa + N'%'

        OR PG.Observacoes LIKE
            N'%' + @Pesquisa + N'%'

        OR CAST(
               PG.Valor AS NVARCHAR(20)
           ) LIKE
            N'%' + @Pesquisa + N'%'

        OR CONVERT(
               NVARCHAR(10),
               PG.DataPagamento,
               103
           ) LIKE
            N'%' + @Pesquisa + N'%'

        OR CONVERT(
               NVARCHAR(10),
               I.DataInicio,
               103
           ) LIKE
            N'%' + @Pesquisa + N'%'

        OR CONVERT(
               NVARCHAR(10),
               I.DataFim,
               103
           ) LIKE
            N'%' + @Pesquisa + N'%'

        OR
        (
            PL.Nome + N' ' +
            CONVERT(
                NVARCHAR(10),
                I.DataInicio,
                103
            ) + N' ' +
            CONVERT(
                NVARCHAR(10),
                I.DataFim,
                103
            )
        ) LIKE
            N'%' + @Pesquisa + N'%'

        OR
        (
            RIGHT(
                N'0' +
                CAST(
                    MONTH(PG.DataPagamento)
                    AS NVARCHAR(2)
                ),
                2
            )
            + N'/' +
            CAST(
                YEAR(PG.DataPagamento)
                AS NVARCHAR(4)
            )
        ) LIKE
            N'%' + @Pesquisa + N'%'

        OR CAST(
               YEAR(PG.DataPagamento)
               AS NVARCHAR(4)
           ) LIKE
            N'%' + @Pesquisa + N'%'

    ORDER BY
        PG.DataPagamento DESC,
        PG.IdPagamento DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_Pagamentos_Confirmar
(
    @IdPagamento INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdInscricao INT;
    DECLARE @EstadoAtual NVARCHAR(30);

    SELECT
        @IdInscricao = IdInscricao,
        @EstadoAtual = Estado
    FROM Pagamentos
    WHERE IdPagamento = @IdPagamento;

    IF @EstadoAtual IS NULL
    BEGIN
        RAISERROR(
            'O pagamento indicado não existe.',
            16,
            1
        );

        RETURN;
    END;

    IF @IdInscricao IS NULL
    BEGIN
        RAISERROR(
            'O pagamento não possui uma inscrição associada.',
            16,
            1
        );

        RETURN;
    END;

    IF @EstadoAtual <> 'Pendente'
    BEGIN
        RAISERROR(
            'Apenas pagamentos pendentes podem ser confirmados.',
            16,
            1
        );

        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE Pagamentos
        SET
            Estado = 'Pago',
            DataConfirmacao = SYSDATETIME()
        WHERE IdPagamento = @IdPagamento;

        UPDATE Inscricoes
        SET Estado = 'Ativa'
        WHERE IdInscricao = @IdInscricao
          AND Estado = 'Pendente';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        THROW;
    END CATCH;
END;
GO


CREATE OR ALTER PROCEDURE dbo.sp_Pagamentos_ObterPorId
(
    @IdPagamento INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PG.IdPagamento,
        PG.IdCliente,
        C.Nome AS NomeCliente,

        PG.IdInscricao,
        PL.Nome AS NomePlano,

        I.DataInicio,
        I.DataFim,

        PG.DataPagamento,
        PG.Valor,
        PG.MetodoPagamento,
        PG.Observacoes,
        PG.Estado,
        PG.ReferenciaExterna,
        PG.IdTransacaoExterna,
        PG.DataConfirmacao

    FROM dbo.Pagamentos AS PG

    INNER JOIN dbo.Clientes AS C
        ON C.IdCliente = PG.IdCliente

    LEFT JOIN dbo.Inscricoes AS I
        ON I.IdInscricao = PG.IdInscricao

    LEFT JOIN dbo.Planos AS PL
        ON PL.IdPlano = I.IdPlano

    WHERE PG.IdPagamento =
          @IdPagamento;
END;
GO

CREATE OR ALTER PROCEDURE sp_Pagamentos_Reembolsar
(
    @IdPagamento INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Pagamentos
        WHERE IdPagamento = @IdPagamento
    )
    BEGIN
        RAISERROR(
            'O pagamento indicado não existe.',
            16,
            1
        );

        RETURN;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Pagamentos
        WHERE IdPagamento = @IdPagamento
          AND Estado = 'Pago'
    )
    BEGIN
        RAISERROR(
            'Apenas pagamentos pagos podem ser reembolsados.',
            16,
            1
        );

        RETURN;
    END;

    UPDATE Pagamentos
    SET Estado = 'Reembolsado'
    WHERE IdPagamento = @IdPagamento;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Pagamentos_AtivarInscricao
ON dbo.Pagamentos
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Se o valor for 0, o pagamento passa automaticamente para Pago
    UPDATE P
    SET
        P.MetodoPagamento = N'Oferta',
        P.Estado = N'Pago',
        P.DataConfirmacao =
            COALESCE(
                P.DataConfirmacao,
                SYSDATETIME()
            ),
        P.ReferenciaExterna = NULL,
        P.IdTransacaoExterna = NULL
    FROM dbo.Pagamentos AS P
    INNER JOIN inserted AS N
        ON N.IdPagamento = P.IdPagamento
    WHERE
        P.Valor = 0;

    -- Se o pagamento estiver Pago, ativa a inscrição
    UPDATE I
    SET
        I.Estado = N'Ativa'
    FROM dbo.Inscricoes AS I
    INNER JOIN inserted AS N
        ON N.IdInscricao = I.IdInscricao
    INNER JOIN dbo.Pagamentos AS P
        ON P.IdPagamento = N.IdPagamento
    WHERE
        P.Estado = N'Pago'
        AND N.IdInscricao IS NOT NULL
        AND I.Estado = N'Pendente';
END;
GO