using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using TCC.Front_end.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace TCC.Front_end.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public CarrinhoController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var carrinho = HttpContext.Session.GetObject<List<CarrinhoViewModel>>("carrinho");

            if (carrinho != null)
            {
                ViewBag.QuantidadeItensCarrinho = carrinho.Sum(c => c.Quantidade);

                carrinho.ForEach(c =>
                {
                    c.ValorTotalProduto = c.ValorProduto * c.Quantidade;
                });

                ViewBag.TotalPedido = carrinho.Sum(c => c.ValorTotalProduto);
            }

            HttpContext.Session.SetObject("carrinho", carrinho);

            return View(carrinho);
        }

        [Authorize]
        public IActionResult FinalizarPedido()
        {
            var carrinho = HttpContext.Session.GetObject<List<CarrinhoViewModel>>("carrinho");

            if (carrinho != null)
            {
                var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var accessToken2 = HttpContext.GetTokenAsync("access_token");

                try
                {
                    var subjectIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub");

                    var pedido = new PedidoViewModel
                    {
                        CodigoPessoa = Convert.ToInt32(subjectIdClaim.Value),
                        ValorTotal = carrinho.Sum(c => c.ValorTotalProduto),
                        Itens = new List<PedidoItemViewModel>()
                    };

                    pedido.Itens.AddRange(
                        carrinho.Select
                        (c => new PedidoItemViewModel
                        {
                            CodigoItem = c.CodigoProduto,
                            QuantidadeItem = c.Quantidade,
                            ValorUnitarioItem = c.ValorProduto,
                            ValorTotalItem = c.ValorTotalProduto
                        })
                        );

                    var client = new HttpClient();

                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken2.Result);

                    var url = this._configuration.GetValue<string>("tcc-api:negocio");

                    var content = client.PostAsJsonAsync(new Uri(url + "/api/Pedidos"), pedido).Result;

                    var codigoPedido = content.Content.ReadAsStringAsync().Result;
                }
                catch (Exception ex)
                {
                    var str = ex.GetBaseException().Message;
                }
            }

            return Json(new { success = true });
        }
    }
}
