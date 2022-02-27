using MillerAPI.DataAccess;
using UserManagement.Authentication;
using UserManagement.UserModels;

namespace UserManagement.UserDataAccess
{
    public interface IUserDAL
    {
        UserID InsertNewUser(SigningUpUser user);
        AuthToken GetAuthToken(UserID id);
        UserID UserIDFromToken(AuthToken token);
        Password GetUserAuthenticationInfo(LoggingInUser user);
    }
}