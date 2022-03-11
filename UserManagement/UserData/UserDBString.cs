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

            @"
        INSERT user(userID, passwordSalt, passwordHash, username, email)
        SELECT @user, @passwordSalt, @passwordHash, @username, @email
        FROM dual WHERE (SELECT COUNT(email) FROM user WHERE email=@email OR username=@username IS NULL);";

        private static string _getAuthTokenForUserBase =
            @"
        UPDATE user_token SET revokedTime=now(6)
        WHERE _fk_user=@user AND revokedTime IS NULL;
        INSERT INTO user_token(_fk_user, tokenID, createTime)
        VALUES(@user, @token, UTC_TIMESTAMP());

        SELECT ut._fk_user, tokenID, createTime, revokedTime, ur._fk_role FROM user_token ut
        LEFT OUTER JOIN user_role ur ON ur._fk_user=ut._fk_user
        WHERE ut._fk_user=@user AND DATE_ADD(createTime, INTERVAL {0} MINUTE) > UTC_TIMESTAMP() AND revokedTime IS NULL
        ORDER BY createTime DESC;
            ";

        internal static string GetAuthTokenForUser(int minute_interval)
        {
            return string.Format(_getAuthTokenForUserBase, minute_interval);
        }

        internal static string GetUserFromAuthToken = 
            @"SELECT _fk_user FROM user_token WHERE tokenID=@token;";
        
        internal static string GetAuthenticatedUserFromAuthToken =
            @"
            SELECT ut._fk_user, u.username, ur._fk_role 
            FROM user_token ut 
            JOIN user u ON u.userID=ut._fk_user
            LEFT OUTER JOIN user_role ur ON ur._fk_user=ut._fk_user
            WHERE ut.tokenID=@token;";

        internal static string LogoutUser = 
            @"UPDATE user_token SET revokedTime=now(6) WHERE tokenID=@token;";
        
    }
}
