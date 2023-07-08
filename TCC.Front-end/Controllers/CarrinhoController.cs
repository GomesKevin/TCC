using Microsoft.AspNetCore.Mvc;
using TCC.Front_end.Models;

namespace TCC.Front_end.Controllers
{
    public class CarrinhoController : Controller
    {
        public IActionResult Index()
        {
            var carrinho = HttpContext.Session.GetObject<List<CarrinhoViewModel>>("carrinho");

            if(carrinho != null)
            {
                ViewBag.QuantidadeItensCarrinho = carrinho.Sum(c => c.Quantidade);

                carrinho.ForEach(c =>
                {
                    c.ValorTotalProduto = c.ValorProduto * c.Quantidade;
                });

                ViewBag.TotalPedido = carrinho.Sum(c => c.ValorTotalProduto);
            }


            return View(carrinho);
        }
    }
}
