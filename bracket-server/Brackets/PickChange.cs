namespace bracket_server.Brackets
{
    public class PickChange : Pick
    {
        public bool Add { get; set; } = false;
        public PickChange(string bracketID, string tournamentID, string gameID, string competitorID, bool add) : 
            base(bracketID, tournamentID, gameID, competitorID)
        {
            Add = add;
        } 
    }
}
