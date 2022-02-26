using MillerAPI.DataAccess;
using UserManagement.Authentication;
using UserManagement.UserModels;

namespace UserManagement.UserDataAccess
{
    public interface IUserDAL
    {
        UserID InsertNewUser(SigningUpUser user);
        AuthToken GetAuthToken(UserID id);
        AuthToken TokenIsValid(AuthToken token);
        Password GetUserAuthenticationInfo(LoggingInUser user);
    }
}