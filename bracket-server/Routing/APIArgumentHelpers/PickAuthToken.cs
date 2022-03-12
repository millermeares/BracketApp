using bracket_server.Brackets;

namespace bracket_server.Routing.APIArgumentHelpers
{
    public class PickAuthToken
    {
        public AuthToken Token { get; set; } = new AuthToken();
        public Pick Pick { get; set; } = Pick.MakeEmpty();
    }
}
