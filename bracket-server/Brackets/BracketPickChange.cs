namespace bracket_server.Brackets
{
    public class BracketPickChange : BracketPick
    {
        public bool Add { get; set; } = false;
        public bool IsSmartPick { get; set; }
        public BracketPickChange(string bracketID, string tournamentID, string gameID, string competitorID, bool add, bool isSmartPick) : 
            base(bracketID, tournamentID, gameID, competitorID)
        {
            Add = add;
            IsSmartPick = isSmartPick;
        } 
        public static new BracketPickChange MakeEmpty()
        {
            return new BracketPickChange("", "", "", "", false, false);
        }
    }
}
