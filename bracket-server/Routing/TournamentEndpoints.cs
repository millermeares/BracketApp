using bracket_server.Routing.APIArgumentHelpers;
using bracket_server.Tournaments;
namespace bracket_server.Routing
{
    public class TournamentEndpoints : EndpointManager
    {
        public static IResult FakeTournament()
        {
            Tournament tournament = FakeTournamentData.MakeFakeNCAATournament();
            return GoodResult(tournament); ;
        }
        public static IResult FakeTournamentSkeleton()
        {

            Tournament tournament = FakeTournamentData.MakeFakeNCAATournamentSkeleton();
            return GoodResult(tournament);
        }

        public static IResult GetCompetitors(GenericID tournament_id, ITournamentDAL tournament_dal)
        {
            try
            {
                List<TournamentCompetitor> competitors = tournament_dal.TournamentCompetitorsForTournament(tournament_id.ID);
                return GoodResult(competitors);
            }
            catch (Exception ex)
            {
                return ResultFromException(tournament_dal.GetDataAccess(), ex);
            }
        }

        public static IResult GetSeedData(ITournamentDAL tournament_dal)
        {
            string tournamentType = "NCAA64Basketball"; // this would probably be a url argument
            try
            {
                return GoodResult(tournament_dal.GetSeedDataForTournamentType(tournamentType));
            }catch(Exception ex)
            {
                return ResultFromException(tournament_dal.GetDataAccess(), ex);
            }
        }

        public override void AddRoutes()
        {
            AddGet("/faketournament", FakeTournament);
            AddGet("/faketournamentskeleton", FakeTournamentSkeleton);
            AddPost("/getcompetitors", GetCompetitors);
            AddGet("/getseeddata", GetSeedData);
        }

        

        

       
    }
}
