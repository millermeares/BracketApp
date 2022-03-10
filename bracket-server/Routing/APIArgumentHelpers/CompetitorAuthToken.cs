namespace bracket_server.Routing.APIArgumentHelpers
{
    public class CompetitorAuthToken
    {
        public AuthToken Token{get; set; } = new AuthToken();
        public Tournaments.TournamentCompetitor Competitor { get; set; } = new Tournaments.TournamentCompetitor();
    }
}
