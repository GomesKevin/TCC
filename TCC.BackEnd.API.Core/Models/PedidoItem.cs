using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.BackEnd.API.Core.Models
{
    public class PedidoItem
    {
        public int Codigo { get; set; }
        public int CodigoPedido { get; set; }
        public int CodigoItem { get; set; }
        public int QuantidadeItem { get; set; }
        public decimal ValorUnitarioItem{ get; set; }
        public decimal ValorTotalItem{ get; set; }
    }
}
