CREATE TABLE AvaliacoesFisicas (
    IdAvaliacao INT IDENTITY(1,1) PRIMARY KEY,
    IdCliente INT NOT NULL,
    Peso DECIMAL(5,2) NOT NULL,
    Altura DECIMAL(4,2) NOT NULL,
    IMC DECIMAL(5,2) NOT NULL,
    MassaGorda DECIMAL(5,2) NOT NULL,
    MassaMuscular DECIMAL(5,2) NOT NULL,
    Observacoes NVARCHAR(255)
);