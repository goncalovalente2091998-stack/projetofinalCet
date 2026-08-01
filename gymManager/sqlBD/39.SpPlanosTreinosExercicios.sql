CREATE OR ALTER PROCEDURE dbo.sp_PlanoTreinoExercicios_ListarPorPlano
(
    @IdPlanoTreino INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PTE.IdPlanoTreinoExercicio,
        PTE.IdPlanoTreino,
        PTE.IdExercicio,

        E.Nome AS NomeExercicio,
        E.GrupoMuscular,
        E.Equipamento,

        PTE.Series,
        PTE.Repeticoes,
        PTE.TempoDescanso,
        PTE.Ordem,
        PTE.Observacoes

    FROM dbo.PlanoTreinoExercicios AS PTE

    INNER JOIN dbo.Exercicios AS E
        ON E.IdExercicio = PTE.IdExercicio

    WHERE
        PTE.IdPlanoTreino = @IdPlanoTreino

    ORDER BY
        PTE.Ordem,
        PTE.IdPlanoTreinoExercicio;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_PlanoTreinoExercicios_Inserir
(
    @IdPlanoTreino INT,
    @IdExercicio INT,
    @Series INT,
    @Repeticoes INT,
    @TempoDescanso INT,
    @Ordem INT,
    @Observacoes NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.PlanosTreino
        WHERE IdPlanoTreino = @IdPlanoTreino
    )
    BEGIN
        THROW 50001,
              'O plano de treino indicado n�o existe.',
              1;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Exercicios
        WHERE IdExercicio = @IdExercicio
          AND Estado = N'Ativo'
    )
    BEGIN
        THROW 50002,
              'O exerc�cio selecionado n�o existe ou est� inativo.',
              1;
    END;

    IF @Series <= 0
    BEGIN
        THROW 50003,
              'O n�mero de s�ries deve ser superior a zero.',
              1;
    END;

    IF @Repeticoes <= 0
    BEGIN
        THROW 50004,
              'O n�mero de repeti��es deve ser superior a zero.',
              1;
    END;

    IF @TempoDescanso < 0
    BEGIN
        THROW 50005,
              'O tempo de descanso n�o pode ser negativo.',
              1;
    END;

    IF @Ordem <= 0
    BEGIN
        THROW 50006,
              'A ordem deve ser superior a zero.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.PlanoTreinoExercicios
        WHERE IdPlanoTreino = @IdPlanoTreino
          AND IdExercicio = @IdExercicio
    )
    BEGIN
        THROW 50007,
              'Este exerc�cio j� est� associado ao plano de treino.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.PlanoTreinoExercicios
        WHERE IdPlanoTreino = @IdPlanoTreino
          AND Ordem = @Ordem
    )
    BEGIN
        THROW 50008,
              'J� existe um exerc�cio com esta ordem no plano.',
              1;
    END;

    INSERT INTO dbo.PlanoTreinoExercicios
    (
        IdPlanoTreino,
        IdExercicio,
        Series,
        Repeticoes,
        TempoDescanso,
        Ordem,
        Observacoes
    )
    VALUES
    (
        @IdPlanoTreino,
        @IdExercicio,
        @Series,
        @Repeticoes,
        @TempoDescanso,
        @Ordem,
        NULLIF(
            LTRIM(RTRIM(@Observacoes)),
            N''
        )
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_PlanoTreinoExercicios_Atualizar
(
    @IdPlanoTreinoExercicio INT,
    @IdExercicio INT,
    @Series INT,
    @Repeticoes INT,
    @TempoDescanso INT,
    @Ordem INT,
    @Observacoes NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdPlanoTreino INT;

    SELECT
        @IdPlanoTreino = IdPlanoTreino
    FROM dbo.PlanoTreinoExercicios
    WHERE IdPlanoTreinoExercicio =
          @IdPlanoTreinoExercicio;

    IF @IdPlanoTreino IS NULL
    BEGIN
        THROW 50001,
              'O exerc�cio associado ao plano n�o existe.',
              1;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Exercicios
        WHERE IdExercicio = @IdExercicio
          AND Estado = N'Ativo'
    )
    BEGIN
        THROW 50002,
              'O exerc�cio selecionado n�o existe ou est� inativo.',
              1;
    END;

    IF @Series <= 0
    BEGIN
        THROW 50003,
              'O n�mero de s�ries deve ser superior a zero.',
              1;
    END;

    IF @Repeticoes <= 0
    BEGIN
        THROW 50004,
              'O n�mero de repeti��es deve ser superior a zero.',
              1;
    END;

    IF @TempoDescanso < 0
    BEGIN
        THROW 50005,
              'O tempo de descanso n�o pode ser negativo.',
              1;
    END;

    IF @Ordem <= 0
    BEGIN
        THROW 50006,
              'A ordem deve ser superior a zero.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.PlanoTreinoExercicios
        WHERE IdPlanoTreino = @IdPlanoTreino
          AND IdExercicio = @IdExercicio
          AND IdPlanoTreinoExercicio <>
              @IdPlanoTreinoExercicio
    )
    BEGIN
        THROW 50007,
              'Este exerc�cio j� est� associado ao plano de treino.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.PlanoTreinoExercicios
        WHERE IdPlanoTreino = @IdPlanoTreino
          AND Ordem = @Ordem
          AND IdPlanoTreinoExercicio <>
              @IdPlanoTreinoExercicio
    )
    BEGIN
        THROW 50008,
              'J� existe outro exerc�cio com esta ordem no plano.',
              1;
    END;

    UPDATE dbo.PlanoTreinoExercicios
    SET
        IdExercicio = @IdExercicio,
        Series = @Series,
        Repeticoes = @Repeticoes,
        TempoDescanso = @TempoDescanso,
        Ordem = @Ordem,
        Observacoes =
            NULLIF(
                LTRIM(RTRIM(@Observacoes)),
                N''
            )
    WHERE IdPlanoTreinoExercicio =
          @IdPlanoTreinoExercicio;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_PlanoTreinoExercicios_Eliminar
(
    @IdPlanoTreinoExercicio INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.PlanoTreinoExercicios
        WHERE IdPlanoTreinoExercicio =
              @IdPlanoTreinoExercicio
    )
    BEGIN
        THROW 50001,
              'O exerc�cio associado ao plano n�o existe.',
              1;
    END;

    DELETE FROM dbo.PlanoTreinoExercicios
    WHERE IdPlanoTreinoExercicio =
          @IdPlanoTreinoExercicio;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_PlanoTreinoExercicios_TrocarOrdem
(
    @IdPlanoTreinoExercicio INT,
    @Direcao NVARCHAR(10)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IdPlanoTreino INT;
    DECLARE @OrdemAtual INT;

    DECLARE @IdOutro INT;
    DECLARE @OrdemOutra INT;

    DECLARE @OrdemTemporaria INT;

    SELECT
        @IdPlanoTreino = IdPlanoTreino,
        @OrdemAtual = Ordem
    FROM dbo.PlanoTreinoExercicios
    WHERE IdPlanoTreinoExercicio =
          @IdPlanoTreinoExercicio;

    IF @IdPlanoTreino IS NULL
    BEGIN
        THROW 50001,
              'O exercício do plano indicado não existe.',
              1;
    END;

    IF @Direcao NOT IN
    (
        N'Subir',
        N'Descer'
    )
    BEGIN
        THROW 50002,
              'A direção indicada não é válida.',
              1;
    END;

    /*
        Procurar o exercício imediatamente anterior
        ou imediatamente seguinte.
    */
    IF @Direcao = N'Subir'
    BEGIN
        SELECT TOP (1)
            @IdOutro =
                IdPlanoTreinoExercicio,

            @OrdemOutra =
                Ordem
        FROM dbo.PlanoTreinoExercicios
        WHERE IdPlanoTreino =
              @IdPlanoTreino

          AND Ordem <
              @OrdemAtual

        ORDER BY
            Ordem DESC,
            IdPlanoTreinoExercicio DESC;
    END;
    ELSE
    BEGIN
        SELECT TOP (1)
            @IdOutro =
                IdPlanoTreinoExercicio,

            @OrdemOutra =
                Ordem
        FROM dbo.PlanoTreinoExercicios
        WHERE IdPlanoTreino =
              @IdPlanoTreino

          AND Ordem >
              @OrdemAtual

        ORDER BY
            Ordem ASC,
            IdPlanoTreinoExercicio ASC;
    END;

    /*
        Se já estiver no primeiro ou no último lugar,
        não há nada para trocar.
    */
    IF @IdOutro IS NULL
    BEGIN
        RETURN;
    END;

    /*
        Valor temporário positivo para respeitar:
        CHECK (Ordem > 0)

        É usado um valor superior a todas as ordens
        existentes neste plano.
    */
    SELECT
        @OrdemTemporaria =
            ISNULL(MAX(Ordem), 0) + 1000
    FROM dbo.PlanoTreinoExercicios
    WHERE IdPlanoTreino =
          @IdPlanoTreino;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.PlanoTreinoExercicios
        SET Ordem =
            @OrdemTemporaria
        WHERE IdPlanoTreinoExercicio =
              @IdPlanoTreinoExercicio;

        UPDATE dbo.PlanoTreinoExercicios
        SET Ordem =
            @OrdemAtual
        WHERE IdPlanoTreinoExercicio =
              @IdOutro;

        UPDATE dbo.PlanoTreinoExercicios
        SET Ordem =
            @OrdemOutra
        WHERE IdPlanoTreinoExercicio =
              @IdPlanoTreinoExercicio;

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