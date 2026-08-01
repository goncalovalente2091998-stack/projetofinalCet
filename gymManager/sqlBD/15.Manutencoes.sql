CREATE TABLE dbo.Manutencoes
(
    IdManutencao INT IDENTITY(1,1)
        CONSTRAINT PK_Manutencoes PRIMARY KEY,

    IdEquipamento INT NOT NULL,

    Tipo NVARCHAR(30) NOT NULL,

    DataAgendada DATE NOT NULL,

    DataRealizacao DATE NULL,

    Descricao NVARCHAR(500) NOT NULL,

    Responsavel NVARCHAR(100) NULL,

    Custo DECIMAL(10,2) NULL,

    Estado NVARCHAR(30) NOT NULL
        CONSTRAINT DF_Manutencoes_Estado
        DEFAULT N'Agendada',

    Observacoes NVARCHAR(500) NULL,

    CONSTRAINT FK_Manutencoes_Equipamentos
        FOREIGN KEY (IdEquipamento)
        REFERENCES dbo.Equipamentos(IdEquipamento),

    CONSTRAINT CK_Manutencoes_Tipo
        CHECK
        (
            Tipo IN
            (
                N'Preventiva',
                N'Corretiva',
                N'Inspeção'
            )
        ),

    CONSTRAINT CK_Manutencoes_Estado
        CHECK
        (
            Estado IN
            (
                N'Agendada',
                N'Em curso',
                N'Concluída',
                N'Cancelada'
            )
        ),

    CONSTRAINT CK_Manutencoes_Custo
        CHECK
        (
            Custo IS NULL
            OR Custo >= 0
        ),

    CONSTRAINT CK_Manutencoes_DataRealizacao
        CHECK
        (
            DataRealizacao IS NULL
            OR DataRealizacao >= DataAgendada
        ),

    CONSTRAINT CK_Manutencoes_DadosConcluida
        CHECK
        (
            Estado <> N'Concluída'
            OR DataRealizacao IS NOT NULL
        )
);
GO