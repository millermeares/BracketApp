﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MillerAPI;
using MillerAPI.DataAccess;
using UserManagement.Authentication;
using UserManagement.Roles;
using UserManagement.UserModels;
using static UserManagement.UserDataAccess.DBHelpers;
namespace UserManagement.UserDataAccess
{
    public class UserDAL : BaseDAL, IUserDAL
    {
        public UserDAL(IDataAccess dataAccess) : base(dataAccess)
        {
            
        }

        public UserID InsertNewUser(SigningUpUser user)
        {
            Password password = user.PasswordToInsert();
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(UserDBString.InsertUser, conn);
                user.SigningUpParameters(cmd);
                password.PasswordParameters(cmd);
                int rows = cmd.ExecuteNonQuery();
                if (rows <= 0)
                {
                    return UserID.MakeEmpty();
                }
                return password.AssociatedID;
            });
        }

        public AuthToken GetAuthToken(UserID id)
        {
            string token_to_insert = Guid.NewGuid().ToString();
            return _dataAccess.DoTransaction((conn, trans) =>
            {
                using DbCommand cmd = GetCommand(UserDBString.GetAuthTokenForUser(15), conn, trans);
                id.UserIDParameter(cmd);
                cmd.AddParameter("@token", token_to_insert);
                using DbDataReader reader = cmd.ExecuteReader();
                return AuthTokenFromReader(reader);
            });
        }

        private AuthToken AuthTokenFromReader(DbDataReader reader)
        {
            AuthToken authToken = AuthToken.MakeEmpty();
            while(reader.Read())
            {
                if(authToken.IsEmpty())
                {
                    string token = GetString(reader, "tokenID");
                    DateTime createTime = GetDatetime(reader, "createTime");
                    authToken  = new AuthToken(token, createTime);
                }
                string role = GetString(reader, "_fk_role");
                authToken.AddRole(new Role() { Name = role });
            }
            return authToken;
        }

        // not sure this is going to return this but we'll see
        public UserID UserIDFromToken(AuthToken token)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(UserDBString.GetUserFromAuthToken, conn);
                token.TokenParameter(cmd);
                using DbDataReader reader = cmd.ExecuteReader();
                if(reader.Read())
                {
                    string user = GetString(reader, "_fk_user");
                    return new UserID(user);
                }
                return UserID.MakeEmpty();
            });
        }

        public Password GetUserAuthenticationInfo(LoggingInUser user)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(UserDBString.GetUserByUsername(), conn);
                user.LoggingInUserParameters(cmd);
                using DbDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string userID = GetString(reader, "userID");
                    string salt = GetString(reader, "passwordSalt");
                    byte[] hash = GetByteArray(reader, "passwordHash");
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

        public DBResult Logout(AuthToken token)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(UserDBString.LogoutUser, conn);
                token.TokenParameter(cmd);
                int rows = cmd.ExecuteNonQuery();
                return rows > 0 ? DBResult.Success : DBResult.NoMatch;
            });
        }

        public override string GetExceptionCategory()
        {
            return "user";
        }

        public AuthenticatedUser AuthenticatedUserFromToken(AuthToken token)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(UserDBString.GetAuthenticatedUserFromAuthToken, conn);
                token.TokenParameter(cmd);
                using DbDataReader reader = cmd.ExecuteReader();
                return SingleAuthenticatedUserFromReader(reader);
            });
        }

        private AuthenticatedUser SingleAuthenticatedUserFromReader(DbDataReader reader)
        {
            AuthenticatedUser user = AuthenticatedUser.MakeEmpty();
            while(reader.Read())
            {
                if(user.IsEmpty())
                {
                    string username = GetString(reader, "username");
                    string id = GetString(reader, "_fk_user");
                    user = new AuthenticatedUser(username, id);
                }
                string role = GetString(reader, "_fk_role");
                if(!string.IsNullOrEmpty(role))
                {
                    user.AddRole(role);
                }
            }
            return user;
        }
    }
}
