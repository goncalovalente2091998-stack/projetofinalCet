CREATE OR ALTER PROCEDURE sp_Aulas_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdAula,
        IdProfessor,
        Nome,
        Horario,
        Lotacao,
        Sala
    FROM Aulas
    ORDER BY Nome;
END;
GO

CREATE OR ALTER PROCEDURE sp_Aulas_ObterPorId
(
    @IdAula INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdAula,
        IdProfessor,
        Nome,
        Horario,
        Lotacao,
        Sala
    FROM Aulas
    WHERE IdAula = @IdAula;
END;
GO

CREATE OR ALTER PROCEDURE sp_Aulas_Inserir
(
    @IdProfessor INT,
    @Nome NVARCHAR(100),
    @Horario NVARCHAR(50),
    @Lotacao INT,
    @Sala NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Aulas
    (
        IdProfessor,
        Nome,
        Horario,
        Lotacao,
        Sala
    )
    VALUES
    (
        @IdProfessor,
        @Nome,
        @Horario,
        @Lotacao,
        @Sala
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Aulas_Atualizar
(
    @IdAula INT,
    @IdProfessor INT,
    @Nome NVARCHAR(100),
    @Horario NVARCHAR(50),
    @Lotacao INT,
    @Sala NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Aulas
    SET
        IdProfessor = @IdProfessor,
        Nome = @Nome,
        Horario = @Horario,
        Lotacao = @Lotacao,
        Sala = @Sala
    WHERE IdAula = @IdAula;
END;
GO

CREATE OR ALTER PROCEDURE sp_Aulas_Eliminar
(
    @IdAula INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Aulas
    WHERE IdAula = @IdAula;
END;
GO