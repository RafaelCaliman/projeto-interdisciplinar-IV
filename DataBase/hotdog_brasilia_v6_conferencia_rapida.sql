-- Hot-dog Brasília - conferência rápida do banco v6 - MySQL 8.0
-- Rodar logo depois de hotdog_brasilia_v6_banco_final.sql.
-- Esperado: tabelas=13, procedures=48, gatilhos=5, permissoes=16,
--           clientes=4, produtos=4, vendas=3, financeiro=5, insumos=3, receitas=6

USE hotdog_brasilia;

SELECT
    (SELECT COUNT(*) FROM information_schema.TABLES
     WHERE TABLE_SCHEMA = 'hotdog_brasilia' AND TABLE_TYPE = 'BASE TABLE') AS tabelas,
    (SELECT COUNT(*) FROM information_schema.ROUTINES
     WHERE ROUTINE_SCHEMA = 'hotdog_brasilia' AND ROUTINE_TYPE = 'PROCEDURE') AS procedures,
    (SELECT COUNT(*) FROM information_schema.TRIGGERS
     WHERE TRIGGER_SCHEMA = 'hotdog_brasilia') AS gatilhos,
    (SELECT COUNT(*) FROM permissao) AS permissoes,
    (SELECT COUNT(*) FROM cliente) AS clientes,
    (SELECT COUNT(*) FROM produto) AS produtos,
    (SELECT COUNT(*) FROM venda) AS vendas,
    (SELECT COUNT(*) FROM financeiro) AS financeiro,
    (SELECT COUNT(*) FROM insumo) AS insumos,
    (SELECT COUNT(*) FROM produto_insumo) AS receitas;

-- Esperado: 45 (simples), 39 (duplo), 57 e 29, com por_receita 1, 1, 0 e 0
CALL sp_produto_listar(NULL);
