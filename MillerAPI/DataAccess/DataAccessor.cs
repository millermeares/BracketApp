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
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw; //todo: log
            }
        }

        public byte[] GetByteArray(DbDataReader dbr, string key)
        {
            MySqlDataReader reader = (MySqlDataReader)dbr;
            int column_number = GetColumnNumber(reader, key);
            return (byte[])reader.GetValue(column_number);
        }

        private static int GetColumnNumber(MySqlDataReader reader, string key)
        {
            for(int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i) == key) return i;
            }
            throw new Exception("name of column not found");
        }

        public DbCommand GetCommand(string cmd_str, DbConnection conn)
        {
            return new MySqlCommand(cmd_str, (MySqlConnection)conn);
        }

        public string GetString(DbDataReader dbr, string key)
        {
            MySqlDataReader reader = (MySqlDataReader)dbr;
            return reader.GetString(key);
        }
    }
}
