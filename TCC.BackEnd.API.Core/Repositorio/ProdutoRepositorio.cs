using Dapper;
using System;
using System.Collections.Generic;
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

                var query = "SELECT CD_PRODUTO, DC_PRODUTO FROM TB_PRODUTO";

                var teste = connection.Query<Produto>(query).ToList();
                var teste2 = connection.Query<Produto>(query);
                var teste3 = connection.Query(query);

            }

            return list;
        }
    }
}
