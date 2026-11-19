-- Hot-dog Brasília - banco de dados final (v6, estoque por insumo, 19/11/2026) - MySQL 8.0
-- Projeto Interdisciplinar IV - UEMG, Unidade Frutal
-- Alunos: Eduardo Pedro Batista, Guilherme da Silva Guerra, Luiz Guilherme Alves Almeida, Rafael Rosa Caliman

-- Tabelas
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

CREATE TABLE insumo (
    id_insumo INT PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(100) NOT NULL UNIQUE,
    estoque INT NOT NULL DEFAULT 0,
    CHECK (estoque >= 0)
);

-- receita: quanto de cada insumo um produto usa (produto sem receita usa produto.estoque)
CREATE TABLE produto_insumo (
    id_produto INT NOT NULL,
    id_insumo INT NOT NULL,
    quantidade INT NOT NULL,
    PRIMARY KEY (id_produto, id_insumo),
    CHECK (quantidade > 0),
    FOREIGN KEY (id_produto) REFERENCES produto(id_produto),
    FOREIGN KEY (id_insumo) REFERENCES insumo(id_insumo)
);

CREATE TABLE venda (
    id_venda INT PRIMARY KEY AUTO_INCREMENT,
    data_hora DATETIME NOT NULL,
    id_cliente INT NOT NULL,
    FOREIGN KEY (id_cliente) REFERENCES cliente(id_cliente)
);

CREATE INDEX idx_venda_data_hora ON venda(data_hora);

-- valor_unitario é preenchido por trigger com o preço do produto na hora da venda (RN16)
CREATE TABLE item_venda (
    id_item INT PRIMARY KEY AUTO_INCREMENT,
    quantidade INT NOT NULL,
    id_venda INT NOT NULL,
    id_produto INT NOT NULL,
    valor_unitario DECIMAL(10,2) NOT NULL DEFAULT 0,
    CHECK (quantidade > 0),
    CHECK (valor_unitario >= 0),
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

CREATE TABLE perfil (
    id_perfil INT PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(30) NOT NULL UNIQUE,
    descricao VARCHAR(150)
);

CREATE TABLE permissao (
    id_permissao INT PRIMARY KEY AUTO_INCREMENT,
    codigo VARCHAR(40) NOT NULL UNIQUE,
    descricao VARCHAR(150) NOT NULL
);

CREATE TABLE perfil_permissao (
    id_perfil INT NOT NULL,
    id_permissao INT NOT NULL,
    PRIMARY KEY (id_perfil, id_permissao),
    FOREIGN KEY (id_perfil) REFERENCES perfil(id_perfil),
    FOREIGN KEY (id_permissao) REFERENCES permissao(id_permissao)
);

-- a senha é guardada como SHA2(salt + senha)
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

-- Triggers
-- RN13, RN20, RN23: bloqueia quantidade inválida e venda acima do estoque; RN16: grava o preço do produto no item
DELIMITER //
CREATE TRIGGER trg_item_venda_valida_estoque
BEFORE INSERT ON item_venda
FOR EACH ROW
BEGIN
    DECLARE v_estoque INT;
    DECLARE v_nome VARCHAR(100);
    DECLARE v_valor DECIMAL(10,2);
    DECLARE v_msg VARCHAR(128);
    DECLARE v_insumo VARCHAR(100) DEFAULT NULL;
    DECLARE v_disp INT;
    DECLARE v_precisa INT;

    IF NEW.quantidade IS NULL OR NEW.quantidade <= 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Quantidade inválida: deve ser maior que zero.';
    END IF;

    SELECT estoque, nome, valor INTO v_estoque, v_nome, v_valor
    FROM produto
    WHERE id_produto = NEW.id_produto
    FOR UPDATE;

    IF EXISTS (SELECT 1 FROM produto_insumo WHERE id_produto = NEW.id_produto) THEN
        SELECT i.nome, i.estoque, r.quantidade * NEW.quantidade
        INTO v_insumo, v_disp, v_precisa
        FROM produto_insumo r
        JOIN insumo i ON i.id_insumo = r.id_insumo
        WHERE r.id_produto = NEW.id_produto
          AND i.estoque < r.quantidade * NEW.quantidade
        ORDER BY i.id_insumo
        LIMIT 1;

        IF v_insumo IS NOT NULL THEN
            SET v_msg = LEFT(CONCAT('Insumo insuficiente para "', v_nome, '": falta ',
                                    v_insumo, ' (disponível ', v_disp,
                                    ', necessário ', v_precisa, ').'), 128);
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = v_msg;
        END IF;
    ELSEIF v_estoque IS NOT NULL AND v_estoque < NEW.quantidade THEN
        SET v_msg = LEFT(CONCAT('Estoque insuficiente para "', v_nome,
                                '": disponível ', v_estoque,
                                ', solicitado ', NEW.quantidade, '.'), 128);
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = v_msg;
    END IF;

    IF v_valor IS NOT NULL THEN
        SET NEW.valor_unitario = v_valor;
    END IF;
END //
DELIMITER ;

-- RN07: baixa o estoque a cada item registrado (insumos da receita ou, sem receita, o próprio produto)
DELIMITER //
CREATE TRIGGER trg_item_venda_baixa_estoque
AFTER INSERT ON item_venda
FOR EACH ROW
BEGIN
    IF EXISTS (SELECT 1 FROM produto_insumo WHERE id_produto = NEW.id_produto) THEN
        UPDATE insumo i
        JOIN produto_insumo r ON r.id_insumo = i.id_insumo
        SET i.estoque = i.estoque - r.quantidade * NEW.quantidade
        WHERE r.id_produto = NEW.id_produto;
    ELSE
        UPDATE produto
        SET estoque = estoque - NEW.quantidade
        WHERE id_produto = NEW.id_produto;
    END IF;
END //
DELIMITER ;

-- alterar a quantidade valida o estoque pela diferença; não permite trocar produto nem venda do item
DELIMITER //
CREATE TRIGGER trg_item_venda_valida_alteracao
BEFORE UPDATE ON item_venda
FOR EACH ROW
BEGIN
    DECLARE v_estoque INT;
    DECLARE v_nome VARCHAR(100);
    DECLARE v_msg VARCHAR(128);
    DECLARE v_insumo VARCHAR(100) DEFAULT NULL;
    DECLARE v_disp INT;
    DECLARE v_precisa INT;

    IF NEW.id_produto <> OLD.id_produto OR NEW.id_venda <> OLD.id_venda THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Não é permitido trocar o produto ou a venda de um item: exclua e inclua outro.';
    END IF;

    SET NEW.valor_unitario = OLD.valor_unitario;

    IF NEW.quantidade IS NULL OR NEW.quantidade <= 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Quantidade inválida: deve ser maior que zero.';
    END IF;

    SELECT estoque, nome INTO v_estoque, v_nome
    FROM produto
    WHERE id_produto = NEW.id_produto
    FOR UPDATE;

    IF EXISTS (SELECT 1 FROM produto_insumo WHERE id_produto = NEW.id_produto) THEN
        SELECT i.nome, i.estoque + r.quantidade * OLD.quantidade, r.quantidade * NEW.quantidade
        INTO v_insumo, v_disp, v_precisa
        FROM produto_insumo r
        JOIN insumo i ON i.id_insumo = r.id_insumo
        WHERE r.id_produto = NEW.id_produto
          AND i.estoque + r.quantidade * OLD.quantidade < r.quantidade * NEW.quantidade
        ORDER BY i.id_insumo
        LIMIT 1;

        IF v_insumo IS NOT NULL THEN
            SET v_msg = LEFT(CONCAT('Insumo insuficiente para "', v_nome, '": falta ',
                                    v_insumo, ' (disponível ', v_disp,
                                    ', necessário ', v_precisa, ').'), 128);
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = v_msg;
        END IF;
    ELSEIF v_estoque IS NOT NULL AND (v_estoque + OLD.quantidade) < NEW.quantidade THEN
        SET v_msg = LEFT(CONCAT('Estoque insuficiente para "', v_nome,
                                '": disponível ', (v_estoque + OLD.quantidade),
                                ', solicitado ', NEW.quantidade, '.'), 128);
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = v_msg;
    END IF;
END //
DELIMITER ;

-- ao alterar a quantidade, ajusta o estoque pela diferença
DELIMITER //
CREATE TRIGGER trg_item_venda_ajusta_estoque
AFTER UPDATE ON item_venda
FOR EACH ROW
BEGIN
    IF EXISTS (SELECT 1 FROM produto_insumo WHERE id_produto = NEW.id_produto) THEN
        UPDATE insumo i
        JOIN produto_insumo r ON r.id_insumo = i.id_insumo
        SET i.estoque = i.estoque + r.quantidade * (OLD.quantidade - NEW.quantidade)
        WHERE r.id_produto = NEW.id_produto;
    ELSE
        UPDATE produto
        SET estoque = estoque + OLD.quantidade - NEW.quantidade
        WHERE id_produto = NEW.id_produto;
    END IF;
END //
DELIMITER ;

-- ao excluir o item, o estoque volta
DELIMITER //
CREATE TRIGGER trg_item_venda_estorna_estoque
AFTER DELETE ON item_venda
FOR EACH ROW
BEGIN
    IF EXISTS (SELECT 1 FROM produto_insumo WHERE id_produto = OLD.id_produto) THEN
        UPDATE insumo i
        JOIN produto_insumo r ON r.id_insumo = i.id_insumo
        SET i.estoque = i.estoque + r.quantidade * OLD.quantidade
        WHERE r.id_produto = OLD.id_produto;
    ELSE
        UPDATE produto
        SET estoque = estoque + OLD.quantidade
        WHERE id_produto = OLD.id_produto;
    END IF;
END //
DELIMITER ;

-- Procedures
-- Cliente (RF01, RF04, RF16 / RN01, RN04, RN10)
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
    IF TRIM(p_nome) = 'Cliente balcão' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Nome reservado: "Cliente balcão" é o cliente padrão do sistema.';
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
    IF EXISTS (SELECT 1 FROM cliente
               WHERE id_cliente = p_id_cliente AND nome = 'Cliente balcão') THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'O Cliente balcão é padrão do sistema e não pode ser alterado.';
    END IF;
    IF p_nome IS NULL OR TRIM(p_nome) = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Nome do cliente é obrigatório.';
    END IF;
    IF p_telefone IS NULL OR TRIM(p_telefone) = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Telefone do cliente é obrigatório.';
    END IF;
    IF TRIM(p_nome) = 'Cliente balcão' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Nome reservado: "Cliente balcão" é o cliente padrão do sistema.';
    END IF;

    UPDATE cliente
    SET nome = TRIM(p_nome),
        telefone = TRIM(p_telefone)
    WHERE id_cliente = p_id_cliente;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE sp_cliente_excluir(
    IN p_id_cliente INT
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM cliente WHERE id_cliente = p_id_cliente) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Cliente não encontrado.';
    END IF;
    IF EXISTS (SELECT 1 FROM cliente
               WHERE id_cliente = p_id_cliente AND nome = 'Cliente balcão') THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'O Cliente balcão é padrão do sistema e não pode ser excluído.';
    END IF;
    IF EXISTS (SELECT 1 FROM venda WHERE id_cliente = p_id_cliente) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Não é possível excluir: o cliente possui vendas registradas.';
    END IF;

    DELETE FROM cliente WHERE id_cliente = p_id_cliente;
END //
DELIMITER ;

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

DELIMITER //
CREATE PROCEDURE sp_cliente_balcao()
BEGIN
    SELECT id_cliente, nome, telefone
    FROM cliente
    WHERE nome = 'Cliente balcão'
    LIMIT 1;
END //
DELIMITER ;

-- Produto (RF02, RF17, RF18, RF24 / RN02, RN11, RN12, RN21, RN22)
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
        estoque = CASE
                      WHEN EXISTS (SELECT 1 FROM produto_insumo r WHERE r.id_produto = p_id_produto)
                      THEN estoque
                      ELSE p_estoque
                  END,
        id_fornecedor = p_id_fornecedor
    WHERE id_produto = p_id_produto;
END //
DELIMITER ;

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

    DELETE FROM produto_insumo WHERE id_produto = p_id_produto;
    DELETE FROM produto WHERE id_produto = p_id_produto;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE sp_produto_listar(
    IN p_busca VARCHAR(100)
)
BEGIN
    SELECT p.id_produto, p.nome, p.valor,
           CASE WHEN c.disponivel IS NULL THEN p.estoque ELSE c.disponivel END AS estoque,
           p.id_fornecedor, f.nome AS fornecedor,
           CASE WHEN c.disponivel IS NULL THEN 0 ELSE 1 END AS por_receita
    FROM produto p
    JOIN fornecedor f ON f.id_fornecedor = p.id_fornecedor
    LEFT JOIN (SELECT r.id_produto, MIN(i.estoque DIV r.quantidade) AS disponivel
               FROM produto_insumo r
               JOIN insumo i ON i.id_insumo = r.id_insumo
               GROUP BY r.id_produto) c ON c.id_produto = p.id_produto
    WHERE p_busca IS NULL OR p.nome LIKE CONCAT('%', p_busca, '%')
    ORDER BY p.nome;
END //
DELIMITER ;

-- Insumo e receita (estoque por insumo, v6)
DELIMITER //
CREATE PROCEDURE sp_insumo_listar(
    IN p_busca VARCHAR(100)
)
BEGIN
    SELECT id_insumo, nome, estoque
    FROM insumo
    WHERE p_busca IS NULL OR p_busca = '' OR nome LIKE CONCAT('%', p_busca, '%')
    ORDER BY nome;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE sp_insumo_inserir(
    IN  p_nome      VARCHAR(100),
    IN  p_estoque   INT,
    OUT p_id_insumo INT
)
BEGIN
    IF p_nome IS NULL OR TRIM(p_nome) = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Nome do insumo é obrigatório.';
    END IF;
    IF p_estoque IS NULL OR p_estoque < 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Estoque inválido: deve ser informado e não pode ser negativo.';
    END IF;
    IF EXISTS (SELECT 1 FROM insumo WHERE nome = TRIM(p_nome)) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Insumo já cadastrado.';
    END IF;

    INSERT INTO insumo (nome, estoque) VALUES (TRIM(p_nome), p_estoque);

    SET p_id_insumo = LAST_INSERT_ID();
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE sp_insumo_atualizar(
    IN p_id_insumo INT,
    IN p_nome      VARCHAR(100)
)
BEGIN
    IF p_id_insumo IS NULL
       OR NOT EXISTS (SELECT 1 FROM insumo WHERE id_insumo = p_id_insumo) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Insumo não encontrado.';
    END IF;
    IF p_nome IS NULL OR TRIM(p_nome) = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Nome do insumo é obrigatório.';
    END IF;
    IF EXISTS (SELECT 1 FROM insumo
               WHERE nome = TRIM(p_nome) AND id_insumo <> p_id_insumo) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Insumo já cadastrado.';
    END IF;

    UPDATE insumo SET nome = TRIM(p_nome) WHERE id_insumo = p_id_insumo;
END //
DELIMITER ;

-- entrada soma ao estoque (compra ou reposição)
DELIMITER //
CREATE PROCEDURE sp_insumo_entrada(
    IN p_id_insumo  INT,
    IN p_quantidade INT
)
BEGIN
    IF p_id_insumo IS NULL
       OR NOT EXISTS (SELECT 1 FROM insumo WHERE id_insumo = p_id_insumo) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Insumo não encontrado.';
    END IF;
    IF p_quantidade IS NULL OR p_quantidade <= 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Quantidade inválida: deve ser maior que zero.';
    END IF;

    UPDATE insumo SET estoque = estoque + p_quantidade WHERE id_insumo = p_id_insumo;
END //
DELIMITER ;

-- ajuste define o estoque pela contagem física
DELIMITER //
CREATE PROCEDURE sp_insumo_ajustar(
    IN p_id_insumo    INT,
    IN p_novo_estoque INT
)
BEGIN
    IF p_id_insumo IS NULL
       OR NOT EXISTS (SELECT 1 FROM insumo WHERE id_insumo = p_id_insumo) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Insumo não encontrado.';
    END IF;
    IF p_novo_estoque IS NULL OR p_novo_estoque < 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Estoque inválido: deve ser informado e não pode ser negativo.';
    END IF;

    UPDATE insumo SET estoque = p_novo_estoque WHERE id_insumo = p_id_insumo;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE sp_insumo_excluir(
    IN p_id_insumo INT
)
BEGIN
    IF p_id_insumo IS NULL
       OR NOT EXISTS (SELECT 1 FROM insumo WHERE id_insumo = p_id_insumo) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Insumo não encontrado.';
    END IF;
    IF EXISTS (SELECT 1 FROM produto_insumo WHERE id_insumo = p_id_insumo) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Não é possível excluir: o insumo faz parte da receita de um produto.';
    END IF;

    DELETE FROM insumo WHERE id_insumo = p_id_insumo;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE sp_receita_listar(
    IN p_id_produto INT
)
BEGIN
    IF p_id_produto IS NULL
       OR NOT EXISTS (SELECT 1 FROM produto WHERE id_produto = p_id_produto) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Produto não encontrado.';
    END IF;

    SELECT r.id_insumo, i.nome AS insumo, r.quantidade, i.estoque
    FROM produto_insumo r
    JOIN insumo i ON i.id_insumo = r.id_insumo
    WHERE r.id_produto = p_id_produto
    ORDER BY i.nome;
END //
DELIMITER ;

-- inclui o insumo na receita ou troca a quantidade
DELIMITER //
CREATE PROCEDURE sp_receita_definir(
    IN p_id_produto INT,
    IN p_id_insumo  INT,
    IN p_quantidade INT
)
BEGIN
    IF p_id_produto IS NULL
       OR NOT EXISTS (SELECT 1 FROM produto WHERE id_produto = p_id_produto) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Produto não encontrado.';
    END IF;
    IF p_id_insumo IS NULL
       OR NOT EXISTS (SELECT 1 FROM insumo WHERE id_insumo = p_id_insumo) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Insumo não encontrado.';
    END IF;
    IF p_quantidade IS NULL OR p_quantidade <= 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Quantidade inválida: deve ser maior que zero.';
    END IF;

    INSERT INTO produto_insumo (id_produto, id_insumo, quantidade)
    VALUES (p_id_produto, p_id_insumo, p_quantidade)
    ON DUPLICATE KEY UPDATE quantidade = p_quantidade;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE sp_receita_remover(
    IN p_id_produto INT,
    IN p_id_insumo  INT
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM produto_insumo
                   WHERE id_produto = p_id_produto AND id_insumo = p_id_insumo) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Esse insumo não faz parte da receita do produto.';
    END IF;

    DELETE FROM produto_insumo
    WHERE id_produto = p_id_produto AND id_insumo = p_id_insumo;
END //
DELIMITER ;

-- Fornecedor (RF03, RF04 / RN03)
DELIMITER //
CREATE PROCEDURE sp_fornecedor_listar()
BEGIN
    SELECT id_fornecedor, nome
    FROM fornecedor
    ORDER BY nome;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE sp_fornecedor_pesquisar(
    IN p_busca VARCHAR(100)
)
BEGIN
    SELECT f.id_fornecedor, f.nome,
           (SELECT COUNT(*) FROM produto p
            WHERE p.id_fornecedor = f.id_fornecedor) AS qtd_produtos
    FROM fornecedor f
    WHERE p_busca IS NULL OR p_busca = '' OR f.nome LIKE CONCAT('%', p_busca, '%')
    ORDER BY f.nome;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE sp_fornecedor_inserir(
    IN  p_nome          VARCHAR(100),
    OUT p_id_fornecedor INT
)
BEGIN
    IF p_nome IS NULL OR TRIM(p_nome) = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Nome do fornecedor é obrigatório.';
    END IF;

    INSERT INTO fornecedor (nome) VALUES (TRIM(p_nome));

    SET p_id_fornecedor = LAST_INSERT_ID();
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE sp_fornecedor_atualizar(
    IN p_id_fornecedor INT,
    IN p_nome          VARCHAR(100)
)
BEGIN
    IF p_id_fornecedor IS NULL
       OR NOT EXISTS (SELECT 1 FROM fornecedor WHERE id_fornecedor = p_id_fornecedor) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Fornecedor não encontrado.';
    END IF;
    IF p_nome IS NULL OR TRIM(p_nome) = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Nome do fornecedor é obrigatório.';
    END IF;

    UPDATE fornecedor
    SET nome = TRIM(p_nome)
    WHERE id_fornecedor = p_id_fornecedor;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE sp_fornecedor_excluir(
    IN p_id_fornecedor INT
)
BEGIN
    IF p_id_fornecedor IS NULL
       OR NOT EXISTS (SELECT 1 FROM fornecedor WHERE id_fornecedor = p_id_fornecedor) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Fornecedor não encontrado.';
    END IF;
    IF EXISTS (SELECT 1 FROM produto WHERE id_fornecedor = p_id_fornecedor) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Não é possível excluir: o fornecedor possui produtos cadastrados.';
    END IF;

    DELETE FROM fornecedor WHERE id_fornecedor = p_id_fornecedor;
END //
DELIMITER ;

-- Venda (RF05, RF06, RF07, RF20, RF23 / RN05, RN06, RN14, RN16, RN23, RN24)
-- Fluxo: sp_venda_inserir, sp_item_venda_inserir (1 ou mais), sp_venda_concluir (ou sp_venda_cancelar).
-- Venda concluída = venda que já tem lançamento no financeiro.
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

    SELECT SUM(quantidade * valor_unitario) INTO v_total
    FROM item_venda
    WHERE id_venda = p_id_venda;

    INSERT INTO financeiro (tipo, descricao, valor, data_lancamento, status_pagamento, id_venda)
    VALUES ('RECEBER', CONCAT('Venda balcão #', p_id_venda), v_total, CURDATE(),
            p_status_pagamento, p_id_venda);

    UPDATE venda SET data_hora = NOW() WHERE id_venda = p_id_venda;

    SET p_total = v_total;
END //
DELIMITER ;

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
           i.valor_unitario AS valor_unitario,
           (i.quantidade * i.valor_unitario) AS subtotal
    FROM item_venda i
    JOIN produto p ON p.id_produto = i.id_produto
    WHERE i.id_venda = p_id_venda
    ORDER BY i.id_item;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE sp_venda_listar(
    IN p_data_ini DATE,
    IN p_data_fim DATE
)
BEGIN
    SELECT v.id_venda, v.data_hora, c.nome AS cliente,
           COALESCE(SUM(i.quantidade * i.valor_unitario), 0) AS total,
           CASE WHEN EXISTS (SELECT 1 FROM financeiro f WHERE f.id_venda = v.id_venda)
                THEN 'CONCLUIDA' ELSE 'EM_ANDAMENTO' END AS situacao
    FROM venda v
    JOIN cliente c ON c.id_cliente = v.id_cliente
    LEFT JOIN item_venda i ON i.id_venda = v.id_venda
    WHERE (p_data_ini IS NULL OR DATE(v.data_hora) >= p_data_ini)
      AND (p_data_fim IS NULL OR DATE(v.data_hora) <= p_data_fim)
    GROUP BY v.id_venda, v.data_hora, c.nome
    ORDER BY v.data_hora DESC, v.id_venda DESC;
END //
DELIMITER ;

-- Financeiro (RF10, RF22, RF25 / RN08, RN17)
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
    IF p_id_venda IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM financeiro WHERE id_venda = p_id_venda) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Venda ainda não concluída: conclua a venda antes de vincular um lançamento.';
    END IF;

    INSERT INTO financeiro (tipo, descricao, valor, data_lancamento, status_pagamento, id_venda)
    VALUES (p_tipo, NULLIF(TRIM(p_descricao), ''), p_valor, p_data_lancamento, p_status, p_id_venda);

    SET p_id_financeiro = LAST_INSERT_ID();
END //
DELIMITER ;

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

-- Relatórios (RF13, RF14, RF15 / RN09, RN18, RN19)
DELIMITER //
CREATE PROCEDURE sp_relatorio_vendas(
    IN p_data_ini DATE,
    IN p_data_fim DATE
)
BEGIN
    SELECT v.id_venda, v.data_hora, c.nome AS cliente,
           COALESCE(SUM(i.quantidade * i.valor_unitario), 0) AS total
    FROM venda v
    JOIN cliente c ON c.id_cliente = v.id_cliente
    LEFT JOIN item_venda i ON i.id_venda = v.id_venda
    WHERE (p_data_ini IS NULL OR DATE(v.data_hora) >= p_data_ini)
      AND (p_data_fim IS NULL OR DATE(v.data_hora) <= p_data_fim)
    GROUP BY v.id_venda, v.data_hora, c.nome
    ORDER BY v.data_hora DESC, v.id_venda DESC;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE sp_relatorio_vendas_resumo(
    IN p_data_ini DATE,
    IN p_data_fim DATE
)
BEGIN
    SELECT COUNT(DISTINCT v.id_venda) AS qtd_vendas,
           COALESCE(SUM(i.quantidade * i.valor_unitario), 0) AS total_vendido,
           ROUND(COALESCE(SUM(i.quantidade * i.valor_unitario) /
                 NULLIF(COUNT(DISTINCT v.id_venda), 0), 0), 2) AS ticket_medio
    FROM venda v
    LEFT JOIN item_venda i ON i.id_venda = v.id_venda
    WHERE (p_data_ini IS NULL OR DATE(v.data_hora) >= p_data_ini)
      AND (p_data_fim IS NULL OR DATE(v.data_hora) <= p_data_fim);
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE sp_relatorio_estoque(
    IN p_limite_baixo INT
)
BEGIN
    SELECT x.id_produto, x.produto, x.estoque, x.fornecedor,
           CASE
               WHEN x.estoque = 0 THEN 'ESGOTADO'
               WHEN x.estoque <= COALESCE(p_limite_baixo, 10) THEN 'BAIXO'
               ELSE 'OK'
           END AS situacao
    FROM (SELECT p.id_produto, p.nome AS produto,
                 CASE WHEN c.disponivel IS NULL THEN p.estoque ELSE c.disponivel END AS estoque,
                 f.nome AS fornecedor
          FROM produto p
          JOIN fornecedor f ON f.id_fornecedor = p.id_fornecedor
          LEFT JOIN (SELECT r.id_produto, MIN(i.estoque DIV r.quantidade) AS disponivel
                     FROM produto_insumo r
                     JOIN insumo i ON i.id_insumo = r.id_insumo
                     GROUP BY r.id_produto) c ON c.id_produto = p.id_produto) x
    ORDER BY x.estoque, x.produto;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE sp_relatorio_insumos(
    IN p_limite_baixo INT
)
BEGIN
    SELECT id_insumo, nome AS insumo, estoque,
           CASE
               WHEN estoque = 0 THEN 'ESGOTADO'
               WHEN estoque <= COALESCE(p_limite_baixo, 10) THEN 'BAIXO'
               ELSE 'OK'
           END AS situacao
    FROM insumo
    ORDER BY estoque, nome;
END //
DELIMITER ;

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

-- Usuário, perfil e log (login e controle de acesso por perfil)
-- O hash é feito no banco: o app só envia login e senha.
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

    SELECT u.id_usuario, u.nome, u.login, u.id_perfil, u.ativo, u.ultimo_login, pf.nome AS perfil
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

-- View
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

-- Dados iniciais
INSERT INTO fornecedor (nome) VALUES
('Distribuidora São Paulo'),
('Frigorífico Central');

INSERT INTO cliente (nome, telefone) VALUES
('João Silva', '34991110001'),
('Maria Souza', '34992220002'),
('Pedro Lima', '34993330003');

INSERT INTO cliente (nome, telefone) VALUES
('Cliente balcão', '00000000000');

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

-- Insumos e receitas (exemplo, a confirmar com o dono): inseridos depois das vendas do seed,
-- para as vendas antigas continuarem baixando produto.estoque (45, 39, 57 e 29)
INSERT INTO insumo (nome, estoque) VALUES
('Pão', 45),
('Salsicha', 78),
('Molho', 45);

-- Hot-dog simples (id 1): 1 pão, 1 salsicha, 1 molho
-- Hot-dog duplo (id 2): 1 pão, 2 salsichas, 1 molho
INSERT INTO produto_insumo (id_produto, id_insumo, quantidade) VALUES
(1, 1, 1), (1, 2, 1), (1, 3, 1),
(2, 1, 1), (2, 2, 2), (2, 3, 1);

INSERT INTO financeiro (tipo, descricao, valor, data_lancamento, status_pagamento, id_venda) VALUES
('RECEBER', 'Venda balcão', 21.00, '2026-05-10', 'PAGO', 1),
('RECEBER', 'Venda balcão', 22.00, '2026-05-10', 'PAGO', 2),
('PAGAR', 'Compra insumos', 150.00, '2026-05-11', 'PENDENTE', NULL),
('PAGAR', 'Pagamento a fornecedor', 50.00, '2026-05-10', 'PAGO', NULL),
('RECEBER', 'Venda balcão', 30.00, '2026-05-11', 'PAGO', 3);

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

-- Usuário da aplicação (RNF07): só executa procedures
CREATE USER IF NOT EXISTS 'hotdog_app'@'localhost'  IDENTIFIED BY 'HotDog@App2026';
CREATE USER IF NOT EXISTS 'hotdog_app'@'127.0.0.1' IDENTIFIED BY 'HotDog@App2026';
GRANT EXECUTE ON hotdog_brasilia.* TO 'hotdog_app'@'localhost';
GRANT EXECUTE ON hotdog_brasilia.* TO 'hotdog_app'@'127.0.0.1';

-- Backup:    mysqldump -u root -p --single-transaction --routines --triggers --databases hotdog_brasilia > backup_hotdog_brasilia.sql
-- Restaurar: mysql -u root -p < backup_hotdog_brasilia.sql
-- (o usuário hotdog_app não vai no backup: rodar de novo só o bloco acima)
--
-- Para trocar as senhas do seed e do hotdog_app, rodar (com as senhas novas, mínimo de 6 caracteres):
-- SET @id_admin    = (SELECT id_usuario FROM usuario WHERE login = 'admin');
-- SET @id_operador = (SELECT id_usuario FROM usuario WHERE login = 'operador');
-- SET @id_gerencia = (SELECT id_usuario FROM usuario WHERE login = 'gerencia');
-- CALL sp_usuario_alterar_senha(@id_admin,    'NOVA_SENHA_ADMIN');
-- CALL sp_usuario_alterar_senha(@id_operador, 'NOVA_SENHA_OPERADOR');
-- CALL sp_usuario_alterar_senha(@id_gerencia, 'NOVA_SENHA_GERENCIA');
-- ALTER USER 'hotdog_app'@'localhost'  IDENTIFIED BY 'NOVA_SENHA_APP';
-- ALTER USER 'hotdog_app'@'127.0.0.1' IDENTIFIED BY 'NOVA_SENHA_APP';
