CREATE OR ALTER PROCEDURE sp_Manutencoes_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdManutencao,
        IdEquipamento,
        DataManutencao,
        Tipo,
        Observacoes
    FROM Manutencoes
    ORDER BY DataManutencao DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_Manutencoes_ObterPorId
(
    @IdManutencao INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdManutencao,
        IdEquipamento,
        DataManutencao,
        Tipo,
        Observacoes
    FROM Manutencoes
    WHERE IdManutencao = @IdManutencao;
END;
GO

CREATE OR ALTER PROCEDURE sp_Manutencoes_Inserir
(
    @IdEquipamento INT,
    @DataManutencao DATE,
    @Tipo NVARCHAR(100),
    @Observacoes NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Manutencoes
    (
        IdEquipamento,
        DataManutencao,
        Tipo,
        Observacoes
    )
    VALUES
    (
        @IdEquipamento,
        @DataManutencao,
        @Tipo,
        @Observacoes
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Manutencoes_Atualizar
(
    @IdManutencao INT,
    @IdEquipamento INT,
    @DataManutencao DATE,
    @Tipo NVARCHAR(100),
    @Observacoes NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Manutencoes
    SET
        IdEquipamento = @IdEquipamento,
        DataManutencao = @DataManutencao,
        Tipo = @Tipo,
        Observacoes = @Observacoes
    WHERE IdManutencao = @IdManutencao;
END;
GO

CREATE OR ALTER PROCEDURE sp_Manutencoes_Eliminar
(
    @IdManutencao INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Manutencoes
    WHERE IdManutencao = @IdManutencao;
END;
GO