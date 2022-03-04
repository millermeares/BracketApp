using MillerAPI.DataAccess;

namespace bracket_server.Tournaments
{
    public class TournamentDAL : BaseDAL, ITournamentDAL
    {
        public TournamentDAL(IDataAccess access) : base(access)
        {

        }

        

    }
}
