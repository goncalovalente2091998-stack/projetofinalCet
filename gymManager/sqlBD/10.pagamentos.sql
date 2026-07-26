CREATE TABLE Pagamentos (
    IdPagamento INT IDENTITY(1,1) PRIMARY KEY,
    IdCliente INT NOT NULL,
    DataPagamento DATE NOT NULL,
    Valor DECIMAL(10,2) NOT NULL,
    MetodoPagamento NVARCHAR(50) NOT NULL,
    Observacoes NVARCHAR(255)
);