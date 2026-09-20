-- =========================================================
-- Projeto Interdisciplinar IV — Hot-dog Brasília
-- Script DDL (tabelas + índices) + Seed de Dados
-- Entrega: 15/set — Modelo Físico + DDL + Conexão C#<->MySQL
-- =========================================================

-- ---------- DDL: criação do banco "do zero" ----------
DROP DATABASE IF EXISTS hotdog_brasilia;
CREATE DATABASE hotdog_brasilia;
USE hotdog_brasilia;

CREATE TABLE fornecedor (
    id_fornecedor INT PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(100) NOT NULL
);

CREATE TABLE cliente (
    id_cliente INT PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(100) NOT NULL,
    telefone VARCHAR(20) NOT NULL
);

CREATE TABLE produto (
    id_produto INT PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(100) NOT NULL,
    valor DECIMAL(10,2) NOT NULL,
    estoque INT NOT NULL,
    id_fornecedor INT NOT NULL,
    CHECK (valor >= 0),
    CHECK (estoque >= 0),
    FOREIGN KEY (id_fornecedor) REFERENCES fornecedor(id_fornecedor)
);

CREATE INDEX idx_produto_nome ON produto(nome);

CREATE TABLE venda (
    id_venda INT PRIMARY KEY AUTO_INCREMENT,
    data_hora DATETIME NOT NULL,
    id_cliente INT NOT NULL,
    FOREIGN KEY (id_cliente) REFERENCES cliente(id_cliente)
);

CREATE INDEX idx_venda_data_hora ON venda(data_hora);

CREATE TABLE item_venda (
    id_item INT PRIMARY KEY AUTO_INCREMENT,
    quantidade INT NOT NULL,
    id_venda INT NOT NULL,
    id_produto INT NOT NULL,
    CHECK (quantidade > 0),
    FOREIGN KEY (id_venda) REFERENCES venda(id_venda),
    FOREIGN KEY (id_produto) REFERENCES produto(id_produto)
);

CREATE TABLE financeiro (
    id_financeiro INT PRIMARY KEY AUTO_INCREMENT,
    tipo ENUM('RECEBER','PAGAR') NOT NULL,
    descricao VARCHAR(150),
    valor DECIMAL(10,2) NOT NULL,
    data_lancamento DATE NOT NULL,
    status_pagamento ENUM('PAGO','PENDENTE') NOT NULL,
    id_venda INT,
    CHECK (valor >= 0),
    FOREIGN KEY (id_venda) REFERENCES venda(id_venda)
);

CREATE INDEX idx_financeiro_data_lancamento ON financeiro(data_lancamento);

-- ---------- DML: dados de teste (seed) ----------
INSERT INTO fornecedor (nome) VALUES
('Distribuidora São Paulo'),
('Frigorífico Central');

INSERT INTO cliente (nome, telefone) VALUES
('João Silva', '34991110001'),
('Maria Souza', '34992220002'),
('Pedro Lima', '34993330003');

INSERT INTO produto (nome, valor, estoque, id_fornecedor) VALUES
('Hot-dog simples', 8.00, 50, 1),
('Hot-dog duplo', 12.00, 40, 1),
('Refrigerante', 5.00, 60, 2),
('Suco natural', 6.00, 30, 2);

INSERT INTO venda (data_hora, id_cliente) VALUES
('2026-05-10 12:00:00', 1),
('2026-05-10 13:30:00', 2),
('2026-05-11 11:00:00', 3);

INSERT INTO item_venda (quantidade, id_venda, id_produto) VALUES
(2, 1, 1),
(1, 1, 3),
(1, 2, 2),
(2, 2, 3),
(3, 3, 1),
(1, 3, 4);

INSERT INTO financeiro (tipo, descricao, valor, data_lancamento, status_pagamento, id_venda) VALUES
('RECEBER', 'Venda balcão', 21.00, '2026-05-10', 'PAGO', 1),
('RECEBER', 'Venda balcão', 22.00, '2026-05-10', 'PAGO', 2),
('PAGAR', 'Compra insumos', 150.00, '2026-05-11', 'PENDENTE', NULL),
('PAGAR', 'Pagamento a fornecedor', 50.00, '2026-05-10', 'PAGO', NULL),
('RECEBER', 'Venda balcão', 30.00, '2026-05-11', 'PAGO', 3);