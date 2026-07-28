CREATE OR ALTER PROCEDURE dbo.sp_ReservasAulas_ListarPorAula
(
    @IdAula INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Aulas
        WHERE IdAula = @IdAula
    )
    BEGIN
        THROW 50001,
              'A aula indicada não existe.',
              1;
    END;

    SELECT
        R.IdReserva,
        R.IdAula,
        A.Nome AS NomeAula,
        A.DataAula,
        A.HoraInicio,
        A.Sala,

        R.IdCliente,
        C.Nome AS NomeCliente,
        C.NIF,

        R.DataReserva,
        R.Estado
    FROM dbo.ReservasAulas AS R

    INNER JOIN dbo.Aulas AS A
        ON A.IdAula = R.IdAula

    INNER JOIN dbo.Clientes AS C
        ON C.IdCliente = R.IdCliente

    WHERE R.IdAula = @IdAula

    ORDER BY
        CASE R.Estado
            WHEN N'Confirmada' THEN 1
            WHEN N'Presente' THEN 2
            WHEN N'Faltou' THEN 3
            WHEN N'Cancelada' THEN 4
            ELSE 5
        END,
        C.Nome ASC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ReservasAulas_Pesquisar
(
    @Pesquisa NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Pesquisa =
        LTRIM(RTRIM(@Pesquisa));

    SELECT
        R.IdReserva,
        R.IdAula,
        A.Nome AS NomeAula,
        A.DataAula,
        A.HoraInicio,
        A.Sala,

        R.IdCliente,
        C.Nome AS NomeCliente,
        C.NIF,

        R.DataReserva,
        R.Estado
    FROM dbo.ReservasAulas AS R

    INNER JOIN dbo.Aulas AS A
        ON A.IdAula = R.IdAula

    INNER JOIN dbo.Clientes AS C
        ON C.IdCliente = R.IdCliente

    WHERE C.Nome LIKE
              N'%' + @Pesquisa + N'%'

       OR C.NIF LIKE
              N'%' + @Pesquisa + N'%'

       OR A.Nome LIKE
              N'%' + @Pesquisa + N'%'

       OR A.Sala LIKE
              N'%' + @Pesquisa + N'%'

       OR R.Estado LIKE
              N'%' + @Pesquisa + N'%'

       OR CONVERT(
              NVARCHAR(10),
              A.DataAula,
              103
          ) LIKE
              N'%' + @Pesquisa + N'%'

       OR CONVERT(
              NVARCHAR(5),
              A.HoraInicio,
              108
          ) LIKE
              N'%' + @Pesquisa + N'%'

    ORDER BY
        A.DataAula DESC,
        A.HoraInicio DESC,
        R.IdReserva DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ReservasAulas_Cancelar
(
    @IdReserva INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoAtual NVARCHAR(20);
    DECLARE @DataAula DATE;
    DECLARE @HoraInicio TIME;

    SELECT
        @EstadoAtual = R.Estado,
        @DataAula = A.DataAula,
        @HoraInicio = A.HoraInicio
    FROM dbo.ReservasAulas AS R

    INNER JOIN dbo.Aulas AS A
        ON A.IdAula = R.IdAula

    WHERE R.IdReserva = @IdReserva;

    IF @EstadoAtual IS NULL
    BEGIN
        THROW 50001,
              'A reserva indicada não existe.',
              1;
    END;

    IF @EstadoAtual = N'Cancelada'
    BEGIN
        THROW 50002,
              'Esta reserva já está cancelada.',
              1;
    END;

    IF @EstadoAtual IN
    (
        N'Presente',
        N'Faltou'
    )
    BEGIN
        THROW 50003,
              'Uma reserva com presença já registada não pode ser cancelada.',
              1;
    END;

    IF
    (
        @DataAula < CAST(GETDATE() AS DATE)
        OR
        (
            @DataAula = CAST(GETDATE() AS DATE)
            AND @HoraInicio <= CAST(GETDATE() AS TIME)
        )
    )
    BEGIN
        THROW 50004,
              'Não é possível cancelar uma reserva depois do início da aula.',
              1;
    END;

    UPDATE dbo.ReservasAulas
    SET Estado = N'Cancelada'
    WHERE IdReserva = @IdReserva;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ReservasAulas_MarcarPresente
(
    @IdReserva INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoAtual NVARCHAR(20);

    SELECT
        @EstadoAtual = Estado
    FROM dbo.ReservasAulas
    WHERE IdReserva = @IdReserva;

    IF @EstadoAtual IS NULL
    BEGIN
        THROW 50001,
              'A reserva indicada não existe.',
              1;
    END;

    IF @EstadoAtual = N'Cancelada'
    BEGIN
        THROW 50002,
              'Uma reserva cancelada não pode ser marcada como presente.',
              1;
    END;

    IF @EstadoAtual = N'Presente'
    BEGIN
        THROW 50003,
              'O cliente já está marcado como presente.',
              1;
    END;

    UPDATE dbo.ReservasAulas
    SET Estado = N'Presente'
    WHERE IdReserva = @IdReserva;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ReservasAulas_MarcarFalta
(
    @IdReserva INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoAtual NVARCHAR(20);

    SELECT
        @EstadoAtual = Estado
    FROM dbo.ReservasAulas
    WHERE IdReserva = @IdReserva;

    IF @EstadoAtual IS NULL
    BEGIN
        THROW 50001,
              'A reserva indicada não existe.',
              1;
    END;

    IF @EstadoAtual = N'Cancelada'
    BEGIN
        THROW 50002,
              'Uma reserva cancelada não pode ser marcada como falta.',
              1;
    END;

    IF @EstadoAtual = N'Faltou'
    BEGIN
        THROW 50003,
              'O cliente já está marcado como faltou.',
              1;
    END;

    UPDATE dbo.ReservasAulas
    SET Estado = N'Faltou'
    WHERE IdReserva = @IdReserva;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Aulas_ObterOcupacao
(
    @IdAula INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Aulas
        WHERE IdAula = @IdAula
    )
    BEGIN
        THROW 50001,
              'A aula indicada não existe.',
              1;
    END;

    SELECT
        A.IdAula,
        A.Lotacao,

        COUNT
        (
            CASE
                WHEN R.Estado IN
                (
                    N'Confirmada',
                    N'Presente'
                )
                THEN 1
            END
        ) AS VagasOcupadas,

        A.Lotacao -
        COUNT
        (
            CASE
                WHEN R.Estado IN
                (
                    N'Confirmada',
                    N'Presente'
                )
                THEN 1
            END
        ) AS VagasDisponiveis

    FROM dbo.Aulas AS A

    LEFT JOIN dbo.ReservasAulas AS R
        ON R.IdAula = A.IdAula

    WHERE A.IdAula = @IdAula

    GROUP BY
        A.IdAula,
        A.Lotacao;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Aulas_CancelarReservas
ON dbo.Aulas
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT UPDATE(Estado)
    BEGIN
        RETURN;
    END;

    UPDATE R
    SET R.Estado = N'Cancelada'
    FROM dbo.ReservasAulas AS R
    INNER JOIN inserted AS I
        ON I.IdAula = R.IdAula
    INNER JOIN deleted AS D
        ON D.IdAula = I.IdAula
    WHERE I.Estado = N'Cancelada'
      AND D.Estado <> N'Cancelada'
      AND R.Estado = N'Confirmada';
END;
GO
CREATE OR ALTER TRIGGER dbo.trg_Aulas_ValidarLotacao
ON dbo.Aulas
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT UPDATE(Lotacao)
    BEGIN
        RETURN;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM inserted AS I
        WHERE
        (
            SELECT COUNT(*)
            FROM dbo.ReservasAulas AS R
            WHERE R.IdAula = I.IdAula
              AND R.Estado IN
              (
                  N'Confirmada',
                  N'Presente'
              )
        ) > I.Lotacao
    )
    BEGIN
        ROLLBACK TRANSACTION;

        THROW 50001,
              'A lotação não pode ser inferior ao número de reservas existentes.',
              1;
    END;
END;
GO