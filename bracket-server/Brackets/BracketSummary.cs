﻿namespace bracket_server.Brackets
{
    public class BracketSummary
    {
        public string BracketID { get; set; }
        public string TournamentName { get; set; } 
        public DateTime CompletionDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string WinnerName { get; set; }
        public int BracketMax { get; set; }
        public int PointsEarned { get; set; }
        public BracketSummary(string bracketID, string tournamentName, DateTime completionDate, DateTime creationDate, string winnerName, int bracketMax, int pointsEarned)
        {
            BracketID = bracketID;
            TournamentName = tournamentName;
            CompletionDate = completionDate;
            CreationDate = creationDate;
            WinnerName = winnerName;
            BracketMax = bracketMax;
            PointsEarned = pointsEarned;
        }

    }
}
