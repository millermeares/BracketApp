using bracket_server.Tournaments;
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
                    return ErrorResult("user invalid information");
                }
                UserID id = user_access.InsertNewUser(user);
                if(id.IsEmpty())
                {
                    return ErrorResult("Email or username already in use");
                }
                AuthToken token = user_access.GetAuthToken(id);
                if (token.IsEmpty())
                {
                    return ErrorResult("something weird happened with a token");
                }
                return GoodResult(token);
            }
            catch (Exception ex)
            {
                return ResultFromException(user_access.GetDataAccess(), ex);
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
                return GoodResult(token);
            }
            catch (Exception ex)
            {
                return ResultFromException(user_access, ex);
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
                return ResultFromException(user_access, ex);
            }
        }

        public static IResult MenuOptionsForUser(AuthToken token, IUserDAL user_dal, ITournamentDAL tournament_dal)
        {
            try
            {
                AuthenticatedUser user = user_dal.AuthenticatedUserFromToken(token);
                if (user.IsEmpty()) return Results.Unauthorized();
                MenuOptions options = BracketUsers.UserMethods.GetMenuOptionsForTournamentUser(tournament_dal);
                options.AddRange(user.GetMenuOptions(user_dal));
                return GoodResult(options);
            }
            catch (Exception ex)
            {
                return ResultFromException(user_dal, ex);
            }
        }

        public static IResult ResultFromException(IUserDAL dal, Exception ex)
        {
            return ResultFromException(dal.GetDataAccess(), ex);
        }

        public override void AddRoutes()
        {
            AddPost("/signup", SignUp);
            AddPost("/login", Login);
            AddPost("/logout", Logout);
            AddPost("/usermenuoptions", MenuOptionsForUser);
        }
    }
}
