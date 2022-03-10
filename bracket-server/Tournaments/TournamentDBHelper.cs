using MillerAPI.DataAccess;
using System.Data.Common;

namespace bracket_server.Tournaments
{
    public static class TournamentDBHelper
    {
        public static void RoundParameters(this Round round, string tournament_id, DbCommand cmd)
        {
            cmd.AddParameter("@tournamentRound", round.OrderWithinTournament);
            cmd.AddParameter("@roundName", round.Name);
            cmd.AddParameter("@tournamentID", tournament_id);
        }

        public static void TournamentParameters(this Tournament tournament, DbCommand cmd)
        {
            cmd.AddParameter("@tournamentID", tournament.ID);
            cmd.AddParameter("@tournamentName", tournament.Name);
        }
    }
}
