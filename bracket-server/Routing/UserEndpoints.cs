using UserManagement;

namespace bracket_server.Routing
{
    public class UserEndpoints : EndpointManager
    {
        public static User SignUp(SigningUpUser user)
        {
            return new User();
        }

        public override void AddRoutes()
        {
            AddPost("/signup", SignUp);
        }
    }
}
