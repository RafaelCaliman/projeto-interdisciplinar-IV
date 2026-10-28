using System;

namespace HotDogBrasilia.Domain
{
    public class Venda
    {
        public int IdVenda { get; set; }
        public DateTime DataHora { get; set; }
        public string NomeCliente { get; set; }
        public decimal Total { get; set; }
    }
}
