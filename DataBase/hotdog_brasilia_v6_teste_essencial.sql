USE hotdog_brasilia;

DROP PROCEDURE IF EXISTS sp_teste_geral;
DROP PROCEDURE IF EXISTS sp_teste_estado;
DROP PROCEDURE IF EXISTS sp_teste_registrar;
DROP TEMPORARY TABLE IF EXISTS teste_resultado;

CREATE TEMPORARY TABLE teste_resultado (
    n         INT AUTO_INCREMENT PRIMARY KEY,
    caso      VARCHAR(80),
    esperado  VARCHAR(160),
    obtido    VARCHAR(160),
    resultado VARCHAR(6)
) ENGINE=MEMORY;

DELIMITER //

CREATE PROCEDURE sp_teste_registrar(
    IN p_caso     VARCHAR(80),
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

CREATE PROCEDURE sp_teste_geral()
BEGIN
    DECLARE v_msg    VARCHAR(255) DEFAULT NULL;
    DECLARE v_txt    VARCHAR(160);
    DECLARE v_ins    VARCHAR(40);
    DECLARE v_prod   VARCHAR(40);
    DECLARE v_balcao INT;
    DECLARE v_admin  INT;
    DECLARE v_perfil INT;
    DECLARE v_venda  INT;
    DECLARE v_venda2 INT;
    DECLARE v_i1     INT;
    DECLARE v_i2     INT;
    DECLARE v_i3     INT;
    DECLARE v_x      INT;
    DECLARE v_total  DECIMAL(10,2);

    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 v_msg = MESSAGE_TEXT;
    END;

    START TRANSACTION;

    SELECT id_cliente INTO v_balcao FROM cliente WHERE nome = 'Cliente balcão';
    SELECT id_usuario INTO v_admin  FROM usuario WHERE login = 'admin';
    SELECT id_perfil  INTO v_perfil FROM perfil  WHERE nome = 'OPERADOR';

    SET v_txt = CONCAT_WS('/',
        (SELECT COUNT(*) FROM information_schema.tables
         WHERE table_schema = 'hotdog_brasilia' AND table_type = 'BASE TABLE'),
        (SELECT COUNT(*) FROM information_schema.routines
         WHERE routine_schema = 'hotdog_brasilia' AND routine_type = 'PROCEDURE'
           AND routine_name NOT LIKE 'sp_teste%'),
        (SELECT COUNT(*) FROM information_schema.triggers
         WHERE trigger_schema = 'hotdog_brasilia'));
    CALL sp_teste_registrar('Estrutura: tabelas / procedures / gatilhos', '13/48/5', v_txt);

    SET v_txt = CONCAT_WS('/',
        (SELECT COUNT(*) FROM fornecedor),
        (SELECT COUNT(*) FROM cliente),
        (SELECT COUNT(*) FROM venda),
        (SELECT COUNT(*) FROM item_venda),
        (SELECT COUNT(*) FROM financeiro),
        (SELECT COUNT(*) FROM usuario),
        (SELECT COUNT(*) FROM insumo),
        (SELECT COUNT(*) FROM produto_insumo),
        (SELECT SUM(quantidade * valor_unitario) FROM item_venda));
    CALL sp_teste_registrar('Seed: forn / cli / vendas / itens / lanç / usuários / insumos / receitas / total',
                            '2/4/3/6/5/3/3/6/73.00', v_txt);

    CALL sp_teste_estado(v_ins, v_prod);
    CALL sp_teste_registrar('Estoque inicial: insumos', '45/78/45', v_ins);
    CALL sp_teste_registrar('Estoque inicial: produtos mostrados', '45/39/57/29', v_prod);

    CALL sp_venda_inserir(v_balcao, v_venda);
    CALL sp_item_venda_inserir(v_venda, 1, 2, v_i1);
    CALL sp_teste_estado(v_ins, v_prod);
    CALL sp_teste_registrar('Vender 2 simples baixa os insumos', '43/76/43|43/38/57/29', CONCAT(v_ins, '|', v_prod));

    CALL sp_item_venda_inserir(v_venda, 3, 1, v_i2);
    CALL sp_teste_estado(v_ins, v_prod);
    CALL sp_teste_registrar('Refrigerante baixa o próprio estoque', '43/76/43|43/38/56/29', CONCAT(v_ins, '|', v_prod));

    UPDATE item_venda SET quantidade = 5 WHERE id_item = v_i1;
    CALL sp_teste_estado(v_ins, v_prod);
    CALL sp_teste_registrar('Alterar o item para 5 ajusta pela diferença', '40/73/40|40/36/56/29', CONCAT(v_ins, '|', v_prod));

    CALL sp_item_venda_excluir(v_i1);
    CALL sp_teste_estado(v_ins, v_prod);
    CALL sp_teste_registrar('Excluir o item devolve os insumos', '45/78/45|45/39/56/29', CONCAT(v_ins, '|', v_prod));

    SET v_msg = NULL;
    CALL sp_item_venda_inserir(v_venda, 2, 40, v_x);
    CALL sp_teste_registrar('Vender 40 duplos: falta insumo',
        'Insumo insuficiente para "Hot-dog duplo": falta Salsicha (disponível 78, necessário 80).',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_item_venda_inserir(v_venda, 3, 999, v_x);
    CALL sp_teste_registrar('Vender 999 refrigerantes',
        'Estoque insuficiente para "Refrigerante": disponível 56, solicitado 999.',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_item_venda_inserir(v_venda, 1, 0, v_x);
    CALL sp_teste_registrar('Item com quantidade zero',
        'Quantidade inválida: deve ser maior que zero.',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    CALL sp_teste_estado(v_ins, v_prod);
    CALL sp_teste_registrar('As recusas não gravam nada', '45/78/45|45/39/56/29', CONCAT(v_ins, '|', v_prod));

    UPDATE produto SET valor = 9.00 WHERE id_produto = 1;
    CALL sp_item_venda_inserir(v_venda, 1, 1, v_i3);
    SET v_txt = CONCAT(
        (SELECT valor_unitario FROM item_venda WHERE id_venda = 1 AND id_produto = 1), '/',
        (SELECT valor_unitario FROM item_venda WHERE id_item = v_i3));
    CALL sp_teste_registrar('Preço da época: item antigo / item novo', '8.00/9.00', v_txt);
    CALL sp_item_venda_excluir(v_i3);

    CALL sp_venda_concluir(v_venda, 'PAGO', v_total);
    CALL sp_teste_registrar('Concluir venda: total', '5.00', CAST(v_total AS CHAR));

    SET v_txt = (SELECT CONCAT(tipo, '/', status_pagamento, '/', valor)
                 FROM financeiro WHERE id_venda = v_venda);
    CALL sp_teste_registrar('Concluir venda: lançamento gerado', 'RECEBER/PAGO/5.00', v_txt);

    SET v_msg = NULL;
    CALL sp_item_venda_inserir(v_venda, 1, 1, v_x);
    CALL sp_teste_registrar('Incluir item em venda concluída',
        'Venda já concluída: não é possível alterar seus itens.',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_venda_concluir(v_venda, 'PAGO', v_total);
    CALL sp_teste_registrar('Concluir venda já concluída',
        'Venda já concluída.',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    CALL sp_venda_inserir(v_balcao, v_venda2);
    SET v_msg = NULL;
    CALL sp_venda_concluir(v_venda2, 'PAGO', v_total);
    CALL sp_teste_registrar('Concluir venda sem itens',
        'Venda sem itens: inclua ao menos um produto.',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    CALL sp_item_venda_inserir(v_venda2, 2, 1, v_x);
    CALL sp_venda_cancelar(v_venda2);
    CALL sp_venda_cancelar(v_venda);
    CALL sp_teste_estado(v_ins, v_prod);
    CALL sp_teste_registrar('Cancelar vendas devolve o estoque', '45/78/45|45/39/57/29', CONCAT(v_ins, '|', v_prod));

    SET v_txt = CONCAT(
        (SELECT COUNT(*) FROM financeiro WHERE id_venda IN (v_venda, v_venda2)), '/',
        (SELECT COUNT(*) FROM venda WHERE id_venda IN (v_venda, v_venda2)));
    CALL sp_teste_registrar('Cancelar remove lançamento e venda', '0/0', v_txt);

    SET v_msg = NULL;
    CALL sp_cliente_excluir(v_balcao);
    CALL sp_teste_registrar('Excluir o Cliente balcão',
        'O Cliente balcão é padrão do sistema e não pode ser excluído.',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_cliente_inserir('Cliente balcão', '34999990000', v_x);
    CALL sp_teste_registrar('Cadastrar cliente com o nome reservado',
        'Nome reservado: "Cliente balcão" é o cliente padrão do sistema.',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_cliente_excluir(1);
    CALL sp_teste_registrar('Excluir cliente com vendas',
        'Não é possível excluir: o cliente possui vendas registradas.',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_produto_excluir(1);
    CALL sp_teste_registrar('Excluir produto já vendido',
        'Não é possível excluir: o produto já foi vendido.',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_fornecedor_excluir(1);
    CALL sp_teste_registrar('Excluir fornecedor com produtos',
        'Não é possível excluir: o fornecedor possui produtos cadastrados.',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_insumo_excluir(1);
    CALL sp_teste_registrar('Excluir insumo que está em receita',
        'Não é possível excluir: o insumo faz parte da receita de um produto.',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_receita_definir(1, 1, 0);
    CALL sp_teste_registrar('Receita com quantidade zero',
        'Quantidade inválida: deve ser maior que zero.',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    CALL sp_produto_atualizar(2, 'Hot-dog duplo', 12.00, 999, 1);
    SET v_txt = CAST((SELECT estoque FROM produto WHERE id_produto = 2) AS CHAR);
    CALL sp_teste_registrar('Produto com receita não grava estoque', '39', v_txt);

    SET v_msg = NULL;
    CALL sp_financeiro_inserir('RECEBER', 'Teste', -5.00, CURDATE(), 'PENDENTE', NULL, v_x);
    CALL sp_teste_registrar('Lançamento com valor negativo',
        'Valor inválido: deve ser informado e não pode ser negativo.',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_financeiro_marcar_pago(1);
    CALL sp_teste_registrar('Marcar como pago lançamento já pago',
        'Lançamento já está pago.',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_usuario_autenticar('admin', 'senhaerrada');
    CALL sp_teste_registrar('Login com senha errada',
        'Login ou senha inválidos.',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_txt = CAST((SELECT COUNT(*) FROM usuario
                      WHERE login = 'admin'
                        AND senha_hash = SHA2(CONCAT(salt, 'admin123'), 256)) AS CHAR);
    CALL sp_teste_registrar('Senha do admin confere com o hash', '1', v_txt);

    SET v_msg = NULL;
    CALL sp_usuario_definir_ativo(v_admin, 0);
    CALL sp_teste_registrar('Inativar o último administrador',
        'Não é possível inativar o último administrador ativo.',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_msg = NULL;
    CALL sp_usuario_inserir('Fulano', 'fulano', '123', v_perfil, v_x);
    CALL sp_teste_registrar('Usuário com senha curta',
        'Senha inválida: mínimo de 6 caracteres.',
        IFNULL(v_msg, 'NAO BLOQUEOU'));

    SET v_txt = (SELECT CONCAT(SUM(admin = 'SIM'), '/', SUM(operador = 'SIM'), '/', SUM(gerencia = 'SIM'))
                 FROM vw_matriz_permissoes);
    CALL sp_teste_registrar('Permissões: ADMIN / OPERADOR / GERENCIA', '16/5/11', v_txt);

    ROLLBACK;

    CALL sp_teste_estado(v_ins, v_prod);
    CALL sp_teste_registrar('Depois do ROLLBACK volta ao seed', '45/78/45|45/39/57/29', CONCAT(v_ins, '|', v_prod));
END //

DELIMITER ;

CALL sp_teste_geral();

SELECT IF(SUM(resultado = 'FALHA') = 0, 'TUDO OK', 'TEM FALHA') AS resumo,
       SUM(resultado = 'OK') AS ok,
       SUM(resultado = 'FALHA') AS falha
FROM teste_resultado;

SELECT n, resultado, caso, esperado, obtido
FROM teste_resultado
ORDER BY n;

DROP PROCEDURE sp_teste_geral;
DROP PROCEDURE sp_teste_estado;
DROP PROCEDURE sp_teste_registrar;
DROP TEMPORARY TABLE teste_resultado;

START TRANSACTION;
SELECT id_cliente INTO @c FROM cliente WHERE nome = 'Cliente balcão';
CALL sp_venda_inserir(@c, @v);
CALL sp_item_venda_inserir(@v, 1, 1, @i);
CALL sp_relatorio_vendas_resumo(NULL, NULL);
CALL sp_venda_concluir(@v, 'PAGO', @t);
CALL sp_relatorio_vendas_resumo(NULL, NULL);
ROLLBACK;

CALL sp_relatorio_estoque(40);
CALL sp_relatorio_vendas_resumo(NULL, NULL);
CALL sp_relatorio_financeiro(NULL, NULL);
CALL sp_relatorio_insumos(50);
SHOW GRANTS FOR 'hotdog_app'@'localhost';
