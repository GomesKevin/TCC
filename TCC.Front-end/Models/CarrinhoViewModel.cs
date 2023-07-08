namespace TCC.Front_end.Models
{
    public class CarrinhoViewModel
    {
        public int CodigoProduto { get; set; }
        public string DescricaoProduto { get; set; }
        public string CaminhoImagemProduto { get; set; }
        public decimal ValorProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorTotalProduto { get; set; }

    }
}
