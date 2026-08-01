CREATE OR ALTER PROCEDURE dbo.sp_Presencas_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        P.IdPresenca,
        P.IdCliente,
        C.Nome AS NomeCliente,
        C.NIF,
        P.DataEntrada,
        P.DataSaida,
        P.Observacoes
    FROM dbo.Presencas AS P
    INNER JOIN dbo.Clientes AS C
        ON C.IdCliente = P.IdCliente
    ORDER BY
        P.DataEntrada DESC,
        P.IdPresenca DESC;
END;
GO


CREATE OR ALTER PROCEDURE dbo.sp_Presencas_ObterPorId
(
    @IdPresenca INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        P.IdPresenca,
        P.IdCliente,
        C.Nome AS NomeCliente,
        C.NIF,
        P.DataEntrada,
        P.DataSaida,
        P.Observacoes
    FROM dbo.Presencas AS P
    INNER JOIN dbo.Clientes AS C
        ON C.IdCliente = P.IdCliente
    WHERE P.IdPresenca = @IdPresenca;
END;
GO


CREATE OR ALTER PROCEDURE dbo.sp_Presencas_RegistarEntrada
(
    @IdCliente INT,
    @Observacoes NVARCHAR(255) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @Observacoes =
        NULLIF(
            LTRIM(RTRIM(@Observacoes)),
            N''
        );

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Clientes
        WHERE IdCliente = @IdCliente
    )
    BEGIN
        THROW 50001,
              'O cliente indicado não existe.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Presencas
        WHERE IdCliente = @IdCliente
          AND DataSaida IS NULL
    )
    BEGIN
        THROW 50002,
              'Este cliente já possui uma entrada aberta.',
              1;
    END;

    INSERT INTO dbo.Presencas
    (
        IdCliente,
        DataEntrada,
        DataSaida,
        Observacoes
    )
    VALUES
    (
        @IdCliente,
        SYSDATETIME(),
        NULL,
        @Observacoes
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Presencas_RegistarSaida
(
    @IdCliente INT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IdPresenca INT;

    SELECT TOP (1)
        @IdPresenca =
            IdPresenca
    FROM dbo.Presencas
    WHERE IdCliente = @IdCliente
      AND DataSaida IS NULL
    ORDER BY DataEntrada DESC;

    IF @IdPresenca IS NULL
    BEGIN
        THROW 50001,
              'Este cliente não possui nenhuma entrada aberta.',
              1;
    END;

    UPDATE dbo.Presencas
    SET DataSaida =
        SYSDATETIME()
    WHERE IdPresenca =
          @IdPresenca;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Presencas_Pesquisar
(
    @Pesquisa NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Pesquisa =
        LTRIM(RTRIM(@Pesquisa));

    SELECT
        P.IdPresenca,
        P.IdCliente,
        C.Nome AS NomeCliente,
        C.NIF,
        P.DataEntrada,
        P.DataSaida,
        P.Observacoes
    FROM dbo.Presencas AS P
    INNER JOIN dbo.Clientes AS C
        ON C.IdCliente = P.IdCliente
    WHERE
        C.Nome LIKE
            N'%' + @Pesquisa + N'%'

        OR C.NIF LIKE
            N'%' + @Pesquisa + N'%'

        OR P.Observacoes LIKE
            N'%' + @Pesquisa + N'%'

        OR CONVERT(
               NVARCHAR(10),
               P.DataEntrada,
               103
           ) LIKE
           N'%' + @Pesquisa + N'%'

        OR CONVERT(
               NVARCHAR(10),
               P.DataSaida,
               103
           ) LIKE
           N'%' + @Pesquisa + N'%'

    ORDER BY
        P.DataEntrada DESC,
        P.IdPresenca DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Presencas_ListarAtivas
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        P.IdPresenca,
        P.IdCliente,
        C.Nome AS NomeCliente,
        C.NIF,
        P.DataEntrada,
        P.DataSaida,
        P.Observacoes
    FROM dbo.Presencas AS P
    INNER JOIN dbo.Clientes AS C
        ON C.IdCliente = P.IdCliente
    WHERE P.DataSaida IS NULL
    ORDER BY
        P.DataEntrada ASC,
        P.IdPresenca ASC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Presencas_Eliminar
(
    @IdPresenca INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Presencas
        WHERE IdPresenca = @IdPresenca
    )
    BEGIN
        THROW 50001,
              'A presença indicada não existe.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Presencas
        WHERE IdPresenca = @IdPresenca
          AND DataSaida IS NULL
    )
    BEGIN
        THROW 50002,
              'Não é possível eliminar uma presença ainda aberta.',
              1;
    END;

    DELETE FROM dbo.Presencas
    WHERE IdPresenca = @IdPresenca;
END;
GO