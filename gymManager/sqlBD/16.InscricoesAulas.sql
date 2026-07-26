CREATE TABLE InscricoesAulas (
    IdInscricao INT IDENTITY(1,1) PRIMARY KEY,
    IdCliente INT NOT NULL,
    IdAula INT NOT NULL,
    DataInscricao DATE NOT NULL
);