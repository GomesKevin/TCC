﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using TCC.Front_end.Models;

namespace TCC.Front_end.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            this.configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            HttpResponseMessage response = await ApiGetAsync("api/produtos", "cadastros");

            response.EnsureSuccessStatusCode();

            var produtos = await response.Content.ReadFromJsonAsync<List<ProdutoViewModel>>();

            var carrinho = HttpContext.Session.GetObject<List<CarrinhoViewModel>>("carrinho");

            if (carrinho != null)
            {
                ViewBag.QuantidadeItensCarrinho = carrinho.Sum(c => c.Quantidade);

            }

            return View(produtos);
        }

        [Authorize]
        public IActionResult Login()
        {
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<RedirectToActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Sobre()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<HttpResponseMessage> ApiGetAsync(string rota, string api)
        {
            var accessToken = HttpContext.GetTokenAsync("access_token").Result;

            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);


            var url = this.configuration.GetValue<string>("tcc-api:" + api);

            var response = await client.GetAsync(url + "/" + rota);

            return response;
        }

        [HttpPost]
        public IActionResult AdicionarItem([FromBody] CarrinhoViewModel model)
        {
            // Obter o carrinho de compras da sessão
            var carrinho = HttpContext.Session.GetObject<List<CarrinhoViewModel>>("carrinho");
            if (carrinho == null)
            {
                // Se não houver um carrinho na sessão, criar um novo
                carrinho = new List<CarrinhoViewModel>();
            }

            if (carrinho.FirstOrDefault(c => c.CodigoProduto == model.CodigoProduto) != null)
                carrinho.Find(c => c.CodigoProduto == model.CodigoProduto).Quantidade += 1;
            else
                // Adicionar o novo item ao carrinho
                carrinho.Add(model);

            // Salvar o carrinho de compras de volta na sessão
            HttpContext.Session.SetObject("carrinho", carrinho);
            ViewBag.QuantidadeItensCarrinho = carrinho.Sum(c => c.Quantidade);

            // Retornar um resultado JSON indicando o sucesso da operação
            return Json(new { success = true });
        }

        [Authorize]
        public async Task<IActionResult> Pedidos()
       
        {
            var codigoUsuario = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;

            HttpResponseMessage response = await ApiGetAsync("api/pedidos/pessoa/" + codigoUsuario, "negocio");

            response.EnsureSuccessStatusCode();

            var pedidos = await response.Content.ReadFromJsonAsync<List<PedidoViewModel>>();

            pedidos = pedidos.OrderByDescending(p => p.Codigo).ToList();

            return View(pedidos);
        }

        [HttpPost]
        public IActionResult RemoverItem([FromBody] CarrinhoViewModel model)
        {
            // Obter o carrinho de compras da sessão
            var carrinho = HttpContext.Session.GetObject<List<CarrinhoViewModel>>("carrinho");
            if (carrinho == null)
            {
                // Se não houver um carrinho na sessão, criar um novo
                carrinho = new List<CarrinhoViewModel>();
            }

            if (carrinho.FirstOrDefault(c => c.CodigoProduto == model.CodigoProduto) != null)
                carrinho.RemoveAll(item => item.CodigoProduto == model.CodigoProduto);

            // Salvar o carrinho de compras de volta na sessão
            HttpContext.Session.SetObject("carrinho", carrinho);
            ViewBag.QuantidadeItensCarrinho = carrinho.Sum(c => c.Quantidade);

            // Retornar um resultado JSON indicando o sucesso da operação
            return Json(new { success = true });
        }
    }
}