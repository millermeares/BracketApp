namespace bracket_server.Brackets
{
    public class PickChange : Pick
    {
        public bool Add { get; set; } = false;
        public bool IsSmartPick { get; set; }
        public PickChange(string bracketID, string tournamentID, string gameID, string competitorID, bool add, bool isSmartPick) : 
            base(bracketID, tournamentID, gameID, competitorID)
        {
            Add = add;
            IsSmartPick = isSmartPick;
        } 
        public static new PickChange MakeEmpty()
        {
            return new PickChange("", "", "", "", false, false);
        }
    }
}
