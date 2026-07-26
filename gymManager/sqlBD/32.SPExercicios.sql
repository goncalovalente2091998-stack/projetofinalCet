CREATE OR ALTER PROCEDURE sp_Exercicios_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdExercicio,
        IdPlanoTreino,
        Nome,
        Series,
        Repeticoes,
        TempoDescanso
    FROM Exercicios
    ORDER BY Nome;
END;
GO

CREATE OR ALTER PROCEDURE sp_Exercicios_ObterPorId
(
    @IdExercicio INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdExercicio,
        IdPlanoTreino,
        Nome,
        Series,
        Repeticoes,
        TempoDescanso
    FROM Exercicios
    WHERE IdExercicio = @IdExercicio;
END;
GO

CREATE OR ALTER PROCEDURE sp_Exercicios_Inserir
(
    @IdPlanoTreino INT,
    @Nome NVARCHAR(100),
    @Series INT,
    @Repeticoes INT,
    @TempoDescanso INT
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Exercicios
    (
        IdPlanoTreino,
        Nome,
        Series,
        Repeticoes,
        TempoDescanso
    )
    VALUES
    (
        @IdPlanoTreino,
        @Nome,
        @Series,
        @Repeticoes,
        @TempoDescanso
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Exercicios_Atualizar
(
    @IdExercicio INT,
    @IdPlanoTreino INT,
    @Nome NVARCHAR(100),
    @Series INT,
    @Repeticoes INT,
    @TempoDescanso INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Exercicios
    SET
        IdPlanoTreino = @IdPlanoTreino,
        Nome = @Nome,
        Series = @Series,
        Repeticoes = @Repeticoes,
        TempoDescanso = @TempoDescanso
    WHERE IdExercicio = @IdExercicio;
END;
GO

CREATE OR ALTER PROCEDURE sp_Exercicios_Eliminar
(
    @IdExercicio INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Exercicios
    WHERE IdExercicio = @IdExercicio;
END;
GO