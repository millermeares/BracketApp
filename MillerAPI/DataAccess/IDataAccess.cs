using System.Data.Common;

namespace MillerAPI.DataAccess
{
    public interface IDataAccess
    {
        T DoQuery<T>(Func<DbConnection, T> func, string connectionID = "Default");
        T DoTransaction<T>(Func<DbConnection, DbTransaction, T> func, string connectionID = "Default");
        DbCommand GetCommand(string cmd_str, DbConnection conn);
        DbCommand GetCommand(string cmd_str, DbConnection conn, DbTransaction trans);
        string GetString(DbDataReader reader, string key);
        DateTime GetDatetime(DbDataReader reader, string key);

        byte[] GetByteArray(DbDataReader reader, string key);
    }
}