using MillerAPI;
using MillerAPI.DataAccess;
using UserManagement;
using UserManagement.Authentication;
using UserManagement.UserDataAccess;
using UserManagement.UserModels;

namespace bracket_server.Routing
{
    public class UserEndpoints : EndpointManager
    {
        // i'd love to have an ILogger here. 
        public UserEndpoints() :base()
        {

        }
        public static IResult SignUp(SigningUpUser user, IUserDAL user_access)
        {
            try
            {
                ValidationResult validation_result = user.Validate();
                if (!validation_result.Valid)
                {
                    return Results.Problem(validation_result.Message);
                }
                UserID id = user_access.InsertNewUser(user);
                AuthToken token = user_access.GetAuthToken(id);
                return Results.Ok(token);
            }
            catch (Exception ex)
            {
                return ResultFromException(ex);
            }
        }

        public static IResult Login(LoggingInUser user, IUserDAL user_access)
        {
            try
            {
                Password password = user_access.GetUserAuthenticationInfo(user);
                if (password.IsEmpty())
                {
                    return ErrorResult("account not found");
                }
                if (!password.Match(user.Password))
                {
                    return ErrorResult("bad password");
                }
                AuthToken token = user_access.GetAuthToken(password.AssociatedID);
                return Results.Ok(token);
            }
            catch (Exception ex)
            {
                return ResultFromException(ex);
            }
        }

        public static IResult Logout(AuthToken token, IUserDAL user_access)
        {
            try
            {
                DBResult result = user_access.Logout(token);
                return ReturnBasedOnDBResult(result);
            }
            catch (Exception ex)
            {
                return ResultFromException(ex);
            }
        }

        private static IResult ResultFromException(Exception ex)
        {
            // ok so here is where i could put common logging stuff. 
            return Results.Problem(ExceptionToString(ex));
        }

        private static string ExceptionToString(Exception ex)
        {
            return ex.Message + Environment.NewLine + ex.StackTrace;
        }

        public override void AddRoutes()
        {
            AddPost("/signup", SignUp);
            AddPost("/login", Login);
            AddPost("/logout", Logout);
        }
    }
}
