using System.Runtime.ConstrainedExecution;

namespace bracket_server.Brackets
{
    public class BracketPerformance
    {
        public string BracketID { get; set; }
        public int PointsEarned { get; }
        public int BracketMax { get; }

        public BracketPerformance(string bracketId, int pointsEarned, int bracketMax) 
        {
            BracketID = bracketId;
            PointsEarned = pointsEarned;
            BracketMax = bracketMax;
        }
    }
}
