namespace bracket_server.Tournaments
{
    public class TournamentCompetitor : Competitor
    {
        public string TournamentID { get; set; } = string.Empty;
        public int Seed { get; set; } = -1;
        public string Division { get; set; } = string.Empty;
        public static TournamentCompetitor MakeEmpty()
        {
            return new TournamentCompetitor();
        }
        public static TournamentCompetitor MakeNew(NewTournamentCompetitor competitor, string tournament_id)
        {
            return new TournamentCompetitor()
            {
                Name = competitor.Name,
                Seed = competitor.Seed,
                Division = competitor.Division,
                ID = Guid.NewGuid().ToString(),
                TournamentID=tournament_id
            };
        }
        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(ID);
        }
    }
}
