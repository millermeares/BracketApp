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

        private static string _getAuthTokenForUserBase = 
            @"
        UPDATE user_token SET loggedOutTime=now(6)
        WHERE _fk_user=@user AND DATE_SUB(UTC_TIMESTAMP(), INTERVAL {0} MINUTE) > expireTime AND revokedTime IS NULL;
        INSERT INTO user_token(_fk_user, tokenID, createTime)
        VALUES(@user, @token, @utc_now);
        SELECT _fk_user, tokenID, expireTime, revokedTime FROM user_token 
        WHERE _fk_user=@user AND DATE_SUB(UTC_timestamp(), INTERVAL {0} MINUTE) > expireTime AND revokedTime IS NULL
        ORDER BY expireTime DESC 
        LIMIT 1;
            ";

        internal static string GetAuthTokenForUser(int minute_interval)
        {
            return string.Format(_getAuthTokenForUserBase, minute_interval);
        }
        
    }
}
