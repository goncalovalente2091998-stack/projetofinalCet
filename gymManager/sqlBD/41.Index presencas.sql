CREATE INDEX IX_Presencas_IdCliente_DataEntrada
ON dbo.Presencas
(
    IdCliente,
    DataEntrada DESC
);
GO

CREATE UNIQUE INDEX UX_Presencas_Cliente_EntradaAberta
ON dbo.Presencas(IdCliente)
WHERE DataSaida IS NULL;
GO