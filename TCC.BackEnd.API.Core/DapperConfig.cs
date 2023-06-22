using Dapper;
using Dapper.FluentMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.BackEnd.API.Core
{
    public class DapperConfig
    {
        public static void InitializeDapper()
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }
    }
}
