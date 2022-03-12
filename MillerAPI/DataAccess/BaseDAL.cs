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
        protected static DateTime MaxDate = new DateTime(9999, 12, 31, 23, 59, 59);

        public abstract string GetExceptionCategory();
        protected readonly IDataAccess _dataAccess;
        public BaseDAL(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public IDataAccess GetDataAccess()
        {
            return _dataAccess;
        }

        public void RecordError(Exception ex)
        {
            _dataAccess.RecordError(ex, GetExceptionCategory());
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
            MySqlCommand cmd = new MySqlCommand(cmd_str, (MySqlConnection)conn);
            cmd.AddParameter("@maxDate", MaxDate);
            return cmd;
        }

        public DbCommand GetCommand(string cmd_string, DbConnection conn, DbTransaction trans)
        {
            MySqlCommand cmd = new MySqlCommand(cmd_string, (MySqlConnection)conn, (MySqlTransaction)trans);
            cmd.AddParameter("@maxDate", MaxDate);
            return cmd;
        }


        public string GetString(DbDataReader dbr, string key, string alternative = "")
        {
            MySqlDataReader reader = (MySqlDataReader)dbr;
            if(IsDBNull(reader, key))
            {
                return alternative;
            }
            return reader.GetString(key);
        }
        private static bool IsDBNull(MySqlDataReader reader, string key)
        {
            return reader.IsDBNull(GetColumnNumber(reader, key));
        }
        public int GetInt(DbDataReader dbr, string key, int alternative = -1)
        {
            MySqlDataReader reader = (MySqlDataReader)dbr;
            if(IsDBNull(reader, key))
            {
                return alternative;
            }
            return reader.GetInt32(key);
        }
        public double GetDouble(DbDataReader dbr, string key, double alternative = 0.0)
        {
            MySqlDataReader reader = (MySqlDataReader)dbr;
            if (IsDBNull(reader, key)) return alternative;
            return reader.GetDouble(key);
        }

        public DateTime GetDatetime(DbDataReader dbr, string key)
        {
            MySqlDataReader reader = (MySqlDataReader)dbr;
            if(IsDBNull(reader, key)) return DateTime.MinValue;
            return reader.GetDateTime(key);
        }

        public bool GetBool(DbDataReader dbr, string key)
        {
            MySqlDataReader reader = (MySqlDataReader)dbr;
            if(IsDBNull(reader, key)) return false;
            return reader.GetBoolean(key);
        }

        protected static List<T> ListFromReader<T>(DbDataReader reader, Func<DbDataReader, T> func)
        {
            List<T> list = new List<T>();
            while(reader.Read())
            {
                list.Add(func(reader));
            }
            return list;
        }

        protected string ScalarStringFromReader(DbCommand cmd)
        {
            object? val = cmd.ExecuteScalar();
            if (val == null) return string.Empty;
            return (string)val;
        }
    }
}
