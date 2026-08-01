CREATE OR ALTER PROCEDURE dbo.sp_Manutencoes_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        M.IdManutencao,
        M.IdEquipamento,
        E.Nome AS NomeEquipamento,
        E.Marca,
        E.Modelo,
        M.Tipo,
        M.DataAgendada,
        M.DataRealizacao,
        M.Descricao,
        M.Responsavel,
        M.Custo,
        M.Estado,
        M.Observacoes
    FROM dbo.Manutencoes AS M

    INNER JOIN dbo.Equipamentos AS E
        ON E.IdEquipamento = M.IdEquipamento

    ORDER BY
        M.DataAgendada DESC,
        M.IdManutencao DESC;
END;
GO


CREATE OR ALTER PROCEDURE dbo.sp_Manutencoes_ObterPorId
(
    @IdManutencao INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        M.IdManutencao,
        M.IdEquipamento,
        E.Nome AS NomeEquipamento,
        E.Marca,
        E.Modelo,
        M.Tipo,
        M.DataAgendada,
        M.DataRealizacao,
        M.Descricao,
        M.Responsavel,
        M.Custo,
        M.Estado,
        M.Observacoes
    FROM dbo.Manutencoes AS M

    INNER JOIN dbo.Equipamentos AS E
        ON E.IdEquipamento = M.IdEquipamento

    WHERE M.IdManutencao =
          @IdManutencao;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Manutencoes_Inserir
(
    @IdEquipamento INT,
    @Tipo NVARCHAR(30),
    @DataAgendada DATE,
    @DataRealizacao DATE = NULL,
    @Descricao NVARCHAR(500),
    @Responsavel NVARCHAR(100) = NULL,
    @Custo DECIMAL(10,2) = NULL,
    @Estado NVARCHAR(30),
    @Observacoes NVARCHAR(500) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @Tipo =
        LTRIM(RTRIM(@Tipo));

    SET @Descricao =
        LTRIM(RTRIM(@Descricao));

    SET @Responsavel =
        NULLIF(
            LTRIM(RTRIM(@Responsavel)),
            N''
        );

    SET @Observacoes =
        NULLIF(
            LTRIM(RTRIM(@Observacoes)),
            N''
        );

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Equipamentos
        WHERE IdEquipamento =
              @IdEquipamento
    )
    BEGIN
        THROW 50001,
              'O equipamento selecionado não existe.',
              1;
    END;

    IF @Tipo NOT IN
    (
        N'Preventiva',
        N'Corretiva',
        N'Inspeção'
    )
    BEGIN
        THROW 50002,
              'O tipo de manutenção não é válido.',
              1;
    END;

    IF @DataAgendada IS NULL
    BEGIN
        THROW 50003,
              'A data agendada é obrigatória.',
              1;
    END;

    IF NULLIF(@Descricao, N'') IS NULL
    BEGIN
        THROW 50004,
              'A descrição da manutenção é obrigatória.',
              1;
    END;

    IF @Estado NOT IN
    (
        N'Agendada',
        N'Em curso',
        N'Concluída',
        N'Cancelada'
    )
    BEGIN
        THROW 50005,
              'O estado da manutenção não é válido.',
              1;
    END;

    IF @Custo IS NOT NULL
       AND @Custo < 0
    BEGIN
        THROW 50006,
              'O custo não pode ser negativo.',
              1;
    END;

    IF @DataRealizacao IS NOT NULL
       AND @DataRealizacao < @DataAgendada
    BEGIN
        THROW 50007,
              'A data de realização não pode ser anterior à data agendada.',
              1;
    END;

    IF @Estado = N'Concluída'
       AND @DataRealizacao IS NULL
    BEGIN
        THROW 50008,
              'Uma manutenção concluída deve ter data de realização.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Manutencoes
        WHERE IdEquipamento = @IdEquipamento
          AND Estado IN
          (
              N'Agendada',
              N'Em curso'
          )
    )
    BEGIN
        THROW 50009,
              'Este equipamento já possui uma manutenção ativa.',
              1;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.Manutencoes
        (
            IdEquipamento,
            Tipo,
            DataAgendada,
            DataRealizacao,
            Descricao,
            Responsavel,
            Custo,
            Estado,
            Observacoes
        )
        VALUES
        (
            @IdEquipamento,
            @Tipo,
            @DataAgendada,
            @DataRealizacao,
            @Descricao,
            @Responsavel,
            @Custo,
            @Estado,
            @Observacoes
        );

        IF @Estado IN
        (
            N'Agendada',
            N'Em curso'
        )
        BEGIN
            UPDATE dbo.Equipamentos
            SET Estado =
                N'Em manutenção'
            WHERE IdEquipamento =
                  @IdEquipamento;
        END;

        IF @Estado = N'Concluída'
        BEGIN
            UPDATE dbo.Equipamentos
            SET Estado =
                N'Operacional'
            WHERE IdEquipamento =
                  @IdEquipamento;
        END;

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

CREATE OR ALTER PROCEDURE dbo.sp_Manutencoes_Atualizar
(
    @IdManutencao INT,
    @IdEquipamento INT,
    @Tipo NVARCHAR(30),
    @DataAgendada DATE,
    @DataRealizacao DATE = NULL,
    @Descricao NVARCHAR(500),
    @Responsavel NVARCHAR(100) = NULL,
    @Custo DECIMAL(10,2) = NULL,
    @Estado NVARCHAR(30),
    @Observacoes NVARCHAR(500) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IdEquipamentoAnterior INT;

    SELECT
        @IdEquipamentoAnterior =
            IdEquipamento
    FROM dbo.Manutencoes
    WHERE IdManutencao =
          @IdManutencao;

    IF @IdEquipamentoAnterior IS NULL
    BEGIN
        THROW 50001,
              'A manutenção indicada não existe.',
              1;
    END;

    SET @Tipo =
        LTRIM(RTRIM(@Tipo));

    SET @Descricao =
        LTRIM(RTRIM(@Descricao));

    SET @Responsavel =
        NULLIF(
            LTRIM(RTRIM(@Responsavel)),
            N''
        );

    SET @Observacoes =
        NULLIF(
            LTRIM(RTRIM(@Observacoes)),
            N''
        );

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Equipamentos
        WHERE IdEquipamento =
              @IdEquipamento
    )
    BEGIN
        THROW 50002,
              'O equipamento selecionado não existe.',
              1;
    END;

    IF @Tipo NOT IN
    (
        N'Preventiva',
        N'Corretiva',
        N'Inspeção'
    )
    BEGIN
        THROW 50003,
              'O tipo de manutenção não é válido.',
              1;
    END;

    IF @DataAgendada IS NULL
    BEGIN
        THROW 50004,
              'A data agendada é obrigatória.',
              1;
    END;

    IF NULLIF(@Descricao, N'') IS NULL
    BEGIN
        THROW 50005,
              'A descrição da manutenção é obrigatória.',
              1;
    END;

    IF @Estado NOT IN
    (
        N'Agendada',
        N'Em curso',
        N'Concluída',
        N'Cancelada'
    )
    BEGIN
        THROW 50006,
              'O estado da manutenção não é válido.',
              1;
    END;

    IF @Custo IS NOT NULL
       AND @Custo < 0
    BEGIN
        THROW 50007,
              'O custo não pode ser negativo.',
              1;
    END;

    IF @DataRealizacao IS NOT NULL
       AND @DataRealizacao < @DataAgendada
    BEGIN
        THROW 50008,
              'A data de realização não pode ser anterior à data agendada.',
              1;
    END;

    IF @Estado = N'Concluída'
       AND @DataRealizacao IS NULL
    BEGIN
        THROW 50009,
              'Uma manutenção concluída deve ter data de realização.',
              1;
    END;

    IF @Estado IN
    (
        N'Em curso',
        N'Concluída'
    )
    AND @Responsavel IS NULL
    BEGIN
        THROW 50010,
              'Deve atribuir um responsável antes de iniciar ou concluir a manutenção.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Manutencoes
        WHERE IdEquipamento =
              @IdEquipamento

          AND IdManutencao <>
              @IdManutencao

          AND Estado IN
          (
              N'Agendada',
              N'Em curso'
          )
    )
    BEGIN
        THROW 50011,
              'Este equipamento já possui outra manutenção ativa.',
              1;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.Manutencoes
        SET
            IdEquipamento =
                @IdEquipamento,

            Tipo =
                @Tipo,

            DataAgendada =
                @DataAgendada,

            DataRealizacao =
                @DataRealizacao,

            Descricao =
                @Descricao,

            Responsavel =
                @Responsavel,

            Custo =
                @Custo,

            Estado =
                @Estado,

            Observacoes =
                @Observacoes

        WHERE IdManutencao =
              @IdManutencao;

        /*
            Se mudou de equipamento, verifica se o anterior
            ainda possui manutenção ativa.
        */
        IF @IdEquipamentoAnterior <>
           @IdEquipamento
        BEGIN
            IF NOT EXISTS
            (
                SELECT 1
                FROM dbo.Manutencoes
                WHERE IdEquipamento =
                      @IdEquipamentoAnterior

                  AND Estado IN
                  (
                      N'Agendada',
                      N'Em curso'
                  )
            )
            BEGIN
                UPDATE dbo.Equipamentos
                SET Estado =
                    N'Operacional'
                WHERE IdEquipamento =
                      @IdEquipamentoAnterior

                  AND Estado <>
                      N'Abatido';
            END;
        END;

        /*
            Manutenção ativa:
            equipamento fica Em manutenção.
        */
        IF @Estado IN
        (
            N'Agendada',
            N'Em curso'
        )
        BEGIN
            UPDATE dbo.Equipamentos
            SET Estado =
                N'Em manutenção'
            WHERE IdEquipamento =
                  @IdEquipamento;
        END;
        ELSE
        BEGIN
            /*
                Concluída ou cancelada:
                só volta a Operacional se não houver
                outra manutenção ativa.
            */
            IF NOT EXISTS
            (
                SELECT 1
                FROM dbo.Manutencoes
                WHERE IdEquipamento =
                      @IdEquipamento

                  AND IdManutencao <>
                      @IdManutencao

                  AND Estado IN
                  (
                      N'Agendada',
                      N'Em curso'
                  )
            )
            BEGIN
                UPDATE dbo.Equipamentos
                SET Estado =
                    N'Operacional'
                WHERE IdEquipamento =
                      @IdEquipamento

                  AND Estado <>
                      N'Abatido';
            END;
        END;

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

CREATE OR ALTER PROCEDURE dbo.sp_Manutencoes_Eliminar
(
    @IdManutencao INT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IdEquipamento INT;
    DECLARE @Estado NVARCHAR(30);

    SELECT
        @IdEquipamento =
            IdEquipamento,

        @Estado =
            Estado

    FROM dbo.Manutencoes

    WHERE IdManutencao =
          @IdManutencao;

    IF @Estado IS NULL
    BEGIN
        THROW 50001,
              'A manutenção indicada não existe.',
              1;
    END;

    IF @Estado = N'Concluída'
    BEGIN
        THROW 50002,
              'Uma manutenção concluída não pode ser eliminada.',
              1;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE FROM dbo.Manutencoes
        WHERE IdManutencao =
              @IdManutencao;

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.Manutencoes
            WHERE IdEquipamento =
                  @IdEquipamento
              AND Estado IN
              (
                  N'Agendada',
                  N'Em curso'
              )
        )
        BEGIN
            UPDATE dbo.Equipamentos
            SET Estado =
                N'Operacional'
            WHERE IdEquipamento =
                  @IdEquipamento
              AND Estado <>
                  N'Abatido';
        END;

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

CREATE OR ALTER PROCEDURE dbo.sp_Manutencoes_Pesquisar
(
    @Pesquisa NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Pesquisa =
        LTRIM(RTRIM(@Pesquisa));

    SELECT
        M.IdManutencao,
        M.IdEquipamento,
        E.Nome AS NomeEquipamento,
        E.Marca,
        E.Modelo,
        M.Tipo,
        M.DataAgendada,
        M.DataRealizacao,
        M.Descricao,
        M.Responsavel,
        M.Custo,
        M.Estado,
        M.Observacoes
    FROM dbo.Manutencoes AS M

    INNER JOIN dbo.Equipamentos AS E
        ON E.IdEquipamento =
           M.IdEquipamento

    WHERE
        E.Nome LIKE
            N'%' + @Pesquisa + N'%'

        OR E.Marca LIKE
            N'%' + @Pesquisa + N'%'

        OR E.Modelo LIKE
            N'%' + @Pesquisa + N'%'

        OR M.Tipo LIKE
            N'%' + @Pesquisa + N'%'

        OR M.Descricao LIKE
            N'%' + @Pesquisa + N'%'

        OR M.Responsavel LIKE
            N'%' + @Pesquisa + N'%'

        OR M.Estado LIKE
            N'%' + @Pesquisa + N'%'

        OR M.Observacoes LIKE
            N'%' + @Pesquisa + N'%'

        OR CONVERT(
               NVARCHAR(10),
               M.DataAgendada,
               103
           ) LIKE
            N'%' + @Pesquisa + N'%'

        OR CONVERT(
               NVARCHAR(10),
               M.DataRealizacao,
               103
           ) LIKE
            N'%' + @Pesquisa + N'%'

        OR CAST(
               M.Custo AS NVARCHAR(30)
           ) LIKE
            N'%' + @Pesquisa + N'%'

    ORDER BY
        M.DataAgendada DESC,
        M.IdManutencao DESC;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Equipamentos_CriarManutencao
ON dbo.Equipamentos
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT UPDATE(Estado)
    BEGIN
        RETURN;
    END;

    /*
        Impede criar uma nova manutenção ativa
        quando já existe outra para o equipamento.
    */
    IF EXISTS
    (
        SELECT 1
        FROM inserted AS I

        INNER JOIN deleted AS D
            ON D.IdEquipamento =
               I.IdEquipamento

        WHERE I.Estado =
              N'Em manutenção'

          AND ISNULL(
                  D.Estado,
                  N''
              ) <>
              N'Em manutenção'

          AND EXISTS
          (
              SELECT 1
              FROM dbo.Manutencoes AS M
              WHERE M.IdEquipamento =
                    I.IdEquipamento

                AND M.Estado IN
                (
                    N'Agendada',
                    N'Em curso'
                )
          )
    )
    BEGIN
        THROW 51001,
              'Este equipamento já possui uma manutenção ativa.',
              1;
    END;

    /*
        Criar automaticamente a manutenção
        para os equipamentos que acabaram de passar
        para o estado Em manutenção.
    */
    INSERT INTO dbo.Manutencoes
    (
        IdEquipamento,
        Tipo,
        DataAgendada,
        DataRealizacao,
        Descricao,
        Responsavel,
        Custo,
        Estado,
        Observacoes
    )
    SELECT
        I.IdEquipamento,

        N'Corretiva',

        CAST(GETDATE() AS DATE),

        NULL,

        N'Pedido criado automaticamente após alteração do estado do equipamento para Em manutenção.',

        NULL,

        NULL,

        N'Agendada',

        N'Responsável por atribuir. Edite esta manutenção e indique o responsável.'

    FROM inserted AS I

    INNER JOIN deleted AS D
        ON D.IdEquipamento =
           I.IdEquipamento

    WHERE I.Estado =
          N'Em manutenção'

      AND ISNULL(
              D.Estado,
              N''
          ) <>
          N'Em manutenção';
END;
GO