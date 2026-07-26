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
    @DataFim DATE,
    @Estado NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

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
        @Estado
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