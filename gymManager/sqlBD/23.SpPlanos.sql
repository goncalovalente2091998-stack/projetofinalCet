CREATE OR ALTER PROCEDURE sp_Planos_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdPlano,
        Nome,
        Preco,
        DuracaoMeses,
        Descricao
    FROM Planos
    ORDER BY Nome;
END;
GO

CREATE OR ALTER PROCEDURE sp_Planos_ObterPorId
(
    @IdPlano INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdPlano,
        Nome,
        Preco,
        DuracaoMeses,
        Descricao
    FROM Planos
    WHERE IdPlano = @IdPlano;
END;
GO

CREATE OR ALTER PROCEDURE sp_Planos_Inserir
(
    @Nome NVARCHAR(100),
    @Preco DECIMAL(10,2),
    @DuracaoMeses INT,
    @Descricao NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Planos
    (
        Nome,
        Preco,
        DuracaoMeses,
        Descricao
    )
    VALUES
    (
        @Nome,
        @Preco,
        @DuracaoMeses,
        @Descricao
    );
END;
GO

CREATE OR ALTER PROCEDURE sp_Planos_Atualizar
(
    @IdPlano INT,
    @Nome NVARCHAR(100),
    @Preco DECIMAL(10,2),
    @DuracaoMeses INT,
    @Descricao NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Planos
    SET
        Nome = @Nome,
        Preco = @Preco,
        DuracaoMeses = @DuracaoMeses,
        Descricao = @Descricao
    WHERE IdPlano = @IdPlano;
END;
GO

CREATE OR ALTER PROCEDURE sp_Planos_Eliminar
(
    @IdPlano INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Planos
    WHERE IdPlano = @IdPlano;
END;
GO

CREATE OR ALTER PROCEDURE sp_Planos_Pesquisar
(
    @Pesquisa NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdPlano,
        Nome,
        Preco,
        DuracaoMeses,
        Descricao
    FROM Planos
    WHERE Nome LIKE '%' + @Pesquisa + '%'
       OR Descricao LIKE '%' + @Pesquisa + '%'
       OR CAST(Preco AS NVARCHAR(20)) LIKE '%' + @Pesquisa + '%'
       OR CAST(DuracaoMeses AS NVARCHAR(10)) LIKE '%' + @Pesquisa + '%'
    ORDER BY Nome;
END;
GO