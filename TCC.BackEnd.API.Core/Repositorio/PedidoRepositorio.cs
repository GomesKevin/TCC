using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.BackEnd.API.Core.Models;

namespace TCC.BackEnd.API.Core.Repositorio
{
    public class PedidoRepositorio : Repositorio
    {
        public int SalvarPedido(Pedido pedido)
        {
            int codigoPedido;

            using (var connection = base.GetConnection())
            {
                connection.Open();

                var query = "INSERT INTO TB_PEDIDO (MO_VALOR_TOTAL, CD_PESSOA) ";
                query += "VALUES (@MO_VALOR_TOTAL, @CD_PESSOA); ";
                query += "SELECT CAST(SCOPE_IDENTITY() AS INT)";

                var parametros = new DynamicParameters();
                parametros.Add("@MO_VALOR_TOTAL", pedido.ValorTotal);
                parametros.Add("@CD_PESSOA", pedido.CodigoPessoa);

                codigoPedido = connection.ExecuteScalar<int>(query, parametros);

                pedido.Itens.ForEach(i =>
                {
                    var queryItem = "INSERT INTO TB_PEDIDO_ITEM (CD_PEDIDO, CD_ITEM, QTD_ITEM, MO_VALOR_UNIT_ITEM, MO_VALOR_TOTAL_ITEM) ";
                    queryItem += "VALUES (@CD_PEDIDO, @CD_ITEM, @QTD_ITEM, @MO_VALOR_UNIT_ITEM, @MO_VALOR_TOTAL_ITEM)";

                    var parametros = new DynamicParameters();
                    parametros.Add("@CD_PEDIDO", codigoPedido);
                    parametros.Add("@CD_ITEM", i.CodigoItem);
                    parametros.Add("@QTD_ITEM", i.QuantidadeItem);
                    parametros.Add("@MO_VALOR_UNIT_ITEM", i.ValorUnitarioItem);
                    parametros.Add("@MO_VALOR_TOTAL_ITEM", i.ValorTotalItem);

                    connection.Execute(queryItem, parametros, commandType: CommandType.Text);

                });

            }

            return codigoPedido;
        }
    }
}
