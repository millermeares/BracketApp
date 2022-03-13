using bracket_server.Tournaments.KenPom;
namespace bracket_server.Routing.APIArgumentHelpers
{
    public class AuthTokenCompetitorKenPomData
    {
        public AuthToken Token { get; set; } = new AuthToken();
        public KenPomDataReference KenPom { get; set; } = new KenPomDataReference();
    }
}
