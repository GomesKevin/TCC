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
        [Authorize]
        [HttpPost]
        public int Post([FromBody] Pedido pedido)
        {
            var repo = new PedidoRepositorio();

            try
            {
                int codigoPedido = repo.SalvarPedido(pedido);
                return codigoPedido;
            }
            catch (Exception)
            {
                // Em caso de erro, você pode retornar um valor padrão ou lançar uma exceção
                throw new Exception("Ocorreu um erro ao salvar o pedido.");
            }
        }
    }
}
