using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace MillerAPI.DataAccess
{
    public enum DBResult { Fail, Success }

    public class DataAccessor : IDataAccess
    {
        private readonly IConfiguration _config;
        public DataAccessor(IConfiguration config)
        {
            _config = config;
        }
        // ok this is the insert part
        public T DoQuery<T>(Func<DbConnection, T> func, string connectionID = "Default")
        {
            try
            {
                using DbConnection conn = new MySqlConnection(_config.GetConnectionString(connectionID));
                conn.Open();
                return func(conn);
            }
            catch (Exception ex)
            {
                throw; //todo: log
            }
        }

        public DbCommand GetCommand(string cmd_str, DbConnection conn)
        {
            return new MySqlCommand(cmd_str, (MySqlConnection)conn);
        }
    }
}
