CREATE TABLE Exercicios (
    IdExercicio INT IDENTITY(1,1) PRIMARY KEY,
    IdPlanoTreino INT NOT NULL,
    Nome NVARCHAR(100) NOT NULL,
    Series INT NOT NULL,
    Repeticoes INT NOT NULL,
    TempoDescanso INT NOT NULL
);