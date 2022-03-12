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
            catch (MySqlException ex)
            {
                RecordError(ex);
                throw;
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
                    RecordError(trans_ex);
                    transaction.Rollback();
                    throw;
                }
            }catch(MySqlException ex)
            {
                RecordError(ex);
                throw;
            }
        }

        public void RecordError(Exception e, string category = "database", string connectionID = "default", bool record_if_fail = true)
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(_config.GetConnectionString(connectionID));
                using DbCommand cmd = new MySqlCommand(ErrorDBString.LogException, conn);
                conn.Open();
                e.ExceptionParameters(cmd, category);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if(record_if_fail)
                {
                    RecordError(ex, category, connectionID, false);
                }
            }
        }
        public void RecordError(MySqlException e, string category = "database", string connectionID = "default", bool record_if_fail = true)
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(_config.GetConnectionString(connectionID));
                using DbCommand cmd = new MySqlCommand(ErrorDBString.LogException, conn);
                conn.Open();
                e.ExceptionParameters(cmd, category);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                if (record_if_fail)
                {
                    RecordError(ex, category, connectionID, false);
                }
            }

        }


    }
}
