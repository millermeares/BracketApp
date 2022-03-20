using bracket_server.Tournaments;

namespace bracket_server.Brackets
{
    public class BracketSmartFillArgs : SmartFillArgs
    {
        public string BracketID { get; set; }
        public BracketSmartFillArgs(ITournamentDAL dal, string bracketID) : base(dal)
        {
            BracketID = bracketID;
        }
        public bool SavePickChange(BracketPickChange change)
        {
            return DAL.SavePickChanges(new List<BracketPickChange>() { change });
        }
        public bool SaveAsPickChanges(List<Pick> changes)
        {
            if (changes.Count == 0) return true;
            return DAL.SavePickChanges(changes.Select(p => (BracketPickChange)p).ToList());
        }

        public override Pick MakePick(Game game, string tournamentID)
        {
            if (game.PredictedWinner == null) throw new ArgumentException("cannot have null game winner when making pick change");
            return new BracketPickChange(BracketID, tournamentID, game.ID, game.PredictedWinner.ID, true, true);
        }






    }
}
