using bracket_server.Brackets;
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
        

        public static void TournamentCompetitorParams(this TournamentCompetitor competitor, DbCommand cmd)
        {
            cmd.AddParameter("@tournamentID", competitor.TournamentID);
            cmd.AddParameter("@competitorID", competitor.ID);
            cmd.AddParameter("@division", competitor.Division);
            cmd.AddParameter("@seed", competitor.Seed);
            cmd.AddParameter("@competitorName", competitor.Name);
        }

        public static void SeedDataParams(this SeedData data, DbCommand cmd)
        {
            cmd.AddParameter("@seedID", data.SeedID);
            cmd.AddParameter("@finalFourOdds", data.FinalFourOdds);
            cmd.AddParameter("@eliteEightOdds", data.EliteEightOdds);
        }

        public static void BracketParameters(this Bracket bracket, DbCommand cmd)
        {
            cmd.AddParameter("@bracketID", bracket);
            cmd.AddParameter("@owner", bracket.Owner.ID);
            cmd.AddParameter("@tournamentID", bracket.Tournament.ID);
            cmd.AddParameter("@bracketCompleted", bracket.Completed);
            cmd.AddParameter("@champTotalPoints", bracket.ChampTotalPoints);
        }
        public static void NewBracketParameters(this NewBracket bracket, DbCommand cmd)
        {
            cmd.AddParameter("@bracketID", bracket.ID);
            cmd.AddParameter("@owner", bracket.Owner.ID);
            cmd.AddParameter("@tournamentID", bracket.TournamentID);
            cmd.AddParameter("@bracketCompleted", false);
            cmd.AddParameter("@champTotalPoints", 120);
        }

        public static void PickParameters(this Pick pick, DbCommand cmd)
        {
            cmd.AddParameter("@gameID", pick.GameID);
            cmd.AddParameter("@tournamentID", pick.TournamentID);
            cmd.AddParameter("@bracketID", pick.BracketID);
            cmd.AddParameter("@competitorID", pick.CompetitorID);
        }
    }
}
