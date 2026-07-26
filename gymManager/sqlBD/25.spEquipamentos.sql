CREATE OR ALTER PROCEDURE sp_Equipamentos_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdEquipamento,
        Nome,
        Marca,
        Estado,
        DataCompra
    FROM Equipamentos
    ORDER BY Nome;
END;
GO

CREATE OR ALTER PROCEDURE sp_Equipamentos_ObterPorId
(
    @IdEquipamento INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdEquipamento,
        Nome,
        Marca,
        Estado,
        DataCompra
    FROM Equipamentos
    WHERE IdEquipamento = @IdEquipamento;
END;
GO

CREATE OR ALTER PROCEDURE sp_Equipamentos_Inserir
(
    @Nome NVARCHAR(100),
    @Marca NVARCHAR(100),
    @Estado NVARCHAR(50),
    @DataCompra DATE
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Equipamentos
    (
        Nome,
        Marca,
        Estado,
        DataCompra
    )
    VALUES
    (
        @Nome,
        @Marca,
        @Estado,
        @DataCompra
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Equipamentos_Atualizar
(
    @IdEquipamento INT,
    @Nome NVARCHAR(100),
    @Marca NVARCHAR(100),
    @Estado NVARCHAR(50),
    @DataCompra DATE
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Equipamentos
    SET
        Nome = @Nome,
        Marca = @Marca,
        Estado = @Estado,
        DataCompra = @DataCompra
    WHERE IdEquipamento = @IdEquipamento;
END;
GO

CREATE OR ALTER PROCEDURE sp_Equipamentos_Eliminar
(
    @IdEquipamento INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Equipamentos
    WHERE IdEquipamento = @IdEquipamento;
END;
GO