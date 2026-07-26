CREATE OR ALTER PROCEDURE sp_PersonalTrainers_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdPT,
        Nome,
        Especialidade,
        Telefone,
        Email,
        ValorHora,
        Estado
    FROM PersonalTrainers
    ORDER BY Nome;
END;
GO

CREATE OR ALTER PROCEDURE sp_PersonalTrainers_ObterPorId
(
    @IdPT INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdPT,
        Nome,
        Especialidade,
        Telefone,
        Email,
        ValorHora,
        Estado
    FROM PersonalTrainers
    WHERE IdPT = @IdPT;
END;
GO

CREATE OR ALTER PROCEDURE sp_PersonalTrainers_Inserir
(
    @Nome NVARCHAR(100),
    @Especialidade NVARCHAR(100),
    @Telefone NVARCHAR(20),
    @Email NVARCHAR(100),
    @ValorHora DECIMAL(10,2),
    @Estado BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO PersonalTrainers
    (
        Nome,
        Especialidade,
        Telefone,
        Email,
        ValorHora,
        Estado
    )
    VALUES
    (
        @Nome,
        @Especialidade,
        @Telefone,
        @Email,
        @ValorHora,
        @Estado
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_PersonalTrainers_Atualizar
(
    @IdPT INT,
    @Nome NVARCHAR(100),
    @Especialidade NVARCHAR(100),
    @Telefone NVARCHAR(20),
    @Email NVARCHAR(100),
    @ValorHora DECIMAL(10,2),
    @Estado BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE PersonalTrainers
    SET
        Nome = @Nome,
        Especialidade = @Especialidade,
        Telefone = @Telefone,
        Email = @Email,
        ValorHora = @ValorHora,
        Estado = @Estado
    WHERE IdPT = @IdPT;
END;
GO

CREATE OR ALTER PROCEDURE sp_PersonalTrainers_Eliminar
(
    @IdPT INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM PersonalTrainers
    WHERE IdPT = @IdPT;
END;
GO

CREATE OR ALTER PROCEDURE sp_PersonalTrainers_Pesquisar
(
    @Pesquisa NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdPT,
        Nome,
        Especialidade,
        Telefone,
        Email,
        ValorHora,
        Estado
    FROM PersonalTrainers
    WHERE Nome LIKE '%' + @Pesquisa + '%'
       OR Especialidade LIKE '%' + @Pesquisa + '%'
       OR Telefone LIKE '%' + @Pesquisa + '%'
       OR Email LIKE '%' + @Pesquisa + '%'
    ORDER BY Nome;
END;
GO