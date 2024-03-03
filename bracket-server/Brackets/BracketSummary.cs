namespace bracket_server.Brackets
{
    public class BracketSummary
    {
        public string BracketID { get; set; }
        public string TournamentName { get; set; }
        public DateTime CompletionDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string WinnerName { get; set; }
        public string TournamentID { get; set; }
        public BracketSummary(string bracketID, string tournamentName, string tournamentID, DateTime completionDate,
            DateTime creationDate, string winnerName)
        {
            TournamentID = tournamentID;
            BracketID = bracketID;
            TournamentName = tournamentName;
            CompletionDate = completionDate;
            CreationDate = creationDate;
            WinnerName = winnerName;
        }
    }
}
