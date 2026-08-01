CREATE TABLE dbo.PlanoTreinoExercicios
(
    IdPlanoTreinoExercicio INT IDENTITY(1,1)
        CONSTRAINT PK_PlanoTreinoExercicios PRIMARY KEY,

    IdPlanoTreino INT NOT NULL,

    IdExercicio INT NOT NULL,

    Series INT NOT NULL,

    Repeticoes INT NOT NULL,

    TempoDescanso INT NOT NULL,

    Ordem INT NOT NULL,

    Observacoes NVARCHAR(255) NULL,

    CONSTRAINT FK_PlanoTreinoExercicios_PlanosTreino
        FOREIGN KEY (IdPlanoTreino)
        REFERENCES dbo.PlanosTreino(IdPlanoTreino),

    CONSTRAINT FK_PlanoTreinoExercicios_Exercicios
        FOREIGN KEY (IdExercicio)
        REFERENCES dbo.Exercicios(IdExercicio),

    CONSTRAINT UQ_PlanoTreinoExercicios_PlanoExercicio
        UNIQUE (IdPlanoTreino, IdExercicio),

    CONSTRAINT CK_PlanoTreinoExercicios_Series
        CHECK (Series > 0),

    CONSTRAINT CK_PlanoTreinoExercicios_Repeticoes
        CHECK (Repeticoes > 0),

    CONSTRAINT CK_PlanoTreinoExercicios_Descanso
        CHECK (TempoDescanso >= 0),

    CONSTRAINT CK_PlanoTreinoExercicios_Ordem
        CHECK (Ordem > 0)
);
GO