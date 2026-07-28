CREATE TABLE Aulas
(
    IdAula INT IDENTITY(1,1) PRIMARY KEY,

    IdProfessor INT NOT NULL,

    Nome NVARCHAR(100) NOT NULL,

    DataAula DATE NOT NULL,

    HoraInicio TIME NOT NULL,

    DuracaoMinutos INT NOT NULL,

    Lotacao INT NOT NULL,

    Sala NVARCHAR(50) NOT NULL,

    Estado NVARCHAR(20) NOT NULL
        CONSTRAINT DF_Aulas_Estado
        DEFAULT 'Agendada',

    CONSTRAINT FK_Aulas_Professores
        FOREIGN KEY (IdProfessor)
        REFERENCES Professores(IdProfessor),

    CONSTRAINT CK_Aulas_Lotacao
        CHECK (Lotacao > 0),

    CONSTRAINT CK_Aulas_Duracao
        CHECK (DuracaoMinutos > 0),

    CONSTRAINT CK_Aulas_Estado
        CHECK
        (
            Estado IN
            (
                'Agendada',
                'Concluída',
                'Cancelada'
            )
        )
);
GO