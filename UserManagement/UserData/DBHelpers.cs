using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Authentication;
using UserManagement.UserModels;
using static MillerAPI.DataAccess.DALHelpers;
namespace UserManagement.UserDataAccess
{
    public static class DBHelpers
    {
        
        public static void SigningUpParameters(this SigningUpUser user, DbCommand command)
        {
            command.Parameters.Add(MakeParameter("@username", user.Username));
            command.Parameters.Add(MakeParameter("@email", user.Email));
        }

        public static void PasswordParameters(this Password password, DbCommand command)
        {
            command.Parameters.Add(MakeParameter("@passwordSalt", password.Salt));
            command.Parameters.Add(MakeParameter("@passwordHash", password.Hash));
            password.AssociatedID.UserIDParameter(command);
        }

        public static void UserIDParameter(this UserID id, DbCommand cmd)
        {
            cmd.Parameters.Add(MakeParameter("@user", id.ID));
        }

        public static void LoggingInUserParameters(this LoggingInUser user, DbCommand cmd)
        {
            cmd.AddParameter("@username", user.Username);
        }

        public static void TokenParameter(this AuthToken token, DbCommand cmd)
        {
            cmd.AddParameter("@token", token.Token);
        }
    }
}
