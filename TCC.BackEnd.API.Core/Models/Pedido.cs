using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.BackEnd.API.Core.Models
{
    public class Pedido
    {
        public int Codigo { get; set; }
        public decimal ValorTotal { get; set; }
        public List<PedidoItem> Itens { get; set; }

    }
}
