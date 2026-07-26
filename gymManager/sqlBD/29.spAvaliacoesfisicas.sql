CREATE OR ALTER PROCEDURE sp_AvaliacoesFisicas_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdAvaliacao,
        IdCliente,
        Peso,
        Altura,
        IMC,
        MassaGorda,
        MassaMuscular,
        Observacoes
    FROM AvaliacoesFisicas
    ORDER BY IdAvaliacao DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_AvaliacoesFisicas_ObterPorId
(
    @IdAvaliacao INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdAvaliacao,
        IdCliente,
        Peso,
        Altura,
        IMC,
        MassaGorda,
        MassaMuscular,
        Observacoes
    FROM AvaliacoesFisicas
    WHERE IdAvaliacao = @IdAvaliacao;
END;
GO

CREATE OR ALTER PROCEDURE sp_AvaliacoesFisicas_Inserir
(
    @IdCliente INT,
    @Peso DECIMAL(5,2),
    @Altura DECIMAL(4,2),
    @IMC DECIMAL(5,2),
    @MassaGorda DECIMAL(5,2),
    @MassaMuscular DECIMAL(5,2),
    @Observacoes NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO AvaliacoesFisicas
    (
        IdCliente,
        Peso,
        Altura,
        IMC,
        MassaGorda,
        MassaMuscular,
        Observacoes
    )
    VALUES
    (
        @IdCliente,
        @Peso,
        @Altura,
        @IMC,
        @MassaGorda,
        @MassaMuscular,
        @Observacoes
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_AvaliacoesFisicas_Atualizar
(
    @IdAvaliacao INT,
    @IdCliente INT,
    @Peso DECIMAL(5,2),
    @Altura DECIMAL(4,2),
    @IMC DECIMAL(5,2),
    @MassaGorda DECIMAL(5,2),
    @MassaMuscular DECIMAL(5,2),
    @Observacoes NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE AvaliacoesFisicas
    SET
        IdCliente = @IdCliente,
        Peso = @Peso,
        Altura = @Altura,
        IMC = @IMC,
        MassaGorda = @MassaGorda,
        MassaMuscular = @MassaMuscular,
        Observacoes = @Observacoes
    WHERE IdAvaliacao = @IdAvaliacao;
END;
GO

CREATE OR ALTER PROCEDURE sp_AvaliacoesFisicas_Eliminar
(
    @IdAvaliacao INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM AvaliacoesFisicas
    WHERE IdAvaliacao = @IdAvaliacao;
END;
GO