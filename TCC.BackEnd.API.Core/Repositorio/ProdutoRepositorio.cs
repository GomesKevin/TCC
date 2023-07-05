using Dapper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.BackEnd.API.Core.Models;

namespace TCC.BackEnd.API.Core.Repositorio
{
    public class ProdutoRepositorio : Repositorio
    {
        public List<Produto> GetProdutos()
        {
            var list = new List<Produto>();

            using (var connection = base.GetConnection())
            {
                connection.Open();

                var query = "SELECT CD_PRODUTO, DC_PRODUTO, DC_CAMINHO_IMAGEM, MO_VALOR FROM TB_PRODUTO";

                var retorno = connection.Query(query);

                list = retorno.Select(p => new Produto
                {
                   Codigo = p.CD_PRODUTO,
                   Descricao = p.DC_PRODUTO,
                   CaminhoImagem = p.DC_CAMINHO_IMAGEM,
                   Valor = p.MO_VALOR
                }).ToList();

            }

            return list;
        }
    }
}
