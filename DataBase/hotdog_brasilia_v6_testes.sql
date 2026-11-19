-- Hot-dog Brasília - testes do banco v6 (estoque por insumo) - MySQL 8.0
-- Rodar logo depois de hotdog_brasilia_v6_banco_final.sql, com o seed intacto.
-- Cada teste mostra a coluna "resultado" com OK ou FALHOU.
-- Os testes desfazem o que fizeram, para o seguinte partir do seed.

USE hotdog_brasilia;

DROP PROCEDURE IF EXISTS tst_espera_erro;
DELIMITER //
CREATE PROCEDURE tst_espera_erro(
    IN p_teste    VARCHAR(10),
    IN p_comando  TEXT,
    IN p_esperado VARCHAR(255)
)
BEGIN
    DECLARE v_msg VARCHAR(255) DEFAULT NULL;
    BEGIN
        DECLARE EXIT HANDLER FOR SQLEXCEPTION
            GET DIAGNOSTICS CONDITION 1 v_msg = MESSAGE_TEXT;
        SET @tst_sql = p_comando;
        PREPARE tst_cmd FROM @tst_sql;
        EXECUTE tst_cmd;
        DEALLOCATE PREPARE tst_cmd;
    END;
    SELECT p_teste AS teste, p_esperado AS esperado, v_msg AS obtido,
           IF(v_msg = p_esperado, 'OK', 'FALHOU') AS resultado;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS tst_estoques;
DELIMITER //
CREATE PROCEDURE tst_estoques(
    IN p_teste VARCHAR(10),
    IN p_pao INT, IN p_salsicha INT, IN p_molho INT,
    IN p_simples INT, IN p_duplo INT, IN p_refri INT, IN p_suco INT
)
BEGIN
    DECLARE v_pao INT; DECLARE v_salsicha INT; DECLARE v_molho INT;
    DECLARE v_simples INT; DECLARE v_duplo INT; DECLARE v_refri INT; DECLARE v_suco INT;

    SELECT estoque INTO v_pao      FROM insumo WHERE nome = 'Pão';
    SELECT estoque INTO v_salsicha FROM insumo WHERE nome = 'Salsicha';
    SELECT estoque INTO v_molho    FROM insumo WHERE nome = 'Molho';

    SELECT MIN(i.estoque DIV r.quantidade) INTO v_simples
    FROM produto_insumo r JOIN insumo i ON i.id_insumo = r.id_insumo WHERE r.id_produto = 1;
    SELECT MIN(i.estoque DIV r.quantidade) INTO v_duplo
    FROM produto_insumo r JOIN insumo i ON i.id_insumo = r.id_insumo WHERE r.id_produto = 2;
    SELECT estoque INTO v_refri FROM produto WHERE id_produto = 3;
    SELECT estoque INTO v_suco  FROM produto WHERE id_produto = 4;

    SELECT p_teste AS teste,
           CONCAT('Pão ', p_pao, ', Salsicha ', p_salsicha, ', Molho ', p_molho,
                  ' | simples ', p_simples, ', duplo ', p_duplo,
                  ', refri ', p_refri, ', suco ', p_suco) AS esperado,
           CONCAT('Pão ', v_pao, ', Salsicha ', v_salsicha, ', Molho ', v_molho,
                  ' | simples ', v_simples, ', duplo ', v_duplo,
                  ', refri ', v_refri, ', suco ', v_suco) AS obtido,
           IF(v_pao = p_pao AND v_salsicha = p_salsicha AND v_molho = p_molho
              AND v_simples = p_simples AND v_duplo = p_duplo
              AND v_refri = p_refri AND v_suco = p_suco, 'OK', 'FALHOU') AS resultado;
END //
DELIMITER ;

SET @balcao = (SELECT id_cliente FROM cliente WHERE nome = 'Cliente balcão');

-- T18: sp_produto_listar logo após o seed: 45, 39, 57, 29 e por_receita 1, 1, 0, 0
CALL sp_produto_listar(NULL);
SELECT 'T18' AS teste,
       '1:45:1 2:39:1 3:57:0 4:29:0' AS esperado,
       (SELECT GROUP_CONCAT(CONCAT(x.id_produto, ':', x.estoque, ':', x.por_receita)
                            ORDER BY x.id_produto SEPARATOR ' ')
        FROM (SELECT p.id_produto,
                     CASE WHEN c.disponivel IS NULL THEN p.estoque ELSE c.disponivel END AS estoque,
                     CASE WHEN c.disponivel IS NULL THEN 0 ELSE 1 END AS por_receita
              FROM produto p
              LEFT JOIN (SELECT r.id_produto, MIN(i.estoque DIV r.quantidade) AS disponivel
                         FROM produto_insumo r JOIN insumo i ON i.id_insumo = r.id_insumo
                         GROUP BY r.id_produto) c ON c.id_produto = p.id_produto) x) AS obtido,
       IF((SELECT COUNT(*) FROM insumo) = 3 AND (SELECT COUNT(*) FROM produto_insumo) = 6,
          'OK (3 insumos, 6 receitas; confira a lista do CALL acima)', 'FALHOU') AS resultado;

-- T19: vender 2 simples -> Pão 43, Salsicha 76, Molho 43; simples 43 e duplo 38
CALL sp_venda_inserir(@balcao, @v19);
CALL sp_item_venda_inserir(@v19, 1, 2, @i19);
CALL sp_venda_concluir(@v19, 'PAGO', @t19);
CALL tst_estoques('T19', 43, 76, 43, 43, 38, 57, 29);
CALL sp_venda_cancelar(@v19);

-- T20: vender 1 duplo e cancelar -> insumos voltam a 45, 78 e 45
CALL sp_venda_inserir(@balcao, @v20);
CALL sp_item_venda_inserir(@v20, 2, 1, @i20);
CALL sp_venda_concluir(@v20, 'PAGO', @t20);
CALL tst_estoques('T20a', 44, 76, 44, 44, 38, 57, 29);
CALL sp_venda_cancelar(@v20);
CALL tst_estoques('T20', 45, 78, 45, 45, 39, 57, 29);

-- T21: vender 40 duplos -> insumo insuficiente e nada gravado (como o app: transação com rollback)
SET @vendas_antes = (SELECT COUNT(*) FROM venda);
START TRANSACTION;
CALL sp_venda_inserir(@balcao, @v21);
CALL tst_espera_erro('T21',
    CONCAT('CALL sp_item_venda_inserir(', @v21, ', 2, 40, @i21)'),
    'Insumo insuficiente para "Hot-dog duplo": falta Salsicha (disponível 78, necessário 80).');
ROLLBACK;
SELECT 'T21b' AS teste, 'nenhuma venda nova' AS esperado,
       CONCAT((SELECT COUNT(*) FROM venda) - @vendas_antes, ' venda(s) nova(s)') AS obtido,
       IF((SELECT COUNT(*) FROM venda) = @vendas_antes, 'OK', 'FALHOU') AS resultado;
CALL tst_estoques('T21c', 45, 78, 45, 45, 39, 57, 29);

-- T22: vender 1 refrigerante -> Refrigerante 56; insumos intactos
CALL sp_venda_inserir(@balcao, @v22);
CALL sp_item_venda_inserir(@v22, 3, 1, @i22);
CALL tst_estoques('T22', 45, 78, 45, 45, 39, 56, 29);
CALL sp_venda_cancelar(@v22);

-- T23: alterar a quantidade de um item de hot-dog e excluir o item (venda ainda não concluída)
CALL sp_venda_inserir(@balcao, @v23);
CALL sp_item_venda_inserir(@v23, 2, 1, @i23);
UPDATE item_venda SET quantidade = 3 WHERE id_item = @i23;
CALL tst_estoques('T23a', 42, 72, 42, 42, 36, 57, 29);
UPDATE item_venda SET quantidade = 2 WHERE id_item = @i23;
CALL tst_estoques('T23b', 43, 74, 43, 43, 37, 57, 29);
CALL tst_espera_erro('T23c',
    CONCAT('UPDATE item_venda SET quantidade = 40 WHERE id_item = ', @i23),
    'Insumo insuficiente para "Hot-dog duplo": falta Salsicha (disponível 78, necessário 80).');
CALL sp_item_venda_excluir(@i23);
CALL tst_estoques('T23d', 45, 78, 45, 45, 39, 57, 29);
CALL sp_venda_cancelar(@v23);

-- T24: insumos
CALL sp_insumo_inserir('Cebola', 10, @id_cebola);
SELECT 'T24a' AS teste, 'Cebola 10' AS esperado,
       (SELECT CONCAT(nome, ' ', estoque) FROM insumo WHERE id_insumo = @id_cebola) AS obtido,
       IF((SELECT estoque FROM insumo WHERE id_insumo = @id_cebola) = 10, 'OK', 'FALHOU') AS resultado;
CALL sp_insumo_entrada(@id_cebola, 5);
SELECT 'T24b' AS teste, 'entrada soma: 15' AS esperado,
       (SELECT estoque FROM insumo WHERE id_insumo = @id_cebola) AS obtido,
       IF((SELECT estoque FROM insumo WHERE id_insumo = @id_cebola) = 15, 'OK', 'FALHOU') AS resultado;
CALL sp_insumo_ajustar(@id_cebola, 7);
SELECT 'T24c' AS teste, 'ajuste define: 7' AS esperado,
       (SELECT estoque FROM insumo WHERE id_insumo = @id_cebola) AS obtido,
       IF((SELECT estoque FROM insumo WHERE id_insumo = @id_cebola) = 7, 'OK', 'FALHOU') AS resultado;
CALL tst_espera_erro('T24d', 'CALL sp_insumo_inserir(''Pão'', 1, @x)', 'Insumo já cadastrado.');
CALL tst_espera_erro('T24e', 'CALL sp_insumo_inserir(''Alho'', -1, @x)',
    'Estoque inválido: deve ser informado e não pode ser negativo.');
CALL tst_espera_erro('T24f', 'CALL sp_insumo_inserir(''  '', 1, @x)', 'Nome do insumo é obrigatório.');
CALL tst_espera_erro('T24g', CONCAT('CALL sp_insumo_atualizar(', @id_cebola, ', ''Molho'')'),
    'Insumo já cadastrado.');
CALL tst_espera_erro('T24h', CONCAT('CALL sp_insumo_entrada(', @id_cebola, ', 0)'),
    'Quantidade inválida: deve ser maior que zero.');
CALL tst_espera_erro('T24i', CONCAT('CALL sp_insumo_ajustar(', @id_cebola, ', -3)'),
    'Estoque inválido: deve ser informado e não pode ser negativo.');
CALL tst_espera_erro('T24j', 'CALL sp_insumo_entrada(9999, 1)', 'Insumo não encontrado.');
CALL tst_espera_erro('T24k', 'CALL sp_insumo_excluir(1)',
    'Não é possível excluir: o insumo faz parte da receita de um produto.');
CALL sp_insumo_excluir(@id_cebola);
SELECT 'T24l' AS teste, 'insumo livre excluído' AS esperado,
       (SELECT COUNT(*) FROM insumo WHERE id_insumo = @id_cebola) AS obtido,
       IF((SELECT COUNT(*) FROM insumo WHERE id_insumo = @id_cebola) = 0, 'OK', 'FALHOU') AS resultado;

-- T25: receita
CALL sp_receita_definir(2, 2, 3);
SELECT 'T25a' AS teste, 'duplo com 3 salsichas: 26, sem linha duplicada' AS esperado,
       CONCAT((SELECT MIN(i.estoque DIV r.quantidade) FROM produto_insumo r
               JOIN insumo i ON i.id_insumo = r.id_insumo WHERE r.id_produto = 2),
              ' / linhas: ', (SELECT COUNT(*) FROM produto_insumo WHERE id_produto = 2)) AS obtido,
       IF((SELECT MIN(i.estoque DIV r.quantidade) FROM produto_insumo r
           JOIN insumo i ON i.id_insumo = r.id_insumo WHERE r.id_produto = 2) = 26
          AND (SELECT COUNT(*) FROM produto_insumo WHERE id_produto = 2) = 3, 'OK', 'FALHOU') AS resultado;
CALL sp_receita_definir(2, 2, 2);
CALL tst_espera_erro('T25b', 'CALL sp_receita_definir(2, 2, 0)',
    'Quantidade inválida: deve ser maior que zero.');
CALL tst_espera_erro('T25c', 'CALL sp_receita_definir(9999, 1, 1)', 'Produto não encontrado.');
CALL tst_espera_erro('T25d', 'CALL sp_receita_definir(2, 9999, 1)', 'Insumo não encontrado.');
CALL tst_espera_erro('T25e', 'CALL sp_receita_remover(3, 1)',
    'Esse insumo não faz parte da receita do produto.');
CALL tst_espera_erro('T25f', 'CALL sp_receita_listar(9999)', 'Produto não encontrado.');
CALL sp_receita_listar(2);
CALL tst_estoques('T25g', 45, 78, 45, 45, 39, 57, 29);

-- T26: relatórios (insumos todos OK com limite 10; estoque igual ao T10 do v5)
CALL sp_relatorio_insumos(NULL);
CALL sp_relatorio_estoque(40);
SELECT 'T26' AS teste, 'insumos OK com limite 10' AS esperado,
       (SELECT GROUP_CONCAT(CONCAT(nome, ' ', estoque) ORDER BY nome SEPARATOR ', ') FROM insumo) AS obtido,
       IF((SELECT COUNT(*) FROM insumo WHERE estoque <= 10) = 0, 'OK', 'FALHOU') AS resultado;

-- T27: sp_produto_atualizar em hot-dog com receita, mandando outro estoque: produto.estoque não muda
SET @guardado = (SELECT estoque FROM produto WHERE id_produto = 1);
CALL sp_produto_atualizar(1, 'Hot-dog simples', 8.00, 999, 1);
SELECT 'T27' AS teste, CONCAT('produto.estoque continua ', @guardado, '; exibido 45') AS esperado,
       CONCAT((SELECT estoque FROM produto WHERE id_produto = 1), '; exibido ',
              (SELECT MIN(i.estoque DIV r.quantidade) FROM produto_insumo r
               JOIN insumo i ON i.id_insumo = r.id_insumo WHERE r.id_produto = 1)) AS obtido,
       IF((SELECT estoque FROM produto WHERE id_produto = 1) = @guardado, 'OK', 'FALHOU') AS resultado;
CALL sp_produto_atualizar(3, 'Refrigerante', 5.00, 57, 2);
SELECT 'T27b' AS teste, 'produto sem receita grava o estoque (57)' AS esperado,
       (SELECT estoque FROM produto WHERE id_produto = 3) AS obtido,
       IF((SELECT estoque FROM produto WHERE id_produto = 3) = 57, 'OK', 'FALHOU') AS resultado;

-- T28: excluir produto novo com receita (apaga a receita junto) e produto já vendido (recusado)
CALL sp_produto_inserir('Hot-dog teste', 9.00, 0, 1, @id_teste);
CALL sp_receita_definir(@id_teste, 1, 1);
CALL sp_produto_excluir(@id_teste);
SELECT 'T28a' AS teste, 'produto e receita apagados' AS esperado,
       CONCAT((SELECT COUNT(*) FROM produto WHERE id_produto = @id_teste), ' produto, ',
              (SELECT COUNT(*) FROM produto_insumo WHERE id_produto = @id_teste), ' receita') AS obtido,
       IF((SELECT COUNT(*) FROM produto WHERE id_produto = @id_teste) = 0
          AND (SELECT COUNT(*) FROM produto_insumo WHERE id_produto = @id_teste) = 0, 'OK', 'FALHOU') AS resultado;
CALL tst_espera_erro('T28b', 'CALL sp_produto_excluir(2)',
    'Não é possível excluir: o produto já foi vendido.');
SELECT 'T28c' AS teste, 'receita do vendido mantida (3 linhas)' AS esperado,
       (SELECT COUNT(*) FROM produto_insumo WHERE id_produto = 2) AS obtido,
       IF((SELECT COUNT(*) FROM produto_insumo WHERE id_produto = 2) = 3, 'OK', 'FALHOU') AS resultado;

-- Conferência final: o banco volta ao seed
CALL tst_estoques('FIM', 45, 78, 45, 45, 39, 57, 29);

DROP PROCEDURE tst_espera_erro;
DROP PROCEDURE tst_estoques;
