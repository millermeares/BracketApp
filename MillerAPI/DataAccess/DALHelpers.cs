using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace MillerAPI.DataAccess
{
    public static class DALHelpers
    {
        public static DbParameter MakeParameter(string key, string value)
        {
            return new MySqlParameter(key, value);
        }

        public static DbParameter MakeParameter(string key, byte[] value)
        {
            return new MySqlParameter(key, value);
        }
        public static DbParameter MakeParameter(string key, object value)
        {
            return new MySqlParameter(key, value);
        }
    }
}
