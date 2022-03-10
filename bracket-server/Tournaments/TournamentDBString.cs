namespace bracket_server.Tournaments
{
    internal static class TournamentDBString
    {
        internal static string CreateTournament = 
            @"INSERT INTO tournament(tournamentID, name, creator)
            VALUES(@tournamentID, @tournamentName, @creator);";

        internal static string InsertRound = 
            @"INSERT INTO tournament_round(_fk_tournament, round, name)
            VALUES(@tournamentID, @tournamentRound, @roundName);";
    }
}
