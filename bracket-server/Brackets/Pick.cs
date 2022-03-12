using bracket_server.Tournaments;

namespace bracket_server.Brackets
{
    public class Pick
    {
        public string BracketID { get; set; }
        public string CompetitorID{ get; set; }
        public string TournamentID { get; set; } 
        public string GameID { get; set; }

        public Pick(string bracketID, string tournamentID, string gameID, string competitorID)
        {
            BracketID = bracketID;
            TournamentID = tournamentID;
            GameID = gameID;
            CompetitorID = competitorID;
        }

        public static Pick MakeEmpty()
        {
            return new Pick(string.Empty, string.Empty, string.Empty, string.Empty);
        }
    }
}
