CREATE PROCEDURE sp_Utilizadores_Login
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
        Perfil
    FROM Utilizadores
    WHERE Email = @Email
      AND PasswordHash = @PasswordHash;
END;
GO

CREATE OR ALTER PROCEDURE sp_Clientes_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdCliente,
        Nome,
        NIF,
        DataNascimento,
        Telefone,
        Email,
        Morada,
        DataInscricao,
        Estado
    FROM Clientes
    ORDER BY Nome;
END;
GO

CREATE OR ALTER PROCEDURE sp_Clientes_ObterPorId
(
    @IdCliente INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Clientes
    WHERE IdCliente = @IdCliente;
END;
GO

CREATE OR ALTER PROCEDURE sp_Clientes_Inserir
(
    @Nome NVARCHAR(100),
    @NIF CHAR(9),
    @DataNascimento DATE,
    @Telefone NVARCHAR(20),
    @Email NVARCHAR(100),
    @Morada NVARCHAR(200),
    @DataInscricao DATE,
    @Estado BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Clientes
    (
        Nome,
        NIF,
        DataNascimento,
        Telefone,
        Email,
        Morada,
        DataInscricao,
        Estado
    )
    VALUES
    (
        @Nome,
        @NIF,
        @DataNascimento,
        @Telefone,
        @Email,
        @Morada,
        @DataInscricao,
        @Estado
    );
END;
GO


EXEC sp_Clientes_Inserir
    @Nome='Jo�o Silva',
    @NIF='123456789',
    @DataNascimento='1999-05-10',
    @Telefone='912345678',
    @Email='joao@email.pt',
    @Morada='Rua Principal',
    @DataInscricao= '2026-07-25',
    @Estado=1;

    CREATE OR ALTER PROCEDURE sp_Clientes_Atualizar
(
    @IdCliente INT,
    @Nome NVARCHAR(100),
    @NIF CHAR(9),
    @DataNascimento DATE,
    @Telefone NVARCHAR(20),
    @Email NVARCHAR(100),
    @Morada NVARCHAR(200),
    @DataInscricao DATE,
    @Estado BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Clientes
    SET
        Nome = @Nome,
        NIF = @NIF,
        DataNascimento = @DataNascimento,
        Telefone = @Telefone,
        Email = @Email,
        Morada = @Morada,
        DataInscricao = @DataInscricao,
        Estado = @Estado
    WHERE IdCliente = @IdCliente;
END;
GO

CREATE OR ALTER PROCEDURE sp_Clientes_Eliminar
(
    @IdCliente INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Clientes
    WHERE IdCliente = @IdCliente;
END;
GO

CREATE OR ALTER PROCEDURE sp_Clientes_Pesquisar
(
    @Pesquisa NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Clientes
    WHERE Nome LIKE '%' + @Pesquisa + '%'
       OR NIF LIKE '%' + @Pesquisa + '%'
       OR Telefone LIKE '%' + @Pesquisa + '%'
       OR Email LIKE '%' + @Pesquisa + '%'
    ORDER BY Nome;
END;
GO