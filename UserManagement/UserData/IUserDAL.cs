using MillerAPI.DataAccess;
using UserManagement.Authentication;
using UserManagement.UserModels;

namespace UserManagement.UserDataAccess
{
    public interface IUserDAL
    {
        public IDataAccess GetDataAccess();
        public void RecordError(Exception ex);
        UserID InsertNewUser(SigningUpUser user);
        AuthToken GetAuthToken(UserID id);
        UserID UserIDFromToken(AuthToken token);
        AuthenticatedUser AuthenticatedUserFromToken(AuthToken token);
        Password GetUserAuthenticationInfo(LoggingInUser user);
        DBResult Logout(AuthToken token);

    }
}