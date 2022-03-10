namespace bracket_server.Routing.APIArgumentHelpers
{
    public class NewCompetitorAuthToken
    {
        public AuthToken Token { get; set; } = AuthToken.MakeEmpty();
        public string TournamentID { get; set; } = string.Empty;
        public Tournaments.NewTournamentCompetitor Competitor { get; set; } = new Tournaments.NewTournamentCompetitor();
    }
}
