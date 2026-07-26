CREATE OR ALTER PROCEDURE sp_AgendamentosPT_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdAgenda,
        IdCliente,
        IdPT,
        DataSessao,
        HoraInicio,
        HoraFim,
        Estado,
        Observacoes
    FROM AgendamentosPT
    ORDER BY DataSessao DESC, HoraInicio;
END;
GO

CREATE OR ALTER PROCEDURE sp_AgendamentosPT_ObterPorId
(
    @IdAgenda INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdAgenda,
        IdCliente,
        IdPT,
        DataSessao,
        HoraInicio,
        HoraFim,
        Estado,
        Observacoes
    FROM AgendamentosPT
    WHERE IdAgenda = @IdAgenda;
END;
GO

CREATE OR ALTER PROCEDURE sp_AgendamentosPT_Inserir
(
    @IdCliente INT,
    @IdPT INT,
    @DataSessao DATE,
    @HoraInicio TIME,
    @HoraFim TIME,
    @Estado NVARCHAR(50),
    @Observacoes NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO AgendamentosPT
    (
        IdCliente,
        IdPT,
        DataSessao,
        HoraInicio,
        HoraFim,
        Estado,
        Observacoes
    )
    VALUES
    (
        @IdCliente,
        @IdPT,
        @DataSessao,
        @HoraInicio,
        @HoraFim,
        @Estado,
        @Observacoes
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_AgendamentosPT_Atualizar
(
    @IdAgenda INT,
    @IdCliente INT,
    @IdPT INT,
    @DataSessao DATE,
    @HoraInicio TIME,
    @HoraFim TIME,
    @Estado NVARCHAR(50),
    @Observacoes NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE AgendamentosPT
    SET
        IdCliente = @IdCliente,
        IdPT = @IdPT,
        DataSessao = @DataSessao,
        HoraInicio = @HoraInicio,
        HoraFim = @HoraFim,
        Estado = @Estado,
        Observacoes = @Observacoes
    WHERE IdAgenda = @IdAgenda;
END;
GO

CREATE OR ALTER PROCEDURE sp_AgendamentosPT_Eliminar
(
    @IdAgenda INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM AgendamentosPT
    WHERE IdAgenda = @IdAgenda;
END;
GO