using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TCC.BackEnd.API.Core.Models;

namespace TCC.BackEnd.API.Negocio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public void Post([FromBody] Pedido pedido)
        {

        }
    }
}
