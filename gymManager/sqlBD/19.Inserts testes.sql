INSERT INTO Utilizadores (Nome, Email, PasswordHash, Perfil)
VALUES
('Administrador', 'admin@gym.pt', 'admin123', 'Administrador'),
('Rececionista', 'rececao@gym.pt', 'rececao123', 'Rececionista');

INSERT INTO Professores (Nome, Especialidade, Telefone, Email)
VALUES
('João Martins', 'CrossFit', '912345678', 'joao@gym.pt'),
('Ana Costa', 'Yoga', '913456789', 'ana@gym.pt');

INSERT INTO Planos (Nome, Preco, DuracaoMeses, Descricao)
VALUES
('Mensal', 35.00, 1, 'Plano mensal'),
('Trimestral', 95.00, 3, 'Plano trimestral'),
('Anual', 320.00, 12, 'Plano anual');

INSERT INTO Clientes
(Nome,NIF,DataNascimento,Telefone,Email,Morada,DataInscricao,Estado)
VALUES
('Carlos Silva','123456789','1998-05-20','911111111','carlos@email.pt','Rua A','2025-01-05',1),

('Maria Ferreira','987654321','1995-03-15','922222222','maria@email.pt','Rua B','2025-02-01',1),

('Pedro Sousa','111222333','2000-10-10','933333333','pedro@email.pt','Rua C','2025-03-01',1);

INSERT INTO PersonalTrainers
(Nome,Especialidade,Telefone,Email,ValorHora,Estado)
VALUES
('Miguel Santos','Hipertrofia','944444444','miguel@gym.pt',25.00,1),

('Ricardo Alves','Perda de Peso','955555555','ricardo@gym.pt',30.00,1);

INSERT INTO Equipamentos
(Nome,Marca,Estado,DataCompra)
VALUES
('Passadeira','Technogym','Operacional','2023-01-10'),

('Bicicleta','Life Fitness','Operacional','2022-06-15'),

('Supino','BH Fitness','Operacional','2021-04-20');

INSERT INTO Aulas
(IdProfessor,Nome,Horario,Lotacao,Sala)
VALUES
(1,'CrossFit','18:00',20,'Sala 1'),

(2,'Yoga','19:00',15,'Sala 2');

INSERT INTO Inscricoes
(IdCliente,IdPlano,DataInicio,DataFim,Estado)
VALUES
(1,1,'2025-01-05','2025-02-05','Ativa'),

(2,2,'2025-02-01','2025-05-01','Ativa'),

(3,3,'2025-03-01','2026-03-01','Ativa');

INSERT INTO Pagamentos
(IdCliente,DataPagamento,Valor,MetodoPagamento,Observacoes)
VALUES
(1,'2025-01-05',35.00,'Multibanco',''),

(2,'2025-02-01',95.00,'MB Way',''),

(3,'2025-03-01',320.00,'Cartão','');


INSERT INTO AvaliacoesFisicas
(IdCliente,Peso,Altura,IMC,MassaGorda,MassaMuscular,Observacoes)
VALUES
(1,80,1.80,24.69,18,40,''),

(2,60,1.65,22.04,24,28,''),

(3,74,1.78,23.36,19,38,'');


INSERT INTO AgendamentosPT
(IdCliente,IdPT,DataSessao,HoraInicio,HoraFim,Estado,Observacoes)
VALUES
(1,1,'2025-07-20','10:00','11:00','Marcada',''),

(2,2,'2025-07-21','15:00','16:00','Marcada','');


INSERT INTO PlanosTreino
(IdCliente,IdPT,NomePlano,Objetivo,DataInicio,DataFim,Observacoes)
VALUES
(1,1,'Hipertrofia A','Ganhar Massa Muscular','2025-07-01','2025-09-01',''),

(2,2,'Emagrecimento','Perda de Peso','2025-07-01','2025-09-01','');

INSERT INTO Exercicios
(IdPlanoTreino,Nome,Series,Repeticoes,TempoDescanso)
VALUES
(1,'Supino Plano',4,10,90),

(1,'Agachamento',4,12,120),

(2,'Passadeira',1,30,0),

(2,'Prancha',3,60,60);

INSERT INTO Manutencoes
(IdEquipamento,DataManutencao,Tipo,Observacoes)
VALUES
(1,'2025-05-01','Preventiva',''),

(2,'2025-06-15','Corretiva','Troca de correia');

INSERT INTO InscricoesAulas
(IdCliente,IdAula,DataInscricao)
VALUES
(1,1,'2025-07-10'),

(2,2,'2025-07-10'),

(3,1,'2025-07-11');