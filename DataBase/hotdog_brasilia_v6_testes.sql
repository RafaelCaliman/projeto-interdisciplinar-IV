-- Hot-dog Brasília - testes de integração do banco (v6)
-- Parte 1: conferências automáticas (uma grade com OK ou FALHA por linha)
-- Parte 2: consultas para conferir pelos valores esperados
-- Parte 3: blocos com transação (terminam em ROLLBACK)
-- Os testes T1, T15, T20, T21 e T22 estão no arquivo hotdog_brasilia_v6_teste_essencial.sql

USE hotdog_brasilia;

DROP PROCEDURE IF EXISTS sp_teste_completo;
DROP PROCEDURE IF EXISTS sp_teste_estado;
DROP PROCEDURE IF EXISTS sp_teste_registrar;
DROP TEMPORARY TABLE IF EXISTS teste_resultado;

CREATE TEMPORARY TABLE teste_resultado (
    n         INT AUTO_INCREMENT PRIMARY KEY,
    caso      VARCHAR(90),
    esperado  VARCHAR(160),
    obtido    VARCHAR(160),
    resultado VARCHAR(6)
) ENGINE=MEMORY;

DELIMITER //

CREATE PROCEDURE sp_teste_registrar(
    IN p_caso     VARCHAR(90),
    IN p_esperado VARCHAR(160),
    IN p_obtido   VARCHAR(160)
)
BEGIN
    INSERT INTO teste_resultado (caso, esperado, obtido, resultado)
    VALUES (p_caso, p_esperado, p_obtido, IF(p_esperado <=> p_obtido, 'OK', 'FALHA'));
END //

CREATE PROCEDURE sp_teste_estado(
    OUT p_insumos  VARCHAR(40),
    OUT p_produtos VARCHAR(40)
)
BEGIN
    SELECT GROUP_CONCAT(estoque ORDER BY id_insumo SEPARATOR '/') INTO p_insumos
    FROM insumo;

    SELECT GROUP_CONCAT(x.estoque ORDER BY x.id_produto SEPARATOR '/') INTO p_produtos
    FROM (SELECT p.id_produto,
                 CASE WHEN c.disponivel IS NULL THEN p.estoque ELSE c.disponivel END AS estoque
          FROM produto p
          LEFT JOIN (SELECT r.id_produto, MIN(i.estoque DIV r.quantidade) AS disponivel
                     FROM produto_insumo r
                     JOIN insumo i ON i.id_insumo = r.id_insumo
                     GROUP BY r.id_produto) c ON c.id_produto = p.id_produto) x;
END //

CREATE PROCEDURE sp_teste_completo()
BEGIN
    DECLARE v_msg    VARCHAR(255) DEFAULT NULL;
    DECLARE v_txt    VARCHAR(160);
    DECLARE v_ei     VARCHAR(40);
    DECLARE v_ep     VARCHAR(40);
    DECLARE v_balcao INT;
    DECLARE v_admin  INT;
    DECLARE v_op     INT;
    DECLARE v_cli    INT;
    DECLARE v_pr     INT;
    DECLARE v_forn   INT;
    DECLARE v_ins    INT;
    DECLARE v_venda  INT;
    DECLARE v_venda2 INT;
    DECLARE v_i1     INT;
    DECLARE v_i2     INT;
    DECLARE v_i3     INT;
    DECLARE v_semente INT;
    DECLARE v_user   INT;
    DECLARE v_x      INT;
    DECLARE v_total  DECIMAL(10,2);

    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 v_msg = MESSAGE_TEXT;
    END;

    START TRANSACTION;

    SELECT id_cliente INTO v_balcao FROM cliente WHERE nome = 'Cliente balcão';
    SELECT id_usuario INTO v_admin  FROM usuario WHERE login = 'admin';
    SELECT id_perfil  INTO v_op     FROM perfil  WHERE nome = 'OPERADOR';

    CALL sp_cliente_inserir('Ana Teste', '34990001111', v_cli);
    SET v_txt = (SELECT CONCAT(nome, '/', telefone) FROM cliente WHERE id_cliente = v_cli);
    CALL sp_teste_registrar('T3 Cliente: cadastrar', 'Ana Teste/34990001111', v_txt);
    CALL sp_cliente_atualizar(v_cli, 'Ana Teste Editada', '34990002222');
    SET v_txt = (SELECT CONCAT(nome, '/', telefone) FROM cliente WHERE id_cliente = v_cli);
    CALL sp_teste_registrar('T3 Cliente: editar', 'Ana Teste Editada/34990002222', v_txt);
    CALL sp_cliente_excluir(v_cli);
    SET v_txt = CAST((SELECT COUNT(*) FROM cliente WHERE id_cliente = v_cli) AS CHAR);
    CALL sp_teste_registrar('T3 Cliente: excluir', '0', v_txt);

    CALL sp_produto_inserir('Batata frita', 10.00, 20, 1, v_pr);
    SET v_txt = (SELECT CONCAT(nome, '/', valor, '/', estoque) FROM produto WHERE id_produto = v_pr);
    CALL sp_teste_registrar('T3 Produto: cadastrar', 'Batata frita/10.00/20', v_txt);
    CALL sp_produto_atualizar(v_pr, 'Batata frita grande', 14.00, 25, 1);
    SET v_txt = (SELECT CONCAT(nome, '/', valor, '/', estoque) FROM produto WHERE id_produto = v_pr);
    CALL sp_teste_registrar('T3 Produto: editar', 'Batata frita grande/14.00/25', v_txt);
    CALL sp_produto_excluir(v_pr);
    SET v_txt = CAST((SELECT COUNT(*) FROM produto WHERE id_produto = v_pr) AS CHAR);
    CALL sp_teste_registrar('T3 Produto: excluir', '0', v_txt);

    SET v_msg = NULL;
    CALL sp_cliente_inserir('', '34999990000', v_x);
    CALL sp_teste_registrar('T4 Cliente sem nome', 'Nome do cliente é obrigatório.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_cliente_inserir('Fulano', '', v_x);
    CALL sp_teste_registrar('T4 Cliente sem telefone', 'Telefone do cliente é obrigatório.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_cliente_atualizar(9999, 'Fulano', '34999990000');
    CALL sp_teste_registrar('T4 Atualizar cliente inexistente', 'Cliente não encontrado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_produto_inserir('', 5.00, 10, 1, v_x);
    CALL sp_teste_registrar('T4 Produto sem nome', 'Nome do produto é obrigatório.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_produto_inserir('Produto Teste', -1.00, 10, 1, v_x);
    CALL sp_teste_registrar('T4 Produto com valor negativo',
        'Valor unitário inválido: deve ser informado e não pode ser negativo.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_produto_inserir('Produto Teste', 5.00, -3, 1, v_x);
    CALL sp_teste_registrar('T4 Produto com estoque negativo',
        'Estoque inválido: deve ser informado e não pode ser negativo.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_produto_inserir('Produto Teste', 5.00, 10, 999, v_x);
    CALL sp_teste_registrar('T4 Produto com fornecedor inexistente',
        'Fornecedor inválido ou não informado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    CALL sp_fornecedor_inserir('Fornecedor Teste', v_forn);
    SET v_txt = (SELECT nome FROM fornecedor WHERE id_fornecedor = v_forn);
    CALL sp_teste_registrar('T12 Fornecedor: cadastrar', 'Fornecedor Teste', v_txt);
    CALL sp_fornecedor_atualizar(v_forn, 'Fornecedor Teste Editado');
    SET v_txt = (SELECT nome FROM fornecedor WHERE id_fornecedor = v_forn);
    CALL sp_teste_registrar('T12 Fornecedor: editar', 'Fornecedor Teste Editado', v_txt);
    CALL sp_fornecedor_excluir(v_forn);
    SET v_txt = CAST((SELECT COUNT(*) FROM fornecedor WHERE id_fornecedor = v_forn) AS CHAR);
    CALL sp_teste_registrar('T12 Fornecedor: excluir', '0', v_txt);

    SET v_msg = NULL;
    CALL sp_fornecedor_inserir('', v_x);
    CALL sp_teste_registrar('T13 Fornecedor sem nome', 'Nome do fornecedor é obrigatório.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_fornecedor_atualizar(9999, 'Fulano');
    CALL sp_teste_registrar('T13 Atualizar fornecedor inexistente', 'Fornecedor não encontrado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_fornecedor_atualizar(1, '');
    CALL sp_teste_registrar('T13 Atualizar fornecedor sem nome', 'Nome do fornecedor é obrigatório.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_fornecedor_excluir(9999);
    CALL sp_teste_registrar('T13 Excluir fornecedor inexistente', 'Fornecedor não encontrado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_cliente_atualizar(v_balcao, 'Outro nome', '34999990000');
    CALL sp_teste_registrar('T16 Alterar o Cliente balcão',
        'O Cliente balcão é padrão do sistema e não pode ser alterado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_cliente_atualizar(1, 'Cliente balcão', '34991110001');
    CALL sp_teste_registrar('T16 Renomear outro cliente para o nome reservado',
        'Nome reservado: "Cliente balcão" é o cliente padrão do sistema.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_venda_inserir(9999, v_x);
    CALL sp_teste_registrar('T9 Venda para cliente inexistente', 'Cliente não encontrado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_venda_cancelar(99999);
    CALL sp_teste_registrar('T9 Cancelar venda inexistente', 'Venda não encontrada.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_item_venda_inserir(99999, 1, 1, v_x);
    CALL sp_teste_registrar('T9 Item em venda inexistente', 'Venda não encontrada.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_item_venda_excluir(99999);
    CALL sp_teste_registrar('T9 Excluir item inexistente', 'Item de venda não encontrado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SELECT MIN(id_item) INTO v_semente FROM item_venda WHERE id_venda = 1;
    SET v_msg = NULL;
    CALL sp_item_venda_excluir(v_semente);
    CALL sp_teste_registrar('T9 Excluir item de venda concluída',
        'Venda já concluída: não é possível alterar seus itens.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    CALL sp_venda_inserir(1, v_venda);

    SET v_msg = NULL;
    CALL sp_item_venda_inserir(v_venda, 9999, 1, v_x);
    CALL sp_teste_registrar('T9 Item com produto inexistente', 'Produto não encontrado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_item_venda_inserir(v_venda, 1, 999, v_x);
    CALL sp_teste_registrar('T2 Vender 999 hot-dogs simples',
        'Insumo insuficiente para "Hot-dog simples": falta Pão (disponível 45, necessário 999).',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    CALL sp_item_venda_inserir(v_venda, 1, 3, v_i1);
    CALL sp_teste_estado(v_ei, v_ep);
    CALL sp_teste_registrar('T19 Vender 3 simples baixa os insumos', '42/75/42|42/37/57/29', CONCAT(v_ei, '|', v_ep));

    SET v_txt = (SELECT GROUP_CONCAT(estoque ORDER BY id_produto SEPARATOR '/')
                 FROM produto WHERE id_produto IN (1, 2));
    CALL sp_teste_registrar('T19 Estoque guardado do hot-dog não muda', '45/39', v_txt);

    SET v_msg = NULL;
    UPDATE item_venda SET quantidade = 999 WHERE id_item = v_i1;
    CALL sp_teste_registrar('T23 Alterar item acima dos insumos',
        'Insumo insuficiente para "Hot-dog simples": falta Pão (disponível 45, necessário 999).',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    UPDATE item_venda SET quantidade = 0 WHERE id_item = v_i1;
    CALL sp_teste_registrar('T9 Alterar item para quantidade zero',
        'Quantidade inválida: deve ser maior que zero.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    UPDATE item_venda SET id_produto = 2 WHERE id_item = v_i1;
    CALL sp_teste_registrar('T9 Trocar o produto de um item',
        'Não é permitido trocar o produto ou a venda de um item: exclua e inclua outro.',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    UPDATE item_venda SET quantidade = 2 WHERE id_item = v_i1;
    CALL sp_teste_estado(v_ei, v_ep);
    CALL sp_teste_registrar('T7 Reduzir o item para 2 devolve a diferença', '43/76/43|43/38/57/29', CONCAT(v_ei, '|', v_ep));

    CALL sp_item_venda_excluir(v_i1);
    CALL sp_teste_estado(v_ei, v_ep);
    CALL sp_teste_registrar('T23 Excluir o item devolve os insumos', '45/78/45|45/39/57/29', CONCAT(v_ei, '|', v_ep));

    CALL sp_item_venda_inserir(v_venda, 2, 2, v_i2);
    CALL sp_teste_estado(v_ei, v_ep);
    CALL sp_teste_registrar('T23 Vender 2 duplos baixa 1 pão, 2 salsichas e 1 molho cada', '43/74/43|43/37/57/29', CONCAT(v_ei, '|', v_ep));

    UPDATE item_venda SET quantidade = 4 WHERE id_item = v_i2;
    CALL sp_teste_estado(v_ei, v_ep);
    CALL sp_teste_registrar('T23 Aumentar os duplos para 4', '41/70/41|41/35/57/29', CONCAT(v_ei, '|', v_ep));

    CALL sp_item_venda_excluir(v_i2);
    CALL sp_teste_estado(v_ei, v_ep);
    CALL sp_teste_registrar('T23 Excluir os duplos devolve os insumos', '45/78/45|45/39/57/29', CONCAT(v_ei, '|', v_ep));

    UPDATE produto SET valor = 9.00 WHERE id_produto = 1;
    SET v_txt = CAST((SELECT SUM(quantidade * valor_unitario) FROM item_venda WHERE id_venda IN (1, 2, 3)) AS CHAR);
    CALL sp_teste_registrar('T11 Mudar o preço não altera as vendas antigas', '73.00', v_txt);

    CALL sp_item_venda_inserir(v_venda, 1, 1, v_i3);
    UPDATE item_venda SET quantidade = 2 WHERE id_item = v_i3;
    SET v_txt = CAST((SELECT valor_unitario FROM item_venda WHERE id_item = v_i3) AS CHAR);
    CALL sp_teste_registrar('T11 Alterar a quantidade mantém o preço da época', '9.00', v_txt);

    CALL sp_item_venda_excluir(v_i3);
    UPDATE produto SET valor = 8.00 WHERE id_produto = 1;

    CALL sp_item_venda_inserir(v_venda, 1, 1, v_i1);

    SET v_msg = NULL;
    CALL sp_venda_concluir(v_venda, 'XPTO', v_total);
    CALL sp_teste_registrar('T9 Concluir com status inválido',
        'Status de pagamento inválido: use PAGO ou PENDENTE.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    CALL sp_venda_concluir(v_venda, 'PENDENTE', v_total);
    SET v_txt = (SELECT CONCAT(tipo, '/', status_pagamento, '/', valor) FROM financeiro WHERE id_venda = v_venda);
    CALL sp_teste_registrar('T8 Concluir como PENDENTE gera o lançamento', 'RECEBER/PENDENTE/8.00', v_txt);

    CALL sp_venda_cancelar(v_venda);
    CALL sp_teste_estado(v_ei, v_ep);
    CALL sp_teste_registrar('T8 Cancelar venda pendente devolve o estoque', '45/78/45|45/39/57/29', CONCAT(v_ei, '|', v_ep));

    SET v_msg = NULL;
    CALL sp_financeiro_inserir('XPTO', 'Teste', 5.00, CURDATE(), 'PENDENTE', NULL, v_x);
    CALL sp_teste_registrar('T9 Lançamento com tipo inválido', 'Tipo inválido: use RECEBER ou PAGAR.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_financeiro_inserir('RECEBER', 'Teste', 5.00, CURDATE(), 'XPTO', NULL, v_x);
    CALL sp_teste_registrar('T9 Lançamento com status inválido', 'Status inválido: use PAGO ou PENDENTE.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_financeiro_inserir('RECEBER', 'Teste', 5.00, NULL, 'PENDENTE', NULL, v_x);
    CALL sp_teste_registrar('T9 Lançamento sem data', 'Data de lançamento é obrigatória.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_financeiro_inserir('RECEBER', 'Teste', 5.00, CURDATE(), 'PENDENTE', 99999, v_x);
    CALL sp_teste_registrar('T17 Vincular a venda inexistente', 'Venda não encontrada.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    CALL sp_venda_inserir(2, v_venda2);
    SET v_msg = NULL;
    CALL sp_financeiro_inserir('RECEBER', 'Teste', 5.00, CURDATE(), 'PENDENTE', v_venda2, v_x);
    CALL sp_teste_registrar('T17 Vincular lançamento a venda em andamento',
        'Venda ainda não concluída: conclua a venda antes de vincular um lançamento.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    CALL sp_financeiro_inserir('RECEBER', 'Parcela teste', 5.00, CURDATE(), 'PENDENTE', 1, v_x);
    SET v_txt = CAST((SELECT COUNT(*) FROM financeiro WHERE id_venda = 1) AS CHAR);
    CALL sp_teste_registrar('T17 Vincular lançamento a venda concluída (permitido)', '2', v_txt);

    CALL sp_financeiro_marcar_pago(v_x);
    SET v_txt = (SELECT status_pagamento FROM financeiro WHERE id_financeiro = v_x);
    CALL sp_teste_registrar('T9 Marcar como pago um lançamento pendente', 'PAGO', v_txt);

    SET v_msg = NULL;
    CALL sp_financeiro_marcar_pago(99999);
    CALL sp_teste_registrar('T9 Marcar como pago lançamento inexistente', 'Lançamento não encontrado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    CALL sp_insumo_inserir('Queijo', 10, v_ins);
    SET v_txt = CAST((SELECT estoque FROM insumo WHERE id_insumo = v_ins) AS CHAR);
    CALL sp_teste_registrar('T24 Insumo: cadastrar com estoque 10', '10', v_txt);
    CALL sp_insumo_entrada(v_ins, 5);
    SET v_txt = CAST((SELECT estoque FROM insumo WHERE id_insumo = v_ins) AS CHAR);
    CALL sp_teste_registrar('T24 Insumo: entrada de 5', '15', v_txt);
    CALL sp_insumo_ajustar(v_ins, 8);
    SET v_txt = CAST((SELECT estoque FROM insumo WHERE id_insumo = v_ins) AS CHAR);
    CALL sp_teste_registrar('T24 Insumo: ajustar para 8', '8', v_txt);
    CALL sp_insumo_atualizar(v_ins, 'Queijo prato');
    SET v_txt = (SELECT nome FROM insumo WHERE id_insumo = v_ins);
    CALL sp_teste_registrar('T24 Insumo: renomear', 'Queijo prato', v_txt);
    CALL sp_insumo_excluir(v_ins);
    SET v_txt = CAST((SELECT COUNT(*) FROM insumo WHERE id_insumo = v_ins) AS CHAR);
    CALL sp_teste_registrar('T24 Insumo: excluir', '0', v_txt);

    SET v_msg = NULL;
    CALL sp_insumo_inserir('', 5, v_x);
    CALL sp_teste_registrar('T24 Insumo sem nome', 'Nome do insumo é obrigatório.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_insumo_inserir('Queijo', -1, v_x);
    CALL sp_teste_registrar('T24 Insumo com estoque negativo',
        'Estoque inválido: deve ser informado e não pode ser negativo.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_insumo_inserir('pão', 5, v_x);
    CALL sp_teste_registrar('T24 Insumo com nome repetido', 'Insumo já cadastrado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_insumo_atualizar(9999, 'Fulano');
    CALL sp_teste_registrar('T24 Atualizar insumo inexistente', 'Insumo não encontrado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_insumo_atualizar(1, '');
    CALL sp_teste_registrar('T24 Atualizar insumo sem nome', 'Nome do insumo é obrigatório.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_insumo_atualizar(2, 'Pão');
    CALL sp_teste_registrar('T24 Atualizar para o nome de outro insumo', 'Insumo já cadastrado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_insumo_entrada(1, 0);
    CALL sp_teste_registrar('T24 Entrada com quantidade zero',
        'Quantidade inválida: deve ser maior que zero.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_insumo_entrada(9999, 5);
    CALL sp_teste_registrar('T24 Entrada em insumo inexistente', 'Insumo não encontrado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_insumo_ajustar(1, -1);
    CALL sp_teste_registrar('T24 Ajuste com estoque negativo',
        'Estoque inválido: deve ser informado e não pode ser negativo.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_insumo_ajustar(9999, 5);
    CALL sp_teste_registrar('T24 Ajuste em insumo inexistente', 'Insumo não encontrado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_insumo_excluir(9999);
    CALL sp_teste_registrar('T24 Excluir insumo inexistente', 'Insumo não encontrado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    CALL sp_produto_inserir('Combo teste', 15.00, 0, 1, v_pr);
    CALL sp_receita_definir(v_pr, 1, 2);
    CALL sp_receita_definir(v_pr, 3, 1);
    SET v_txt = CAST((SELECT COUNT(*) FROM produto_insumo WHERE id_produto = v_pr) AS CHAR);
    CALL sp_teste_registrar('T25 Receita: 2 insumos', '2', v_txt);

    CALL sp_receita_definir(v_pr, 1, 3);
    SET v_txt = (SELECT CONCAT(COUNT(*), '/', MAX(CASE WHEN id_insumo = 1 THEN quantidade END))
                 FROM produto_insumo WHERE id_produto = v_pr);
    CALL sp_teste_registrar('T25 Trocar a quantidade não duplica a linha', '2/3', v_txt);

    CALL sp_teste_estado(v_ei, v_ep);
    CALL sp_teste_registrar('T25 Estoque do produto novo pela receita (45 DIV 3)', '45/39/57/29/15', v_ep);

    CALL sp_receita_remover(v_pr, 1);
    SET v_txt = CAST((SELECT COUNT(*) FROM produto_insumo WHERE id_produto = v_pr) AS CHAR);
    CALL sp_teste_registrar('T25 Remover um insumo da receita', '1', v_txt);

    SET v_msg = NULL;
    CALL sp_receita_definir(1, 1, -2);
    CALL sp_teste_registrar('T25 Quantidade negativa na receita',
        'Quantidade inválida: deve ser maior que zero.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_receita_definir(9999, 1, 1);
    CALL sp_teste_registrar('T25 Receita de produto inexistente', 'Produto não encontrado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_receita_definir(1, 9999, 1);
    CALL sp_teste_registrar('T25 Receita com insumo inexistente', 'Insumo não encontrado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_receita_remover(3, 1);
    CALL sp_teste_registrar('T25 Remover insumo de produto sem receita',
        'Esse insumo não faz parte da receita do produto.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_receita_listar(9999);
    CALL sp_teste_registrar('T25 Listar receita de produto inexistente', 'Produto não encontrado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    CALL sp_produto_excluir(v_pr);
    SET v_txt = CONCAT((SELECT COUNT(*) FROM produto WHERE id_produto = v_pr), '/',
                       (SELECT COUNT(*) FROM produto_insumo WHERE id_produto = v_pr));
    CALL sp_teste_registrar('T28 Excluir produto novo apaga também a receita', '0/0', v_txt);

    CALL sp_produto_excluir(1);
    SET v_txt = CAST((SELECT COUNT(*) FROM produto_insumo WHERE id_produto = 1) AS CHAR);
    CALL sp_teste_registrar('T28 Produto já vendido recusado mantém a receita', '3', v_txt);

    CALL sp_produto_atualizar(3, 'Refrigerante', 5.00, 100, 2);
    SET v_txt = CAST((SELECT estoque FROM produto WHERE id_produto = 3) AS CHAR);
    CALL sp_teste_registrar('T27 Produto sem receita grava o estoque', '100', v_txt);

    SET v_msg = NULL;
    CALL sp_produto_atualizar(2, 'Hot-dog duplo', 12.00, -1, 1);
    CALL sp_teste_registrar('T27 Hot-dog com estoque negativo continua recusado',
        'Estoque inválido: deve ser informado e não pode ser negativo.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_usuario_autenticar('naoexiste', 'admin123');
    CALL sp_teste_registrar('T6 Login inexistente', 'Login ou senha inválidos.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_usuario_autenticar('', '');
    CALL sp_teste_registrar('T6 Login e senha vazios', 'Login ou senha inválidos.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    CALL sp_usuario_inserir('Usuário Inativo', 'inativo', 'inativo123', v_op, v_user);
    CALL sp_usuario_definir_ativo(v_user, 0);
    SET v_msg = NULL;
    CALL sp_usuario_autenticar('inativo', 'inativo123');
    CALL sp_teste_registrar('T6 Login de usuário inativo', 'Usuário inativo. Procure o administrador.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_txt = CAST((SELECT COUNT(*) FROM log_acao WHERE acao = 'LOGIN_FALHA') AS CHAR);
    CALL sp_teste_registrar('T6 As falhas de login ficam no log', '3', v_txt);

    SET v_msg = NULL;
    CALL sp_usuario_inserir('Outro Admin', 'ADMIN', 'abc12345', v_op, v_x);
    CALL sp_teste_registrar('T6 Login já em uso', 'Login já está em uso.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_usuario_inserir('', 'fulano', 'fulano123', v_op, v_x);
    CALL sp_teste_registrar('T6 Usuário sem nome', 'Nome do usuário é obrigatório.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_usuario_inserir('Fulano', '', 'fulano123', v_op, v_x);
    CALL sp_teste_registrar('T6 Usuário sem login', 'Login é obrigatório.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_usuario_inserir('Fulano', 'fulano', 'fulano123', 999, v_x);
    CALL sp_teste_registrar('T6 Perfil inexistente', 'Perfil inválido ou não informado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_usuario_alterar_senha(v_admin, '123');
    CALL sp_teste_registrar('T6 Trocar senha por uma com menos de 6 caracteres',
        'Senha inválida: mínimo de 6 caracteres.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_usuario_atualizar(v_admin, 'Administrador', v_op);
    CALL sp_teste_registrar('T6 Rebaixar o último administrador',
        'Não é possível rebaixar o último administrador ativo.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_usuario_atualizar(9999, 'Fulano', v_op);
    CALL sp_teste_registrar('T6 Atualizar usuário inexistente', 'Usuário não encontrado.', IFNULL(v_msg, 'NAO BLOQUEOU'));

    CALL sp_usuario_alterar_senha(v_admin, 'nova1234');
    SET v_txt = CAST((SELECT COUNT(*) FROM usuario
                      WHERE login = 'admin' AND senha_hash = SHA2(CONCAT(salt, 'nova1234'), 256)) AS CHAR);
    CALL sp_teste_registrar('T5 A senha nova do administrador confere com o hash', '1', v_txt);

    ROLLBACK;

    CALL sp_teste_estado(v_ei, v_ep);
    CALL sp_teste_registrar('Depois do ROLLBACK volta ao seed', '45/78/45|45/39/57/29', CONCAT(v_ei, '|', v_ep));
END //

DELIMITER ;

CALL sp_teste_completo();

SELECT IF(SUM(resultado = 'FALHA') = 0, 'TUDO OK', 'TEM FALHA') AS resumo,
       SUM(resultado = 'OK') AS ok,
       SUM(resultado = 'FALHA') AS falha
FROM teste_resultado;

SELECT n, resultado, caso, esperado, obtido
FROM teste_resultado
ORDER BY n;

DROP PROCEDURE sp_teste_completo;
DROP PROCEDURE sp_teste_estado;
DROP PROCEDURE sp_teste_registrar;
DROP TEMPORARY TABLE teste_resultado;

-- Parte 2: consultas

CALL sp_usuario_listar();
-- esperado: 3 usuários ativos: Administrador, Gerente, Operador de Balcão

CALL sp_perfil_listar();
-- esperado: ADMIN, OPERADOR, GERENCIA

SELECT * FROM vw_matriz_permissoes ORDER BY permissao;
-- esperado: 16 linhas

CALL sp_cliente_listar(NULL);
-- esperado: Cliente balcão, João Silva, Maria Souza, Pedro Lima

CALL sp_cliente_balcao();
-- esperado: 1 linha (id 4, Cliente balcão, 00000000000)

CALL sp_fornecedor_listar();
-- esperado: Distribuidora São Paulo, Frigorífico Central

CALL sp_fornecedor_pesquisar(NULL);
-- esperado: os dois fornecedores, qtd_produtos 2 e 2

CALL sp_produto_listar(NULL);
-- esperado: Hot-dog duplo 39 (por_receita 1), Hot-dog simples 45 (1), Refrigerante 57 (0), Suco natural 29 (0)

CALL sp_insumo_listar(NULL);
-- esperado: Molho 45, Pão 45, Salsicha 78

CALL sp_receita_listar(2);
-- esperado: Molho 1, Pão 1, Salsicha 2

CALL sp_venda_itens(1);
-- esperado: Hot-dog simples 2 x 8.00 = 16.00; Refrigerante 1 x 5.00 = 5.00

CALL sp_relatorio_vendas(NULL, NULL);
-- esperado: #3 Pedro Lima 30.00, #2 Maria Souza 22.00, #1 João Silva 21.00

CALL sp_relatorio_vendas_resumo('2026-05-11', '2026-05-11');
-- esperado: 1 venda, total 30.00, ticket 30.00

CALL sp_financeiro_listar('PAGAR', NULL, NULL, NULL);
-- esperado: Compra insumos 150.00 PENDENTE e Pagamento a fornecedor 50.00 PAGO

CALL sp_venda_listar('2026-05-11', '2026-05-11');
-- esperado: #3 Pedro Lima 30.00 CONCLUIDA

CALL sp_relatorio_estoque(NULL);
-- esperado: Suco natural 29 OK, Hot-dog duplo 39 OK, Hot-dog simples 45 OK, Refrigerante 57 OK

CALL sp_relatorio_insumos(NULL);
-- esperado: Molho 45 OK, Pão 45 OK, Salsicha 78 OK

-- Parte 3: blocos com transação

START TRANSACTION;
CALL sp_venda_inserir(1, @v);
CALL sp_item_venda_inserir(@v, 1, 1, @i);
CALL sp_venda_listar(NULL, NULL);
-- esperado: a venda do dia (João Silva, 8.00, EM_ANDAMENTO) no topo e as 3 do seed CONCLUIDA
ROLLBACK;

START TRANSACTION;
SET @a = (SELECT id_usuario FROM usuario WHERE login = 'admin');
CALL sp_usuario_autenticar('admin', 'admin123');
-- esperado: 1 linha (Administrador, ADMIN)
CALL sp_log_registrar(@a, 'CONSULTA', 'relatorio', NULL, 'Teste de log');
CALL sp_log_listar(NULL, NULL, NULL);
-- esperado: CONSULTA e LOGIN
CALL sp_permissoes_do_usuario(@a);
-- esperado: 16 permissões
ROLLBACK;

START TRANSACTION;
CALL sp_insumo_ajustar(2, 0);
CALL sp_relatorio_insumos(NULL);
-- esperado: Salsicha 0 ESGOTADO, Molho 45 OK, Pão 45 OK
CALL sp_relatorio_estoque(NULL);
-- esperado: Hot-dog duplo 0 ESGOTADO, Hot-dog simples 0 ESGOTADO, Suco natural 29 OK, Refrigerante 57 OK
ROLLBACK;

SELECT (SELECT COUNT(*) FROM fornecedor)      AS fornecedores,
       (SELECT COUNT(*) FROM cliente)         AS clientes,
       (SELECT COUNT(*) FROM produto)         AS produtos,
       (SELECT COUNT(*) FROM venda)           AS vendas,
       (SELECT COUNT(*) FROM item_venda)      AS itens_venda,
       (SELECT COUNT(*) FROM financeiro)      AS lancamentos,
       (SELECT COUNT(*) FROM usuario)         AS usuarios,
       (SELECT COUNT(*) FROM log_acao)        AS logs,
       (SELECT COUNT(*) FROM insumo)          AS insumos,
       (SELECT COUNT(*) FROM produto_insumo)  AS receitas,
       (SELECT SUM(quantidade * valor_unitario) FROM item_venda) AS total_dos_itens,
       (SELECT GROUP_CONCAT(estoque ORDER BY id_insumo SEPARATOR ' / ') FROM insumo) AS estoque_insumos;
-- esperado: 2, 4, 4, 3, 6, 5, 3, 0, 3, 6, 73.00, 45 / 78 / 45

SHOW GRANTS FOR 'hotdog_app'@'localhost';
-- esperado: USAGE em *.* e EXECUTE em hotdog_brasilia.*
