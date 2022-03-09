using System.Data.Common;

namespace MillerAPI.DataAccess
{
    public interface IDataAccess
    {
        T DoQuery<T>(Func<DbConnection, T> func, string connectionID = "Default");
        T DoTransaction<T>(Func<DbConnection, DbTransaction, T> func, string connectionID = "Default");
        void RecordError(Exception ex, string category = "database", string connectionID = "Default");
    }
}