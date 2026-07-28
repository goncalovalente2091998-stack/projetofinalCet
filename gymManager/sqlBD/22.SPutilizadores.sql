CREATE OR ALTER PROCEDURE sp_Utilizadores_Login
(
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdUtilizador,
        Nome,
        Email,
        Perfil
    FROM Utilizadores
    WHERE Email = @Email
      AND PasswordHash = @PasswordHash;
END;
GO

CREATE OR ALTER PROCEDURE sp_Utilizadores_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdUtilizador,
        Nome,
        Email,
        Perfil
    FROM Utilizadores
    ORDER BY Nome;
END;
GO

CREATE OR ALTER PROCEDURE sp_Utilizadores_ObterPorId
(
    @IdUtilizador INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdUtilizador,
        Nome,
        Email,
        Perfil
    FROM Utilizadores
    WHERE IdUtilizador = @IdUtilizador;
END;
GO

CREATE OR ALTER PROCEDURE sp_Utilizadores_Inserir
(
    @Nome NVARCHAR(100),
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @Perfil NVARCHAR(30)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Utilizadores WHERE Email = @Email)
    BEGIN
        RAISERROR('Já existe um utilizador com este email.', 16, 1);
        RETURN;
    END;

    INSERT INTO Utilizadores
    (
        Nome,
        Email,
        PasswordHash,
        Perfil
    )
    VALUES
    (
        @Nome,
        @Email,
        @PasswordHash,
        @Perfil
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Utilizadores_Atualizar
(
    @IdUtilizador INT,
    @Nome NVARCHAR(100),
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @Perfil NVARCHAR(30)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM Utilizadores
        WHERE Email = @Email
          AND IdUtilizador <> @IdUtilizador
    )
    BEGIN
        RAISERROR('Já existe outro utilizador com este email.', 16, 1);
        RETURN;
    END;

    UPDATE Utilizadores
    SET
        Nome = @Nome,
        Email = @Email,
        PasswordHash = @PasswordHash,
        Perfil = @Perfil
    WHERE IdUtilizador = @IdUtilizador;
END;
GO

CREATE OR ALTER PROCEDURE sp_Utilizadores_Eliminar
(
    @IdUtilizador INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Utilizadores
    WHERE IdUtilizador = @IdUtilizador;
END;
GO