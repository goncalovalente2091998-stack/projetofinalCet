CREATE TABLE PlanosTreino (
    IdPlanoTreino INT IDENTITY(1,1) PRIMARY KEY,
    IdCliente INT NOT NULL,
    IdPT INT NOT NULL,
    NomePlano NVARCHAR(100) NOT NULL,
    Objetivo NVARCHAR(255) NOT NULL,
    DataInicio DATE NOT NULL,
    DataFim DATE NOT NULL,
    Observacoes NVARCHAR(255)
);