CREATE TABLE dbo.EventosAgenda
(
    IdEvento INT IDENTITY(1,1)
        CONSTRAINT PK_EventosAgenda PRIMARY KEY,

    Titulo NVARCHAR(150) NOT NULL,

    Tipo NVARCHAR(30) NOT NULL,

    DataInicio DATETIME2(0) NOT NULL,

    DataFim DATETIME2(0) NOT NULL,

    /*
        Sessão PT:
        - IdPT obrigatório
        - IdCliente obrigatório

        Aula:
        - IdProfessor obrigatório
        - IdAula obrigatório
    */
    IdPT INT NULL,

    IdProfessor INT NULL,

    IdCliente INT NULL,

    IdAula INT NULL,

    Localizacao NVARCHAR(100) NULL,

    Descricao NVARCHAR(500) NULL,

    Estado NVARCHAR(20) NOT NULL
        CONSTRAINT DF_EventosAgenda_Estado
        DEFAULT N'Agendado',

    CONSTRAINT FK_EventosAgenda_PersonalTrainers
        FOREIGN KEY (IdPT)
        REFERENCES dbo.PersonalTrainers(IdPT),

    CONSTRAINT FK_EventosAgenda_Professores
        FOREIGN KEY (IdProfessor)
        REFERENCES dbo.Professores(IdProfessor),

    CONSTRAINT FK_EventosAgenda_Clientes
        FOREIGN KEY (IdCliente)
        REFERENCES dbo.Clientes(IdCliente),

    CONSTRAINT FK_EventosAgenda_Aulas
        FOREIGN KEY (IdAula)
        REFERENCES dbo.Aulas(IdAula),

    CONSTRAINT CK_EventosAgenda_Tipo
        CHECK
        (
            Tipo IN
            (
                N'Sessão PT',
                N'Aula'
            )
        ),

    CONSTRAINT CK_EventosAgenda_Estado
        CHECK
        (
            Estado IN
            (
                N'Agendado',
                N'Concluído',
                N'Cancelado'
            )
        ),

    CONSTRAINT CK_EventosAgenda_Datas
        CHECK
        (
            DataFim > DataInicio
        ),

    CONSTRAINT CK_EventosAgenda_Associacao
        CHECK
        (
            (
                Tipo = N'Sessão PT'
                AND IdPT IS NOT NULL
                AND IdCliente IS NOT NULL
                AND IdProfessor IS NULL
                AND IdAula IS NULL
            )
            OR
            (
                Tipo = N'Aula'
                AND IdPT IS NULL
                AND IdCliente IS NULL
                AND IdProfessor IS NOT NULL
                AND IdAula IS NOT NULL
            )
        )
);
GO
