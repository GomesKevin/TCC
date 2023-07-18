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
            //var from = new EmailAddress("1410625@sga.pucminas.br", "Example User");
            //var subject = "Sending with SendGrid is Fun";
            //var to = new EmailAddress("kgomes975@gmail.com", "Example User");
            //var plainTextContent = "and easy to do anywhere, even with C#";
            //var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            //var response = await client.SendEmailAsync(msg);

            var accessToken = HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken.Result);

            var url = this._configuration.GetValue<string>("tcc-api:cadastros");

            var response = await client.GetAsync(url + "/api/pessoas/" + pedido.CodigoPessoa);

            var json = await response.Content.ReadAsStringAsync();
            var pessoa = JsonConvert.DeserializeObject<Pessoa>(json);

            var clientSendGrid = new SendGridClient("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");
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("1410625@sga.pucminas.br", "EasyMarket"),
                Subject = pessoa.Nome,
                PlainTextContent = "Resumo do pedido Nº: " + pedido.Codigo + ".",
                HtmlContent = "<strong>" + "Resumo do pedido Nº: " + pedido.Codigo + ". </strong>"
            };
            msg.AddTo(new EmailAddress(pessoa.Email, pessoa.Nome));
            var responseEmail = await clientSendGrid.SendEmailAsync(msg);
        }
    }
}
