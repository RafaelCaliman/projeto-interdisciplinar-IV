namespace HotDogBrasilia.Domain
{
    // Um produto dentro de uma venda (linha do carrinho ou item já gravado).
    public class ItemVenda
    {
        public int IdItem { get; set; }
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }

        public decimal Subtotal
        {
            get { return Quantidade * ValorUnitario; }
        }
    }
}
