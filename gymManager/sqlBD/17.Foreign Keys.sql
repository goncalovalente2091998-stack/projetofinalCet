ALTER TABLE Aulas
ADD CONSTRAINT FK_Aulas_Professores
FOREIGN KEY (IdProfessor)
REFERENCES Professores(IdProfessor);



ALTER TABLE Inscricoes
ADD CONSTRAINT FK_Inscricoes_Clientes
FOREIGN KEY (IdCliente)
REFERENCES Clientes(IdCliente);



ALTER TABLE Inscricoes
ADD CONSTRAINT FK_Inscricoes_Planos
FOREIGN KEY (IdPlano)
REFERENCES Planos(IdPlano);



ALTER TABLE Pagamentos
ADD CONSTRAINT FK_Pagamentos_Clientes
FOREIGN KEY (IdCliente)
REFERENCES Clientes(IdCliente);



ALTER TABLE AvaliacoesFisicas
ADD CONSTRAINT FK_AvaliacoesFisicas_Clientes
FOREIGN KEY (IdCliente)
REFERENCES Clientes(IdCliente);



ALTER TABLE AgendamentosPT
ADD CONSTRAINT FK_AgendamentosPT_Clientes
FOREIGN KEY (IdCliente)
REFERENCES Clientes(IdCliente);



ALTER TABLE AgendamentosPT
ADD CONSTRAINT FK_AgendamentosPT_PT
FOREIGN KEY (IdPT)
REFERENCES PersonalTrainers(IdPT);



ALTER TABLE PlanosTreino
ADD CONSTRAINT FK_PlanosTreino_Clientes
FOREIGN KEY (IdCliente)
REFERENCES Clientes(IdCliente);



ALTER TABLE PlanosTreino
ADD CONSTRAINT FK_PlanosTreino_PT
FOREIGN KEY (IdPT)
REFERENCES PersonalTrainers(IdPT);



ALTER TABLE Exercicios
ADD CONSTRAINT FK_Exercicios_PlanosTreino
FOREIGN KEY (IdPlanoTreino)
REFERENCES PlanosTreino(IdPlanoTreino);



ALTER TABLE Manutencoes
ADD CONSTRAINT FK_Manutencoes_Equipamentos
FOREIGN KEY (IdEquipamento)
REFERENCES Equipamentos(IdEquipamento);


ALTER TABLE InscricoesAulas
ADD CONSTRAINT FK_InscricoesAulas_Clientes
FOREIGN KEY (IdCliente)
REFERENCES Clientes(IdCliente);



ALTER TABLE InscricoesAulas
ADD CONSTRAINT FK_InscricoesAulas_Aulas
FOREIGN KEY (IdAula)
REFERENCES Aulas(IdAula);