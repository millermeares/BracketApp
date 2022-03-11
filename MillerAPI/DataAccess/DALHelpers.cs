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
        public static void AddParameter(this DbCommand cmd, string key, object? value)
        {
            cmd.Parameters.Add(MakeParameter(key, value));
        }
        public static DbParameter MakeParameter(string key, string? value)
        {
            return new MySqlParameter(key, value);
        }

        public static DbParameter MakeParameter(string key, byte[] value)
        {
            return new MySqlParameter(key, value);
        }
        public static DbParameter MakeParameter(string key, object? value)
        {
            return new MySqlParameter(key, value);
        }

        internal static void ExceptionParameters(this Exception e, DbCommand cmd, string category)
        {
            cmd.AddParameter("@message", e.Message);
            cmd.AddParameter("@callstack", e.StackTrace);
            cmd.AddParameter("@exceptionSource", e.Source);
            cmd.AddParameter("@category", category);
            cmd.AddParameter("@errorKey", Guid.NewGuid().ToString());
        }
        internal static void ExceptionParameters(this MySqlException e, DbCommand cmd, string category)
        {
            cmd.AddParameter("@message", e.Message + " - " + e.InnerException?.Message);
            cmd.AddParameter("@callstack", e.StackTrace);
            cmd.AddParameter("@exceptionSource", e.Source);
            cmd.AddParameter("@category", category);
            cmd.AddParameter("@errorKey", Guid.NewGuid().ToString());
        }
    }
}
