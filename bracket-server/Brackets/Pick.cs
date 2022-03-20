namespace bracket_server.Brackets
{
    public class Pick
    {
        public string CompetitorID { get; set; }
        public string TournamentID { get; set; }
        public string GameID { get; set; }
        public Pick(string tournamentID, string gameID, string competitorID)
        {
            TournamentID = tournamentID;
            GameID = gameID;
            CompetitorID = competitorID;
        }
        public static Pick MakeEmpty()
        {
            return new Pick("", "", "");
        }
    }
}
