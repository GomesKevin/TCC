using Microsoft.AspNetCore.Authentication;
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
            HttpResponseMessage response = await ApiGetAsync("api/produtos");

            response.EnsureSuccessStatusCode();

            var produtos = await response.Content.ReadFromJsonAsync<List<ProdutoViewModel>>();

            return View(produtos);
        }

        [Authorize]
        public IActionResult Login()
        {
            return View("Index");
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

        private async Task<HttpResponseMessage> ApiGetAsync(string rota)
        {
            //var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var url = this.configuration.GetValue<string>("tcc-api:cadastros");

            var response = await client.GetAsync(url + "/" + rota);

            return response;
        }

        [HttpPost]
        public IActionResult AdicionarItem([FromBody]ProdutoViewModel model)
        {
            // Obter o carrinho de compras da sessão
            var carrinho = HttpContext.Session.GetObject<List<ProdutoViewModel>>("carrinho");
            if (carrinho == null)
            {
                // Se não houver um carrinho na sessão, criar um novo
                carrinho = new List<ProdutoViewModel>();
            }

            if(carrinho.FirstOrDefault(c => c.Codigo == model.Codigo) != null)
                carrinho.Find(c => c.Codigo == model.Codigo).Quantidade += 1;
            else
            // Adicionar o novo item ao carrinho
            carrinho.Add(model);

            // Salvar o carrinho de compras de volta na sessão
            HttpContext.Session.SetObject("carrinho", carrinho);

            // Retornar um resultado JSON indicando o sucesso da operação
            return Json(new { success = true });
        }
    }
}