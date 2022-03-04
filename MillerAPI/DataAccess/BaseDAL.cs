using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using MySql.Data.MySqlClient;
namespace MillerAPI.DataAccess
{
    public abstract class BaseDAL
    {
        protected readonly IDataAccess _dataAccess;
        public BaseDAL(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public byte[] GetByteArray(DbDataReader dbr, string key)
        {
            MySqlDataReader reader = (MySqlDataReader)dbr;
            int column_number = GetColumnNumber(reader, key);
            return (byte[])reader.GetValue(column_number);
        }

        private static int GetColumnNumber(MySqlDataReader reader, string key)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i) == key) return i;
            }
            throw new Exception("name of column not found");
        }

        public DbCommand GetCommand(string cmd_str, DbConnection conn)
        {
            return new MySqlCommand(cmd_str, (MySqlConnection)conn);
        }

        public DbCommand GetCommand(string cmd_string, DbConnection conn, DbTransaction trans)
        {
            return new MySqlCommand(cmd_string, (MySqlConnection)conn, (MySqlTransaction)trans);
        }


        public string GetString(DbDataReader dbr, string key)
        {
            MySqlDataReader reader = (MySqlDataReader)dbr;
            return reader.GetString(key);
        }

        public DateTime GetDatetime(DbDataReader dbr, string key)
        {
            MySqlDataReader reader = (MySqlDataReader)dbr;
            return reader.GetDateTime(key);
        }
    }
}
