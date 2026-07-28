CREATE OR ALTER PROCEDURE sp_Professores_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdProfessor,
        Nome,
        Especialidade,
        Telefone,
        Email
    FROM Professores
    ORDER BY Nome;
END;
GO

CREATE OR ALTER PROCEDURE sp_Professores_ObterPorId
(
    @IdProfessor INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdProfessor,
        Nome,
        Especialidade,
        Telefone,
        Email
    FROM Professores
    WHERE IdProfessor = @IdProfessor;
END;
GO

CREATE OR ALTER PROCEDURE sp_Professores_Inserir
(
    @Nome NVARCHAR(100),
    @Especialidade NVARCHAR(100),
    @Telefone NVARCHAR(20),
    @Email NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Professores
    (
        Nome,
        Especialidade,
        Telefone,
        Email
    )
    VALUES
    (
        @Nome,
        @Especialidade,
        @Telefone,
        @Email
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Professores_Atualizar
(
    @IdProfessor INT,
    @Nome NVARCHAR(100),
    @Especialidade NVARCHAR(100),
    @Telefone NVARCHAR(20),
    @Email NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Professores
    SET
        Nome = @Nome,
        Especialidade = @Especialidade,
        Telefone = @Telefone,
        Email = @Email
    WHERE IdProfessor = @IdProfessor;
END;
GO

CREATE OR ALTER PROCEDURE sp_Professores_Atualizar
(
    @IdProfessor INT,
    @Nome NVARCHAR(100),
    @Especialidade NVARCHAR(100),
    @Telefone NVARCHAR(20),
    @Email NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Professores
    SET
        Nome = @Nome,
        Especialidade = @Especialidade,
        Telefone = @Telefone,
        Email = @Email
    WHERE IdProfessor = @IdProfessor;
END;
GO
CREATE OR ALTER PROCEDURE sp_Professores_Pesquisar
(
    @Pesquisa NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdProfessor,
        Nome,
        Especialidade,
        Telefone,
        Email
    FROM Professores
    WHERE Nome LIKE '%' + @Pesquisa + '%'
       OR Especialidade LIKE '%' + @Pesquisa + '%'
       OR Telefone LIKE '%' + @Pesquisa + '%'
       OR Email LIKE '%' + @Pesquisa + '%'
    ORDER BY Nome;
END;
GO