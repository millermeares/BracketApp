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
            string token_to_insert = Guid.NewGuid().ToString();
            return _dataAccess.DoTransaction((conn, trans) =>
            {
                using DbCommand cmd = _dataAccess.GetCommand(UserDBString.GetAuthTokenForUser(15), conn, trans);
                id.UserIDParameter(cmd);
                cmd.AddParameter("@token", token_to_insert);
                using DbDataReader reader = cmd.ExecuteReader();
                if(reader.Read())
                {
                    string token = _dataAccess.GetString(reader, "tokenID");
                    DateTime createTime = _dataAccess.GetDatetime(reader, "createTime");
                    return new AuthToken(token, createTime);
                }
                return AuthToken.MakeEmpty();
            });
        }

        // not sure this is going to return this but we'll see
        public UserID UserIDFromToken(AuthToken token)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = _dataAccess.GetCommand(UserDBString.GetUserFromAuthToken, conn);
                token.TokenParameter(cmd);
                using DbDataReader reader = cmd.ExecuteReader();
                if(reader.Read())
                {
                    string user = _dataAccess.GetString(reader, "_fk_user");
                    return new UserID(user);
                }
                return UserID.MakeEmpty();
            });
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
                    string userID = _dataAccess.GetString(reader, "userID");
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
