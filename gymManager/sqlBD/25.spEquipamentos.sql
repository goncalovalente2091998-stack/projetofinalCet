CREATE OR ALTER PROCEDURE dbo.sp_Equipamentos_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdEquipamento,
        Nome,
        Categoria,
        Marca,
        Modelo,
        NumeroSerie,
        DataAquisicao,
        Localizacao,
        Estado,
        Observacoes
    FROM dbo.Equipamentos
    ORDER BY
        Nome ASC,
        IdEquipamento ASC;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_Equipamentos_ObterPorId
(
    @IdEquipamento INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdEquipamento,
        Nome,
        Categoria,
        Marca,
        Modelo,
        NumeroSerie,
        DataAquisicao,
        Localizacao,
        Estado,
        Observacoes
    FROM dbo.Equipamentos
    WHERE IdEquipamento = @IdEquipamento;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Equipamentos_Inserir
(
    @Nome NVARCHAR(100),
    @Categoria NVARCHAR(50),
    @Marca NVARCHAR(50) = NULL,
    @Modelo NVARCHAR(50) = NULL,
    @NumeroSerie NVARCHAR(100) = NULL,
    @DataAquisicao DATE = NULL,
    @Localizacao NVARCHAR(100),
    @Estado NVARCHAR(30),
    @Observacoes NVARCHAR(500) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Nome = LTRIM(RTRIM(@Nome));
    SET @Categoria = LTRIM(RTRIM(@Categoria));
    SET @Marca = NULLIF(LTRIM(RTRIM(@Marca)), N'');
    SET @Modelo = NULLIF(LTRIM(RTRIM(@Modelo)), N'');
    SET @NumeroSerie = NULLIF(LTRIM(RTRIM(@NumeroSerie)), N'');
    SET @Localizacao = LTRIM(RTRIM(@Localizacao));
    SET @Observacoes = NULLIF(LTRIM(RTRIM(@Observacoes)), N'');

    IF NULLIF(@Nome, N'') IS NULL
        THROW 50001, 'O nome do equipamento é obrigatório.', 1;

    IF NULLIF(@Categoria, N'') IS NULL
        THROW 50002, 'A categoria é obrigatória.', 1;

    IF NULLIF(@Localizacao, N'') IS NULL
        THROW 50003, 'A localização é obrigatória.', 1;

    IF @Estado NOT IN
    (
        N'Operacional',
        N'Em manutenção',
        N'Fora de serviço',
        N'Abatido'
    )
    BEGIN
        THROW 50004, 'O estado indicado não é válido.', 1;
    END;

    IF @NumeroSerie IS NOT NULL
       AND EXISTS
       (
           SELECT 1
           FROM dbo.Equipamentos
           WHERE NumeroSerie = @NumeroSerie
       )
    BEGIN
        THROW 50005, 'Já existe um equipamento com este número de série.', 1;
    END;

    INSERT INTO dbo.Equipamentos
    (
        Nome,
        Categoria,
        Marca,
        Modelo,
        NumeroSerie,
        DataAquisicao,
        Localizacao,
        Estado,
        Observacoes
    )
    VALUES
    (
        @Nome,
        @Categoria,
        @Marca,
        @Modelo,
        @NumeroSerie,
        @DataAquisicao,
        @Localizacao,
        @Estado,
        @Observacoes
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Equipamentos_Atualizar
(
    @IdEquipamento INT,
    @Nome NVARCHAR(100),
    @Categoria NVARCHAR(50),
    @Marca NVARCHAR(50) = NULL,
    @Modelo NVARCHAR(50) = NULL,
    @NumeroSerie NVARCHAR(100) = NULL,
    @DataAquisicao DATE = NULL,
    @Localizacao NVARCHAR(100),
    @Estado NVARCHAR(30),
    @Observacoes NVARCHAR(500) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Nome = LTRIM(RTRIM(@Nome));
    SET @Categoria = LTRIM(RTRIM(@Categoria));
    SET @Marca = NULLIF(LTRIM(RTRIM(@Marca)), N'');
    SET @Modelo = NULLIF(LTRIM(RTRIM(@Modelo)), N'');
    SET @NumeroSerie = NULLIF(LTRIM(RTRIM(@NumeroSerie)), N'');
    SET @Localizacao = LTRIM(RTRIM(@Localizacao));
    SET @Observacoes = NULLIF(LTRIM(RTRIM(@Observacoes)), N'');

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Equipamentos
        WHERE IdEquipamento = @IdEquipamento
    )
    BEGIN
        THROW 50001, 'O equipamento indicado não existe.', 1;
    END;

    IF NULLIF(@Nome, N'') IS NULL
        THROW 50002, 'O nome do equipamento é obrigatório.', 1;

    IF NULLIF(@Categoria, N'') IS NULL
        THROW 50003, 'A categoria é obrigatória.', 1;

    IF NULLIF(@Localizacao, N'') IS NULL
        THROW 50004, 'A localização é obrigatória.', 1;

    IF @Estado NOT IN
    (
        N'Operacional',
        N'Em manutenção',
        N'Fora de serviço',
        N'Abatido'
    )
    BEGIN
        THROW 50005, 'O estado indicado não é válido.', 1;
    END;

    IF @NumeroSerie IS NOT NULL
       AND EXISTS
       (
           SELECT 1
           FROM dbo.Equipamentos
           WHERE NumeroSerie = @NumeroSerie
             AND IdEquipamento <> @IdEquipamento
       )
    BEGIN
        THROW 50006, 'Já existe outro equipamento com este número de série.', 1;
    END;

    UPDATE dbo.Equipamentos
    SET
        Nome = @Nome,
        Categoria = @Categoria,
        Marca = @Marca,
        Modelo = @Modelo,
        NumeroSerie = @NumeroSerie,
        DataAquisicao = @DataAquisicao,
        Localizacao = @Localizacao,
        Estado = @Estado,
        Observacoes = @Observacoes
    WHERE IdEquipamento = @IdEquipamento;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Equipamentos_Eliminar
(
    @IdEquipamento INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Estado NVARCHAR(30);

    SELECT
        @Estado = Estado
    FROM dbo.Equipamentos
    WHERE IdEquipamento = @IdEquipamento;

    IF @Estado IS NULL
    BEGIN
        THROW 50001, 'O equipamento indicado não existe.', 1;
    END;

    IF @Estado = N'Em manutenção'
    BEGIN
        THROW 50002, 'Um equipamento em manutenção não pode ser eliminado.', 1;
    END;

    IF @Estado = N'Abatido'
    BEGIN
        THROW 50003, 'Um equipamento abatido não pode ser eliminado.', 1;
    END;

    IF OBJECT_ID(N'dbo.Manutencoes', N'U') IS NOT NULL
       AND EXISTS
       (
           SELECT 1
           FROM dbo.Manutencoes
           WHERE IdEquipamento = @IdEquipamento
       )
    BEGIN
        THROW 50004,
              'O equipamento possui registos de manutenção e não pode ser eliminado.',
              1;
    END;

    DELETE FROM dbo.Equipamentos
    WHERE IdEquipamento = @IdEquipamento;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Equipamentos_Pesquisar
(
    @Pesquisa NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Pesquisa = LTRIM(RTRIM(@Pesquisa));

    SELECT
        IdEquipamento,
        Nome,
        Categoria,
        Marca,
        Modelo,
        NumeroSerie,
        DataAquisicao,
        Localizacao,
        Estado,
        Observacoes
    FROM dbo.Equipamentos
    WHERE
        Nome LIKE N'%' + @Pesquisa + N'%'
        OR Categoria LIKE N'%' + @Pesquisa + N'%'
        OR Marca LIKE N'%' + @Pesquisa + N'%'
        OR Modelo LIKE N'%' + @Pesquisa + N'%'
        OR NumeroSerie LIKE N'%' + @Pesquisa + N'%'
        OR Localizacao LIKE N'%' + @Pesquisa + N'%'
        OR Estado LIKE N'%' + @Pesquisa + N'%'
        OR Observacoes LIKE N'%' + @Pesquisa + N'%'
        OR CONVERT(
               NVARCHAR(10),
               DataAquisicao,
               103
           ) LIKE N'%' + @Pesquisa + N'%'
    ORDER BY
        Nome ASC,
        IdEquipamento ASC;
END;
GO