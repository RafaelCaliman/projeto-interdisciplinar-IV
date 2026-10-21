-- =====================================================================
-- PROJETO INTERDISCIPLINAR IV - Hot-dog Brasilia
-- Script SQL completo (MySQL)
-- Entrega: 24/set - Regras de Negocio no BD + CRUDs Essenciais
--
-- Empresa: Hot-dog Brasilia  |  Resp.: Rogerio Rocha Caliman
-- Curso:   Sistemas de Informacao - UEMG - Unidade Frutal
-- Alunos:  Eduardo Pedro Batista - Guilherme da Silva Guerra
--          Luiz Guilherme Alves Almeida - Rafael Rosa Caliman
-- Ano:     2026
-- =====================================================================


-- ---------------------------------------------------------------------
-- 1. DDL - Criacao do banco e das tabelas
-- ---------------------------------------------------------------------

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


-- ---------------------------------------------------------------------
-- 2. TRIGGERS - Regras de negocio no banco
-- ---------------------------------------------------------------------

DROP TRIGGER IF EXISTS trg_item_venda_valida_estoque;
DROP TRIGGER IF EXISTS trg_item_venda_baixa_estoque;

-- RN13 / RN20 (RF09, RF19): nao permite vender mais do que o estoque disponivel.
DELIMITER //
CREATE TRIGGER trg_item_venda_valida_estoque
BEFORE INSERT ON item_venda
FOR EACH ROW
BEGIN
    DECLARE v_estoque INT;
    DECLARE v_nome VARCHAR(100);
    DECLARE v_msg VARCHAR(128);

    SELECT estoque, nome INTO v_estoque, v_nome
    FROM produto
    WHERE id_produto = NEW.id_produto
    FOR UPDATE;

    IF v_estoque IS NOT NULL AND v_estoque < NEW.quantidade THEN
        SET v_msg = LEFT(CONCAT('Estoque insuficiente para "', v_nome,
                                '": disponível ', v_estoque,
                                ', solicitado ', NEW.quantidade, '.'), 128);
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = v_msg;
    END IF;
END //
DELIMITER ;

-- RN07 (RF08): a cada item de venda registrado, reduz o estoque do produto.
DELIMITER //
CREATE TRIGGER trg_item_venda_baixa_estoque
AFTER INSERT ON item_venda
FOR EACH ROW
BEGIN
    UPDATE produto
    SET estoque = estoque - NEW.quantidade
    WHERE id_produto = NEW.id_produto;
END //
DELIMITER ;


-- ---------------------------------------------------------------------
-- 3. STORED PROCEDURES - CRUD de Cliente e Produto
--    (validacoes de RF16, RF17, RF18 / RN10, RN11, RN12 dentro do banco)
-- ---------------------------------------------------------------------

DROP PROCEDURE IF EXISTS sp_cliente_inserir;
DROP PROCEDURE IF EXISTS sp_cliente_atualizar;
DROP PROCEDURE IF EXISTS sp_cliente_excluir;
DROP PROCEDURE IF EXISTS sp_cliente_listar;
DROP PROCEDURE IF EXISTS sp_produto_inserir;
DROP PROCEDURE IF EXISTS sp_produto_atualizar;
DROP PROCEDURE IF EXISTS sp_produto_excluir;
DROP PROCEDURE IF EXISTS sp_produto_listar;
DROP PROCEDURE IF EXISTS sp_fornecedor_listar;

-- ----- CLIENTE -----
DELIMITER //
CREATE PROCEDURE sp_cliente_inserir(
    IN  p_nome       VARCHAR(100),
    IN  p_telefone   VARCHAR(20),
    OUT p_id_cliente INT
)
BEGIN
    IF p_nome IS NULL OR TRIM(p_nome) = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Nome do cliente é obrigatório.';
    END IF;
    IF p_telefone IS NULL OR TRIM(p_telefone) = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Telefone do cliente é obrigatório.';
    END IF;

    INSERT INTO cliente (nome, telefone)
    VALUES (TRIM(p_nome), TRIM(p_telefone));

    SET p_id_cliente = LAST_INSERT_ID();
END //
DELIMITER ;

-- Update (RF04)
DELIMITER //
CREATE PROCEDURE sp_cliente_atualizar(
    IN p_id_cliente INT,
    IN p_nome       VARCHAR(100),
    IN p_telefone   VARCHAR(20)
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM cliente WHERE id_cliente = p_id_cliente) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Cliente não encontrado.';
    END IF;
    IF p_nome IS NULL OR TRIM(p_nome) = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Nome do cliente é obrigatório.';
    END IF;
    IF p_telefone IS NULL OR TRIM(p_telefone) = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Telefone do cliente é obrigatório.';
    END IF;

    UPDATE cliente
    SET nome = TRIM(p_nome),
        telefone = TRIM(p_telefone)
    WHERE id_cliente = p_id_cliente;
END //
DELIMITER ;

-- Delete: bloqueia se o cliente ja tem vendas (preserva o historico).
DELIMITER //
CREATE PROCEDURE sp_cliente_excluir(
    IN p_id_cliente INT
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM cliente WHERE id_cliente = p_id_cliente) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Cliente não encontrado.';
    END IF;
    IF EXISTS (SELECT 1 FROM venda WHERE id_cliente = p_id_cliente) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Não é possível excluir: o cliente possui vendas registradas.';
    END IF;

    DELETE FROM cliente WHERE id_cliente = p_id_cliente;
END //
DELIMITER ;

-- Read: lista todos (p_busca NULL) ou filtra pelo nome.
DELIMITER //
CREATE PROCEDURE sp_cliente_listar(
    IN p_busca VARCHAR(100)
)
BEGIN
    SELECT id_cliente, nome, telefone
    FROM cliente
    WHERE p_busca IS NULL OR nome LIKE CONCAT('%', p_busca, '%')
    ORDER BY nome;
END //
DELIMITER ;

-- ----- PRODUTO -----

-- Create: nome obrigatorio (RF17/RN12), valor >= 0 (RF18/RN11), estoque >= 0
-- e fornecedor existente (RF02).
DELIMITER //
CREATE PROCEDURE sp_produto_inserir(
    IN  p_nome          VARCHAR(100),
    IN  p_valor         DECIMAL(10,2),
    IN  p_estoque       INT,
    IN  p_id_fornecedor INT,
    OUT p_id_produto    INT
)
BEGIN
    IF p_nome IS NULL OR TRIM(p_nome) = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Nome do produto é obrigatório.';
    END IF;
    IF p_valor IS NULL OR p_valor < 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Valor unitário inválido: deve ser informado e não pode ser negativo.';
    END IF;
    IF p_estoque IS NULL OR p_estoque < 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Estoque inválido: deve ser informado e não pode ser negativo.';
    END IF;
    IF p_id_fornecedor IS NULL
       OR NOT EXISTS (SELECT 1 FROM fornecedor WHERE id_fornecedor = p_id_fornecedor) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Fornecedor inválido ou não informado.';
    END IF;

    INSERT INTO produto (nome, valor, estoque, id_fornecedor)
    VALUES (TRIM(p_nome), p_valor, p_estoque, p_id_fornecedor);

    SET p_id_produto = LAST_INSERT_ID();
END //
DELIMITER ;

-- Update (RF04, RF18)
DELIMITER //
CREATE PROCEDURE sp_produto_atualizar(
    IN p_id_produto    INT,
    IN p_nome          VARCHAR(100),
    IN p_valor         DECIMAL(10,2),
    IN p_estoque       INT,
    IN p_id_fornecedor INT
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM produto WHERE id_produto = p_id_produto) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Produto não encontrado.';
    END IF;
    IF p_nome IS NULL OR TRIM(p_nome) = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Nome do produto é obrigatório.';
    END IF;
    IF p_valor IS NULL OR p_valor < 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Valor unitário inválido: deve ser informado e não pode ser negativo.';
    END IF;
    IF p_estoque IS NULL OR p_estoque < 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Estoque inválido: deve ser informado e não pode ser negativo.';
    END IF;
    IF p_id_fornecedor IS NULL
       OR NOT EXISTS (SELECT 1 FROM fornecedor WHERE id_fornecedor = p_id_fornecedor) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Fornecedor inválido ou não informado.';
    END IF;

    UPDATE produto
    SET nome = TRIM(p_nome),
        valor = p_valor,
        estoque = p_estoque,
        id_fornecedor = p_id_fornecedor
    WHERE id_produto = p_id_produto;
END //
DELIMITER ;

-- Delete: bloqueia se o produto ja foi vendido (preserva o historico de vendas).
DELIMITER //
CREATE PROCEDURE sp_produto_excluir(
    IN p_id_produto INT
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM produto WHERE id_produto = p_id_produto) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Produto não encontrado.';
    END IF;
    IF EXISTS (SELECT 1 FROM item_venda WHERE id_produto = p_id_produto) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Não é possível excluir: o produto já foi vendido.';
    END IF;

    DELETE FROM produto WHERE id_produto = p_id_produto;
END //
DELIMITER ;

-- Read: lista todos (p_busca NULL) ou busca pelo nome (RF24), com o fornecedor.
DELIMITER //
CREATE PROCEDURE sp_produto_listar(
    IN p_busca VARCHAR(100)
)
BEGIN
    SELECT p.id_produto, p.nome, p.valor, p.estoque,
           p.id_fornecedor, f.nome AS fornecedor
    FROM produto p
    JOIN fornecedor f ON f.id_fornecedor = p.id_fornecedor
    WHERE p_busca IS NULL OR p.nome LIKE CONCAT('%', p_busca, '%')
    ORDER BY p.nome;
END //
DELIMITER ;

-- ----- FORNECEDOR (apenas leitura, para preencher o ComboBox da tela de Produto) -----
DELIMITER //
CREATE PROCEDURE sp_fornecedor_listar()
BEGIN
    SELECT id_fornecedor, nome
    FROM fornecedor
    ORDER BY nome;
END //
DELIMITER ;


-- ---------------------------------------------------------------------
-- 4. DML - Insercao de dados de teste (seed)
-- ---------------------------------------------------------------------

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


-- ---------------------------------------------------------------------
-- 5. Conferencia apos o seed
-- ---------------------------------------------------------------------

SELECT id_produto, nome, estoque
FROM produto
ORDER BY id_produto;


-- ---------------------------------------------------------------------
-- 6. TESTES DE INTEGRACAO (evidencias da entrega)
-- ---------------------------------------------------------------------

-- ===== TESTE 1 (caminho feliz): o trigger baixa o estoque automaticamente (RN07) =====
START TRANSACTION;

SELECT 'ANTES' AS momento, id_produto, nome, estoque
FROM produto WHERE id_produto = 1;

INSERT INTO venda (data_hora, id_cliente) VALUES (NOW(), 1);
SET @id_venda_teste = LAST_INSERT_ID();
INSERT INTO item_venda (quantidade, id_venda, id_produto) VALUES (2, @id_venda_teste, 1);

SELECT 'DEPOIS' AS momento, id_produto, nome, estoque
FROM produto WHERE id_produto = 1;

ROLLBACK;

SELECT 'APOS ROLLBACK' AS momento, id_produto, nome, estoque
FROM produto WHERE id_produto = 1;   -- volta para 45


-- ===== TESTE 2 (caso de erro): estoque insuficiente e bloqueado (RN13/RN20) =====
DROP PROCEDURE IF EXISTS sp_teste_estoque_insuficiente;

DELIMITER //
CREATE PROCEDURE sp_teste_estoque_insuficiente()
BEGIN
    DECLARE v_msg VARCHAR(255);
    DECLARE v_id_venda INT;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 v_msg = MESSAGE_TEXT;
        ROLLBACK;
        SELECT 'ERRO ESPERADO - bloqueado pelo trigger' AS resultado, v_msg AS mensagem;
        SELECT id_produto, nome, estoque AS estoque_apos_tentativa
        FROM produto WHERE id_produto = 1;
    END;

    START TRANSACTION;
    INSERT INTO venda (data_hora, id_cliente) VALUES (NOW(), 1);
    SET v_id_venda = LAST_INSERT_ID();
    INSERT INTO item_venda (quantidade, id_venda, id_produto) VALUES (999, v_id_venda, 1);

    -- Se chegou aqui, o trigger NAO bloqueou: o teste falhou.
    ROLLBACK;
    SELECT '*** FALHA: a venda de 999 unidades nao foi bloqueada ***' AS resultado;
END //
DELIMITER ;

CALL sp_teste_estoque_insuficiente();
DROP PROCEDURE sp_teste_estoque_insuficiente;


-- ===== TESTE 3 (caminho feliz): CRUD de Cliente e Produto pelas procedures =====
START TRANSACTION;

-- Cliente: Create -> Read -> Update -> Read -> Delete -> Read
CALL sp_cliente_inserir('Ana Teste', '34990001111', @id_cliente_teste);
SELECT @id_cliente_teste AS id_cliente_criado;
CALL sp_cliente_listar('Ana');
CALL sp_cliente_atualizar(@id_cliente_teste, 'Ana Teste Editada', '34990002222');
CALL sp_cliente_listar('Ana');
CALL sp_cliente_excluir(@id_cliente_teste);
CALL sp_cliente_listar('Ana');   -- vazio

-- Produto: Create -> Read (busca por nome) -> Update -> Read -> Delete -> Read
CALL sp_produto_inserir('Batata frita', 10.00, 20, 1, @id_produto_teste);
SELECT @id_produto_teste AS id_produto_criado;
CALL sp_produto_listar('Batata');
CALL sp_produto_atualizar(@id_produto_teste, 'Batata frita grande', 14.00, 25, 1);
CALL sp_produto_listar('Batata');
CALL sp_produto_excluir(@id_produto_teste);
CALL sp_produto_listar('Batata');   -- vazio

ROLLBACK;


-- ===== TESTE 4 (casos de erro): validacoes minimas dos CRUDs =====
DROP PROCEDURE IF EXISTS sp_teste_validacoes;
DROP TEMPORARY TABLE IF EXISTS teste_validacoes;

CREATE TEMPORARY TABLE teste_validacoes (
    caso     VARCHAR(80),
    mensagem VARCHAR(255)
);

DELIMITER //
CREATE PROCEDURE sp_teste_validacoes()
BEGIN
    DECLARE v_msg VARCHAR(255) DEFAULT NULL;
    DECLARE v_id INT;

    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 v_msg = MESSAGE_TEXT;
    END;

    SET v_msg = NULL;
    CALL sp_cliente_inserir('', '34999990000', v_id);
    INSERT INTO teste_validacoes VALUES ('Cliente sem nome (RF16)',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_cliente_inserir('Fulano', '', v_id);
    INSERT INTO teste_validacoes VALUES ('Cliente sem telefone (RF16)',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_cliente_atualizar(9999, 'Fulano', '34999990000');
    INSERT INTO teste_validacoes VALUES ('Atualizar cliente inexistente',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_cliente_excluir(1);
    INSERT INTO teste_validacoes VALUES ('Excluir cliente que já tem vendas',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_produto_inserir('', 5.00, 10, 1, v_id);
    INSERT INTO teste_validacoes VALUES ('Produto sem nome (RF17)',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_produto_inserir('Produto Teste', -1.00, 10, 1, v_id);
    INSERT INTO teste_validacoes VALUES ('Produto com valor negativo (RF18)',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_produto_inserir('Produto Teste', 5.00, -3, 1, v_id);
    INSERT INTO teste_validacoes VALUES ('Produto com estoque negativo',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_produto_inserir('Produto Teste', 5.00, 10, 999, v_id);
    INSERT INTO teste_validacoes VALUES ('Produto com fornecedor inexistente',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_produto_excluir(1);
    INSERT INTO teste_validacoes VALUES ('Excluir produto que já foi vendido',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));
END //
DELIMITER ;

CALL sp_teste_validacoes();

SELECT caso, mensagem FROM teste_validacoes;

DROP PROCEDURE sp_teste_validacoes;
DROP TEMPORARY TABLE teste_validacoes;


-- ---------------------------------------------------------------------
-- Conferencia final
-- ---------------------------------------------------------------------
SELECT id_produto, nome, estoque FROM produto ORDER BY id_produto;
SELECT (SELECT COUNT(*) FROM cliente) AS clientes,
       (SELECT COUNT(*) FROM venda)   AS vendas,
       (SELECT COUNT(*) FROM item_venda) AS itens_venda;
