using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillerAPI.DataAccess
{
    public static class DALHelpers
    {
        public static DbParameter MakeParameter(string key, string value)
        {
            return new MySql.Data.MySqlClient.MySqlParameter(key, value);
        }
    }
}
