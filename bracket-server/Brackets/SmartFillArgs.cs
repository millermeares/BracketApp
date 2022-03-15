using bracket_server.Tournaments;
using bracket_server.Tournaments.KenPom;

namespace bracket_server.Brackets
{
    public class SmartFillArgs
    {
        public ITournamentDAL DAL { get; set; }
        public string BracketID { get; set; }
        public SeedDataCollection SeedData { get; private set; } = new SeedDataCollection();
        public bool SavePicks { get; set; }
        public KenPomDataCollection KenPom { get; private set; } = new KenPomDataCollection();
        public int AutoWinSpread { get; private set; } = 10;
        public int MaxUnderdogRuns { get; private set; } = 2;
        public int BigUnderdogThreshold { get; private set; } = 7;
        public SmartFillArgs(ITournamentDAL dal, string bracketID)
        {
            DAL = dal;
            BracketID = bracketID;
        }

        public void SetSeedData(SeedDataCollection seeds)
        {
            SeedData = seeds;
        }

        public void SetKenPomData(KenPomDataCollection data)
        {
            KenPom = data;
        }


        public bool SavePickChange(PickChange change)
        {
            return DAL.SavePickChanges(new List<PickChange>() { change });
        }
        public bool SavePickChanges(List<PickChange> changes)
        {
            if (changes.Count == 0) return true;
            return DAL.SavePickChanges(changes);
        }

        public PickChange MakePickChange(Game game, string tournamentID)
        {
            if (game.Winner == null) throw new ArgumentException("cannot have null game winner when making pick change");
            return new PickChange(BracketID, tournamentID, game.ID, game.Winner.ID, true, true);
        }
        
    }
}
