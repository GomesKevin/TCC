using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TCC.BackEnd.API.Core.Models;
using TCC.BackEnd.API.Core.Repositorio;

namespace TCC.BackEnd.API.Negocio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public PedidosController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        [Authorize]
        [HttpPost]
        public int Post([FromBody] Pedido pedido)
        {
            var repo = new PedidoRepositorio();

            try
            {
                int codigoPedido = repo.SalvarPedido(pedido);

                pedido.Codigo = codigoPedido;

                var accessToken = HttpContext.GetTokenAsync("access_token");
                
                var client = new HttpClient();

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken.Result);

                var url = this._configuration.GetValue<string>("tcc-api:comunicacao");

                var content = client.PostAsJsonAsync(new Uri(url + "/api/Comunicacoes/EnviarResumoPedido"), pedido).Result;


                return codigoPedido;
            }
            catch (Exception)
            {
                // Em caso de erro, você pode retornar um valor padrão ou lançar uma exceção
                throw new Exception("Ocorreu um erro ao salvar o pedido.");
            }
        }


        [Authorize]
        [HttpGet("pessoa/{codigoPessoa}")]
        public ActionResult<IEnumerable<Pedido>> GetPedidos(int codigoPessoa)
        {
            var repo = new PedidoRepositorio();

            return repo.GetPedidos(codigoPessoa);
        }

        [Authorize]
        [HttpGet("{codigoPedido}")]
        public ActionResult<Pedido> GetPedido(int codigoPedido)
        {
            var repo = new PedidoRepositorio();

            return repo.GetPedido(codigoPedido);
        }
    }
}
