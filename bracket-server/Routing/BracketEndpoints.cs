using UserManagement.UserDataAccess;
using bracket_server.Tournaments;
using bracket_server.Brackets;
namespace bracket_server.Routing
{
    public class BracketEndpoints : EndpointManager
    {
        // intheory, this route would all be /bracket
        public static IResult GetFakeBracket(IUserDAL access)
        {
            Tournament tournament = Tournaments.FakeTournamentData.MakeFakeNCAATournament();
            Bracket bracket = Bracket.MakeEmptyBracket(tournament);
            return Results.Ok(bracket);
        }
        public override void AddRoutes()
        {
            AddGet("/bracket/fakebracket", GetFakeBracket);
        }

       
    }
}
