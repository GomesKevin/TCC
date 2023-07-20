using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TCC.BackEnd.API.Core;
using TCC.BackEnd.API.Core.Models;
using TCC.BackEnd.API.Core.Repositorio;

namespace TCC.BackEnd.API.Cadastros.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntegracoesController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cacheService;

        public IntegracoesController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ICacheService cacheService)
        {
            _httpContextAccessor = httpContextAccessor;
            this._configuration = configuration;
            this._cacheService = cacheService;
        }

        [Authorize]
        [HttpGet("{nomeIntegracao}")]
        public ActionResult<TokenIntegracao> Get(string nomeIntegracao)
        {
            var repo = new IntegracaoRepositorio(this._cacheService);

            return repo.GetToken(nomeIntegracao);
        }
    }
}
