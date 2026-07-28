CREATE OR ALTER PROCEDURE sp_Inscricoes_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdInscricao,
        IdCliente,
        IdPlano,
        DataInicio,
        DataFim,
        Estado
    FROM Inscricoes
    ORDER BY DataInicio DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_Inscricoes_ObterPorId
(
    @IdInscricao INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdInscricao,
        IdCliente,
        IdPlano,
        DataInicio,
        DataFim,
        Estado
    FROM Inscricoes
    WHERE IdInscricao = @IdInscricao;
END;
GO

CREATE OR ALTER PROCEDURE sp_Inscricoes_Inserir
(
    @IdCliente INT,
    @IdPlano INT,
    @DataInicio DATE,
    @DataFim DATE
)
AS
BEGIN
    SET NOCOUNT ON;

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

    -- Validar plano
    IF NOT EXISTS
    (
        SELECT 1
        FROM Planos
        WHERE IdPlano = @IdPlano
    )
    BEGIN
        RAISERROR(
            'O plano selecionado não existe.',
            16,
            1
        );

        RETURN;
    END;

    -- Validar datas
    IF @DataFim < @DataInicio
    BEGIN
        RAISERROR(
            'A data final não pode ser anterior à data inicial.',
            16,
            1
        );

        RETURN;
    END;

    -- Impedir inscrições sobrepostas
    IF EXISTS
    (
        SELECT 1
        FROM Inscricoes
        WHERE IdCliente = @IdCliente
          AND Estado IN
          (
              'Pendente',
              'Ativa',
              'Suspensa'
          )
          AND @DataInicio <= DataFim
          AND @DataFim >= DataInicio
    )
    BEGIN
        RAISERROR(
            'O cliente já possui uma inscrição nesse período.',
            16,
            1
        );

        RETURN;
    END;

    INSERT INTO Inscricoes
    (
        IdCliente,
        IdPlano,
        DataInicio,
        DataFim,
        Estado
    )
    VALUES
    (
        @IdCliente,
        @IdPlano,
        @DataInicio,
        @DataFim,
        'Pendente'
    );
END;
GO
CREATE OR ALTER PROCEDURE sp_Inscricoes_Atualizar
(
    @IdInscricao INT,
    @IdCliente INT,
    @IdPlano INT,
    @DataInicio DATE,
    @DataFim DATE,
    @Estado NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Inscricoes
        WHERE IdInscricao = @IdInscricao
    )
    BEGIN
        RAISERROR(
            'A inscrição indicada não existe.',
            16,
            1
        );

        RETURN;
    END;

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
        FROM Planos
        WHERE IdPlano = @IdPlano
    )
    BEGIN
        RAISERROR(
            'O plano selecionado não existe.',
            16,
            1
        );

        RETURN;
    END;

    IF @DataFim < @DataInicio
    BEGIN
        RAISERROR(
            'A data final não pode ser anterior à data inicial.',
            16,
            1
        );

        RETURN;
    END;

    IF @Estado NOT IN
    (
        'Pendente',
        'Ativa',
        'Suspensa',
        'Terminada',
        'Cancelada'
    )
    BEGIN
        RAISERROR(
            'O estado da inscrição não é válido.',
            16,
            1
        );

        RETURN;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM Inscricoes
        WHERE IdCliente = @IdCliente
          AND IdInscricao <> @IdInscricao
          AND Estado IN ('Pendente', 'Ativa', 'Suspensa')
          AND @DataInicio <= DataFim
          AND @DataFim >= DataInicio
    )
    BEGIN
        RAISERROR(
            'O cliente já possui outra inscrição nesse período.',
            16,
            1
        );

        RETURN;
    END;

    UPDATE Inscricoes
    SET
        IdCliente = @IdCliente,
        IdPlano = @IdPlano,
        DataInicio = @DataInicio,
        DataFim = @DataFim,
        Estado = @Estado
    WHERE IdInscricao = @IdInscricao;
END;
GO

CREATE OR ALTER PROCEDURE sp_Inscricoes_Eliminar
(
    @IdInscricao INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Inscricoes
    WHERE IdInscricao = @IdInscricao;
END;
GO
CREATE OR ALTER PROCEDURE sp_Inscricoes_ListarAtivasPorCliente
(
    @IdCliente INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        I.IdInscricao,
        I.IdCliente,
        I.IdPlano,
        P.Nome AS NomePlano,
        P.Preco,
        P.DuracaoMeses,
        I.DataInicio,
        I.DataFim,
        I.Estado
    FROM Inscricoes AS I

    INNER JOIN Planos AS P
        ON P.IdPlano = I.IdPlano

    WHERE I.IdCliente = @IdCliente
      AND I.Estado = 'Pendente'

      AND NOT EXISTS
      (
          SELECT 1
          FROM Pagamentos AS PG
          WHERE PG.IdInscricao = I.IdInscricao
            AND PG.Estado IN ('Pago', 'Pendente')
      )

    ORDER BY
        I.DataInicio DESC,
        I.IdInscricao DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_Inscricoes_AtualizarEstadosExpirados
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Inscricoes
    SET Estado = 'Terminada'
    WHERE Estado IN ('Ativa', 'Suspensa')
      AND DataFim < CAST(GETDATE() AS DATE);
END;
GO

CREATE OR ALTER TRIGGER trg_Inscricoes_CriarPagamento
ON Inscricoes
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

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
    SELECT
        I.IdCliente,
        I.IdInscricao,
        CAST(GETDATE() AS DATE),
        P.Preco,
        'Pagamento Posterior',
        'Pagamento criado automaticamente.',
        'Pendente',
        NULL,
        NULL,
        NULL
    FROM inserted I
    INNER JOIN Planos P
        ON P.IdPlano = I.IdPlano;
END;
GO