CREATE OR ALTER PROCEDURE dbo.sp_Exercicios_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdExercicio,
        Nome,
        GrupoMuscular,
        Equipamento,
        Descricao,
        Dificuldade,
        Estado
    FROM dbo.Exercicios
    ORDER BY
        Nome ASC,
        IdExercicio ASC;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_Exercicios_ObterPorId
(
    @IdExercicio INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdExercicio,
        Nome,
        GrupoMuscular,
        Equipamento,
        Descricao,
        Dificuldade,
        Estado
    FROM dbo.Exercicios
    WHERE IdExercicio = @IdExercicio;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_Exercicios_Inserir
(
    @Nome NVARCHAR(100),
    @GrupoMuscular NVARCHAR(50),
    @Equipamento NVARCHAR(100),
    @Descricao NVARCHAR(500),
    @Dificuldade NVARCHAR(20),
    @Estado NVARCHAR(20)
)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Nome =
        LTRIM(RTRIM(@Nome));

    SET @GrupoMuscular =
        LTRIM(RTRIM(@GrupoMuscular));

    SET @Equipamento =
        NULLIF(
            LTRIM(RTRIM(@Equipamento)),
            N''
        );

    SET @Descricao =
        NULLIF(
            LTRIM(RTRIM(@Descricao)),
            N''
        );

    IF NULLIF(@Nome, N'') IS NULL
    BEGIN
        THROW 50001,
              'O nome do exercício é obrigatório.',
              1;
    END;

    IF NULLIF(@GrupoMuscular, N'') IS NULL
    BEGIN
        THROW 50002,
              'O grupo muscular é obrigatório.',
              1;
    END;

    IF @Dificuldade NOT IN
    (
        N'Iniciante',
        N'Intermédio',
        N'Avançado'
    )
    BEGIN
        THROW 50003,
              'A dificuldade indicada não é válida.',
              1;
    END;

    IF @Estado NOT IN
    (
        N'Ativo',
        N'Inativo'
    )
    BEGIN
        THROW 50004,
              'O estado indicado não é válido.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Exercicios
        WHERE Nome = @Nome
    )
    BEGIN
        THROW 50005,
              'Já existe um exercício com este nome.',
              1;
    END;

    INSERT INTO dbo.Exercicios
    (
        Nome,
        GrupoMuscular,
        Equipamento,
        Descricao,
        Dificuldade,
        Estado
    )
    VALUES
    (
        @Nome,
        @GrupoMuscular,
        @Equipamento,
        @Descricao,
        @Dificuldade,
        @Estado
    );
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_Exercicios_Atualizar
(
    @IdExercicio INT,
    @Nome NVARCHAR(100),
    @GrupoMuscular NVARCHAR(50),
    @Equipamento NVARCHAR(100),
    @Descricao NVARCHAR(500),
    @Dificuldade NVARCHAR(20),
    @Estado NVARCHAR(20)
)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Nome =
        LTRIM(RTRIM(@Nome));

    SET @GrupoMuscular =
        LTRIM(RTRIM(@GrupoMuscular));

    SET @Equipamento =
        NULLIF(
            LTRIM(RTRIM(@Equipamento)),
            N''
        );

    SET @Descricao =
        NULLIF(
            LTRIM(RTRIM(@Descricao)),
            N''
        );

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Exercicios
        WHERE IdExercicio = @IdExercicio
    )
    BEGIN
        THROW 50001,
              'O exercício indicado não existe.',
              1;
    END;

    IF NULLIF(@Nome, N'') IS NULL
    BEGIN
        THROW 50002,
              'O nome do exercício é obrigatório.',
              1;
    END;

    IF NULLIF(@GrupoMuscular, N'') IS NULL
    BEGIN
        THROW 50003,
              'O grupo muscular é obrigatório.',
              1;
    END;

    IF @Dificuldade NOT IN
    (
        N'Iniciante',
        N'Intermédio',
        N'Avançado'
    )
    BEGIN
        THROW 50004,
              'A dificuldade indicada não é válida.',
              1;
    END;

    IF @Estado NOT IN
    (
        N'Ativo',
        N'Inativo'
    )
    BEGIN
        THROW 50005,
              'O estado indicado não é válido.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Exercicios
        WHERE Nome = @Nome
          AND IdExercicio <> @IdExercicio
    )
    BEGIN
        THROW 50006,
              'Já existe outro exercício com este nome.',
              1;
    END;

    UPDATE dbo.Exercicios
    SET
        Nome = @Nome,
        GrupoMuscular = @GrupoMuscular,
        Equipamento = @Equipamento,
        Descricao = @Descricao,
        Dificuldade = @Dificuldade,
        Estado = @Estado
    WHERE IdExercicio = @IdExercicio;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_Exercicios_Eliminar
(
    @IdExercicio INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Exercicios
        WHERE IdExercicio = @IdExercicio
    )
    BEGIN
        THROW 50001,
              'O exercício indicado não existe.',
              1;
    END;

    /*
        Esta verificação será usada quando criarmos
        a tabela PlanoTreinoExercicios.
    */
    IF OBJECT_ID(
           N'dbo.PlanoTreinoExercicios',
           N'U'
       ) IS NOT NULL
    BEGIN
        IF EXISTS
        (
            SELECT 1
            FROM dbo.PlanoTreinoExercicios
            WHERE IdExercicio = @IdExercicio
        )
        BEGIN
            THROW 50002,
                  'Este exercício está associado a um plano de treino e não pode ser eliminado.',
                  1;
        END;
    END;

    DELETE FROM dbo.Exercicios
    WHERE IdExercicio = @IdExercicio;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Exercicios_Pesquisar
(
    @Pesquisa NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Pesquisa =
        LTRIM(RTRIM(@Pesquisa));

    SELECT
        IdExercicio,
        Nome,
        GrupoMuscular,
        Equipamento,
        Descricao,
        Dificuldade,
        Estado
    FROM dbo.Exercicios
    WHERE Nome LIKE
              N'%' + @Pesquisa + N'%'

       OR GrupoMuscular LIKE
              N'%' + @Pesquisa + N'%'

       OR Equipamento LIKE
              N'%' + @Pesquisa + N'%'

       OR Descricao LIKE
              N'%' + @Pesquisa + N'%'

       OR Dificuldade LIKE
              N'%' + @Pesquisa + N'%'

       OR Estado LIKE
              N'%' + @Pesquisa + N'%'

    ORDER BY
        Nome ASC,
        IdExercicio ASC;
END;
GO