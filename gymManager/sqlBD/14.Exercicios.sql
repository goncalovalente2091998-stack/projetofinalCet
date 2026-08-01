CREATE TABLE dbo.Exercicios
(
    IdExercicio INT IDENTITY(1,1)
        CONSTRAINT PK_Exercicios PRIMARY KEY,

    Nome NVARCHAR(100) NOT NULL,

    GrupoMuscular NVARCHAR(50) NOT NULL,

    Equipamento NVARCHAR(100) NULL,

    Descricao NVARCHAR(500) NULL,

    Dificuldade NVARCHAR(20) NOT NULL
        CONSTRAINT DF_Exercicios_Dificuldade
        DEFAULT N'Intermédio',

    Estado NVARCHAR(20) NOT NULL
        CONSTRAINT DF_Exercicios_Estado
        DEFAULT N'Ativo',

    CONSTRAINT UQ_Exercicios_Nome
        UNIQUE (Nome),

    CONSTRAINT CK_Exercicios_Dificuldade
        CHECK
        (
            Dificuldade IN
            (
                N'Iniciante',
                N'Intermédio',
                N'Avançado'
            )
        ),

    CONSTRAINT CK_Exercicios_Estado
        CHECK
        (
            Estado IN
            (
                N'Ativo',
                N'Inativo'
            )
        )
);
GO