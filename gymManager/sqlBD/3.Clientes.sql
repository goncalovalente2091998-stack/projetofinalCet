CREATE TABLE Clientes (
    IdCliente INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    NIF CHAR(9) UNIQUE,
    DataNascimento DATE NOT NULL,
    Telefone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100),
    Morada NVARCHAR(200),
    DataInscricao DATE NOT NULL,
    Estado BIT NOT NULL;
);