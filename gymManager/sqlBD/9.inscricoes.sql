CREATE TABLE Inscricoes (
    IdInscricao INT IDENTITY(1,1) PRIMARY KEY,
    IdCliente INT NOT NULL,
    IdPlano INT NOT NULL,
    DataInicio DATE NOT NULL,
    DataFim DATE NOT NULL,
    Estado NVARCHAR(50) NOT NULL
);