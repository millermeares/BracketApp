using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MillerAPI;
using MillerAPI.DataAccess;
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
            string id = Guid.NewGuid().ToString();
            return _dataAccess.DoQuery(conn =>
            {
                using (DbCommand cmd = _dataAccess.GetCommand(UserDBString.InsertUser, conn))
                {
                    user.SigningUpParameters(cmd);
                    int rows = cmd.ExecuteNonQuery();
                    if(rows <= 0)
                    {
                        throw new Exception($"Unexpected number of affected rows {rows}");
                    }
                    return new UserID
                    {
                        ID = id
                    };
                }
            });
        }

        public AuthToken GetAuthToken(UserID id)
        {
            return _dataAccess.DoQuery(conn =>
            {
                return AuthToken.Make();
            });
        }

        public AuthToken TokenIsValid(AuthToken token)
        {
            return AuthToken.Make();
        }
    }
}
