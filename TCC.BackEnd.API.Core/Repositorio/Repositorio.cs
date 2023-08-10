using Microsoft.Data.SqlClient;
using System.Data;

namespace TCC.BackEnd.API.Core.Repositorio
{
    public class Repositorio
    {
        const string connectionString = "Server=tcp:tcc.database.windows.net,1433;Initial Catalog=easyMarket;Persist Security Info=False;User ID=1410625@sga.pucminas.br;Password=Gomes2022;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Authentication=\"Active Directory Password\";";

        public IDbConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
