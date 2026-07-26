CREATE OR ALTER PROCEDURE sp_Pagamentos_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdPagamento,
        IdCliente,
        DataPagamento,
        Valor,
        MetodoPagamento,
        Observacoes
    FROM Pagamentos
    ORDER BY DataPagamento DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_Pagamentos_ObterPorId
(
    @IdPagamento INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdPagamento,
        IdCliente,
        DataPagamento,
        Valor,
        MetodoPagamento,
        Observacoes
    FROM Pagamentos
    WHERE IdPagamento = @IdPagamento;
END;
GO

CREATE OR ALTER PROCEDURE sp_Pagamentos_Inserir
(
    @IdCliente INT,
    @DataPagamento DATE,
    @Valor DECIMAL(10,2),
    @MetodoPagamento NVARCHAR(50),
    @Observacoes NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Pagamentos
    (
        IdCliente,
        DataPagamento,
        Valor,
        MetodoPagamento,
        Observacoes
    )
    VALUES
    (
        @IdCliente,
        @DataPagamento,
        @Valor,
        @MetodoPagamento,
        @Observacoes
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Pagamentos_Atualizar
(
    @IdPagamento INT,
    @IdCliente INT,
    @DataPagamento DATE,
    @Valor DECIMAL(10,2),
    @MetodoPagamento NVARCHAR(50),
    @Observacoes NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Pagamentos
    SET
        IdCliente = @IdCliente,
        DataPagamento = @DataPagamento,
        Valor = @Valor,
        MetodoPagamento = @MetodoPagamento,
        Observacoes = @Observacoes
    WHERE IdPagamento = @IdPagamento;
END;
GO

CREATE OR ALTER PROCEDURE sp_Pagamentos_Eliminar
(
    @IdPagamento INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Pagamentos
    WHERE IdPagamento = @IdPagamento;
END;
GO