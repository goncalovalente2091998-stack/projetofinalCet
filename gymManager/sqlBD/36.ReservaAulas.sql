CREATE TABLE dbo.ReservasAulas
(
    IdReserva INT IDENTITY(1,1)
        CONSTRAINT PK_ReservasAulas PRIMARY KEY,

    IdAula INT NOT NULL,

    IdCliente INT NOT NULL,

    DataReserva DATETIME2 NOT NULL
        CONSTRAINT DF_ReservasAulas_DataReserva
        DEFAULT SYSDATETIME(),

    Estado NVARCHAR(20) NOT NULL
        CONSTRAINT DF_ReservasAulas_Estado
        DEFAULT N'Confirmada',

    CONSTRAINT FK_ReservasAulas_Aulas
        FOREIGN KEY (IdAula)
        REFERENCES dbo.Aulas(IdAula),

    CONSTRAINT FK_ReservasAulas_Clientes
        FOREIGN KEY (IdCliente)
        REFERENCES dbo.Clientes(IdCliente),

    CONSTRAINT UQ_ReservasAulas_AulaCliente
        UNIQUE (IdAula, IdCliente),

    CONSTRAINT CK_ReservasAulas_Estado
        CHECK
        (
            Estado IN
            (
                N'Confirmada',
                N'Cancelada',
                N'Presente',
                N'Faltou'
            )
        )
);
GO