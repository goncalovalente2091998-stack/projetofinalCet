CREATE INDEX IX_EventosAgenda_DataInicio
ON dbo.EventosAgenda
(
    DataInicio
);
GO 
CREATE INDEX IX_EventosAgenda_IdPT_DataInicio
ON dbo.EventosAgenda
(
    IdPT,
    DataInicio
);
GO