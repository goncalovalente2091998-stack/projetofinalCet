CREATE TABLE Pagamentos (
    IdPagamento INT IDENTITY(1,1) PRIMARY KEY,
    IdCliente INT NOT NULL,
    DataPagamento DATE NOT NULL,
    Valor DECIMAL(10,2) NOT NULL,
    MetodoPagamento NVARCHAR(50) NOT NULL,
    Observacoes NVARCHAR(255)
);

ALTER TABLE Pagamentos
ADD
    Estado NVARCHAR(30) NOT NULL
        CONSTRAINT DF_Pagamentos_Estado DEFAULT 'Pendente',

    ReferenciaExterna NVARCHAR(150) NULL,

    IdTransacaoExterna NVARCHAR(150) NULL,

    DataConfirmacao DATETIME2 NULL;
GO

ALTER TABLE Pagamentos
ADD IdInscricao INT NULL;
GO

ALTER TABLE Pagamentos
ADD CONSTRAINT FK_Pagamentos_Inscricoes
FOREIGN KEY (IdInscricao)
REFERENCES Inscricoes(IdInscricao);
GO