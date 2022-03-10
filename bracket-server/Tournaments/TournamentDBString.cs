﻿namespace bracket_server.Tournaments
{
    internal static class TournamentDBString
    {
        internal static string CreateTournament = 
            @"INSERT INTO tournament(tournamentID, name, creator)
            VALUES(@tournamentID, @tournamentName, @creator);";

        internal static string InsertRound = 
            @"INSERT INTO tournament_round(_fk_tournament, round, name)
            VALUES(@tournamentID, @tournamentRound, @roundName);";

        internal static string InsertDivision = 
            @"INSERT INTO tournament_division(_fk_tournament, divisionName)
            VALUES(@tournamentID, @divisionName);";

        internal static string AllTournaments = 
            @"SELECT name, tournamentID from tournament;";

        internal static string DeleteTournament = 
            @"
        DELETE FROM tournament_division WHERE _fk_tournament=@tournamentID;
        DELETE FROM tournament_round WHERE _fk_tournament=@tournamentID;
        DELETE FROM tournament WHERE tournamentID=@tournamentID;
            ";

        internal static string GetCompetitorsForTournament = 
            @"SELECT * FROM competitor_tournament WHERE _fk_tournament=@tournamentID
            ORDER BY _fk_division, _fk_seed;";

        internal static string InsertTournamentCompetitor = 
            @"INSERT INTO competitor_tournament(_fk_tournament, _fk_division, _fk_seed, competitorID, competitorName)
            VALUES(@tournamentID, @division, @seed, @competitorID, @competitorName);";

        internal static string DeleteTournamentCompetitor =
            @"DELETE FROM competitor_tournament WHERE _fk_tournament=@tournamentID AND competitorID=@competitorID;";
    }
}
