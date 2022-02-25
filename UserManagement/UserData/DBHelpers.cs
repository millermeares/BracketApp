using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MillerAPI.DataAccess.DALHelpers;
namespace UserManagement.UserDataAccess
{
    internal static class DBHelpers
    {
        public static void SigningUpParameters(this SigningUpUser user, DbCommand command)
        {
            command.Parameters.Add(MakeParameter("@username", user.Username));
            command.Parameters.Add(MakeParameter("@user", user.Username));
            var password = user.PasswordToInsert();
            command.Parameters.Add(MakeParameter("@passwordSalt", password.Salt));
            command.Parameters.Add(MakeParameter("@passwordHash", password.Hash));
            command.Parameters.Add(MakeParameter("@email", user.Username));
        }
    }
}
