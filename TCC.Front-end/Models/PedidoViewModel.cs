namespace TCC.Front_end.Models
{
    public class PedidoViewModel
    {
        public int Codigo { get; set; }
        public decimal ValorTotal { get; set; }
        public List<PedidoItemViewModel> Itens { get; set; }
        public int CodigoPessoa { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
