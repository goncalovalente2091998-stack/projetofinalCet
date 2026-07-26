CREATE OR ALTER PROCEDURE sp_PlanosTreino_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdPlanoTreino,
        IdCliente,
        IdPT,
        NomePlano,
        Objetivo,
        DataInicio,
        DataFim,
        Observacoes
    FROM PlanosTreino
    ORDER BY DataInicio DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_PlanosTreino_ObterPorId
(
    @IdPlanoTreino INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdPlanoTreino,
        IdCliente,
        IdPT,
        NomePlano,
        Objetivo,
        DataInicio,
        DataFim,
        Observacoes
    FROM PlanosTreino
    WHERE IdPlanoTreino = @IdPlanoTreino;
END;
GO

CREATE OR ALTER PROCEDURE sp_PlanosTreino_Inserir
(
    @IdCliente INT,
    @IdPT INT,
    @NomePlano NVARCHAR(100),
    @Objetivo NVARCHAR(255),
    @DataInicio DATE,
    @DataFim DATE,
    @Observacoes NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO PlanosTreino
    (
        IdCliente,
        IdPT,
        NomePlano,
        Objetivo,
        DataInicio,
        DataFim,
        Observacoes
    )
    VALUES
    (
        @IdCliente,
        @IdPT,
        @NomePlano,
        @Objetivo,
        @DataInicio,
        @DataFim,
        @Observacoes
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_PlanosTreino_Atualizar
(
    @IdPlanoTreino INT,
    @IdCliente INT,
    @IdPT INT,
    @NomePlano NVARCHAR(100),
    @Objetivo NVARCHAR(255),
    @DataInicio DATE,
    @DataFim DATE,
    @Observacoes NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE PlanosTreino
    SET
        IdCliente = @IdCliente,
        IdPT = @IdPT,
        NomePlano = @NomePlano,
        Objetivo = @Objetivo,
        DataInicio = @DataInicio,
        DataFim = @DataFim,
        Observacoes = @Observacoes
    WHERE IdPlanoTreino = @IdPlanoTreino;
END;
GO

CREATE OR ALTER PROCEDURE sp_PlanosTreino_Eliminar
(
    @IdPlanoTreino INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM PlanosTreino
    WHERE IdPlanoTreino = @IdPlanoTreino;
END;
GO