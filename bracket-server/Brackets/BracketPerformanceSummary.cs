namespace bracket_server.Brackets
{
    public class BracketPerformanceSummary : BracketSummary
    {
        public int BracketMax { get; set; }
        public int PointsEarned { get; set; }
        public BracketPerformanceSummary(string bracketID, string tournamentName, string tournamentID, DateTime completionDate, 
            DateTime creationDate, string winnerName, int bracketMax, int pointsEarned)
            : base(bracketID, tournamentName, tournamentID, completionDate, creationDate, winnerName)
        {
            BracketMax = bracketMax;
            PointsEarned = pointsEarned;
        }

    }
}
