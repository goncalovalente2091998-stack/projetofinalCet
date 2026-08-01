CREATE OR ALTER PROCEDURE dbo.sp_AvaliacoesFisicas_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        AF.IdAvaliacao,
        AF.IdCliente,
        C.Nome AS NomeCliente,
        AF.IdPT,
        PT.Nome AS NomePT,
        AF.DataAvaliacao,
        AF.Peso,
        AF.Altura,
        AF.IMC,
        AF.MassaGorda,
        AF.MassaMuscular,
        AF.Observacoes,
        AF.Estado
    FROM dbo.AvaliacoesFisicas AS AF
    INNER JOIN dbo.Clientes AS C
        ON C.IdCliente = AF.IdCliente
    INNER JOIN dbo.PersonalTrainers AS PT
        ON PT.IdPT = AF.IdPT
    ORDER BY
        AF.DataAvaliacao DESC,
        AF.IdAvaliacao DESC;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_AvaliacoesFisicas_ObterPorId
(
    @IdAvaliacao INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        AF.IdAvaliacao,
        AF.IdCliente,
        C.Nome AS NomeCliente,
        AF.IdPT,
        PT.Nome AS NomePT,
        AF.DataAvaliacao,
        AF.Peso,
        AF.Altura,
        AF.IMC,
        AF.MassaGorda,
        AF.MassaMuscular,
        AF.Observacoes,
        AF.Estado
    FROM dbo.AvaliacoesFisicas AS AF
    INNER JOIN dbo.Clientes AS C
        ON C.IdCliente = AF.IdCliente
    INNER JOIN dbo.PersonalTrainers AS PT
        ON PT.IdPT = AF.IdPT
    WHERE AF.IdAvaliacao = @IdAvaliacao;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_AvaliacoesFisicas_Inserir
(
    @IdCliente INT,
    @IdPT INT,
    @DataAvaliacao DATE,
    @Peso DECIMAL(5,2) = NULL,
    @Altura DECIMAL(4,2) = NULL,
    @MassaGorda DECIMAL(5,2) = NULL,
    @MassaMuscular DECIMAL(5,2) = NULL,
    @Observacoes NVARCHAR(255) = NULL,
    @Estado NVARCHAR(20)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IMC DECIMAL(5,2) = NULL;

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

    IF @DataAvaliacao IS NULL
    BEGIN
        THROW 50003,
              'A data da avaliação é obrigatória.',
              1;
    END;

    IF @Estado NOT IN
    (
        N'Agendada',
        N'Concluída',
        N'Cancelada'
    )
    BEGIN
        THROW 50004,
              'O estado da avaliação não é válido.',
              1;
    END;

    IF @Estado = N'Concluída'
    BEGIN
        IF @Peso IS NULL OR @Peso <= 0
        BEGIN
            THROW 50005,
                  'O peso é obrigatório numa avaliação concluída.',
                  1;
        END;

        IF @Altura IS NULL OR @Altura <= 0
        BEGIN
            THROW 50006,
                  'A altura é obrigatória numa avaliação concluída.',
                  1;
        END;

        IF @MassaGorda IS NULL
           OR @MassaGorda < 0
           OR @MassaGorda > 100
        BEGIN
            THROW 50007,
                  'A massa gorda deve estar entre 0 e 100.',
                  1;
        END;

        IF @MassaMuscular IS NULL
           OR @MassaMuscular <= 0
        BEGIN
            THROW 50008,
                  'A massa muscular é obrigatória numa avaliação concluída.',
                  1;
        END;

        SET @IMC =
            ROUND(
                @Peso /
                NULLIF(@Altura * @Altura, 0),
                2
            );
    END;
    ELSE
    BEGIN
        SET @Peso = NULL;
        SET @Altura = NULL;
        SET @IMC = NULL;
        SET @MassaGorda = NULL;
        SET @MassaMuscular = NULL;
    END;

    INSERT INTO dbo.AvaliacoesFisicas
    (
        IdCliente,
        IdPT,
        DataAvaliacao,
        Peso,
        Altura,
        IMC,
        MassaGorda,
        MassaMuscular,
        Observacoes,
        Estado
    )
    VALUES
    (
        @IdCliente,
        @IdPT,
        @DataAvaliacao,
        @Peso,
        @Altura,
        @IMC,
        @MassaGorda,
        @MassaMuscular,
        @Observacoes,
        @Estado
    );
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_AvaliacoesFisicas_Atualizar
(
    @IdAvaliacao INT,
    @IdCliente INT,
    @IdPT INT,
    @DataAvaliacao DATE,
    @Peso DECIMAL(5,2) = NULL,
    @Altura DECIMAL(4,2) = NULL,
    @MassaGorda DECIMAL(5,2) = NULL,
    @MassaMuscular DECIMAL(5,2) = NULL,
    @Observacoes NVARCHAR(255) = NULL,
    @Estado NVARCHAR(20)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IMC DECIMAL(5,2) = NULL;

    SET @Observacoes =
        NULLIF(
            LTRIM(RTRIM(@Observacoes)),
            N''
        );

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.AvaliacoesFisicas
        WHERE IdAvaliacao = @IdAvaliacao
    )
    BEGIN
        THROW 50001,
              'A avaliação física indicada não existe.',
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

    IF @DataAvaliacao IS NULL
    BEGIN
        THROW 50004,
              'A data da avaliação é obrigatória.',
              1;
    END;

    IF @Estado NOT IN
    (
        N'Agendada',
        N'Concluída',
        N'Cancelada'
    )
    BEGIN
        THROW 50005,
              'O estado da avaliação não é válido.',
              1;
    END;

    IF @Estado = N'Concluída'
    BEGIN
        IF @Peso IS NULL OR @Peso <= 0
        BEGIN
            THROW 50006,
                  'O peso é obrigatório numa avaliação concluída.',
                  1;
        END;

        IF @Altura IS NULL OR @Altura <= 0
        BEGIN
            THROW 50007,
                  'A altura é obrigatória numa avaliação concluída.',
                  1;
        END;

        IF @MassaGorda IS NULL
           OR @MassaGorda < 0
           OR @MassaGorda > 100
        BEGIN
            THROW 50008,
                  'A massa gorda deve estar entre 0 e 100.',
                  1;
        END;

        IF @MassaMuscular IS NULL
           OR @MassaMuscular <= 0
        BEGIN
            THROW 50009,
                  'A massa muscular é obrigatória numa avaliação concluída.',
                  1;
        END;

        SET @IMC =
            ROUND(
                @Peso /
                NULLIF(@Altura * @Altura, 0),
                2
            );
    END;
    ELSE
    BEGIN
        SET @Peso = NULL;
        SET @Altura = NULL;
        SET @IMC = NULL;
        SET @MassaGorda = NULL;
        SET @MassaMuscular = NULL;
    END;

    UPDATE dbo.AvaliacoesFisicas
    SET
        IdCliente = @IdCliente,
        IdPT = @IdPT,
        DataAvaliacao = @DataAvaliacao,
        Peso = @Peso,
        Altura = @Altura,
        IMC = @IMC,
        MassaGorda = @MassaGorda,
        MassaMuscular = @MassaMuscular,
        Observacoes = @Observacoes,
        Estado = @Estado
    WHERE IdAvaliacao = @IdAvaliacao;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_AvaliacoesFisicas_Eliminar
(
    @IdAvaliacao INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Estado NVARCHAR(20);

    SELECT
        @Estado = Estado
    FROM dbo.AvaliacoesFisicas
    WHERE IdAvaliacao = @IdAvaliacao;

    IF @Estado IS NULL
    BEGIN
        THROW 50001,
              'A avaliação física indicada não existe.',
              1;
    END;

    IF @Estado = N'Concluída'
    BEGIN
        THROW 50002,
              'Uma avaliação concluída não pode ser eliminada.',
              1;
    END;

    DELETE FROM dbo.AvaliacoesFisicas
    WHERE IdAvaliacao = @IdAvaliacao;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_AvaliacoesFisicas_Pesquisar
(
    @Pesquisa NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Pesquisa =
        LTRIM(RTRIM(@Pesquisa));

    SELECT
        AF.IdAvaliacao,
        AF.IdCliente,
        C.Nome AS NomeCliente,
        AF.IdPT,
        PT.Nome AS NomePT,
        AF.DataAvaliacao,
        AF.Peso,
        AF.Altura,
        AF.IMC,
        AF.MassaGorda,
        AF.MassaMuscular,
        AF.Observacoes,
        AF.Estado
    FROM dbo.AvaliacoesFisicas AS AF
    INNER JOIN dbo.Clientes AS C
        ON C.IdCliente = AF.IdCliente
    INNER JOIN dbo.PersonalTrainers AS PT
        ON PT.IdPT = AF.IdPT
    WHERE
        C.Nome LIKE N'%' + @Pesquisa + N'%'
        OR PT.Nome LIKE N'%' + @Pesquisa + N'%'
        OR AF.Estado LIKE N'%' + @Pesquisa + N'%'
        OR AF.Observacoes LIKE N'%' + @Pesquisa + N'%'
        OR CONVERT(
               NVARCHAR(10),
               AF.DataAvaliacao,
               103
           ) LIKE N'%' + @Pesquisa + N'%'
        OR CAST(AF.Peso AS NVARCHAR(20))
           LIKE N'%' + @Pesquisa + N'%'
        OR CAST(AF.Altura AS NVARCHAR(20))
           LIKE N'%' + @Pesquisa + N'%'
        OR CAST(AF.IMC AS NVARCHAR(20))
           LIKE N'%' + @Pesquisa + N'%'
        OR CAST(AF.MassaGorda AS NVARCHAR(20))
           LIKE N'%' + @Pesquisa + N'%'
        OR CAST(AF.MassaMuscular AS NVARCHAR(20))
           LIKE N'%' + @Pesquisa + N'%'
    ORDER BY
        AF.DataAvaliacao DESC,
        AF.IdAvaliacao DESC;
END;
GO