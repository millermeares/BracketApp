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
        public int Points { get; set; } = 0;
        public int MaxPoints { get; set; } = 0;
        
        
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

        public List<BracketPickChange> GetPickChanges(BracketPick p)
        {
            return Tournament.GetPickChanges(p);
        }

        private BracketSmartFillArgs GetSmartFillArgs(ITournamentDAL dal)
        {
            BracketSmartFillArgs args = new BracketSmartFillArgs(dal, ID);
            args.FillAllDbArgs(Tournament);
            return args;
        }

        public void SmartFill(ITournamentDAL dal)
        {
            BracketSmartFillArgs smartFillArgs = GetSmartFillArgs(dal);
            List<Pick> changes = Tournament.Smartfill(smartFillArgs);
            smartFillArgs.SaveAsPickChanges(changes);
        }

       
    }
}
