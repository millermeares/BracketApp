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
        public static void DivisionParameters(this Division division, DbCommand cmd)
        {
            cmd.AddParameter("@tournamentID", division.Tournament);
            cmd.AddParameter("@divisionName", division.Name);
        }

        public static void TournamentCompetitorParams(this TournamentCompetitor competitor, DbCommand cmd)
        {
            cmd.AddParameter("@tournamentID", competitor.TournamentID);
            cmd.AddParameter("@competitorID", competitor.ID);
            cmd.AddParameter("@division", competitor.Division);
            cmd.AddParameter("@seed", competitor.Seed);
            cmd.AddParameter("@competitorName", competitor.Name);
        }
    }
}
