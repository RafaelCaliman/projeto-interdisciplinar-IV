USE hotdog_brasilia;

CALL sp_relatorio_estoque(40);
-- esperado: Suco natural 29 BAIXO, Hot-dog duplo 39 BAIXO, Hot-dog simples 45 OK, Refrigerante 57 OK

CALL sp_relatorio_vendas(NULL, NULL);
-- esperado: #3 Pedro Lima 30.00, #2 Maria Souza 22.00, #1 João Silva 21.00

CALL sp_relatorio_vendas_resumo(NULL, NULL);
-- esperado: 3 vendas, total 73.00, ticket médio 24.33

CALL sp_relatorio_vendas_resumo('2026-05-11', '2026-05-11');
-- esperado: 1 venda, total 30.00, ticket médio 30.00

CALL sp_relatorio_financeiro(NULL, NULL);
-- esperado: recebido 73.00, a receber 0.00, pago 50.00, a pagar 150.00, saldo realizado 23.00, saldo previsto -127.00

CALL sp_financeiro_listar('PAGAR', NULL, NULL, NULL);
-- esperado: Compra insumos 150.00 PENDENTE e Pagamento a fornecedor 50.00 PAGO
