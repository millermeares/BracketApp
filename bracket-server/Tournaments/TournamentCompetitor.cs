namespace bracket_server.Tournaments
{
    public class TournamentCompetitor : Competitor
    {
        public int Seed { get; set; } = -1;
        public int Quad;
    }
}
