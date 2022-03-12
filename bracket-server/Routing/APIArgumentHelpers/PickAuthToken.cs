using bracket_server.Brackets;

namespace bracket_server.Routing.APIArgumentHelpers
{
    public class PickAuthToken
    {
        public AuthToken Token { get; set; } = new AuthToken();
        public PickChange PickChange { get; set; } = PickChange.MakeEmpty();
    }
}
