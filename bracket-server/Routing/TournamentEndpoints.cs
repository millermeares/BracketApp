using bracket_server.Tournaments;
namespace bracket_server.Routing
{
    public class TournamentEndpoints : EndpointManager
    {
        public static IResult FakeTournament()
        {
            Tournament tournament = FakeTournamentData.MakeFakeNCAATournament();
            return Results.Ok(tournament);
        }
        public static IResult FakeTournamentSkeleton()
        {
            Tournament tournament = FakeTournamentData.MakeFakeNCAATournamentSkeleton();
            return Results.Ok(tournament);
        }
        public override void AddRoutes()
        {
            AddGet("/home/faketournament", FakeTournament);
            AddGet("/home/faketournamentskeleton", FakeTournamentSkeleton);

        }
    }
}
