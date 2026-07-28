CREATE OR ALTER PROCEDURE dbo.sp_Aulas_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        A.IdAula,
        A.IdProfessor,
        P.Nome AS NomeProfessor,
        A.Nome,
        A.DataAula,
        A.HoraInicio,
        A.DuracaoMinutos,
        A.Lotacao,
        A.Sala,
        A.Estado,

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
        ) AS VagasOcupadas

    FROM dbo.Aulas AS A

    INNER JOIN dbo.Professores AS P
        ON P.IdProfessor = A.IdProfessor

    LEFT JOIN dbo.ReservasAulas AS R
        ON R.IdAula = A.IdAula

    GROUP BY
        A.IdAula,
        A.IdProfessor,
        P.Nome,
        A.Nome,
        A.DataAula,
        A.HoraInicio,
        A.DuracaoMinutos,
        A.Lotacao,
        A.Sala,
        A.Estado

    ORDER BY
        A.DataAula ASC,
        A.HoraInicio ASC,
        A.IdAula ASC;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_Aulas_ObterPorId
(
    @IdAula INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        A.IdAula,
        A.IdProfessor,
        P.Nome AS NomeProfessor,
        A.Nome,
        A.DataAula,
        A.HoraInicio,
        A.DuracaoMinutos,
        A.Lotacao,
        A.Sala,
        A.Estado,

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
        ) AS VagasOcupadas

    FROM dbo.Aulas AS A

    INNER JOIN dbo.Professores AS P
        ON P.IdProfessor = A.IdProfessor

    LEFT JOIN dbo.ReservasAulas AS R
        ON R.IdAula = A.IdAula

    WHERE A.IdAula = @IdAula

    GROUP BY
        A.IdAula,
        A.IdProfessor,
        P.Nome,
        A.Nome,
        A.DataAula,
        A.HoraInicio,
        A.DuracaoMinutos,
        A.Lotacao,
        A.Sala,
        A.Estado;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_Aulas_Inserir
(
    @IdProfessor INT,
    @Nome NVARCHAR(100),
    @DataAula DATE,
    @HoraInicio TIME,
    @DuracaoMinutos INT,
    @Lotacao INT,
    @Sala NVARCHAR(50),
    @Estado NVARCHAR(20)
)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Nome = LTRIM(RTRIM(@Nome));
    SET @Sala = LTRIM(RTRIM(@Sala));

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Professores
        WHERE IdProfessor = @IdProfessor
    )
    BEGIN
        THROW 50001,
              'O professor selecionado não existe.',
              1;
    END;

    IF NULLIF(@Nome, '') IS NULL
    BEGIN
        THROW 50002,
              'O nome da aula é obrigatório.',
              1;
    END;

    IF @DataAula IS NULL
    BEGIN
        THROW 50003,
              'A data da aula é obrigatória.',
              1;
    END;

    IF @HoraInicio IS NULL
    BEGIN
        THROW 50004,
              'A hora de início é obrigatória.',
              1;
    END;

    IF @DuracaoMinutos <= 0
    BEGIN
        THROW 50005,
              'A duração da aula deve ser superior a zero.',
              1;
    END;

    IF @Lotacao <= 0
    BEGIN
        THROW 50006,
              'A lotação da aula deve ser superior a zero.',
              1;
    END;

    IF NULLIF(@Sala, '') IS NULL
    BEGIN
        THROW 50007,
              'A sala é obrigatória.',
              1;
    END;

    IF @Estado NOT IN
    (
        'Agendada',
        'Concluída',
        'Cancelada'
    )
    BEGIN
        THROW 50008,
              'O estado da aula não é válido.',
              1;
    END;

    /*
        Minutos desde a meia-noite:

        Início da nova aula
        Fim da nova aula

        Exemplo:
        18:30 = 18 * 60 + 30 = 1110 minutos
    */
    DECLARE @NovoInicioMinutos INT =
        DATEPART(HOUR, @HoraInicio) * 60
        + DATEPART(MINUTE, @HoraInicio);

    DECLARE @NovoFimMinutos INT =
        @NovoInicioMinutos
        + @DuracaoMinutos;

    IF @NovoFimMinutos > 1440
    BEGIN
        THROW 50009,
              'A aula não pode terminar depois da meia-noite.',
              1;
    END;

    -- Conflito de horário do professor
    IF @Estado <> 'Cancelada'
       AND EXISTS
       (
           SELECT 1
           FROM dbo.Aulas AS A
           WHERE A.IdProfessor = @IdProfessor
             AND A.DataAula = @DataAula
             AND A.Estado <> 'Cancelada'

             AND @NovoInicioMinutos <
                 (
                     DATEPART(HOUR, A.HoraInicio) * 60
                     + DATEPART(MINUTE, A.HoraInicio)
                     + A.DuracaoMinutos
                 )

             AND @NovoFimMinutos >
                 (
                     DATEPART(HOUR, A.HoraInicio) * 60
                     + DATEPART(MINUTE, A.HoraInicio)
                 )
       )
    BEGIN
        THROW 50010,
              'O professor já possui outra aula nesse horário.',
              1;
    END;

    -- Conflito de horário da sala
    IF @Estado <> 'Cancelada'
       AND EXISTS
       (
           SELECT 1
           FROM dbo.Aulas AS A
           WHERE A.Sala = @Sala
             AND A.DataAula = @DataAula
             AND A.Estado <> 'Cancelada'

             AND @NovoInicioMinutos <
                 (
                     DATEPART(HOUR, A.HoraInicio) * 60
                     + DATEPART(MINUTE, A.HoraInicio)
                     + A.DuracaoMinutos
                 )

             AND @NovoFimMinutos >
                 (
                     DATEPART(HOUR, A.HoraInicio) * 60
                     + DATEPART(MINUTE, A.HoraInicio)
                 )
       )
    BEGIN
        THROW 50011,
              'A sala já está ocupada nesse horário.',
              1;
    END;

    INSERT INTO dbo.Aulas
    (
        IdProfessor,
        Nome,
        DataAula,
        HoraInicio,
        DuracaoMinutos,
        Lotacao,
        Sala,
        Estado
    )
    VALUES
    (
        @IdProfessor,
        @Nome,
        @DataAula,
        @HoraInicio,
        @DuracaoMinutos,
        @Lotacao,
        @Sala,
        @Estado
    );
END;
GO
    INSERT INTO dbo.Aulas
    (
        IdProfessor,
        Nome,
        DataAula,
        HoraInicio,
        DuracaoMinutos,
        Lotacao,
        Sala,
        Estado
    )
    VALUES
    (
        @IdProfessor,
        LTRIM(RTRIM(@Nome)),
        @DataAula,
        @HoraInicio,
        @DuracaoMinutos,
        @Lotacao,
        LTRIM(RTRIM(@Sala)),
        @Estado
    );
END;
GO


CREATE OR ALTER PROCEDURE dbo.sp_Aulas_Atualizar
(
    @IdAula INT,
    @IdProfessor INT,
    @Nome NVARCHAR(100),
    @DataAula DATE,
    @HoraInicio TIME,
    @DuracaoMinutos INT,
    @Lotacao INT,
    @Sala NVARCHAR(50),
    @Estado NVARCHAR(20)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EstadoAtual NVARCHAR(20);
    DECLARE @ReservasAtivas INT;

    SET @Nome =
        LTRIM(RTRIM(@Nome));

    SET @Sala =
        LTRIM(RTRIM(@Sala));

    ----------------------------------------------------
    -- VALIDAR EXISTÊNCIA DA AULA
    ----------------------------------------------------

    SELECT
        @EstadoAtual = Estado
    FROM dbo.Aulas
    WHERE IdAula = @IdAula;

    IF @EstadoAtual IS NULL
    BEGIN
        THROW 50001,
              'A aula indicada não existe.',
              1;
    END;

    ----------------------------------------------------
    -- PROTEGER ESTADOS FINAIS
    ----------------------------------------------------

    IF @EstadoAtual = N'Concluída'
       AND @Estado <> N'Concluída'
    BEGIN
        THROW 50002,
              'Uma aula concluída não pode voltar para outro estado.',
              1;
    END;

    IF @EstadoAtual = N'Cancelada'
       AND @Estado <> N'Cancelada'
    BEGIN
        THROW 50003,
              'Uma aula cancelada não pode ser reativada.',
              1;
    END;

    ----------------------------------------------------
    -- VALIDAR PROFESSOR
    ----------------------------------------------------

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Professores
        WHERE IdProfessor = @IdProfessor
    )
    BEGIN
        THROW 50004,
              'O professor selecionado não existe.',
              1;
    END;

    ----------------------------------------------------
    -- VALIDAR NOME
    ----------------------------------------------------

    IF NULLIF(@Nome, N'') IS NULL
    BEGIN
        THROW 50005,
              'O nome da aula é obrigatório.',
              1;
    END;

    ----------------------------------------------------
    -- VALIDAR DATA
    ----------------------------------------------------

    IF @DataAula IS NULL
    BEGIN
        THROW 50006,
              'A data da aula é obrigatória.',
              1;
    END;

    ----------------------------------------------------
    -- VALIDAR HORA
    ----------------------------------------------------

    IF @HoraInicio IS NULL
    BEGIN
        THROW 50007,
              'A hora de início é obrigatória.',
              1;
    END;

    ----------------------------------------------------
    -- VALIDAR DURAÇÃO
    ----------------------------------------------------

    IF @DuracaoMinutos <= 0
    BEGIN
        THROW 50008,
              'A duração da aula deve ser superior a zero.',
              1;
    END;

    IF @DuracaoMinutos > 720
    BEGIN
        THROW 50009,
              'A duração da aula não pode ser superior a 12 horas.',
              1;
    END;

    ----------------------------------------------------
    -- VALIDAR LOTAÇÃO
    ----------------------------------------------------

    IF @Lotacao <= 0
    BEGIN
        THROW 50010,
              'A lotação da aula deve ser superior a zero.',
              1;
    END;

    IF @Lotacao > 1000
    BEGIN
        THROW 50011,
              'A lotação indicada é demasiado elevada.',
              1;
    END;

    ----------------------------------------------------
    -- VALIDAR SALA
    ----------------------------------------------------

    IF NULLIF(@Sala, N'') IS NULL
    BEGIN
        THROW 50012,
              'A sala é obrigatória.',
              1;
    END;

    ----------------------------------------------------
    -- VALIDAR ESTADO
    ----------------------------------------------------

    IF @Estado NOT IN
    (
        N'Agendada',
        N'Concluída',
        N'Cancelada'
    )
    BEGIN
        THROW 50013,
              'O estado da aula não é válido.',
              1;
    END;

    ----------------------------------------------------
    -- VALIDAR FIM DA AULA
    ----------------------------------------------------

    DECLARE @NovoInicioMinutos INT =
        DATEPART(HOUR, @HoraInicio) * 60
        + DATEPART(MINUTE, @HoraInicio);

    DECLARE @NovoFimMinutos INT =
        @NovoInicioMinutos
        + @DuracaoMinutos;

    IF @NovoFimMinutos > 1440
    BEGIN
        THROW 50014,
              'A aula não pode terminar depois da meia-noite.',
              1;
    END;

    ----------------------------------------------------
    -- VALIDAR LOTAÇÃO FACE ÀS RESERVAS
    ----------------------------------------------------

    SELECT
        @ReservasAtivas =
            COUNT(*)
    FROM dbo.ReservasAulas
    WHERE IdAula = @IdAula
      AND Estado IN
      (
          N'Confirmada',
          N'Presente'
      );

    IF @Lotacao < @ReservasAtivas
    BEGIN
        THROW 50015,
              'A lotação não pode ser inferior ao número de reservas existentes.',
              1;
    END;

    ----------------------------------------------------
    -- CONFLITO DE HORÁRIO DO PROFESSOR
    ----------------------------------------------------

    IF @Estado <> N'Cancelada'
       AND EXISTS
       (
           SELECT 1
           FROM dbo.Aulas AS A
           WHERE A.IdAula <> @IdAula
             AND A.IdProfessor = @IdProfessor
             AND A.DataAula = @DataAula
             AND A.Estado <> N'Cancelada'

             AND @NovoInicioMinutos <
                 (
                     DATEPART(HOUR, A.HoraInicio) * 60
                     + DATEPART(MINUTE, A.HoraInicio)
                     + A.DuracaoMinutos
                 )

             AND @NovoFimMinutos >
                 (
                     DATEPART(HOUR, A.HoraInicio) * 60
                     + DATEPART(MINUTE, A.HoraInicio)
                 )
       )
    BEGIN
        THROW 50016,
              'O professor já possui outra aula nesse horário.',
              1;
    END;

    ----------------------------------------------------
    -- CONFLITO DE HORÁRIO DA SALA
    ----------------------------------------------------

    IF @Estado <> N'Cancelada'
       AND EXISTS
       (
           SELECT 1
           FROM dbo.Aulas AS A
           WHERE A.IdAula <> @IdAula
             AND A.Sala = @Sala
             AND A.DataAula = @DataAula
             AND A.Estado <> N'Cancelada'

             AND @NovoInicioMinutos <
                 (
                     DATEPART(HOUR, A.HoraInicio) * 60
                     + DATEPART(MINUTE, A.HoraInicio)
                     + A.DuracaoMinutos
                 )

             AND @NovoFimMinutos >
                 (
                     DATEPART(HOUR, A.HoraInicio) * 60
                     + DATEPART(MINUTE, A.HoraInicio)
                 )
       )
    BEGIN
        THROW 50017,
              'A sala já está ocupada nesse horário.',
              1;
    END;

    ----------------------------------------------------
    -- ATUALIZAR A AULA
    ----------------------------------------------------

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.Aulas
        SET
            IdProfessor =
                @IdProfessor,

            Nome =
                @Nome,

            DataAula =
                @DataAula,

            HoraInicio =
                @HoraInicio,

            DuracaoMinutos =
                @DuracaoMinutos,

            Lotacao =
                @Lotacao,

            Sala =
                @Sala,

            Estado =
                @Estado

        WHERE IdAula =
            @IdAula;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        THROW;
    END CATCH;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_Aulas_Eliminar
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

    -- Verificar reservas do módulo novo
    IF OBJECT_ID(N'dbo.ReservasAulas', N'U') IS NOT NULL
       AND EXISTS
       (
           SELECT 1
           FROM dbo.ReservasAulas
           WHERE IdAula = @IdAula
       )
    BEGIN
        THROW 50002,
              'Esta aula possui reservas associadas e não pode ser eliminada. Cancele a aula para preservar o histórico.',
              1;
    END;

    -- Verificar inscrições do módulo antigo
    IF OBJECT_ID(N'dbo.InscricoesAulas', N'U') IS NOT NULL
       AND EXISTS
       (
           SELECT 1
           FROM dbo.InscricoesAulas
           WHERE IdAula = @IdAula
       )
    BEGIN
        THROW 50003,
              'Esta aula possui clientes inscritos e não pode ser eliminada. Cancele a aula para preservar o histórico.',
              1;
    END;

    DELETE FROM dbo.Aulas
    WHERE IdAula = @IdAula;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Aulas_Pesquisar
(
    @Pesquisa NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Pesquisa =
        LTRIM(RTRIM(@Pesquisa));

    SELECT
        A.IdAula,
        A.IdProfessor,
        P.Nome AS NomeProfessor,
        A.Nome,
        A.DataAula,
        A.HoraInicio,
        A.DuracaoMinutos,
        A.Lotacao,
        A.Sala,
        A.Estado,

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
        ) AS VagasOcupadas

    FROM dbo.Aulas AS A

    INNER JOIN dbo.Professores AS P
        ON P.IdProfessor = A.IdProfessor

    LEFT JOIN dbo.ReservasAulas AS R
        ON R.IdAula = A.IdAula

    WHERE
        A.Nome LIKE
            N'%' + @Pesquisa + N'%'

        OR P.Nome LIKE
            N'%' + @Pesquisa + N'%'

        OR A.Sala LIKE
            N'%' + @Pesquisa + N'%'

        OR A.Estado LIKE
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

        OR CAST(
               A.DuracaoMinutos
               AS NVARCHAR(10)
           ) LIKE
            N'%' + @Pesquisa + N'%'

        OR CAST(
               A.Lotacao
               AS NVARCHAR(10)
           ) LIKE
            N'%' + @Pesquisa + N'%'

    GROUP BY
        A.IdAula,
        A.IdProfessor,
        P.Nome,
        A.Nome,
        A.DataAula,
        A.HoraInicio,
        A.DuracaoMinutos,
        A.Lotacao,
        A.Sala,
        A.Estado

    ORDER BY
        A.DataAula ASC,
        A.HoraInicio ASC,
        A.IdAula ASC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Aulas_AtualizarEstados
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AgoraMinutos INT =
        DATEPART(HOUR, GETDATE()) * 60
        + DATEPART(MINUTE, GETDATE());

    DECLARE @AulasConcluidas TABLE
    (
        IdAula INT PRIMARY KEY
    );

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.Aulas
        SET Estado = N'Concluída'
        OUTPUT inserted.IdAula
            INTO @AulasConcluidas(IdAula)
        WHERE Estado = N'Agendada'
          AND
          (
              DataAula < CAST(GETDATE() AS DATE)

              OR

              (
                  DataAula = CAST(GETDATE() AS DATE)
                  AND
                  (
                      DATEPART(HOUR, HoraInicio) * 60
                      + DATEPART(MINUTE, HoraInicio)
                      + DuracaoMinutos
                  ) <= @AgoraMinutos
              )
          );

        UPDATE R
        SET R.Estado = N'Faltou'
        FROM dbo.ReservasAulas AS R

        INNER JOIN @AulasConcluidas AS A
            ON A.IdAula = R.IdAula

        WHERE R.Estado = N'Confirmada';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        THROW;
    END CATCH;
END;
GO