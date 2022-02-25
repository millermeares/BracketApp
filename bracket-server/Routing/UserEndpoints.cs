using MillerAPI;
using MillerAPI.DataAccess;
using UserManagement;
using UserManagement.UserDataAccess;
using UserManagement.UserModels;

namespace bracket_server.Routing
{
    public class UserEndpoints : EndpointManager
    {
        public UserEndpoints() :base()
        {

        }
        public static IResult SignUp(SigningUpUser user, IUserDAL user_access)
        {
            ValidationResult validation_result = user.Validate();
            if(!validation_result.Valid)
            {
                return Results.Problem(validation_result.Message);
            }
            UserID id = user_access.InsertNewUser(user);
            AuthToken token = user_access.GetAuthToken(id);
            return Results.Ok(token);
        }

        public static AuthToken Login(LoggingInUser user, IUserDAL user_access)
        {

            return AuthToken.Make();
        }

        public override void AddRoutes()
        {
            AddPost("/signup", SignUp);
            AddPost("/login", Login);
        }
    }
}
