CREATE OR ALTER PROCEDURE dbo.sp_EventosAgenda_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        E.IdEvento,
        E.Titulo,
        E.Tipo,
        E.DataInicio,
        E.DataFim,
        E.IdPT,
        PT.Nome AS NomePT,
        E.IdCliente,
        C.Nome AS NomeCliente,
        E.IdAula,
        A.Nome AS NomeAula,
        E.Localizacao,
        E.Descricao,
        E.Estado
    FROM dbo.EventosAgenda AS E

    LEFT JOIN dbo.PersonalTrainers AS PT
        ON PT.IdPT = E.IdPT

    LEFT JOIN dbo.Clientes AS C
        ON C.IdCliente = E.IdCliente

    LEFT JOIN dbo.Aulas AS A
        ON A.IdAula = E.IdAula

    ORDER BY
        E.DataInicio ASC,
        E.IdEvento ASC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_EventosAgenda_ObterPorId
(
    @IdEvento INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        E.IdEvento,
        E.Titulo,
        E.Tipo,
        E.DataInicio,
        E.DataFim,
        E.IdPT,
        PT.Nome AS NomePT,
        E.IdCliente,
        C.Nome AS NomeCliente,
        E.IdAula,
        A.Nome AS NomeAula,
        E.Localizacao,
        E.Descricao,
        E.Estado
    FROM dbo.EventosAgenda AS E

    LEFT JOIN dbo.PersonalTrainers AS PT
        ON PT.IdPT = E.IdPT

    LEFT JOIN dbo.Clientes AS C
        ON C.IdCliente = E.IdCliente

    LEFT JOIN dbo.Aulas AS A
        ON A.IdAula = E.IdAula

    WHERE E.IdEvento = @IdEvento;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_EventosAgenda_ListarPorPeriodo
(
    @DataInicio DATETIME2(0),
    @DataFim DATETIME2(0),
    @IdPT INT = NULL,
    @Tipo NVARCHAR(30) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @DataFim <= @DataInicio
    BEGIN
        THROW 50001,
              'O período indicado não é válido.',
              1;
    END;

    SELECT
        E.IdEvento,
        E.Titulo,
        E.Tipo,
        E.DataInicio,
        E.DataFim,

        E.IdPT,
        E.IdProfessor,

        CASE
            WHEN E.Tipo = N'Sessão PT'
                THEN PT.Nome
            WHEN E.Tipo = N'Aula'
                THEN P.Nome
            ELSE N''
        END AS NomePT,

        E.IdCliente,
        C.Nome AS NomeCliente,

        E.IdAula,
        A.Nome AS NomeAula,

        E.Localizacao,
        E.Descricao,
        E.Estado

    FROM dbo.EventosAgenda AS E

    LEFT JOIN dbo.PersonalTrainers AS PT
        ON PT.IdPT = E.IdPT

    LEFT JOIN dbo.Professores AS P
        ON P.IdProfessor = E.IdProfessor

    LEFT JOIN dbo.Clientes AS C
        ON C.IdCliente = E.IdCliente

    LEFT JOIN dbo.Aulas AS A
        ON A.IdAula = E.IdAula

    WHERE
        E.DataInicio < @DataFim
        AND E.DataFim > @DataInicio

        AND
        (
            @Tipo IS NULL
            OR E.Tipo = @Tipo
        )

        AND
        (
            @IdPT IS NULL

            OR
            (
                E.Tipo = N'Sessão PT'
                AND E.IdPT = @IdPT
            )

            OR
            (
                E.Tipo = N'Aula'
                AND E.IdProfessor = @IdPT
            )
        )

    ORDER BY
        E.DataInicio,
        E.IdEvento;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_EventosAgenda_Inserir
(
    @Titulo NVARCHAR(150),
    @Tipo NVARCHAR(30),
    @DataInicio DATETIME2(0),
    @DataFim DATETIME2(0),

    @IdPT INT = NULL,
    @IdProfessor INT = NULL,
    @IdCliente INT = NULL,
    @IdAula INT = NULL,

    @Localizacao NVARCHAR(100) = NULL,
    @Descricao NVARCHAR(500) = NULL,
    @Estado NVARCHAR(20) = N'Agendado'
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @Titulo =
        LTRIM(RTRIM(@Titulo));

    SET @Tipo =
        LTRIM(RTRIM(@Tipo));

    SET @Localizacao =
        NULLIF(
            LTRIM(RTRIM(@Localizacao)),
            N''
        );

    SET @Descricao =
        NULLIF(
            LTRIM(RTRIM(@Descricao)),
            N''
        );

    IF NULLIF(@Titulo, N'') IS NULL
    BEGIN
        THROW 50001,
              'O título do agendamento é obrigatório.',
              1;
    END;

    IF @Tipo <> N'Sessão PT'
    BEGIN
        THROW 50002,
              'A Agenda apenas permite criar sessões de Personal Training.',
              1;
    END;

    IF @DataFim <= @DataInicio
    BEGIN
        THROW 50003,
              'A data e hora de fim devem ser posteriores ao início.',
              1;
    END;

    IF @IdPT IS NULL
    BEGIN
        THROW 50004,
              'Selecione o personal trainer.',
              1;
    END;

    IF @IdCliente IS NULL
    BEGIN
        THROW 50005,
              'Selecione o cliente.',
              1;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.PersonalTrainers
        WHERE IdPT = @IdPT
    )
    BEGIN
        THROW 50006,
              'O personal trainer selecionado não existe.',
              1;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Clientes
        WHERE IdCliente = @IdCliente
    )
    BEGIN
        THROW 50007,
              'O cliente selecionado não existe.',
              1;
    END;

    /*
        Numa sessão PT, estes campos têm de ficar NULL.
    */
    SET @IdProfessor = NULL;
    SET @IdAula = NULL;

    IF @Estado NOT IN
    (
        N'Agendado',
        N'Concluído',
        N'Cancelado'
    )
    BEGIN
        THROW 50008,
              'O estado indicado não é válido.',
              1;
    END;

    /*
        Impede sobreposição do mesmo Personal Trainer.
    */
    IF @Estado = N'Agendado'
       AND EXISTS
       (
           SELECT 1
           FROM dbo.EventosAgenda
           WHERE IdPT = @IdPT
             AND Estado = N'Agendado'
             AND @DataInicio < DataFim
             AND @DataFim > DataInicio
       )
    BEGIN
        THROW 50009,
              'O personal trainer já possui um agendamento neste horário.',
              1;
    END;

    /*
        Impede sobreposição do mesmo cliente.
    */
    IF @Estado = N'Agendado'
       AND EXISTS
       (
           SELECT 1
           FROM dbo.EventosAgenda
           WHERE IdCliente = @IdCliente
             AND Estado = N'Agendado'
             AND @DataInicio < DataFim
             AND @DataFim > DataInicio
       )
    BEGIN
        THROW 50010,
              'O cliente já possui um agendamento neste horário.',
              1;
    END;

    INSERT INTO dbo.EventosAgenda
    (
        Titulo,
        Tipo,
        DataInicio,
        DataFim,
        IdPT,
        IdProfessor,
        IdCliente,
        IdAula,
        Localizacao,
        Descricao,
        Estado
    )
    VALUES
    (
        @Titulo,
        N'Sessão PT',
        @DataInicio,
        @DataFim,
        @IdPT,
        NULL,
        @IdCliente,
        NULL,
        @Localizacao,
        @Descricao,
        @Estado
    );

    SELECT
        CAST(SCOPE_IDENTITY() AS INT)
        AS IdEvento;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_EventosAgenda_Atualizar
(
    @IdEvento INT,
    @Titulo NVARCHAR(150),
    @Tipo NVARCHAR(30),
    @DataInicio DATETIME2(0),
    @DataFim DATETIME2(0),
    @IdPT INT = NULL,
    @IdProfessor INT = NULL,
    @IdCliente INT = NULL,
    @IdAula INT = NULL,
    @Localizacao NVARCHAR(100) = NULL,
    @Descricao NVARCHAR(500) = NULL,
    @Estado NVARCHAR(20)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.EventosAgenda
        WHERE IdEvento = @IdEvento
    )
    BEGIN
        THROW 50001,
              'O evento indicado não existe.',
              1;
    END;

    SET @Titulo =
        LTRIM(RTRIM(@Titulo));

    SET @Tipo =
        LTRIM(RTRIM(@Tipo));

    SET @Localizacao =
        NULLIF(
            LTRIM(RTRIM(@Localizacao)),
            N''
        );

    SET @Descricao =
        NULLIF(
            LTRIM(RTRIM(@Descricao)),
            N''
        );

    IF NULLIF(@Titulo, N'') IS NULL
    BEGIN
        THROW 50002,
              'O título é obrigatório.',
              1;
    END;

    IF @DataFim <= @DataInicio
    BEGIN
        THROW 50003,
              'A data de fim deve ser posterior à data de início.',
              1;
    END;

    IF @Tipo = N'Sessão PT'
    BEGIN
        IF @IdPT IS NULL OR @IdCliente IS NULL
        BEGIN
            THROW 50004,
                  'A sessão deve ter um personal trainer e um cliente.',
                  1;
        END;

        SET @IdProfessor =
            NULL;

        SET @IdAula =
            NULL;
    END;
    ELSE IF @Tipo = N'Aula'
    BEGIN
        IF @IdProfessor IS NULL OR @IdAula IS NULL
        BEGIN
            THROW 50005,
                  'A aula deve ter um professor e uma aula associada.',
                  1;
        END;

        SET @IdPT =
            NULL;

        SET @IdCliente =
            NULL;
    END;
    ELSE
    BEGIN
        THROW 50006,
              'O tipo de evento não é válido.',
              1;
    END;

    IF @Tipo = N'Sessão PT'
       AND @Estado = N'Agendado'
       AND EXISTS
       (
           SELECT 1
           FROM dbo.EventosAgenda
           WHERE IdEvento <> @IdEvento
             AND Tipo = N'Sessão PT'
             AND IdPT = @IdPT
             AND Estado = N'Agendado'
             AND @DataInicio < DataFim
             AND @DataFim > DataInicio
       )
    BEGIN
        THROW 50007,
              'O personal trainer já possui um evento neste horário.',
              1;
    END;

    IF @Tipo = N'Aula'
       AND @Estado = N'Agendado'
       AND EXISTS
       (
           SELECT 1
           FROM dbo.EventosAgenda
           WHERE IdEvento <> @IdEvento
             AND Tipo = N'Aula'
             AND IdProfessor = @IdProfessor
             AND Estado = N'Agendado'
             AND @DataInicio < DataFim
             AND @DataFim > DataInicio
       )
    BEGIN
        THROW 50008,
              'O professor já possui uma aula neste horário.',
              1;
    END;

    UPDATE dbo.EventosAgenda
    SET
        Titulo = @Titulo,
        Tipo = @Tipo,
        DataInicio = @DataInicio,
        DataFim = @DataFim,
        IdPT = @IdPT,
        IdProfessor = @IdProfessor,
        IdCliente = @IdCliente,
        IdAula = @IdAula,
        Localizacao = @Localizacao,
        Descricao = @Descricao,
        Estado = @Estado
    WHERE IdEvento = @IdEvento;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_EventosAgenda_Cancelar
(
    @IdEvento INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.EventosAgenda
        WHERE IdEvento = @IdEvento
    )
    BEGIN
        THROW 50001,
              'O evento indicado n�o existe.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.EventosAgenda
        WHERE IdEvento = @IdEvento
          AND Estado = N'Conclu�do'
    )
    BEGIN
        THROW 50002,
              'N�o � poss�vel cancelar um evento conclu�do.',
              1;
    END;

    UPDATE dbo.EventosAgenda
    SET Estado = N'Cancelado'
    WHERE IdEvento = @IdEvento;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_EventosAgenda_Concluir
(
    @IdEvento INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.EventosAgenda
        WHERE IdEvento = @IdEvento
    )
    BEGIN
        THROW 50001,
              'O evento indicado n�o existe.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.EventosAgenda
        WHERE IdEvento = @IdEvento
          AND Estado = N'Cancelado'
    )
    BEGIN
        THROW 50002,
              'N�o � poss�vel concluir um evento cancelado.',
              1;
    END;

    UPDATE dbo.EventosAgenda
    SET Estado = N'Conclu�do'
    WHERE IdEvento = @IdEvento;
END;
GO

CREATE OR ALTER TRIGGER dbo.TR_Aulas_SincronizarAgenda
ON dbo.Aulas
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    /*
     * Atualiza eventos que j� existem.
     */
    UPDATE E
    SET
        E.Titulo =
            I.Nome,

        E.Tipo =
            N'Aula',

        E.DataInicio =
            DATEADD
            (
                SECOND,
                DATEDIFF
                (
                    SECOND,
                    CAST('00:00:00' AS TIME),
                    I.HoraInicio
                ),
                CAST(I.DataAula AS DATETIME2(0))
            ),

        E.DataFim =
            DATEADD
            (
                MINUTE,
                I.DuracaoMinutos,
                DATEADD
                (
                    SECOND,
                    DATEDIFF
                    (
                        SECOND,
                        CAST('00:00:00' AS TIME),
                        I.HoraInicio
                    ),
                    CAST(I.DataAula AS DATETIME2(0))
                )
            ),

        E.IdProfessor =
            I.IdProfessor,

        E.IdPT =
            NULL,

        E.IdCliente =
            NULL,

        E.Localizacao =
            I.Sala,

        E.Estado =
            CASE I.Estado
                WHEN N'Agendada' THEN N'Agendado'
                WHEN N'Conclu�da' THEN N'Conclu�do'
                WHEN N'Cancelada' THEN N'Cancelado'
                ELSE N'Agendado'
            END
    FROM dbo.EventosAgenda AS E
    INNER JOIN inserted AS I
        ON I.IdAula = E.IdAula
    WHERE E.Tipo = N'Aula';

    /*
     * Insere aulas que ainda n�o existem na agenda.
     */
    INSERT INTO dbo.EventosAgenda
    (
        Titulo,
        Tipo,
        DataInicio,
        DataFim,
        IdPT,
        IdProfessor,
        IdCliente,
        IdAula,
        Localizacao,
        Descricao,
        Estado
    )
    SELECT
        I.Nome,

        N'Aula',

        DATEADD
        (
            SECOND,
            DATEDIFF
            (
                SECOND,
                CAST('00:00:00' AS TIME),
                I.HoraInicio
            ),
            CAST(I.DataAula AS DATETIME2(0))
        ),

        DATEADD
        (
            MINUTE,
            I.DuracaoMinutos,
            DATEADD
            (
                SECOND,
                DATEDIFF
                (
                    SECOND,
                    CAST('00:00:00' AS TIME),
                    I.HoraInicio
                ),
                CAST(I.DataAula AS DATETIME2(0))
            )
        ),

        NULL,
        I.IdProfessor,
        NULL,
        I.IdAula,
        I.Sala,
        NULL,

        CASE I.Estado
            WHEN N'Agendada' THEN N'Agendado'
            WHEN N'Conclu�da' THEN N'Conclu�do'
            WHEN N'Cancelada' THEN N'Cancelado'
            ELSE N'Agendado'
        END
    FROM inserted AS I
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.EventosAgenda AS E
        WHERE E.IdAula = I.IdAula
          AND E.Tipo = N'Aula'
    );
END;
GO