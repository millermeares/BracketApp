using bracket_server.Tournaments;

namespace bracket_server.Brackets
{
    public class BracketPick : Pick
    {
        public string BracketID { get; set; }
        

        public BracketPick(string bracketID, string tournamentID, string gameID, string competitorID)
            :base(tournamentID, gameID, competitorID)
        {
            BracketID = bracketID;
        }

        public static BracketPick MakeEmpty()
        {
            return new BracketPick(string.Empty, string.Empty, string.Empty, string.Empty);
        }
    }
}
