CREATE TABLE dbo.PlanosTreino
(
    IdPlanoTreino INT IDENTITY(1,1)
        CONSTRAINT PK_PlanosTreino PRIMARY KEY,

    IdCliente INT NOT NULL,

    IdPT INT NOT NULL,

    NomePlano NVARCHAR(100) NOT NULL,

    Objetivo NVARCHAR(255) NOT NULL,

    DataInicio DATE NOT NULL,

    DataFim DATE NOT NULL,

    Observacoes NVARCHAR(255) NULL,

    Estado NVARCHAR(20) NOT NULL
        CONSTRAINT DF_PlanosTreino_Estado
        DEFAULT N'Ativo',

    CONSTRAINT FK_PlanosTreino_Clientes
        FOREIGN KEY (IdCliente)
        REFERENCES dbo.Clientes(IdCliente),

    CONSTRAINT FK_PlanosTreino_PersonalTrainers
        FOREIGN KEY (IdPT)
        REFERENCES dbo.PersonalTrainers(IdPT),

    CONSTRAINT CK_PlanosTreino_Datas
        CHECK (DataFim >= DataInicio),

    CONSTRAINT CK_PlanosTreino_Estado
        CHECK
        (
            Estado IN
            (
                N'Ativo',
                N'Concluído',
                N'Cancelado'
            )
        )
);
GO