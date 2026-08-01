CREATE TABLE dbo.Equipamentos
(
    IdEquipamento INT IDENTITY(1,1)
        CONSTRAINT PK_Equipamentos PRIMARY KEY,

    Nome NVARCHAR(100) NOT NULL,

    Categoria NVARCHAR(50) NOT NULL,

    Marca NVARCHAR(100) NOT NULL,

    Modelo NVARCHAR(50) NULL,

    NumeroSerie NVARCHAR(100) NULL,

    DataAquisicao DATE NOT NULL,

    Localizacao NVARCHAR(100) NOT NULL,

    Estado NVARCHAR(50) NOT NULL
        CONSTRAINT DF_Equipamentos_Estado
        DEFAULT N'Operacional',

    Observacoes NVARCHAR(500) NULL,

    CONSTRAINT UQ_Equipamentos_NumeroSerie
        UNIQUE (NumeroSerie),

    CONSTRAINT CK_Equipamentos_Estado
        CHECK
        (
            Estado IN
            (
                N'Operacional',
                N'Em manutenção',
                N'Fora de serviço',
                N'Abatido'
            )
        )
);
GO