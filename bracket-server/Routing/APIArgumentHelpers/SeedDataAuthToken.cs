using bracket_server.Tournaments;

namespace bracket_server.Routing.APIArgumentHelpers
{
    public class SeedDataAuthToken
    {
        public SeedData Seed { get; set; } = new SeedData();
        public AuthToken Token { get; set; } = new AuthToken();
    }
}
