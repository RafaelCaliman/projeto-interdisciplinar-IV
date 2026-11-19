namespace HotDogBrasilia.Domain
{
    public class Produto
    {
        public int IdProduto { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public int Estoque { get; set; }
        public int IdFornecedor { get; set; }
        public string NomeFornecedor { get; set; }
        public bool PorReceita { get; set; }
    }
}
