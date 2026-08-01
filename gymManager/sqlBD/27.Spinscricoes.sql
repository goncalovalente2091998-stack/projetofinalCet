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

    -- Verificar se a inscrição existe
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

    -- Não permitir eliminar inscrições com pagamentos
    IF EXISTS
    (
        SELECT 1
        FROM Pagamentos
        WHERE IdInscricao = @IdInscricao
    )
    BEGIN
        RAISERROR(
            'Esta inscrição possui pagamentos associados e não pode ser eliminada. Mantenha-a para preservar o histórico financeiro.',
            16,
            1
        );

        RETURN;
    END;

    DELETE FROM Inscricoes
    WHERE IdInscricao = @IdInscricao;
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

CREATE OR ALTER PROCEDURE sp_Inscricoes_Renovar
(
    @IdInscricao INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdCliente INT;
    DECLARE @IdPlano INT;
    DECLARE @DuracaoMeses INT;
    DECLARE @EstadoAtual NVARCHAR(50);
    DECLARE @DataFimAtual DATE;
    DECLARE @NovaDataInicio DATE;
    DECLARE @NovaDataFim DATE;

    -- Obter os dados da inscrição atual
    SELECT
        @IdCliente = I.IdCliente,
        @IdPlano = I.IdPlano,
        @EstadoAtual = I.Estado,
        @DataFimAtual = I.DataFim
    FROM Inscricoes AS I
    WHERE I.IdInscricao = @IdInscricao;

    -- Validar existência
    IF @IdCliente IS NULL
    BEGIN
        RAISERROR(
            'A inscrição indicada não existe.',
            16,
            1
        );

        RETURN;
    END;

    -- Apenas inscrições terminadas ou canceladas
    IF @EstadoAtual NOT IN ('Terminada', 'Cancelada')
    BEGIN
        RAISERROR(
            'Apenas inscrições terminadas ou canceladas podem ser renovadas.',
            16,
            1
        );

        RETURN;
    END;

    -- Obter duração do plano
    SELECT
        @DuracaoMeses = DuracaoMeses
    FROM Planos
    WHERE IdPlano = @IdPlano;

    IF @DuracaoMeses IS NULL OR @DuracaoMeses <= 0
    BEGIN
        RAISERROR(
            'O plano associado não possui uma duração válida.',
            16,
            1
        );

        RETURN;
    END;

    -- A nova inscrição começa hoje se a anterior já terminou.
    -- Se a data final ainda estiver no futuro, começa no dia seguinte.
    SET @NovaDataInicio =
        CASE
            WHEN @DataFimAtual >= CAST(GETDATE() AS DATE)
                THEN DATEADD(DAY, 1, @DataFimAtual)
            ELSE CAST(GETDATE() AS DATE)
        END;

    SET @NovaDataFim =
        DATEADD(
            MONTH,
            @DuracaoMeses,
            @NovaDataInicio
        );

    -- Impedir sobreposição com outra inscrição ativa/pendente/suspensa
    IF EXISTS
    (
        SELECT 1
        FROM Inscricoes
        WHERE IdCliente = @IdCliente
          AND Estado IN ('Pendente', 'Ativa', 'Suspensa')
          AND @NovaDataInicio <= DataFim
          AND @NovaDataFim >= DataInicio
    )
    BEGIN
        RAISERROR(
            'O cliente já possui outra inscrição ativa, pendente ou suspensa nesse período.',
            16,
            1
        );

        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

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
            @NovaDataInicio,
            @NovaDataFim,
            'Pendente'
        );

        /*
            O trigger trg_Inscricoes_CriarPagamento
            cria automaticamente o pagamento pendente.
        */

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

CREATE OR ALTER PROCEDURE dbo.sp_Inscricoes_GerarPagamento
(
    @IdInscricao INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdCliente INT;
    DECLARE @IdPlano INT;
    DECLARE @EstadoInscricao NVARCHAR(50);
    DECLARE @Preco DECIMAL(10,2);

    SELECT
        @IdCliente = I.IdCliente,
        @IdPlano = I.IdPlano,
        @EstadoInscricao = I.Estado
    FROM dbo.Inscricoes AS I
    WHERE I.IdInscricao = @IdInscricao;

    IF @IdCliente IS NULL
    BEGIN
        THROW 50001,
              'A inscrição indicada não existe.',
              1;
    END;

    IF @EstadoInscricao <> 'Pendente'
    BEGIN
        THROW 50002,
              'Apenas inscrições pendentes podem gerar um pagamento.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Pagamentos
        WHERE IdInscricao = @IdInscricao
          AND Estado IN ('Pendente', 'Pago')
    )
    BEGIN
        THROW 50003,
              'Já existe um pagamento pendente ou pago para esta inscrição.',
              1;
    END;

    SELECT
        @Preco = Preco
    FROM dbo.Planos
    WHERE IdPlano = @IdPlano;

    IF @Preco IS NULL OR @Preco <= 0
    BEGIN
        THROW 50004,
              'O plano associado não possui um preço válido.',
              1;
    END;

    INSERT INTO dbo.Pagamentos
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
        CAST(GETDATE() AS DATE),
        @Preco,
        'Pagamento Posterior',
        'Pagamento recriado para a inscrição pendente.',
        'Pendente',
        NULL,
        NULL,
        NULL
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Inscricoes_ListarDisponiveisParaPagamento
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
    FROM dbo.Inscricoes AS I
    INNER JOIN dbo.Planos AS P
        ON P.IdPlano = I.IdPlano
    WHERE
        I.IdCliente = @IdCliente
        AND I.Estado IN
        (
            N'Ativa',
            N'Ativo',
            N'Pendente'
        )
    ORDER BY
        I.DataInicio DESC,
        I.IdInscricao DESC;
END;
GO