CREATE OR ALTER PROCEDURE sp_InscricoesAulas_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdInscricao,
        IdCliente,
        IdAula,
        DataInscricao
    FROM InscricoesAulas
    ORDER BY DataInscricao DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_InscricoesAulas_ObterPorId
(
    @IdInscricao INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdInscricao,
        IdCliente,
        IdAula,
        DataInscricao
    FROM InscricoesAulas
    WHERE IdInscricao = @IdInscricao;
END;
GO

CREATE OR ALTER PROCEDURE sp_InscricoesAulas_Inserir
(
    @IdCliente INT,
    @IdAula INT,
    @DataInscricao DATE
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO InscricoesAulas
    (
        IdCliente,
        IdAula,
        DataInscricao
    )
    VALUES
    (
        @IdCliente,
        @IdAula,
        @DataInscricao
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_InscricoesAulas_Atualizar
(
    @IdInscricao INT,
    @IdCliente INT,
    @IdAula INT,
    @DataInscricao DATE
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE InscricoesAulas
    SET
        IdCliente = @IdCliente,
        IdAula = @IdAula,
        DataInscricao = @DataInscricao
    WHERE IdInscricao = @IdInscricao;
END;
GO

CREATE OR ALTER PROCEDURE sp_InscricoesAulas_Eliminar
(
    @IdInscricao INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM InscricoesAulas
    WHERE IdInscricao = @IdInscricao;
END;
GO