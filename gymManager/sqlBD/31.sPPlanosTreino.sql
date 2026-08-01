CREATE OR ALTER PROCEDURE dbo.sp_PlanosTreino_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PT.IdPlanoTreino,
        PT.IdCliente,
        C.Nome AS NomeCliente,
        PT.IdPT,
        P.Nome AS NomePT,
        PT.NomePlano,
        PT.Objetivo,
        PT.DataInicio,
        PT.DataFim,
        PT.Observacoes,
        PT.Estado
    FROM dbo.PlanosTreino AS PT

    INNER JOIN dbo.Clientes AS C
        ON C.IdCliente = PT.IdCliente

    INNER JOIN dbo.PersonalTrainers AS P
        ON P.IdPT = PT.IdPT

    ORDER BY
        PT.DataInicio DESC,
        PT.IdPlanoTreino DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_PlanosTreino_ObterPorId
(
    @IdPlanoTreino INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PT.IdPlanoTreino,
        PT.IdCliente,
        C.Nome AS NomeCliente,
        PT.IdPT,
        P.Nome AS NomePT,
        PT.NomePlano,
        PT.Objetivo,
        PT.DataInicio,
        PT.DataFim,
        PT.Observacoes,
        PT.Estado
    FROM dbo.PlanosTreino AS PT

    INNER JOIN dbo.Clientes AS C
        ON C.IdCliente = PT.IdCliente

    INNER JOIN dbo.PersonalTrainers AS P
        ON P.IdPT = PT.IdPT

    WHERE PT.IdPlanoTreino = @IdPlanoTreino;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_PlanosTreino_Inserir
(
    @IdCliente INT,
    @IdPT INT,
    @NomePlano NVARCHAR(100),
    @Objetivo NVARCHAR(255),
    @DataInicio DATE,
    @DataFim DATE,
    @Observacoes NVARCHAR(255),
    @Estado NVARCHAR(20)
)
AS
BEGIN
    SET NOCOUNT ON;

    SET @NomePlano =
        LTRIM(RTRIM(@NomePlano));

    SET @Objetivo =
        LTRIM(RTRIM(@Objetivo));

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
              'O cliente selecionado não existe.',
              1;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.PersonalTrainers
        WHERE IdPT = @IdPT
    )
    BEGIN
        THROW 50002,
              'O personal trainer selecionado não existe.',
              1;
    END;

    IF NULLIF(@NomePlano, N'') IS NULL
    BEGIN
        THROW 50003,
              'O nome do plano é obrigatório.',
              1;
    END;

    IF NULLIF(@Objetivo, N'') IS NULL
    BEGIN
        THROW 50004,
              'O objetivo é obrigatório.',
              1;
    END;

    IF @DataFim < @DataInicio
    BEGIN
        THROW 50005,
              'A data final não pode ser anterior à data inicial.',
              1;
    END;

    IF @Estado NOT IN
    (
        N'Ativo',
        N'Concluído',
        N'Cancelado'
    )
    BEGIN
        THROW 50006,
              'O estado do plano não é válido.',
              1;
    END;

    INSERT INTO dbo.PlanosTreino
    (
        IdCliente,
        IdPT,
        NomePlano,
        Objetivo,
        DataInicio,
        DataFim,
        Observacoes,
        Estado
    )
    VALUES
    (
        @IdCliente,
        @IdPT,
        @NomePlano,
        @Objetivo,
        @DataInicio,
        @DataFim,
        @Observacoes,
        @Estado
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_PlanosTreino_Atualizar
(
    @IdPlanoTreino INT,
    @IdCliente INT,
    @IdPT INT,
    @NomePlano NVARCHAR(100),
    @Objetivo NVARCHAR(255),
    @DataInicio DATE,
    @DataFim DATE,
    @Observacoes NVARCHAR(255),
    @Estado NVARCHAR(20)
)
AS
BEGIN
    SET NOCOUNT ON;

    SET @NomePlano =
        LTRIM(RTRIM(@NomePlano));

    SET @Objetivo =
        LTRIM(RTRIM(@Objetivo));

    SET @Observacoes =
        NULLIF(
            LTRIM(RTRIM(@Observacoes)),
            N''
        );

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.PlanosTreino
        WHERE IdPlanoTreino = @IdPlanoTreino
    )
    BEGIN
        THROW 50001,
              'O plano de treino indicado não existe.',
              1;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Clientes
        WHERE IdCliente = @IdCliente
    )
    BEGIN
        THROW 50002,
              'O cliente selecionado não existe.',
              1;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.PersonalTrainers
        WHERE IdPT = @IdPT
    )
    BEGIN
        THROW 50003,
              'O personal trainer selecionado não existe.',
              1;
    END;

    IF NULLIF(@NomePlano, N'') IS NULL
    BEGIN
        THROW 50004,
              'O nome do plano é obrigatório.',
              1;
    END;

    IF NULLIF(@Objetivo, N'') IS NULL
    BEGIN
        THROW 50005,
              'O objetivo é obrigatório.',
              1;
    END;

    IF @DataFim < @DataInicio
    BEGIN
        THROW 50006,
              'A data final não pode ser anterior à data inicial.',
              1;
    END;

    IF @Estado NOT IN
    (
        N'Ativo',
        N'Concluído',
        N'Cancelado'
    )
    BEGIN
        THROW 50007,
              'O estado do plano não é válido.',
              1;
    END;

    UPDATE dbo.PlanosTreino
    SET
        IdCliente = @IdCliente,
        IdPT = @IdPT,
        NomePlano = @NomePlano,
        Objetivo = @Objetivo,
        DataInicio = @DataInicio,
        DataFim = @DataFim,
        Observacoes = @Observacoes,
        Estado = @Estado
    WHERE IdPlanoTreino = @IdPlanoTreino;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_PlanosTreino_Eliminar
(
    @IdPlanoTreino INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.PlanosTreino
        WHERE IdPlanoTreino = @IdPlanoTreino
    )
    BEGIN
        THROW 50001,
              'O plano de treino indicado não existe.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.PlanoTreinoExercicios
        WHERE IdPlanoTreino = @IdPlanoTreino
    )
    BEGIN
        THROW 50002,
              'O plano possui exercícios associados e não pode ser eliminado.',
              1;
    END;

    DELETE FROM dbo.PlanosTreino
    WHERE IdPlanoTreino = @IdPlanoTreino;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_PlanosTreino_Pesquisar
(
    @Pesquisa NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Pesquisa =
        LTRIM(RTRIM(@Pesquisa));

    SELECT
        PT.IdPlanoTreino,
        PT.IdCliente,
        C.Nome AS NomeCliente,
        PT.IdPT,
        P.Nome AS NomePT,
        PT.NomePlano,
        PT.Objetivo,
        PT.DataInicio,
        PT.DataFim,
        PT.Observacoes,
        PT.Estado
    FROM dbo.PlanosTreino AS PT

    INNER JOIN dbo.Clientes AS C
        ON C.IdCliente = PT.IdCliente

    INNER JOIN dbo.PersonalTrainers AS P
        ON P.IdPT = PT.IdPT

    WHERE C.Nome LIKE
              N'%' + @Pesquisa + N'%'

       OR P.Nome LIKE
              N'%' + @Pesquisa + N'%'

       OR PT.NomePlano LIKE
              N'%' + @Pesquisa + N'%'

       OR PT.Objetivo LIKE
              N'%' + @Pesquisa + N'%'

       OR PT.Estado LIKE
              N'%' + @Pesquisa + N'%'

       OR PT.Observacoes LIKE
              N'%' + @Pesquisa + N'%'

       OR CONVERT(
              NVARCHAR(10),
              PT.DataInicio,
              103
          ) LIKE
              N'%' + @Pesquisa + N'%'

       OR CONVERT(
              NVARCHAR(10),
              PT.DataFim,
              103
          ) LIKE
              N'%' + @Pesquisa + N'%'

    ORDER BY
        PT.DataInicio DESC,
        PT.IdPlanoTreino DESC;
END;
GO