namespace TCC.Front_end.Models
{
    public class PedidoItemViewModel
    {
        public int Codigo { get; set; }
        public int CodigoPedido { get; set; }
        public int CodigoItem { get; set; }
        public int QuantidadeItem { get; set; }
        public decimal ValorUnitarioItem { get; set; }
        public decimal ValorTotalItem { get; set; }
    }
}
