CREATE TABLE dbo.Presencas
(
    IdPresenca INT IDENTITY(1,1)
        CONSTRAINT PK_Presencas PRIMARY KEY,

    IdCliente INT NOT NULL,

    DataEntrada DATETIME2(0) NOT NULL
        CONSTRAINT DF_Presencas_DataEntrada
        DEFAULT SYSDATETIME(),

    DataSaida DATETIME2(0) NULL,

    Observacoes NVARCHAR(255) NULL,

    CONSTRAINT FK_Presencas_Clientes
        FOREIGN KEY (IdCliente)
        REFERENCES dbo.Clientes(IdCliente),

    CONSTRAINT CK_Presencas_DataSaida
        CHECK
        (
            DataSaida IS NULL
            OR DataSaida >= DataEntrada
        )
);
GO