using UserManagement;

namespace bracket_server.Routing
{
    public class UserEndpoints : EndpointManager
    {
        public UserEndpoints() :base()
        {

        }
        public static AuthToken SignUp(SigningUpUser user)
        {
            return new AuthToken();
        }

        public override void AddRoutes()
        {
            AddPost("/signup", SignUp);
        }
    }
}
