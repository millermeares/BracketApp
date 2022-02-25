using MillerAPI.DataAccess;
using UserManagement.UserModels;

namespace UserManagement.UserDataAccess
{
    public interface IUserDAL
    {
        UserID InsertNewUser(SigningUpUser user);
        public AuthToken GetAuthToken(UserID id);
        public AuthToken TokenIsValid(AuthToken token);
    }
}