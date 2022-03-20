using bracket_server.Brackets;

namespace bracket_server.Routing.APIArgumentHelpers
{
    public class OutcomeAuthToken
    {
        public AuthToken Token { get; set; } = new AuthToken();
        public Pick Outcome { get; set; } = Pick.MakeEmpty();
    }
}
