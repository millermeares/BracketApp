using bracket_server.Routing.APIArgumentHelpers;
using bracket_server.Tournaments;
using bracket_server.Tournaments.KenPom;

namespace bracket_server.Brackets
{
    public class Bracket
    {
        public Tournament Tournament { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        internal UserID Owner { get; set; }
        public bool Completed { get; set; } = false;
        public int ChampTotalPoints { get; set; } = 120;
        public DateTime CreationTime { get; set; }
        
        
        public Bracket(Tournament tournament, string id, string name, UserID owner)
        {
            Owner = owner;
            Name = name;
            ID = id;
            Tournament = tournament;
        }

        public static Bracket MakeEmpty()
        {
            return new Bracket(new Tournament(), string.Empty, string.Empty, new UserID());
        }
        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(ID);
        }

        public static Bracket GetForNewOrLatestBracketForUser(UserID userID, string live_tournament_id, ITournamentDAL tournament_dal)
        {
            GenericID bracket_id = tournament_dal.GetUnfinishedLatestBracketIDForTournamentForUser(userID, live_tournament_id);
            if(bracket_id.IsEmpty())
            {
                NewBracket new_bracket = new NewBracket(live_tournament_id, userID);
                bool success = tournament_dal.InsertBracket(new_bracket);
                if(!success)
                {
                    return MakeEmpty(); // error state
                }
                bracket_id.ID = new_bracket.ID; 
            }
            return tournament_dal.GetBracket(bracket_id.ID);
        }

        public bool SaveableState()
        {
            return Tournament.FullyPopulated();
        }

        public List<PickChange> GetPickChanges(Pick p)
        {
            return Tournament.GetPickChanges(p);
        }

        private SmartFillArgs GetSmartFillArgs(ITournamentDAL dal)
        {
            SmartFillArgs args = new SmartFillArgs(dal, ID);
            args.SetSeedData(GetSeedDataCollection(dal));
            args.SetKenPomData(GetKenPomCollection(dal));
            return args;
        }

        public void AutoFill(ITournamentDAL dal)
        {
            SmartFillArgs smartFillArgs = GetSmartFillArgs(dal);
            Tournament.Autofill(smartFillArgs);
        }

        private SeedDataCollection GetSeedDataCollection(ITournamentDAL dal)
        {
            List<SeedData> seed_data = dal.GetSeedDataForTournamentType(Tournament.TournamentType);
            return new SeedDataCollection(seed_data);
        }

        private KenPomDataCollection GetKenPomCollection(ITournamentDAL dal)
        {
            Dictionary<string, KenPomData> data = dal.KenPomDataForTournament(Tournament.ID);
            return new KenPomDataCollection(data);
        }
    }
}
