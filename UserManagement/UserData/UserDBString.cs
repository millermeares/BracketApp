using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.UserDataAccess
{
    internal static class UserDBString
    {
        private static string _userSelectParams = "u.userID, u.passwordSalt, u.passwordHash, u.username, u.email";
        internal static string GetUserByUsername()
        {
            return $"SELECT {_userSelectParams} FROM user u WHERE u.username=@username;";
        }

        internal static string GetUserById()
        {
            return $"SELECT {_userSelectParams} FROM user u WHERE u.userID=@username;";
        }

        internal static string InsertUser = 
            
            @"INSERT user(userID, passwordSalt, passwordHash, username, email)
            VALUES(@user, @passwordSalt, @passwordHash, @username, @email);";
    }
}
