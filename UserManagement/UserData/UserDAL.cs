using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MillerAPI;
using MillerAPI.DataAccess;
using UserManagement.Authentication;
using UserManagement.UserModels;
using static UserManagement.UserDataAccess.DBHelpers;
namespace UserManagement.UserDataAccess
{
    public class UserDAL : IUserDAL
    {
        private readonly IDataAccess _dataAccess;
        public UserDAL(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public UserID InsertNewUser(SigningUpUser user)
        {
            Password password = user.PasswordToInsert();
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = _dataAccess.GetCommand(UserDBString.InsertUser, conn);
                user.SigningUpParameters(cmd);
                password.PasswordParameters(cmd);
                int rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                {
                    throw new Exception($"Unexpected number of affected rows {rows}");
                }
                return password.AssociatedID;
            });
        }

        public AuthToken GetAuthToken(UserID id)
        {
            return _dataAccess.DoQuery(conn =>
            {
                return AuthToken.Make();
            });
        }

        // not sure this is going to return this but we'll see
        public AuthToken TokenIsValid(AuthToken token)
        {
            return AuthToken.Make();
        }

        public Password GetUserAuthenticationInfo(LoggingInUser user)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = _dataAccess.GetCommand(UserDBString.GetUserByUsername(), conn);
                user.LoggingInUserParameters(cmd);
                using DbDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string userID = _dataAccess.GetString(reader, "username");
                    string salt = _dataAccess.GetString(reader, "passwordSalt");
                    byte[] hash = _dataAccess.GetByteArray(reader, "passwordHash");
                    return new Password()
                    {
                        AssociatedID = new UserID(userID),
                        Hash = hash,
                        Salt = salt
                    };
                }
                return Password.EmptyPassword();
                
            });
        }
    }
}
