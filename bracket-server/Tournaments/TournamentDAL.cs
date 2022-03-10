using MillerAPI.DataAccess;
using System.Data.Common;

namespace bracket_server.Tournaments
{
    public class TournamentDAL : BaseDAL, ITournamentDAL
    {
        
        public TournamentDAL(IDataAccess access) : base(access)
        {

        }

        // for now, this is going to just create the ncaa tournament. it can be changed in the future if needed. but i'm hardcoding the rounds for now..

        public Tournament CreateTournament(UserID user_id, string tournament_name)
        {
            List<Round> rounds = NCAARounds();
            Tournament tournament = Tournament.New(tournament_name);
            return _dataAccess.DoTransaction((conn, trans) =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.CreateTournament, conn);
                int rows = cmd.ExecuteNonQuery();
                if (rows <= 0) return Tournament.MakeEmpty();
                DBResult result = InsertRounds(rounds, tournament, trans, conn);
                if (result != DBResult.Success)
                {
                    throw new Exception("Error inserting rounds");
                }
                return tournament;
            });
        }

        public DBResult InsertRounds(List<Round> rounds, Tournament tournament, DbTransaction trans, DbConnection conn)
        {
            try
            {
                foreach (Round round in rounds)
                {
                    InsertRound(round, tournament, trans, conn);
                }
                return DBResult.Success;
            }
            catch (Exception ex)
            {
                _dataAccess.RecordError(ex);
                return DBResult.Fail;
            }

        }

        public void InsertRound(Round round, Tournament tournament, DbTransaction trans, DbConnection conn)
        {
            
            using DbCommand cmd = GetCommand(TournamentDBString.InsertRound, conn);
            round.RoundParameters(tournament.ID, cmd);
            int rows = cmd.ExecuteNonQuery();
            if(rows <= 0)
            {
                throw new Exception($"Unexpected number of rows {rows} when inserting tournament round");
            }
        }



        public override string GetExceptionCategory()
        {
            return "tournament";
        }

        private List<Round> NCAARounds()
        {
            return new List<Round>()
            { 
                new Round(1, "Seed Round"), 
                new Round(2, "Round of 32"), 
                new Round(3, "Sweet 16"),
                new Round(4, "Elite 8"), 
                new Round(5, "Final 4"),
                new Round(6, "Championship")
            };
        }
    }
}
