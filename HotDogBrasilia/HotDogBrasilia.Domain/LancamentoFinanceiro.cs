using System;

namespace HotDogBrasilia.Domain
{
    public class LancamentoFinanceiro
    {
        public int IdFinanceiro { get; set; }
        public string Tipo { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataLancamento { get; set; }
        public string StatusPagamento { get; set; }
        public int? IdVenda { get; set; }
    }
}
