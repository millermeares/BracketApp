using MillerAPI.DataAccess;
using System.Data.Common;
using System.Text;

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

        internal static string InsertDivision = 
            @"INSERT INTO tournament_division(_fk_tournament, divisionName)
            VALUES(@tournamentID, @divisionName);";

        internal static string AllTournaments = 
            @"SELECT name, tournamentID, tournamentFinalized from tournament;";

        internal static string GetTournamentTopLevelByID = 
            @"SELECT name, tournamentID, tournamentFinalized FROM tournament WHERE tournamentID=@tournamentID;";

        internal static string DeleteTournament = 
            @"
        DELETE FROM tournament_division WHERE _fk_tournament=@tournamentID;
            ";

        internal static string GetCompetitorsForTournament = 
            @"SELECT * FROM competitor_tournament WHERE _fk_tournament=@tournamentID
            ORDER BY _fk_division, _fk_seed;";

        internal static string InsertTournamentCompetitor = 
            @"INSERT INTO competitor_tournament(_fk_tournament, _fk_division, _fk_seed, competitorID, competitorName)
            VALUES(@tournamentID, @division, @seed, @competitorID, @competitorName);";

        internal static string DeleteTournamentCompetitor =
            @"DELETE FROM competitor_tournament WHERE _fk_tournament=@tournamentID AND competitorID=@competitorID;";

        internal static string GetSeedDataForTournament =
            @"
        SELECT s.seedID, s._fk_tournamentType, sd.finalFourOdds, sd.eliteEightOdds
        FROM seed s
        LEFT JOIN seed_data sd ON s.seedID=sd._fk_seed
        WHERE s._fk_tournamentType=@tournamentType
        ORDER BY s.seedID;
            ";

        internal static string UpdateSeedData = 
            @"
        DELETE FROM seed_data WHERE _fk_seed=@seedID;
        INSERT INTO seed_data(_fk_seed, finalFourOdds, eliteEightOdds)
        VALUES(@seedID, @finalFourOdds, @eliteEightOdds);
            ";


        private static string _finalizeTournamentBase = 
            @"
        INSERT INTO tournament_game(_fk_tournament, _fk_division, _fk_tournamentRound, gameID, _fk_leftGame, _fk_rightGame)
        VALUES
        {0};
        
        INSERT INTO game_participant(_fk_competitor, _fk_game)
        VALUES
        {1};
        
        UPDATE tournament SET tournamentFinalized=true WHERE tournamentID=@tournamentID;
            ";
        internal static string FinalizeTournament(Tournament tournament, DbCommand cmd)
        {
            List<Game> games = tournament.ChampionshipGame.Flatten();
            return string.Format(_finalizeTournamentBase,
                GetInsertTournamentGameValuesString(cmd, games), GetInsertCompetitorsString(cmd, games)
                );
        }

        private static string GetInsertTournamentGameValuesString(DbCommand cmd, List<Game> games)
        {
            //(_fk_tournament, _fk_division, _fk_tournamentRound, gameID, _fk_leftGame, _fk_rightGame)
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < games.Count; i++)
            {
                string division_param_name = $"@gameTDivision{i}";
                string game_round_name = $"@gameTRound{i}";
                string game_id = $"@gameTID{i}";
                string game_left_id = $"@gameTLGID{i}";
                string game_right_id = $"@gameTRGID{i}";
                cmd.AddParameter(division_param_name, games[i].Division);
                cmd.AddParameter(game_round_name, games[i].Round);
                cmd.AddParameter(game_id, games[i].ID);
                cmd.AddParameter(game_left_id, games[i].LeftGame == null ? null : games[i].LeftGame.ID);
                

                cmd.AddParameter(game_right_id, games[i].RightGame == null ? null : games[i].RightGame.ID);
                sb.Append($"(@tournamentID, {division_param_name}, {game_round_name}, {game_id}, {game_left_id}, {game_right_id})");
                if(i != games.Count - 1)
                {
                    sb.Append(",");
                }
            }
            return sb.ToString();
        }

        private static string GetInsertCompetitorsString(DbCommand cmd, List<Game> games)
        {
            //_fk_competitor, _fk_game
            StringBuilder sb = new StringBuilder();
            List<Game> competitor_games = games.Where(g => g.HasCompetitors()).ToList();
            for(int i = 0; i < competitor_games.Count; i++)
            {
                string game_id = $"@gameCID{i}";
                cmd.AddParameter(game_id, competitor_games[i].ID);

                string competitor_1_id = $"@gameC1ID{i}";
                cmd.AddParameter(competitor_1_id, competitor_games[i].Competitor1?.ID);

                string competitor_2_id = $"@gameC2ID{i}";
                cmd.AddParameter(competitor_2_id, competitor_games[i].Competitor2?.ID);

                sb.Append($"({competitor_1_id},{game_id}),");
                sb.Append($"({competitor_2_id},{game_id})");
                if(i != competitor_games.Count - 1)
                {
                    sb.Append(",");
                }
            }
            return sb.ToString();
        }

        internal static string InsertStringParam = 
            @"
        UPDATE string_params SET endDate=now(6) WHERE paramKey=@key AND endDate=@maxDate;
        INSERT INTO string_params(paramKey, paramValue)
        VALUES(@key, @value);
";
        internal static string GetStringParam = 
            @"SELECT paramvalue FROM string_params WHERE paramKey=@key AND endDate=@maxDate;";

        internal static string GetTournamentByID =
            @"
        SELECT tournamentID, name, creator, _fk_type, tournamentFinalized, 
	        g._fk_division AS gameDivision, _fk_tournamentRound, gameID, _fk_leftGame, _fk_rightGame, 
	        c._fk_seed, c.competitorName, c.competitorID, c._fk_division AS competitorDivision
        FROM tournament t 
        JOIN tournament_game g ON g._fk_tournament=t.tournamentID
        LEFT OUTER JOIN game_participant p ON p._fk_game=g.gameID
        LEFT OUTER JOIN competitor_tournament c ON c.competitorID=p._fk_competitor AND t.tournamentID=c._fk_tournament
        WHERE t.tournamentID=@tournamentID;
            ";

        private static string GetBracketBase =
            @"
        SELECT tournamentID, name, creator, _fk_type, tournamentFinalized, 
	        g._fk_division AS gameDivision, _fk_tournamentRound, gameID, _fk_leftGame, _fk_rightGame, g._fk_competitor_Winner,
	        c._fk_seed, c.competitorName, c.competitorID, c._fk_division AS competitorDivision, 
            bgp._fk_competitor AS winnerPick, bgp._fk_game AS gamePick, b.bracketID, b.completed, b.champTotalPoints, b.creationTime
        FROM user_bracket b 
        JOIN tournament t ON t.tournamentID = b._fk_tournament
        JOIN tournament_game g ON g._fk_tournament=t.tournamentID
        LEFT OUTER JOIN game_participant p ON p._fk_game=g.gameID
        LEFT OUTER JOIN competitor_tournament c ON c.competitorID=p._fk_competitor AND t.tournamentID=c._fk_tournament
        LEFT OUTER JOIN bracket_game_prediction bgp ON g.gameID=bgp._fk_game AND bgp._fk_bracket=b.bracketID
        WHERE b.bracketID={0}
        ORDER BY g._fk_tournamentRound, gameDivision, g.gameID;";

        internal static string LatestBracketForUserString = 
            @"
        SELECT bracketID FROM user_bracket b 
        WHERE b._fk_user=@userID
        ORDER BY creationTime DESC LIMIT 1;
            ";

        internal static string GetLatestBracketForTournamentForUser =
            @"
        SELECT bracketID FROM user_bracket b 
        WHERE b._fk_user=@userID AND _fk_tournament=@tournamentID
        ORDER BY creationTime DESC LIMIT 1;
        ";

        internal static string GetBracketByID()
        {
            return string.Format(GetBracketBase, "@bracketID");
        }

        internal static string LatestBracketForUser()
        {
            string latest_bracket_query = $"({LatestBracketForUserString})";
            return string.Format(GetBracketBase, latest_bracket_query);
        }

        internal static string InsertNewBracket =
            @"INSERT INTO user_bracket(_fk_user, _fk_tournament, bracketID, creationTime)
            VALUES(@owner, @tournamentID, @bracketID, now(6));";
    }
}
