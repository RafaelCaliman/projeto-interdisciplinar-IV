-- =====================================================================
-- PROJETO INTERDISCIPLINAR IV - Hot-dog Brasilia
-- Script SQL do banco FINAL (v3) - MySQL 8.0
--
-- Base: v2 (entrega 24/set: regras no BD + CRUDs de Cliente e Produto)
-- Novidades v3:
--   20/out - fluxo de venda, estorno de estoque, login, perfis e logs
--   21/nov - procedures de relatorio (vendas, estoque, financeiro)
--
-- ATENCAO: este script APAGA e recria o banco hotdog_brasilia.
-- Rodar com o app fechado. Executar no Workbench 8.0 (mesmo formato do v2).
--
-- Empresa: Hot-dog Brasilia  |  Resp.: Rogerio Rocha Caliman
-- Curso:   Sistemas de Informacao - UEMG - Unidade Frutal
-- Alunos:  Eduardo Pedro Batista - Guilherme da Silva Guerra
--          Luiz Guilherme Alves Almeida - Rafael Rosa Caliman
-- Ano:     2026
--
-- Usuarios iniciais (TROCAR as senhas antes da apresentacao):
--   admin / admin123   operador / operador123   gerencia / gerencia123
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

-- ----- NOVAS (v3): controle de acesso e auditoria -----

-- Perfis do sistema (ADMIN, OPERADOR, GERENCIA).
CREATE TABLE perfil (
    id_perfil INT PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(30) NOT NULL UNIQUE,
    descricao VARCHAR(150)
);

-- Acoes que o app pode liberar ou bloquear (ex.: CLIENTE_EXCLUIR).
CREATE TABLE permissao (
    id_permissao INT PRIMARY KEY AUTO_INCREMENT,
    codigo VARCHAR(40) NOT NULL UNIQUE,
    descricao VARCHAR(150) NOT NULL
);

-- Matriz: quais permissoes cada perfil possui.
CREATE TABLE perfil_permissao (
    id_perfil INT NOT NULL,
    id_permissao INT NOT NULL,
    PRIMARY KEY (id_perfil, id_permissao),
    FOREIGN KEY (id_perfil) REFERENCES perfil(id_perfil),
    FOREIGN KEY (id_permissao) REFERENCES permissao(id_permissao)
);

-- Quem usa o sistema (nao confundir com cliente: cliente so compra).
-- A senha nunca e guardada em texto: SHA2(salt + senha).
CREATE TABLE usuario (
    id_usuario INT PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(100) NOT NULL,
    login VARCHAR(50) NOT NULL UNIQUE,
    senha_hash CHAR(64) NOT NULL,
    salt CHAR(32) NOT NULL,
    id_perfil INT NOT NULL,
    ativo TINYINT(1) NOT NULL DEFAULT 1,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ultimo_login DATETIME NULL,
    CHECK (ativo IN (0,1)),
    FOREIGN KEY (id_perfil) REFERENCES perfil(id_perfil)
);

-- Log minimo: quem (id_usuario), o que (acao/entidade) e quando (data_hora).
-- id_usuario fica NULL em tentativa de login com usuario desconhecido.
CREATE TABLE log_acao (
    id_log INT PRIMARY KEY AUTO_INCREMENT,
    data_hora DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    id_usuario INT NULL,
    acao VARCHAR(30) NOT NULL,
    entidade VARCHAR(40),
    id_registro INT,
    detalhe VARCHAR(255),
    FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario)
);

CREATE INDEX idx_log_data_hora ON log_acao(data_hora);


-- ---------------------------------------------------------------------
-- 2. TRIGGERS - Regras de negocio no banco
-- ---------------------------------------------------------------------

DROP TRIGGER IF EXISTS trg_item_venda_valida_estoque;
DROP TRIGGER IF EXISTS trg_item_venda_baixa_estoque;
DROP TRIGGER IF EXISTS trg_item_venda_valida_alteracao;
DROP TRIGGER IF EXISTS trg_item_venda_ajusta_estoque;
DROP TRIGGER IF EXISTS trg_item_venda_estorna_estoque;

-- RN13 / RN20 / RN23: bloqueia quantidade invalida e venda acima do estoque.
DELIMITER //
CREATE TRIGGER trg_item_venda_valida_estoque
BEFORE INSERT ON item_venda
FOR EACH ROW
BEGIN
    DECLARE v_estoque INT;
    DECLARE v_nome VARCHAR(100);
    DECLARE v_msg VARCHAR(128);

    IF NEW.quantidade IS NULL OR NEW.quantidade <= 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Quantidade inválida: deve ser maior que zero.';
    END IF;

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

-- RN07: a cada item de venda registrado, reduz o estoque do produto.
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

-- v3: alterar a quantidade de um item valida o estoque pela diferenca.
-- Nao permite trocar o produto nem a venda do item (excluir e incluir outro).
DELIMITER //
CREATE TRIGGER trg_item_venda_valida_alteracao
BEFORE UPDATE ON item_venda
FOR EACH ROW
BEGIN
    DECLARE v_estoque INT;
    DECLARE v_nome VARCHAR(100);
    DECLARE v_msg VARCHAR(128);

    IF NEW.id_produto <> OLD.id_produto OR NEW.id_venda <> OLD.id_venda THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Não é permitido trocar o produto ou a venda de um item: exclua e inclua outro.';
    END IF;

    IF NEW.quantidade IS NULL OR NEW.quantidade <= 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Quantidade inválida: deve ser maior que zero.';
    END IF;

    SELECT estoque, nome INTO v_estoque, v_nome
    FROM produto
    WHERE id_produto = NEW.id_produto
    FOR UPDATE;

    IF v_estoque IS NOT NULL AND (v_estoque + OLD.quantidade) < NEW.quantidade THEN
        SET v_msg = LEFT(CONCAT('Estoque insuficiente para "', v_nome,
                                '": disponível ', (v_estoque + OLD.quantidade),
                                ', solicitado ', NEW.quantidade, '.'), 128);
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = v_msg;
    END IF;
END //
DELIMITER ;

-- v3: ao alterar a quantidade de um item, ajusta o estoque pela diferenca.
DELIMITER //
CREATE TRIGGER trg_item_venda_ajusta_estoque
AFTER UPDATE ON item_venda
FOR EACH ROW
BEGIN
    UPDATE produto
    SET estoque = estoque + OLD.quantidade - NEW.quantidade
    WHERE id_produto = NEW.id_produto;
END //
DELIMITER ;

-- v3 (estorno): ao excluir um item de venda, o estoque volta.
DELIMITER //
CREATE TRIGGER trg_item_venda_estorna_estoque
AFTER DELETE ON item_venda
FOR EACH ROW
BEGIN
    UPDATE produto
    SET estoque = estoque + OLD.quantidade
    WHERE id_produto = OLD.id_produto;
END //
DELIMITER ;


-- ---------------------------------------------------------------------
-- 3. STORED PROCEDURES
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
DROP PROCEDURE IF EXISTS sp_venda_inserir;
DROP PROCEDURE IF EXISTS sp_item_venda_inserir;
DROP PROCEDURE IF EXISTS sp_item_venda_excluir;
DROP PROCEDURE IF EXISTS sp_venda_concluir;
DROP PROCEDURE IF EXISTS sp_venda_cancelar;
DROP PROCEDURE IF EXISTS sp_venda_itens;
DROP PROCEDURE IF EXISTS sp_financeiro_inserir;
DROP PROCEDURE IF EXISTS sp_financeiro_marcar_pago;
DROP PROCEDURE IF EXISTS sp_financeiro_listar;
DROP PROCEDURE IF EXISTS sp_relatorio_vendas;
DROP PROCEDURE IF EXISTS sp_relatorio_vendas_resumo;
DROP PROCEDURE IF EXISTS sp_relatorio_estoque;
DROP PROCEDURE IF EXISTS sp_relatorio_financeiro;
DROP PROCEDURE IF EXISTS sp_usuario_inserir;
DROP PROCEDURE IF EXISTS sp_usuario_atualizar;
DROP PROCEDURE IF EXISTS sp_usuario_alterar_senha;
DROP PROCEDURE IF EXISTS sp_usuario_definir_ativo;
DROP PROCEDURE IF EXISTS sp_usuario_listar;
DROP PROCEDURE IF EXISTS sp_usuario_autenticar;
DROP PROCEDURE IF EXISTS sp_perfil_listar;
DROP PROCEDURE IF EXISTS sp_permissoes_do_usuario;
DROP PROCEDURE IF EXISTS sp_log_registrar;
DROP PROCEDURE IF EXISTS sp_log_listar;

-- ===== 3.1 CLIENTE (RF01, RF04, RF16 / RN01, RN04, RN10) =====
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

-- ===== 3.2 PRODUTO (RF02, RF17, RF18, RF24 / RN02, RN11, RN12, RN21, RN22) =====
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

-- ===== 3.3 FORNECEDOR (apenas leitura, para o ComboBox da tela de Produto) =====
DELIMITER //
CREATE PROCEDURE sp_fornecedor_listar()
BEGIN
    SELECT id_fornecedor, nome
    FROM fornecedor
    ORDER BY nome;
END //
DELIMITER ;

-- ===== 3.4 FLUXO DE VENDA (v3 - RF05, RF06, RF07, RF20, RF23 / RN05, RN06, RN14, RN16, RN23, RN24) =====
-- Sequencia no app: sp_venda_inserir -> sp_item_venda_inserir (1 ou mais)
--                   -> sp_venda_concluir. Para desistir: sp_venda_cancelar.
-- Uma venda "concluida" e a que ja tem lancamento no financeiro.
-- O app deve chamar esta sequencia dentro de uma MySqlTransaction.

DELIMITER //
CREATE PROCEDURE sp_venda_inserir(
    IN  p_id_cliente INT,
    OUT p_id_venda   INT
)
BEGIN
    IF p_id_cliente IS NULL
       OR NOT EXISTS (SELECT 1 FROM cliente WHERE id_cliente = p_id_cliente) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Cliente não encontrado.';
    END IF;

    INSERT INTO venda (data_hora, id_cliente)
    VALUES (NOW(), p_id_cliente);

    SET p_id_venda = LAST_INSERT_ID();
END //
DELIMITER ;

-- Adiciona um item. O trigger valida o estoque e da baixa (RN07, RN13, RN20).
DELIMITER //
CREATE PROCEDURE sp_item_venda_inserir(
    IN  p_id_venda   INT,
    IN  p_id_produto INT,
    IN  p_quantidade INT,
    OUT p_id_item    INT
)
BEGIN
    IF p_id_venda IS NULL
       OR NOT EXISTS (SELECT 1 FROM venda WHERE id_venda = p_id_venda) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Venda não encontrada.';
    END IF;
    IF EXISTS (SELECT 1 FROM financeiro WHERE id_venda = p_id_venda) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Venda já concluída: não é possível alterar seus itens.';
    END IF;
    IF p_id_produto IS NULL
       OR NOT EXISTS (SELECT 1 FROM produto WHERE id_produto = p_id_produto) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Produto não encontrado.';
    END IF;
    IF p_quantidade IS NULL OR p_quantidade <= 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Quantidade inválida: deve ser maior que zero.';
    END IF;

    INSERT INTO item_venda (quantidade, id_venda, id_produto)
    VALUES (p_quantidade, p_id_venda, p_id_produto);

    SET p_id_item = LAST_INSERT_ID();
END //
DELIMITER ;

-- Remove um item de venda em andamento. O trigger devolve o estoque.
DELIMITER //
CREATE PROCEDURE sp_item_venda_excluir(
    IN p_id_item INT
)
BEGIN
    DECLARE v_id_venda INT;

    SELECT id_venda INTO v_id_venda
    FROM item_venda
    WHERE id_item = p_id_item;

    IF v_id_venda IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Item de venda não encontrado.';
    END IF;
    IF EXISTS (SELECT 1 FROM financeiro WHERE id_venda = v_id_venda) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Venda já concluída: não é possível alterar seus itens.';
    END IF;

    DELETE FROM item_venda WHERE id_item = p_id_item;
END //
DELIMITER ;

-- Conclui a venda: exige ao menos 1 item (RN14), calcula o total (RN16, RN24),
-- grava a data/hora da conclusao (RN06) e lanca a receita no financeiro.
DELIMITER //
CREATE PROCEDURE sp_venda_concluir(
    IN  p_id_venda         INT,
    IN  p_status_pagamento VARCHAR(10),
    OUT p_total            DECIMAL(10,2)
)
BEGIN
    DECLARE v_total DECIMAL(10,2);

    IF p_id_venda IS NULL
       OR NOT EXISTS (SELECT 1 FROM venda WHERE id_venda = p_id_venda) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Venda não encontrada.';
    END IF;
    IF EXISTS (SELECT 1 FROM financeiro WHERE id_venda = p_id_venda) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Venda já concluída.';
    END IF;
    IF NOT EXISTS (SELECT 1 FROM item_venda WHERE id_venda = p_id_venda) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Venda sem itens: inclua ao menos um produto.';
    END IF;
    IF p_status_pagamento IS NULL OR p_status_pagamento NOT IN ('PAGO','PENDENTE') THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Status de pagamento inválido: use PAGO ou PENDENTE.';
    END IF;

    SELECT SUM(i.quantidade * p.valor) INTO v_total
    FROM item_venda i
    JOIN produto p ON p.id_produto = i.id_produto
    WHERE i.id_venda = p_id_venda;

    INSERT INTO financeiro (tipo, descricao, valor, data_lancamento, status_pagamento, id_venda)
    VALUES ('RECEBER', CONCAT('Venda balcão #', p_id_venda), v_total, CURDATE(),
            p_status_pagamento, p_id_venda);

    UPDATE venda SET data_hora = NOW() WHERE id_venda = p_id_venda;

    SET p_total = v_total;
END //
DELIMITER ;

-- Cancela a venda inteira: os itens saem (o trigger devolve o estoque),
-- o lancamento financeiro e a venda tambem.
DELIMITER //
CREATE PROCEDURE sp_venda_cancelar(
    IN p_id_venda INT
)
BEGIN
    IF p_id_venda IS NULL
       OR NOT EXISTS (SELECT 1 FROM venda WHERE id_venda = p_id_venda) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Venda não encontrada.';
    END IF;

    DELETE FROM item_venda WHERE id_venda = p_id_venda;
    DELETE FROM financeiro WHERE id_venda = p_id_venda;
    DELETE FROM venda WHERE id_venda = p_id_venda;
END //
DELIMITER ;

-- Itens de uma venda com subtotal (RN16).
DELIMITER //
CREATE PROCEDURE sp_venda_itens(
    IN p_id_venda INT
)
BEGIN
    IF p_id_venda IS NULL
       OR NOT EXISTS (SELECT 1 FROM venda WHERE id_venda = p_id_venda) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Venda não encontrada.';
    END IF;

    SELECT i.id_item, p.nome AS produto, i.quantidade,
           p.valor AS valor_unitario,
           (i.quantidade * p.valor) AS subtotal
    FROM item_venda i
    JOIN produto p ON p.id_produto = i.id_produto
    WHERE i.id_venda = p_id_venda
    ORDER BY i.id_item;
END //
DELIMITER ;

-- ===== 3.5 FINANCEIRO (v3 - RF10, RF22, RF25 / RN08, RN17) =====
DELIMITER //
CREATE PROCEDURE sp_financeiro_inserir(
    IN  p_tipo            VARCHAR(10),
    IN  p_descricao       VARCHAR(150),
    IN  p_valor           DECIMAL(10,2),
    IN  p_data_lancamento DATE,
    IN  p_status          VARCHAR(10),
    IN  p_id_venda        INT,
    OUT p_id_financeiro   INT
)
BEGIN
    IF p_tipo IS NULL OR p_tipo NOT IN ('RECEBER','PAGAR') THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Tipo inválido: use RECEBER ou PAGAR.';
    END IF;
    IF p_valor IS NULL OR p_valor < 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Valor inválido: deve ser informado e não pode ser negativo.';
    END IF;
    IF p_data_lancamento IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Data de lançamento é obrigatória.';
    END IF;
    IF p_status IS NULL OR p_status NOT IN ('PAGO','PENDENTE') THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Status inválido: use PAGO ou PENDENTE.';
    END IF;
    IF p_id_venda IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM venda WHERE id_venda = p_id_venda) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Venda não encontrada.';
    END IF;

    INSERT INTO financeiro (tipo, descricao, valor, data_lancamento, status_pagamento, id_venda)
    VALUES (p_tipo, NULLIF(TRIM(p_descricao), ''), p_valor, p_data_lancamento, p_status, p_id_venda);

    SET p_id_financeiro = LAST_INSERT_ID();
END //
DELIMITER ;

-- RF25: PENDENTE -> PAGO.
DELIMITER //
CREATE PROCEDURE sp_financeiro_marcar_pago(
    IN p_id_financeiro INT
)
BEGIN
    DECLARE v_status VARCHAR(10);

    SELECT status_pagamento INTO v_status
    FROM financeiro
    WHERE id_financeiro = p_id_financeiro;

    IF v_status IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lançamento não encontrado.';
    END IF;
    IF v_status = 'PAGO' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Lançamento já está pago.';
    END IF;

    UPDATE financeiro
    SET status_pagamento = 'PAGO'
    WHERE id_financeiro = p_id_financeiro;
END //
DELIMITER ;

-- Lista lancamentos. Filtros NULL (ou vazio) = todos.
DELIMITER //
CREATE PROCEDURE sp_financeiro_listar(
    IN p_tipo     VARCHAR(10),
    IN p_status   VARCHAR(10),
    IN p_data_ini DATE,
    IN p_data_fim DATE
)
BEGIN
    SELECT f.id_financeiro, f.tipo, f.descricao, f.valor,
           f.data_lancamento, f.status_pagamento, f.id_venda
    FROM financeiro f
    WHERE (p_tipo IS NULL OR p_tipo = '' OR f.tipo = p_tipo)
      AND (p_status IS NULL OR p_status = '' OR f.status_pagamento = p_status)
      AND (p_data_ini IS NULL OR f.data_lancamento >= p_data_ini)
      AND (p_data_fim IS NULL OR f.data_lancamento <= p_data_fim)
    ORDER BY f.data_lancamento DESC, f.id_financeiro DESC;
END //
DELIMITER ;

-- ===== 3.6 RELATORIOS (v3 - RF13, RF14, RF15 / RN09, RN18, RN19) =====
-- Datas NULL = sem limite. O total da venda usa o valor atual do produto (RN16).

-- RF13: vendas por periodo (uma linha por venda).
DELIMITER //
CREATE PROCEDURE sp_relatorio_vendas(
    IN p_data_ini DATE,
    IN p_data_fim DATE
)
BEGIN
    SELECT v.id_venda, v.data_hora, c.nome AS cliente,
           COALESCE(SUM(i.quantidade * p.valor), 0) AS total
    FROM venda v
    JOIN cliente c ON c.id_cliente = v.id_cliente
    LEFT JOIN item_venda i ON i.id_venda = v.id_venda
    LEFT JOIN produto p ON p.id_produto = i.id_produto
    WHERE (p_data_ini IS NULL OR DATE(v.data_hora) >= p_data_ini)
      AND (p_data_fim IS NULL OR DATE(v.data_hora) <= p_data_fim)
    GROUP BY v.id_venda, v.data_hora, c.nome
    ORDER BY v.data_hora DESC, v.id_venda DESC;
END //
DELIMITER ;

-- RF13: total do periodo (quantidade de vendas, total vendido, ticket medio).
DELIMITER //
CREATE PROCEDURE sp_relatorio_vendas_resumo(
    IN p_data_ini DATE,
    IN p_data_fim DATE
)
BEGIN
    SELECT COUNT(DISTINCT v.id_venda) AS qtd_vendas,
           COALESCE(SUM(i.quantidade * p.valor), 0) AS total_vendido,
           ROUND(COALESCE(SUM(i.quantidade * p.valor) /
                 NULLIF(COUNT(DISTINCT v.id_venda), 0), 0), 2) AS ticket_medio
    FROM venda v
    LEFT JOIN item_venda i ON i.id_venda = v.id_venda
    LEFT JOIN produto p ON p.id_produto = i.id_produto
    WHERE (p_data_ini IS NULL OR DATE(v.data_hora) >= p_data_ini)
      AND (p_data_fim IS NULL OR DATE(v.data_hora) <= p_data_fim);
END //
DELIMITER ;

-- RF14: estoque por produto, com situacao. Limite NULL = 10 unidades.
DELIMITER //
CREATE PROCEDURE sp_relatorio_estoque(
    IN p_limite_baixo INT
)
BEGIN
    SELECT p.id_produto, p.nome AS produto, p.estoque,
           f.nome AS fornecedor,
           CASE
               WHEN p.estoque = 0 THEN 'ESGOTADO'
               WHEN p.estoque <= COALESCE(p_limite_baixo, 10) THEN 'BAIXO'
               ELSE 'OK'
           END AS situacao
    FROM produto p
    JOIN fornecedor f ON f.id_fornecedor = p.id_fornecedor
    ORDER BY p.estoque, p.nome;
END //
DELIMITER ;

-- RF11, RF12, RF15: entradas, saidas e saldo do periodo.
--   recebido / pago          = lancamentos PAGO
--   a_receber / a_pagar      = lancamentos PENDENTE
--   saldo_realizado          = recebido - pago
--   saldo_previsto           = (recebido + a_receber) - (pago + a_pagar)
DELIMITER //
CREATE PROCEDURE sp_relatorio_financeiro(
    IN p_data_ini DATE,
    IN p_data_fim DATE
)
BEGIN
    SELECT t.recebido, t.a_receber, t.pago, t.a_pagar,
           (t.recebido - t.pago) AS saldo_realizado,
           ((t.recebido + t.a_receber) - (t.pago + t.a_pagar)) AS saldo_previsto
    FROM (
        SELECT
            COALESCE(SUM(CASE WHEN tipo = 'RECEBER' AND status_pagamento = 'PAGO'
                              THEN valor END), 0) AS recebido,
            COALESCE(SUM(CASE WHEN tipo = 'RECEBER' AND status_pagamento = 'PENDENTE'
                              THEN valor END), 0) AS a_receber,
            COALESCE(SUM(CASE WHEN tipo = 'PAGAR' AND status_pagamento = 'PAGO'
                              THEN valor END), 0) AS pago,
            COALESCE(SUM(CASE WHEN tipo = 'PAGAR' AND status_pagamento = 'PENDENTE'
                              THEN valor END), 0) AS a_pagar
        FROM financeiro
        WHERE (p_data_ini IS NULL OR data_lancamento >= p_data_ini)
          AND (p_data_fim IS NULL OR data_lancamento <= p_data_fim)
    ) t;
END //
DELIMITER ;

-- ===== 3.7 ACESSO: USUARIO, PERFIL E LOG (v3 - login e controle por perfil) =====
-- O hash e feito no proprio banco, entao o app so envia login e senha.
-- Obs.: abordagem didatica (SHA-256 com salt); em producao usar bcrypt/Argon2.

-- Cadastro de usuario. Senha minima de 6 caracteres.
DELIMITER //
CREATE PROCEDURE sp_usuario_inserir(
    IN  p_nome       VARCHAR(100),
    IN  p_login      VARCHAR(50),
    IN  p_senha      VARCHAR(100),
    IN  p_id_perfil  INT,
    OUT p_id_usuario INT
)
BEGIN
    DECLARE v_salt CHAR(32);

    IF p_nome IS NULL OR TRIM(p_nome) = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Nome do usuário é obrigatório.';
    END IF;
    IF p_login IS NULL OR TRIM(p_login) = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Login é obrigatório.';
    END IF;
    IF EXISTS (SELECT 1 FROM usuario WHERE login = TRIM(p_login)) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Login já está em uso.';
    END IF;
    IF p_senha IS NULL OR CHAR_LENGTH(p_senha) < 6 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Senha inválida: mínimo de 6 caracteres.';
    END IF;
    IF p_id_perfil IS NULL
       OR NOT EXISTS (SELECT 1 FROM perfil WHERE id_perfil = p_id_perfil) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Perfil inválido ou não informado.';
    END IF;

    SET v_salt = REPLACE(UUID(), '-', '');

    INSERT INTO usuario (nome, login, senha_hash, salt, id_perfil)
    VALUES (TRIM(p_nome), TRIM(p_login), SHA2(CONCAT(v_salt, p_senha), 256), v_salt, p_id_perfil);

    SET p_id_usuario = LAST_INSERT_ID();
END //
DELIMITER ;

-- Altera nome e perfil. Nao deixa o sistema sem administrador ativo.
DELIMITER //
CREATE PROCEDURE sp_usuario_atualizar(
    IN p_id_usuario INT,
    IN p_nome       VARCHAR(100),
    IN p_id_perfil  INT
)
BEGIN
    DECLARE v_perfil_atual VARCHAR(30);
    DECLARE v_perfil_novo  VARCHAR(30);

    IF p_id_usuario IS NULL
       OR NOT EXISTS (SELECT 1 FROM usuario WHERE id_usuario = p_id_usuario) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Usuário não encontrado.';
    END IF;
    IF p_nome IS NULL OR TRIM(p_nome) = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Nome do usuário é obrigatório.';
    END IF;

    SELECT nome INTO v_perfil_novo FROM perfil WHERE id_perfil = p_id_perfil;
    IF v_perfil_novo IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Perfil inválido ou não informado.';
    END IF;

    SELECT pf.nome INTO v_perfil_atual
    FROM usuario u
    JOIN perfil pf ON pf.id_perfil = u.id_perfil
    WHERE u.id_usuario = p_id_usuario;

    IF v_perfil_atual = 'ADMIN' AND v_perfil_novo <> 'ADMIN'
       AND NOT EXISTS (SELECT 1
                       FROM usuario u2
                       JOIN perfil pf2 ON pf2.id_perfil = u2.id_perfil
                       WHERE pf2.nome = 'ADMIN' AND u2.ativo = 1
                         AND u2.id_usuario <> p_id_usuario) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Não é possível rebaixar o último administrador ativo.';
    END IF;

    UPDATE usuario
    SET nome = TRIM(p_nome),
        id_perfil = p_id_perfil
    WHERE id_usuario = p_id_usuario;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE sp_usuario_alterar_senha(
    IN p_id_usuario INT,
    IN p_senha_nova VARCHAR(100)
)
BEGIN
    DECLARE v_salt CHAR(32);

    IF p_id_usuario IS NULL
       OR NOT EXISTS (SELECT 1 FROM usuario WHERE id_usuario = p_id_usuario) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Usuário não encontrado.';
    END IF;
    IF p_senha_nova IS NULL OR CHAR_LENGTH(p_senha_nova) < 6 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Senha inválida: mínimo de 6 caracteres.';
    END IF;

    SET v_salt = REPLACE(UUID(), '-', '');

    UPDATE usuario
    SET salt = v_salt,
        senha_hash = SHA2(CONCAT(v_salt, p_senha_nova), 256)
    WHERE id_usuario = p_id_usuario;
END //
DELIMITER ;

-- Em vez de excluir (preserva o historico do log), o usuario e inativado.
DELIMITER //
CREATE PROCEDURE sp_usuario_definir_ativo(
    IN p_id_usuario INT,
    IN p_ativo      TINYINT
)
BEGIN
    IF p_id_usuario IS NULL
       OR NOT EXISTS (SELECT 1 FROM usuario WHERE id_usuario = p_id_usuario) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Usuário não encontrado.';
    END IF;
    IF p_ativo IS NULL OR p_ativo NOT IN (0, 1) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Situação inválida: use 1 (ativo) ou 0 (inativo).';
    END IF;

    IF p_ativo = 0
       AND EXISTS (SELECT 1
                   FROM usuario u
                   JOIN perfil pf ON pf.id_perfil = u.id_perfil
                   WHERE u.id_usuario = p_id_usuario AND pf.nome = 'ADMIN')
       AND NOT EXISTS (SELECT 1
                       FROM usuario u2
                       JOIN perfil pf2 ON pf2.id_perfil = u2.id_perfil
                       WHERE pf2.nome = 'ADMIN' AND u2.ativo = 1
                         AND u2.id_usuario <> p_id_usuario) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Não é possível inativar o último administrador ativo.';
    END IF;

    UPDATE usuario SET ativo = p_ativo WHERE id_usuario = p_id_usuario;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE sp_usuario_listar()
BEGIN
    SELECT u.id_usuario, u.nome, u.login, pf.nome AS perfil,
           u.ativo, u.ultimo_login
    FROM usuario u
    JOIN perfil pf ON pf.id_perfil = u.id_perfil
    ORDER BY u.nome;
END //
DELIMITER ;

-- Login. Retorna id, nome, login e perfil do usuario autenticado.
-- Registra LOGIN ou LOGIN_FALHA no log. Mensagem unica para login/senha errados.
DELIMITER //
CREATE PROCEDURE sp_usuario_autenticar(
    IN p_login VARCHAR(50),
    IN p_senha VARCHAR(100)
)
BEGIN
    DECLARE v_id    INT;
    DECLARE v_ativo TINYINT;

    SELECT id_usuario, ativo INTO v_id, v_ativo
    FROM usuario
    WHERE login = TRIM(p_login)
      AND senha_hash = SHA2(CONCAT(salt, IFNULL(p_senha, '')), 256)
    LIMIT 1;

    IF v_id IS NULL THEN
        INSERT INTO log_acao (id_usuario, acao, entidade, detalhe)
        VALUES (NULL, 'LOGIN_FALHA', 'usuario', LEFT(CONCAT('login=', IFNULL(p_login, '')), 255));
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Login ou senha inválidos.';
    END IF;

    IF v_ativo = 0 THEN
        INSERT INTO log_acao (id_usuario, acao, entidade, id_registro, detalhe)
        VALUES (v_id, 'LOGIN_FALHA', 'usuario', v_id, 'usuario inativo');
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Usuário inativo. Procure o administrador.';
    END IF;

    UPDATE usuario SET ultimo_login = NOW() WHERE id_usuario = v_id;

    INSERT INTO log_acao (id_usuario, acao, entidade, id_registro)
    VALUES (v_id, 'LOGIN', 'usuario', v_id);

    SELECT u.id_usuario, u.nome, u.login, u.id_perfil, pf.nome AS perfil
    FROM usuario u
    JOIN perfil pf ON pf.id_perfil = u.id_perfil
    WHERE u.id_usuario = v_id;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE sp_perfil_listar()
BEGIN
    SELECT id_perfil, nome, descricao
    FROM perfil
    ORDER BY id_perfil;
END //
DELIMITER ;

-- Codigos de permissao do usuario (o app libera/bloqueia telas e botoes).
DELIMITER //
CREATE PROCEDURE sp_permissoes_do_usuario(
    IN p_id_usuario INT
)
BEGIN
    SELECT pm.codigo
    FROM usuario u
    JOIN perfil_permissao pp ON pp.id_perfil = u.id_perfil
    JOIN permissao pm ON pm.id_permissao = pp.id_permissao
    WHERE u.id_usuario = p_id_usuario
    ORDER BY pm.codigo;
END //
DELIMITER ;

-- Log: o app chama depois de cada acao relevante (quem, o que, quando).
-- Ex.: CALL sp_log_registrar(1, 'INSERIR', 'cliente', 7, 'Cliente Ana Teste');
DELIMITER //
CREATE PROCEDURE sp_log_registrar(
    IN p_id_usuario  INT,
    IN p_acao        VARCHAR(30),
    IN p_entidade    VARCHAR(40),
    IN p_id_registro INT,
    IN p_detalhe     VARCHAR(255)
)
BEGIN
    IF p_acao IS NULL OR TRIM(p_acao) = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Ação do log é obrigatória.';
    END IF;
    IF p_id_usuario IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM usuario WHERE id_usuario = p_id_usuario) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Usuário não encontrado.';
    END IF;

    INSERT INTO log_acao (id_usuario, acao, entidade, id_registro, detalhe)
    VALUES (p_id_usuario, UPPER(TRIM(p_acao)), NULLIF(TRIM(p_entidade), ''),
            p_id_registro, NULLIF(TRIM(p_detalhe), ''));
END //
DELIMITER ;

-- Consulta o log. Filtros NULL = todos.
DELIMITER //
CREATE PROCEDURE sp_log_listar(
    IN p_data_ini   DATE,
    IN p_data_fim   DATE,
    IN p_id_usuario INT
)
BEGIN
    SELECT l.id_log, l.data_hora,
           COALESCE(u.nome, '(não identificado)') AS usuario,
           l.acao, l.entidade, l.id_registro, l.detalhe
    FROM log_acao l
    LEFT JOIN usuario u ON u.id_usuario = l.id_usuario
    WHERE (p_data_ini IS NULL OR DATE(l.data_hora) >= p_data_ini)
      AND (p_data_fim IS NULL OR DATE(l.data_hora) <= p_data_fim)
      AND (p_id_usuario IS NULL OR l.id_usuario = p_id_usuario)
    ORDER BY l.data_hora DESC, l.id_log DESC;
END //
DELIMITER ;


-- ---------------------------------------------------------------------
-- 4. VIEW - Matriz de permissoes por perfil (evidencia "perfis/permite")
-- ---------------------------------------------------------------------

CREATE OR REPLACE VIEW vw_matriz_permissoes AS
SELECT pm.codigo AS permissao,
       pm.descricao,
       CASE WHEN SUM(pf.nome = 'ADMIN')    > 0 THEN 'SIM' ELSE '-' END AS admin,
       CASE WHEN SUM(pf.nome = 'GERENCIA') > 0 THEN 'SIM' ELSE '-' END AS gerencia,
       CASE WHEN SUM(pf.nome = 'OPERADOR') > 0 THEN 'SIM' ELSE '-' END AS operador
FROM permissao pm
LEFT JOIN perfil_permissao pp ON pp.id_permissao = pm.id_permissao
LEFT JOIN perfil pf ON pf.id_perfil = pp.id_perfil
GROUP BY pm.id_permissao, pm.codigo, pm.descricao;


-- ---------------------------------------------------------------------
-- 5. DML - Insercao de dados iniciais (seed)
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

-- ----- Seed de acesso (v3) -----
INSERT INTO perfil (nome, descricao) VALUES
('ADMIN', 'Administrador: acesso total, inclusive usuários e logs'),
('OPERADOR', 'Operador de balcão: cadastra clientes e registra vendas'),
('GERENCIA', 'Gerência: produtos, financeiro, cancelamento de venda e relatórios');

INSERT INTO permissao (codigo, descricao) VALUES
('CLIENTE_VER',       'Consultar clientes'),
('CLIENTE_GRAVAR',    'Cadastrar e editar clientes'),
('CLIENTE_EXCLUIR',   'Excluir clientes'),
('PRODUTO_VER',       'Consultar produtos e estoque'),
('PRODUTO_GRAVAR',    'Cadastrar e editar produtos'),
('PRODUTO_EXCLUIR',   'Excluir produtos'),
('FORNECEDOR_VER',    'Consultar fornecedores'),
('FORNECEDOR_GRAVAR', 'Cadastrar e editar fornecedores'),
('VENDA_VER',         'Consultar vendas'),
('VENDA_REGISTRAR',   'Registrar e concluir vendas'),
('VENDA_CANCELAR',    'Cancelar vendas (devolve o estoque)'),
('FINANCEIRO_VER',    'Consultar contas a pagar e a receber'),
('FINANCEIRO_GRAVAR', 'Lançar contas e marcar como pagas'),
('RELATORIO_VER',     'Gerar relatórios'),
('USUARIO_GERENCIAR', 'Cadastrar, editar e inativar usuários'),
('LOG_VER',           'Consultar o log de ações');

-- Matriz proposta (ajustar conforme as regras definidas com o responsavel da empresa).
INSERT INTO perfil_permissao (id_perfil, id_permissao)
SELECT p.id_perfil, m.id_permissao
FROM perfil p
CROSS JOIN permissao m
WHERE p.nome = 'ADMIN';

INSERT INTO perfil_permissao (id_perfil, id_permissao)
SELECT p.id_perfil, m.id_permissao
FROM perfil p
JOIN permissao m ON m.codigo IN ('CLIENTE_VER','CLIENTE_GRAVAR','PRODUTO_VER',
                                 'VENDA_VER','VENDA_REGISTRAR')
WHERE p.nome = 'OPERADOR';

INSERT INTO perfil_permissao (id_perfil, id_permissao)
SELECT p.id_perfil, m.id_permissao
FROM perfil p
JOIN permissao m ON m.codigo IN ('CLIENTE_VER','PRODUTO_VER','PRODUTO_GRAVAR',
                                 'FORNECEDOR_VER','FORNECEDOR_GRAVAR',
                                 'VENDA_VER','VENDA_CANCELAR',
                                 'FINANCEIRO_VER','FINANCEIRO_GRAVAR',
                                 'RELATORIO_VER','LOG_VER')
WHERE p.nome = 'GERENCIA';

SET @perfil_admin    = (SELECT id_perfil FROM perfil WHERE nome = 'ADMIN');
SET @perfil_operador = (SELECT id_perfil FROM perfil WHERE nome = 'OPERADOR');
SET @perfil_gerencia = (SELECT id_perfil FROM perfil WHERE nome = 'GERENCIA');

CALL sp_usuario_inserir('Administrador',      'admin',    'admin123',    @perfil_admin,    @id_usuario_seed);
CALL sp_usuario_inserir('Operador de Balcão', 'operador', 'operador123', @perfil_operador, @id_usuario_seed);
CALL sp_usuario_inserir('Gerente',            'gerencia', 'gerencia123', @perfil_gerencia, @id_usuario_seed);


-- ---------------------------------------------------------------------
-- 6. Conferencia apos o seed
-- ---------------------------------------------------------------------

SELECT id_produto, nome, estoque
FROM produto
ORDER BY id_produto;   -- esperado: 45 / 39 / 57 / 29

CALL sp_usuario_listar();   -- esperado: 3 usuarios ativos (admin, operador, gerencia)


-- ---------------------------------------------------------------------
-- 7. TESTES DE INTEGRACAO (evidencias)
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


-- ===== TESTE 5 (caminho feliz): login, permissoes por perfil e log =====
START TRANSACTION;

CALL sp_usuario_autenticar('admin', 'admin123');       -- 1 linha: Administrador / ADMIN
CALL sp_usuario_autenticar('operador', 'operador123'); -- 1 linha: Operador de Balcão / OPERADOR

SET @id_admin_teste = (SELECT id_usuario FROM usuario WHERE login = 'admin');
CALL sp_permissoes_do_usuario(@id_admin_teste);        -- 16 permissoes

CALL sp_log_registrar(@id_admin_teste, 'CONSULTA', 'relatorio', NULL, 'Teste de log');
CALL sp_log_listar(NULL, NULL, NULL);   -- 2 LOGIN + 1 CONSULTA

SELECT * FROM vw_matriz_permissoes ORDER BY permissao;   -- matriz perfis x permissoes

CALL sp_perfil_listar();   -- ADMIN, OPERADOR, GERENCIA

CALL sp_usuario_alterar_senha(@id_admin_teste, 'nova1234');
CALL sp_usuario_autenticar('admin', 'nova1234');       -- 1 linha: a senha nova funciona

ROLLBACK;


-- ===== TESTE 6 (casos de erro): validacoes de acesso e usuarios =====
DROP PROCEDURE IF EXISTS sp_teste_acesso;
DROP TEMPORARY TABLE IF EXISTS teste_acesso;

-- ENGINE=MEMORY: as linhas do teste nao somem com o ROLLBACK.
CREATE TEMPORARY TABLE teste_acesso (
    caso     VARCHAR(80),
    mensagem VARCHAR(255)
) ENGINE=MEMORY;

DELIMITER //
CREATE PROCEDURE sp_teste_acesso()
BEGIN
    DECLARE v_msg VARCHAR(255) DEFAULT NULL;
    DECLARE v_id INT;
    DECLARE v_id_admin INT;
    DECLARE v_perfil_operador INT;

    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 v_msg = MESSAGE_TEXT;
    END;

    START TRANSACTION;

    SELECT id_perfil INTO v_perfil_operador FROM perfil WHERE nome = 'OPERADOR';
    SELECT id_usuario INTO v_id_admin FROM usuario WHERE login = 'admin';

    SET v_msg = NULL;
    CALL sp_usuario_autenticar('admin', 'senhaerrada');
    INSERT INTO teste_acesso VALUES ('Login com senha errada',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_usuario_autenticar('naoexiste', 'admin123');
    INSERT INTO teste_acesso VALUES ('Login inexistente',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_usuario_autenticar('', '');
    INSERT INTO teste_acesso VALUES ('Login e senha vazios',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_usuario_inserir('Usuário Inativo', 'inativo', 'inativo123', v_perfil_operador, v_id);
    CALL sp_usuario_definir_ativo(v_id, 0);
    CALL sp_usuario_autenticar('inativo', 'inativo123');
    INSERT INTO teste_acesso VALUES ('Login de usuário inativo',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_usuario_inserir('Outro Admin', 'ADMIN', 'abc12345', v_perfil_operador, v_id);
    INSERT INTO teste_acesso VALUES ('Login já em uso',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_usuario_inserir('Fulano', 'fulano', '123', v_perfil_operador, v_id);
    INSERT INTO teste_acesso VALUES ('Senha com menos de 6 caracteres',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_usuario_inserir('', 'fulano', 'fulano123', v_perfil_operador, v_id);
    INSERT INTO teste_acesso VALUES ('Usuário sem nome',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_usuario_inserir('Fulano', 'fulano', 'fulano123', 999, v_id);
    INSERT INTO teste_acesso VALUES ('Perfil inexistente',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_usuario_alterar_senha(v_id_admin, '123');
    INSERT INTO teste_acesso VALUES ('Trocar senha por uma com menos de 6 caracteres',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_usuario_definir_ativo(v_id_admin, 0);
    INSERT INTO teste_acesso VALUES ('Inativar o último administrador',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_usuario_atualizar(v_id_admin, 'Administrador', v_perfil_operador);
    INSERT INTO teste_acesso VALUES ('Rebaixar o último administrador',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    ROLLBACK;
END //
DELIMITER ;

CALL sp_teste_acesso();

SELECT caso, mensagem FROM teste_acesso;

DROP PROCEDURE sp_teste_acesso;
DROP TEMPORARY TABLE teste_acesso;


-- ===== TESTE 7 (caminho feliz): estorno e ajuste de estoque ao alterar/excluir item =====
START TRANSACTION;

SELECT 'ANTES' AS momento, id_produto, nome, estoque
FROM produto WHERE id_produto = 1;                        -- 45

INSERT INTO venda (data_hora, id_cliente) VALUES (NOW(), 1);
SET @id_venda_t7 = LAST_INSERT_ID();
INSERT INTO item_venda (quantidade, id_venda, id_produto) VALUES (3, @id_venda_t7, 1);
SET @id_item_t7 = LAST_INSERT_ID();

SELECT 'APOS VENDER 3' AS momento, id_produto, nome, estoque
FROM produto WHERE id_produto = 1;                        -- 42

UPDATE item_venda SET quantidade = 5 WHERE id_item = @id_item_t7;
SELECT 'APOS AUMENTAR PARA 5' AS momento, id_produto, nome, estoque
FROM produto WHERE id_produto = 1;                        -- 40

UPDATE item_venda SET quantidade = 2 WHERE id_item = @id_item_t7;
SELECT 'APOS REDUZIR PARA 2' AS momento, id_produto, nome, estoque
FROM produto WHERE id_produto = 1;                        -- 43

DELETE FROM item_venda WHERE id_item = @id_item_t7;
SELECT 'APOS EXCLUIR O ITEM' AS momento, id_produto, nome, estoque
FROM produto WHERE id_produto = 1;                        -- 45

ROLLBACK;


-- ===== TESTE 8 (caminho feliz): fluxo de venda completo pelas procedures =====
START TRANSACTION;

CALL sp_venda_inserir(1, @id_venda_t8);
CALL sp_item_venda_inserir(@id_venda_t8, 1, 2, @id_item1_t8);   -- 2 Hot-dog simples
CALL sp_item_venda_inserir(@id_venda_t8, 3, 1, @id_item2_t8);   -- 1 Refrigerante

SELECT id_produto, nome, estoque
FROM produto WHERE id_produto IN (1, 3)
ORDER BY id_produto;                                      -- 43 e 56

-- Item incluido por engano e removido antes de concluir: o estoque volta.
CALL sp_item_venda_inserir(@id_venda_t8, 4, 1, @id_item3_t8);   -- 1 Suco natural
SELECT id_produto, nome, estoque
FROM produto WHERE id_produto = 4;                        -- 28
CALL sp_item_venda_excluir(@id_item3_t8);
SELECT id_produto, nome, estoque
FROM produto WHERE id_produto = 4;                        -- 29

CALL sp_venda_concluir(@id_venda_t8, 'PAGO', @total_t8);
SELECT @total_t8 AS total_da_venda;                       -- 21.00 (2 x 8,00 + 1 x 5,00)

SELECT tipo, descricao, valor, status_pagamento
FROM financeiro WHERE id_venda = @id_venda_t8;            -- RECEBER 21.00 PAGO

CALL sp_venda_itens(@id_venda_t8);                        -- 2 itens com subtotal

CALL sp_venda_cancelar(@id_venda_t8);

SELECT id_produto, nome, estoque
FROM produto WHERE id_produto IN (1, 3)
ORDER BY id_produto;                                      -- volta para 45 e 57

SELECT COUNT(*) AS vendas_com_esse_id
FROM venda WHERE id_venda = @id_venda_t8;                 -- 0

ROLLBACK;


-- ===== TESTE 9 (casos de erro): venda, estoque e financeiro =====
DROP PROCEDURE IF EXISTS sp_teste_regras_venda;
DROP TEMPORARY TABLE IF EXISTS teste_regras_venda;

CREATE TEMPORARY TABLE teste_regras_venda (
    caso     VARCHAR(80),
    mensagem VARCHAR(255)
) ENGINE=MEMORY;

DELIMITER //
CREATE PROCEDURE sp_teste_regras_venda()
BEGIN
    DECLARE v_msg VARCHAR(255) DEFAULT NULL;
    DECLARE v_id INT;
    DECLARE v_venda INT;
    DECLARE v_venda_vazia INT;
    DECLARE v_item INT;
    DECLARE v_item_seed INT;
    DECLARE v_total DECIMAL(10,2);

    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 v_msg = MESSAGE_TEXT;
    END;

    START TRANSACTION;

    CALL sp_venda_inserir(1, v_venda);
    CALL sp_item_venda_inserir(v_venda, 1, 2, v_item);
    CALL sp_venda_inserir(2, v_venda_vazia);

    SET v_msg = NULL;
    UPDATE item_venda SET quantidade = 999 WHERE id_item = v_item;
    INSERT INTO teste_regras_venda VALUES ('Alterar item acima do estoque (RN13)',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    UPDATE item_venda SET quantidade = 0 WHERE id_item = v_item;
    INSERT INTO teste_regras_venda VALUES ('Alterar item para quantidade zero (RN23)',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    UPDATE item_venda SET id_produto = 2 WHERE id_item = v_item;
    INSERT INTO teste_regras_venda VALUES ('Trocar o produto de um item',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_item_venda_inserir(v_venda, 1, 0, v_id);
    INSERT INTO teste_regras_venda VALUES ('Item com quantidade zero (RN23)',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_item_venda_inserir(v_venda, 1, 999, v_id);
    INSERT INTO teste_regras_venda VALUES ('Item acima do estoque (RN13/RN20)',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_venda_inserir(9999, v_id);
    INSERT INTO teste_regras_venda VALUES ('Venda para cliente inexistente',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_venda_concluir(v_venda_vazia, 'PAGO', v_total);
    INSERT INTO teste_regras_venda VALUES ('Concluir venda sem itens (RN14)',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_venda_concluir(1, 'PAGO', v_total);
    INSERT INTO teste_regras_venda VALUES ('Concluir venda já concluída',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_item_venda_inserir(1, 1, 1, v_id);
    INSERT INTO teste_regras_venda VALUES ('Incluir item em venda concluída',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_item_venda_excluir(99999);
    INSERT INTO teste_regras_venda VALUES ('Excluir item inexistente',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    SELECT MIN(id_item) INTO v_item_seed FROM item_venda WHERE id_venda = 1;
    CALL sp_item_venda_excluir(v_item_seed);
    INSERT INTO teste_regras_venda VALUES ('Excluir item de venda concluída',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_financeiro_inserir('RECEBER', 'Teste', -5.00, CURDATE(), 'PENDENTE', NULL, v_id);
    INSERT INTO teste_regras_venda VALUES ('Lançamento com valor negativo (RN17)',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_financeiro_inserir('XPTO', 'Teste', 5.00, CURDATE(), 'PENDENTE', NULL, v_id);
    INSERT INTO teste_regras_venda VALUES ('Lançamento com tipo inválido',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    SET v_msg = NULL;
    CALL sp_financeiro_marcar_pago(1);
    INSERT INTO teste_regras_venda VALUES ('Marcar como pago lançamento já pago',
        IFNULL(v_msg, '*** FALHA: não bloqueou ***'));

    ROLLBACK;
END //
DELIMITER ;

CALL sp_teste_regras_venda();

SELECT caso, mensagem FROM teste_regras_venda;

DROP PROCEDURE sp_teste_regras_venda;
DROP TEMPORARY TABLE teste_regras_venda;


-- ===== TESTE 10 (caminho feliz): relatorios com os dados do seed =====
CALL sp_relatorio_estoque(40);
-- esperado: Suco natural 29 BAIXO, Hot-dog duplo 39 BAIXO, Hot-dog simples 45 OK, Refrigerante 57 OK

CALL sp_relatorio_vendas(NULL, NULL);
-- esperado: 3 vendas: #3 Pedro Lima 30.00, #2 Maria Souza 22.00, #1 João Silva 21.00

CALL sp_relatorio_vendas_resumo(NULL, NULL);
-- esperado: qtd_vendas 3, total_vendido 73.00, ticket_medio 24.33

CALL sp_relatorio_vendas_resumo('2026-05-11', '2026-05-11');
-- esperado: qtd_vendas 1, total_vendido 30.00, ticket_medio 30.00

CALL sp_relatorio_financeiro(NULL, NULL);
-- esperado: recebido 73.00, a_receber 0.00, pago 50.00, a_pagar 150.00,
--           saldo_realizado 23.00, saldo_previsto -127.00

CALL sp_financeiro_listar('PAGAR', NULL, NULL, NULL);
-- esperado: 2 lancamentos (Compra insumos PENDENTE, Pagamento a fornecedor PAGO)


-- ---------------------------------------------------------------------
-- 8. Conferencia final (apos todos os testes, o banco deve estar como no seed)
-- ---------------------------------------------------------------------
SELECT id_produto, nome, estoque FROM produto ORDER BY id_produto;   -- 45 / 39 / 57 / 29
SELECT (SELECT COUNT(*) FROM cliente)    AS clientes,                -- 3
       (SELECT COUNT(*) FROM venda)      AS vendas,                  -- 3
       (SELECT COUNT(*) FROM item_venda) AS itens_venda,             -- 6
       (SELECT COUNT(*) FROM financeiro) AS lancamentos,             -- 5
       (SELECT COUNT(*) FROM usuario)    AS usuarios,                -- 3
       (SELECT COUNT(*) FROM log_acao)   AS logs;                    -- 0
