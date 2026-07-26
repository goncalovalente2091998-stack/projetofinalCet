CREATE TABLE Manutencoes (
    IdManutencao INT IDENTITY(1,1) PRIMARY KEY,
    IdEquipamento INT NOT NULL,
    DataManutencao DATE NOT NULL,
    Tipo NVARCHAR(100) NOT NULL,
    Observacoes NVARCHAR(255)
);