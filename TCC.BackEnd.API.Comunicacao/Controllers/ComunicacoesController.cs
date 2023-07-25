using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using TCC.BackEnd.API.Core.Models;

namespace TCC.BackEnd.API.Comunicacao.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComunicacoesController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public ComunicacoesController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        [Authorize]
        [HttpPost("EnviarResumoPedido")]
        public async Task PostEnviarResumoPedido([FromBody] Pedido pedido)
        {
            var accessToken = HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken.Result);

            var urlCadastros = this._configuration.GetValue<string>("tcc-api:cadastros");

            var responsePessoa = await client.GetAsync(urlCadastros + "/api/pessoas/" + pedido.CodigoPessoa);
            var jsonPessoa = await responsePessoa.Content.ReadAsStringAsync();
            var pessoa = JsonConvert.DeserializeObject<Pessoa>(jsonPessoa);

            var responseTokenIntegracao = await client.GetAsync(urlCadastros + "/api/integracoes/SendGrid");
            var jsonTokenIntegracao = await responseTokenIntegracao.Content.ReadAsStringAsync();
            var tokenIntegracao = JsonConvert.DeserializeObject<TokenIntegracao>(jsonTokenIntegracao);

            var urlNegocio = this._configuration.GetValue<string>("tcc-api:negocio");

            var responsePedidoAtualizado = await client.GetAsync(urlNegocio + "/api/pedidos/" + pedido.Codigo);
            var jsonPedidoAtualizado = await responsePedidoAtualizado.Content.ReadAsStringAsync();
            var pedidoAtualizado = JsonConvert.DeserializeObject<Pedido>(jsonPedidoAtualizado);



            var clientSendGrid = new SendGridClient(tokenIntegracao.Token);

            var htmlContent = "<div class=\"accordion-body\"> ";
            htmlContent += " <span class=\"font-30\"><b>Data:</b> " + pedidoAtualizado.DataCriacao.ToShortDateString() + " - <b>Valor total:</b> " + pedidoAtualizado.ValorTotal + "</span>  ";
            htmlContent += " <br/> ";
            htmlContent += " <b>Itens:</b>  ";
            foreach (var item in pedidoAtualizado.Itens)
            {
                htmlContent += " <br /> ";
                htmlContent += " <span> " + item.QuantidadeItem + "x - " + item.NomeItem + "</span>";
            }
            htmlContent += "</div>";


            var msg = new SendGridMessage()
            {
                From = new EmailAddress("1410625@sga.pucminas.br", "EasyMarket"),
                Subject = "Pedido #" + pedidoAtualizado.Codigo + " confirmado!",
                PlainTextContent = htmlContent,
                HtmlContent = htmlContent
            };
            msg.AddTo(new EmailAddress(pessoa.Email, pessoa.Nome));
            var responseEmail = await clientSendGrid.SendEmailAsync(msg);
        }
    }
}
