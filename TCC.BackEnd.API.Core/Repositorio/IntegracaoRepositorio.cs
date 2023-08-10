using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.BackEnd.API.Core.Models;

namespace TCC.BackEnd.API.Core.Repositorio
{
    public class IntegracaoRepositorio : Repositorio
    {
        private readonly ICacheService _cacheService;

        public IntegracaoRepositorio(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }
        public TokenIntegracao GetToken(string nomeIntegracao)
        {
            var cacheKey = "token_" + nomeIntegracao; // Uma chave única para cada token de integração

            var tokenIntegracao = _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var tokenIntegracao = new TokenIntegracao();

                using (var connection = base.GetConnection())
                {
                    connection.Open();

                    var query = "SELECT DC_INTEGRACAO, DC_TOKEN FROM TB_TOKEN_INTEGRACAO ";
                    query += "WHERE DC_INTEGRACAO = @DC_INTEGRACAO ";

                    var parametros = new DynamicParameters();
                    parametros.Add("@DC_INTEGRACAO", nomeIntegracao);

                    var retorno = await connection.QueryFirstOrDefaultAsync(query, parametros);

                    if (retorno != null)
                    {
                        tokenIntegracao.NomeIntegracao = retorno.DC_INTEGRACAO;
                        tokenIntegracao.Token = retorno.DC_TOKEN;
                    }
                }

                return tokenIntegracao;
            }, TimeSpan.FromMinutes(5)).Result; // Aguardar o resultado do cache

            return tokenIntegracao;
        }
    }
}
