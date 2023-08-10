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
    public class PessoaRepositorio : Repositorio
    {
        public Pessoa GetPessoa(int codigoPessoa)
        {
            var pessoa = new Pessoa();

            using (var connection = base.GetConnection())
            {
                connection.Open();

                var query = "SELECT CD_PESSOA, NOME, DC_EMAIL FROM TB_PESSOA ";
                query += "WHERE CD_PESSOA = @CD_PESSOA ";

                var parametros = new DynamicParameters();
                parametros.Add("@CD_PESSOA", codigoPessoa);

                var retorno = connection.QueryFirstOrDefault(query, parametros);


                if (retorno != null)
                {
                    pessoa.Codigo = retorno.CD_PESSOA;
                    pessoa.Nome = retorno.NOME;
                    pessoa.Email = retorno.DC_EMAIL;
                }
            }

            return pessoa;
        }
    }
}
