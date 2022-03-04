﻿using bracket_server.Tournaments;
namespace bracket_server.Routing
{
    public class TournamentEndpoints : EndpointManager
    {
        public static IResult FakeTournament()
        {
            Tournament tournament = FakeTournamentData.MakeFakeNCAATournament();
            return Results.Ok(tournament);
        }
        public override void AddRoutes()
        {
            AddGet("/home/faketournament", FakeTournament);
        }
    }
}
