CREATE TABLE dbo.AvaliacoesFisicas
(
    IdAvaliacao INT IDENTITY(1,1)
        CONSTRAINT PK_AvaliacoesFisicas PRIMARY KEY,

    IdCliente INT NOT NULL,

    IdPT INT NOT NULL,

    DataAvaliacao DATE NOT NULL
        CONSTRAINT DF_AvaliacoesFisicas_Data
        DEFAULT (CAST(GETDATE() AS DATE)),

    Peso DECIMAL(5,2) NULL,

    Altura DECIMAL(4,2) NULL,

    IMC DECIMAL(5,2) NULL,

    MassaGorda DECIMAL(5,2) NULL,

    MassaMuscular DECIMAL(5,2) NULL,

    Observacoes NVARCHAR(255) NULL,

    Estado NVARCHAR(20) NOT NULL
        CONSTRAINT DF_AvaliacoesFisicas_Estado
        DEFAULT N'Agendada',

    CONSTRAINT FK_AvaliacoesFisicas_Clientes
        FOREIGN KEY (IdCliente)
        REFERENCES dbo.Clientes(IdCliente),

    CONSTRAINT FK_AvaliacoesFisicas_PersonalTrainers
        FOREIGN KEY (IdPT)
        REFERENCES dbo.PersonalTrainers(IdPT),

    CONSTRAINT CK_AvaliacoesFisicas_Peso
        CHECK
        (
            Peso IS NULL
            OR Peso > 0
        ),

    CONSTRAINT CK_AvaliacoesFisicas_Altura
        CHECK
        (
            Altura IS NULL
            OR Altura > 0
        ),

    CONSTRAINT CK_AvaliacoesFisicas_IMC
        CHECK
        (
            IMC IS NULL
            OR IMC > 0
        ),

    CONSTRAINT CK_AvaliacoesFisicas_MassaGorda
        CHECK
        (
            MassaGorda IS NULL
            OR MassaGorda BETWEEN 0 AND 100
        ),

    CONSTRAINT CK_AvaliacoesFisicas_MassaMuscular
        CHECK
        (
            MassaMuscular IS NULL
            OR MassaMuscular > 0
        ),

    CONSTRAINT CK_AvaliacoesFisicas_Estado
        CHECK
        (
            Estado IN
            (
                N'Agendada',
                N'Concluída',
                N'Cancelada'
            )
        ),

    CONSTRAINT CK_AvaliacoesFisicas_DadosConcluida
        CHECK
        (
            Estado <> N'Concluída'
            OR
            (
                Peso IS NOT NULL
                AND Altura IS NOT NULL
                AND IMC IS NOT NULL
                AND MassaGorda IS NOT NULL
                AND MassaMuscular IS NOT NULL
            )
        )
);
GO