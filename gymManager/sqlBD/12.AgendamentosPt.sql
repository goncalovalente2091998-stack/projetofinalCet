CREATE TABLE AgendamentosPT (
    IdAgenda INT IDENTITY(1,1) PRIMARY KEY,
    IdCliente INT NOT NULL,
    IdPT INT NOT NULL,
    DataSessao DATE NOT NULL,
    HoraInicio TIME NOT NULL,
    HoraFim TIME NOT NULL,
    Estado NVARCHAR(50) NOT NULL,
    Observacoes NVARCHAR(255)
);