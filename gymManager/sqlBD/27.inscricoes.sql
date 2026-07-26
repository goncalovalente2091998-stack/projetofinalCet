CREATE OR ALTER PROCEDURE sp_Inscricoes_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        I.IdInscricao,
        I.IdCliente,
        C.Nome AS NomeCliente,
        I.IdPlano,
        P.Nome AS NomePlano,
        I.DataInicio,
        I.DataFim,
        CASE
            WHEN I.Estado = 'Terminada'
                THEN 'Terminada'
            WHEN I.DataFim < CAST(GETDATE() AS DATE)
                THEN 'Terminada'
            ELSE I.Estado
        END AS Estado
    FROM Inscricoes AS I
    INNER JOIN Clientes AS C
        ON C.IdCliente = I.IdCliente
    INNER JOIN Planos AS P
        ON P.IdPlano = I.IdPlano
    ORDER BY I.DataInicio DESC;
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
        I.IdInscricao,
        I.IdCliente,
        C.Nome AS NomeCliente,
        I.IdPlano,
        P.Nome AS NomePlano,
        I.DataInicio,
        I.DataFim,
        CASE
            WHEN I.Estado = 'Terminada'
                THEN 'Terminada'
            WHEN I.DataFim < CAST(GETDATE() AS DATE)
                THEN 'Terminada'
            ELSE I.Estado
        END AS Estado
    FROM Inscricoes AS I
    INNER JOIN Clientes AS C
        ON C.IdCliente = I.IdCliente
    INNER JOIN Planos AS P
        ON P.IdPlano = I.IdPlano
    WHERE I.IdInscricao = @IdInscricao;
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

    IF NOT EXISTS
    (
        SELECT 1
        FROM Clientes
        WHERE IdCliente = @IdCliente
    )
    BEGIN
        RAISERROR('O cliente selecionado não existe.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Planos
        WHERE IdPlano = @IdPlano
    )
    BEGIN
        RAISERROR('O plano selecionado não existe.', 16, 1);
        RETURN;
    END;

    IF @DataFim < @DataInicio
    BEGIN
        RAISERROR(
            'A data de fim não pode ser anterior à data de início.',
            16,
            1
        );
        RETURN;
    END;

    IF @Estado NOT IN ('Ativa', 'Suspensa', 'Terminada')
    BEGIN
        RAISERROR('O estado indicado não é válido.', 16, 1);
        RETURN;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM Inscricoes
        WHERE IdCliente = @IdCliente
          AND Estado = 'Ativa'
          AND @DataInicio <= DataFim
          AND @DataFim >= DataInicio
    )
    BEGIN
        RAISERROR(
            'O cliente já possui uma inscrição ativa nesse período.',
            16,
            1
        );
        RETURN;
    END;

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

    IF NOT EXISTS
    (
        SELECT 1
        FROM Inscricoes
        WHERE IdInscricao = @IdInscricao
    )
    BEGIN
        RAISERROR('A inscrição indicada não existe.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Clientes
        WHERE IdCliente = @IdCliente
    )
    BEGIN
        RAISERROR('O cliente selecionado não existe.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Planos
        WHERE IdPlano = @IdPlano
    )
    BEGIN
        RAISERROR('O plano selecionado não existe.', 16, 1);
        RETURN;
    END;

    IF @DataFim < @DataInicio
    BEGIN
        RAISERROR(
            'A data de fim não pode ser anterior à data de início.',
            16,
            1
        );
        RETURN;
    END;

    IF @Estado NOT IN ('Ativa', 'Suspensa', 'Terminada')
    BEGIN
        RAISERROR('O estado indicado não é válido.', 16, 1);
        RETURN;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM Inscricoes
        WHERE IdCliente = @IdCliente
          AND IdInscricao <> @IdInscricao
          AND Estado = 'Ativa'
          AND @DataInicio <= DataFim
          AND @DataFim >= DataInicio
    )
    BEGIN
        RAISERROR(
            'O cliente já possui outra inscrição ativa nesse período.',
            16,
            1
        );
        RETURN;
    END;

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

    IF NOT EXISTS
    (
        SELECT 1
        FROM Inscricoes
        WHERE IdInscricao = @IdInscricao
    )
    BEGIN
        RAISERROR('A inscrição indicada não existe.', 16, 1);
        RETURN;
    END;

    DELETE FROM Inscricoes
    WHERE IdInscricao = @IdInscricao;
END;
GO


CREATE OR ALTER PROCEDURE sp_Inscricoes_Pesquisar
(
    @Pesquisa NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        I.IdInscricao,
        I.IdCliente,
        C.Nome AS NomeCliente,
        I.IdPlano,
        P.Nome AS NomePlano,
        I.DataInicio,
        I.DataFim,
        CASE
            WHEN I.Estado = 'Terminada'
                THEN 'Terminada'
            WHEN I.DataFim < CAST(GETDATE() AS DATE)
                THEN 'Terminada'
            ELSE I.Estado
        END AS Estado
    FROM Inscricoes AS I
    INNER JOIN Clientes AS C
        ON C.IdCliente = I.IdCliente
    INNER JOIN Planos AS P
        ON P.IdPlano = I.IdPlano
    WHERE C.Nome LIKE '%' + @Pesquisa + '%'
       OR C.NIF LIKE '%' + @Pesquisa + '%'
       OR P.Nome LIKE '%' + @Pesquisa + '%'
       OR
       (
           CASE
               WHEN I.Estado = 'Terminada'
                   THEN 'Terminada'
               WHEN I.DataFim < CAST(GETDATE() AS DATE)
                   THEN 'Terminada'
               ELSE I.Estado
           END
       ) LIKE '%' + @Pesquisa + '%'
    ORDER BY I.DataInicio DESC;
END;
GO