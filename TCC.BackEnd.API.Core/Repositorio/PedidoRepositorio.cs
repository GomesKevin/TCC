using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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
            DateTime dataCriacao = DateTime.Now; // Obtém a data e hora atual do sistema
            dataCriacao = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dataCriacao, "E. South America Standard Time");

            using (var connection = base.GetConnection())
            {
                connection.Open();

                var query = "INSERT INTO TB_PEDIDO (MO_VALOR_TOTAL, CD_PESSOA, DT_CRIACAO) ";
                query += "VALUES (@MO_VALOR_TOTAL, @CD_PESSOA, @DT_CRIACAO); ";
                query += "SELECT CAST(SCOPE_IDENTITY() AS INT)";

                var parametros = new DynamicParameters();
                parametros.Add("@MO_VALOR_TOTAL", pedido.ValorTotal);
                parametros.Add("@CD_PESSOA", pedido.CodigoPessoa);
                parametros.Add("@DT_CRIACAO", dataCriacao);

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

        public List<Pedido> GetPedidos(int codigoPessoa)
        {
            var pedidos = new List<Pedido>();

            using (var connection = base.GetConnection())
            {
                connection.Open();

                var query = "SELECT CD_PEDIDO, MO_VALOR_TOTAL, CD_PESSOA, DT_CRIACAO FROM TB_PEDIDO WHERE CD_PESSOA = @CD_PESSOA";

                var parametros = new DynamicParameters();
                parametros.Add("@CD_PESSOA", codigoPessoa);

                var retorno = connection.Query(query, parametros);

                pedidos = retorno.Select(p => new Pedido
                {
                    Codigo = p.CD_PEDIDO,
                    ValorTotal = p.MO_VALOR_TOTAL,
                    CodigoPessoa = p.CD_PESSOA,
                    DataCriacao = p.DT_CRIACAO
                }).ToList();

                pedidos.ForEach(p =>
                {
                    var queryItem = "SELECT PI.CD_PEDIDO_ITEM, PI.CD_PEDIDO, PI.CD_ITEM, PI.QTD_ITEM, PI.MO_VALOR_UNIT_ITEM, PI.MO_VALOR_TOTAL_ITEM, P.DC_PRODUTO ";
                    queryItem += " FROM TB_PEDIDO_ITEM PI ";
                    queryItem += "INNER JOIN TB_PRODUTO P ON PI.CD_ITEM  = P.CD_PRODUTO ";
                    queryItem += "WHERE PI.CD_PEDIDO = @CD_PEDIDO";

                    var parametros = new DynamicParameters();
                    parametros.Add("@CD_PEDIDO", p.Codigo);

                    var retorno = connection.Query(queryItem, parametros);


                    p.Itens = retorno.Select(p => new PedidoItem
                    {
                        Codigo = p.CD_PEDIDO_ITEM,
                        CodigoPedido = p.CD_PEDIDO,
                        CodigoItem = p.CD_ITEM,
                        QuantidadeItem = p.QTD_ITEM,
                        ValorUnitarioItem = p.MO_VALOR_UNIT_ITEM,
                        ValorTotalItem = p.MO_VALOR_TOTAL_ITEM,
                        NomeItem = p.DC_PRODUTO
                    }).ToList();
                });

            }

            return pedidos;
        }

        public Pedido GetPedido(int codigoPedido)
        {
            var pedido = new Pedido();

            using (var connection = base.GetConnection())
            {
                connection.Open();

                var query = "SELECT CD_PEDIDO, MO_VALOR_TOTAL, CD_PESSOA, DT_CRIACAO FROM TB_PEDIDO WHERE CD_PEDIDO = @CD_PEDIDO";

                var parametros = new DynamicParameters();
                parametros.Add("@CD_PEDIDO", codigoPedido);

                var retorno = connection.QueryFirstOrDefault(query, parametros);

                if (retorno != null)
                {
                    pedido.Codigo = retorno.CD_PEDIDO;
                    pedido.ValorTotal = retorno.MO_VALOR_TOTAL;
                    pedido.CodigoPessoa = retorno.CD_PESSOA;
                    pedido.DataCriacao = retorno.DT_CRIACAO;


                    var queryItem = "SELECT PI.CD_PEDIDO_ITEM, PI.CD_PEDIDO, PI.CD_ITEM, PI.QTD_ITEM, PI.MO_VALOR_UNIT_ITEM, PI.MO_VALOR_TOTAL_ITEM, P.DC_PRODUTO ";
                    queryItem += " FROM TB_PEDIDO_ITEM PI ";
                    queryItem += "INNER JOIN TB_PRODUTO P ON PI.CD_ITEM  = P.CD_PRODUTO ";
                    queryItem += "WHERE PI.CD_PEDIDO = @CD_PEDIDO";

                    var parametrosItens = new DynamicParameters();
                    parametrosItens.Add("@CD_PEDIDO", pedido.Codigo);

                    var retornoItem = connection.Query(queryItem, parametros);

                    pedido.Itens = retornoItem.Select(p => new PedidoItem
                    {
                        Codigo = p.CD_PEDIDO_ITEM,
                        CodigoPedido = p.CD_PEDIDO,
                        CodigoItem = p.CD_ITEM,
                        QuantidadeItem = p.QTD_ITEM,
                        ValorUnitarioItem = p.MO_VALOR_UNIT_ITEM,
                        ValorTotalItem = p.MO_VALOR_TOTAL_ITEM,
                        NomeItem = p.DC_PRODUTO
                    }).ToList();
                }
            }

            return pedido;
        }

    }
}
