using Microsoft.AspNetCore.Mvc;
using TCC.BackEnd.API.Core.Models;
using TCC.BackEnd.API.Core.Repositorio;

namespace TCC.BackEnd.API.Cadastros.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : Controller
    {
        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            var repo = new ProdutoRepositorio();

            return repo.GetProdutos();
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
