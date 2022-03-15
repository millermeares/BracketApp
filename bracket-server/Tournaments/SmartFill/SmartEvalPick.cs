using bracket_server.Brackets;

namespace bracket_server.Tournaments.SmartFill
{
    public class SmartEvalPick : Pick
    {
        public int Round { get; private set; }
        public SmartEvalPick(string tournamentID, string gameID, string competitorID, int round) : base(tournamentID, gameID, competitorID)
        {
            Round = round;
        }
    }
}
