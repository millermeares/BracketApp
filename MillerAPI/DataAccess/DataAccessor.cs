using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace MillerAPI.DataAccess
{
    public enum DBResult { Fail, NoMatch, Success }

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

        public T DoTransaction<T>(Func<DbConnection, DbTransaction, T> func, string connectionID="default")
        {
            try
            {
                using DbConnection conn = new MySqlConnection(_config.GetConnectionString(connectionID));
                conn.Open();
                using DbTransaction transaction = conn.BeginTransaction();
                try
                {
                    T result = func(conn, transaction);
                    transaction.Commit();
                    return result;
                }catch (Exception trans_ex)
                {
                    transaction.Rollback();
                    System.Diagnostics.Debug.WriteLine(trans_ex.Message);
                    throw;
                }
            }catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
        }

        
    }
}
