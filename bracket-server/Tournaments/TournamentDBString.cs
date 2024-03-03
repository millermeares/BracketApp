using bracket_server.Brackets;
using MillerAPI.DataAccess;
using System.Data.Common;
using System.Net.NetworkInformation;
using System.Text;

namespace bracket_server.Tournaments
{
    internal static class TournamentDBString
    {
        internal static string CreateTournament =
            @"INSERT INTO tournament(tournamentID, name, creator)
            SELECT @tournamentID, @tournamentName, @creator
            WHERE 0=(SELECT COUNT(*) FROM tournament WHERE name=@tournamentName);";

        internal static string InsertRound =
            @"INSERT INTO tournament_round(_fk_tournament, round, name)
            VALUES(@tournamentID, @tournamentRound, @roundName);";

        internal static string RoundsForTournament =
            @"SELECT r.round, r.name 
            FROM tournament_round r
            JOIN tournament t ON t._fk_type=_fk_tournamentType
            WHERE t.tournamentID=@tournamentID;";

        internal static string InsertDivision =
            @"INSERT INTO tournament_division(_fk_tournament, divisionName)
            VALUES(@tournamentID, @divisionName);";

        internal static string AllTournaments =
            @"SELECT name, tournamentID, tournamentFinalized, createdTime, _fk_type AS tournamentType from tournament;";

        internal static string GetTournamentTopLevelByID =
            @"SELECT name, tournamentID, tournamentFinalized, createdTime, _fk_type AS tournamentType FROM tournament WHERE tournamentID=@tournamentID;";

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
            for (int i = 0; i < games.Count; i++)
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
                if (i != games.Count - 1)
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
            for (int i = 0; i < competitor_games.Count; i++)
            {
                string game_id = $"@gameCID{i}";
                cmd.AddParameter(game_id, competitor_games[i].ID);

                string competitor_1_id = $"@gameC1ID{i}";
                cmd.AddParameter(competitor_1_id, competitor_games[i].Competitor1?.ID);

                string competitor_2_id = $"@gameC2ID{i}";
                cmd.AddParameter(competitor_2_id, competitor_games[i].Competitor2?.ID);

                sb.Append($"({competitor_1_id},{game_id}),");
                sb.Append($"({competitor_2_id},{game_id})");
                if (i != competitor_games.Count - 1)
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

        private static string GetBracketBase =
            @"
        SELECT tournamentID, name, t.createdTime, creator, t._fk_type AS tournamentType, tournamentFinalized, b._fk_user AS userID, b.completionTime,
	        g._fk_division AS gameDivision, _fk_tournamentRound, gameID, _fk_leftGame, _fk_rightGame, g._fk_competitor_Winner,
	        c._fk_seed, c.competitorName, c.competitorID, c._fk_division AS competitorDivision, 
            bgp._fk_competitor AS winnerPick, bgp._fk_game AS gamePick, b.bracketID, b.completed, b.champTotalPoints, b.creationTime
        FROM user_bracket b 
        JOIN tournament t ON t.tournamentID = b._fk_tournament
        JOIN tournament_game g ON g._fk_tournament=t.tournamentID
        LEFT OUTER JOIN game_participant p ON p._fk_game=g.gameID AND g._fk_leftGame IS NULL AND g._fk_rightGame IS NULL
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

        internal static string GetLatestUnfinishedBracketForTournamentForUser =
            @"
        SELECT bracketID FROM user_bracket b 
        WHERE b._fk_user=@userID AND _fk_tournament=@tournamentID AND completed IS FALSE
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
            VALUES(@userID, @tournamentID, @bracketID, now(6));
            ";

        internal static string CompleteBracket =
        @"
        UPDATE user_bracket SET completed=TRUE, completionTime=now(6)
        WHERE _fk_user=@userID AND _fk_tournament=@tournamentID AND bracketID=@bracketID;
        ";

        private static string _insertPickChangesBase =
            @"
        INSERT INTO bracket_game_prediction(_fk_bracket, _fk_tournament, _fk_game, _fk_competitor, pickTime, isSmartPick)
        VALUES
        {0};";

        private static string _deletePickChangesBase =
            @"
        DELETE FROM bracket_game_prediction WHERE (_fk_bracket, _fk_game) IN ({0});
        ";

        private static string MakeInsertPickChangesString(List<BracketPickChange> changes, DbCommand cmd)
        {
            StringBuilder sb_val = new StringBuilder();
            for (int i = 0; i < changes.Count; i++)
            {
                string p_bracket_name = $"@IBracketPick{i}";
                string p_comp_name = $"@ICompPick{i}";
                string p_game_name = $"@IGamePick{i}";
                string p_tournament_name = $"@ITournamentPick{i}";
                string p_smart_pick = $"@ISmartPick{i}";
                cmd.AddParameter(p_bracket_name, changes[i].BracketID);
                cmd.AddParameter(p_comp_name, changes[i].CompetitorID);
                cmd.AddParameter(p_game_name, changes[i].GameID);
                cmd.AddParameter(p_tournament_name, changes[i].TournamentID);
                cmd.AddParameter(p_smart_pick, changes[i].IsSmartPick);
                sb_val.Append($"({p_bracket_name}, {p_tournament_name}, {p_game_name}, {p_comp_name}, now(6),{p_smart_pick})");
                if (i != changes.Count - 1)
                {
                    sb_val.Append(",");
                }
            }
            return string.Format(_insertPickChangesBase, sb_val.ToString());
        }

        private static string MakeDeletePickChangesString(List<BracketPickChange> changes, DbCommand cmd)
        {
            StringBuilder sb_val = new StringBuilder();
            for (int i = 0; i < changes.Count; i++)
            {
                string p_bracket_name = $"@DBracketPick{i}";
                string p_game_name = $"@DGamePick{i}";
                cmd.AddParameter(p_bracket_name, changes[i].BracketID);
                cmd.AddParameter(p_game_name, changes[i].GameID);
                sb_val.Append($"({p_bracket_name},{p_game_name})");
                if (i != changes.Count - 1)
                {
                    sb_val.Append(",");
                }
            }
            return string.Format(_deletePickChangesBase, sb_val.ToString());
        }

        internal static string SavePickChanges(List<BracketPickChange> changes, DbCommand cmd)
        {
            List<BracketPickChange> insert_changes = changes.Where(c => c.Add).ToList();
            List<BracketPickChange> delete_changes = changes.Where(c => !c.Add).ToList();
            StringBuilder sb = new StringBuilder();
            if (delete_changes.Count > 0)
            {
                sb.Append(MakeDeletePickChangesString(delete_changes, cmd));
            }
            if (insert_changes.Count > 0)
            {
                sb.Append(MakeInsertPickChangesString(insert_changes, cmd));
            }

            return sb.ToString();
        }

        internal static string GetBracketSummaryForUser =
            @"
        SELECT b.bracketID, b.creationTime, b.completionTime, t.name AS tournamentName, 
            tournamentID, subMaxRound.maxRound AS maxRound, c.competitorName AS champName,
            br.bracketMax, br.pointsEarned, br._fk_bracket
		FROM user_bracket b
        JOIN tournament t ON t.tournamentID=b._fk_tournament
        JOIN (
			SELECT r._fk_tournamentType, MAX(r.round) as maxRound
            FROM tournament_round r
            GROUP BY r._fk_tournamentType
        ) subMaxRound ON subMaxRound._fk_tournamentType=t._fk_type
        JOIN tournament_game champ_game ON champ_game._fk_tournament=t.tournamentID AND champ_game._fk_tournamentRound=subMaxRound.maxRound
        JOIN bracket_game_prediction bgp ON b.bracketID=bgp._fk_bracket AND bgp._fk_game=champ_game.gameID
        JOIN competitor_tournament c ON bgp._fk_competitor=c.competitorID AND c._fk_tournament=champ_game._fk_tournament
        JOIN bracket_result br ON br._fk_bracket=b.bracketID
		WHERE b._fk_user=@userId
        ORDER BY b.completionTime DESC;
            ";

        internal static string CalculateBracketPerformance =
            @"
        SELECT b.bracketID, SUM(IF(bgp._fk_competitor=g._fk_competitor_winner, r.rewardPoints, 0)) AS 'pointsEarned', SUM(r.rewardPoints) AS 'trueMax', 
        SUM(IF(team_loss._fk_competitor IS NULL, 0, r.rewardPoints)) AS 'potentialLost'
        FROM user_bracket b
        JOIN tournament t ON t.tournamentID=b._fk_tournament
        JOIN tournament_game g ON t.tournamentID=g._fk_tournament
        JOIN bracket_game_prediction bgp ON b.bracketID=bgp._fk_bracket AND g.gameID=bgp._fk_game
        JOIN tournament_round r ON t._fk_type=r._fk_tournamentType AND r.round=g._fk_tournamentRound
		LEFT OUTER JOIN (
            #todo: i want to add constraints to this - as of now, this subquery will go across all tournaments all time. not just ones that user is in. that seems bad. 
			SELECT g.gameID, g._fk_tournamentRound, gp._fk_competitor, g._fk_tournament FROM tournament_game g
			JOIN game_participant gp ON gp._fk_game=g.gameID AND g._fk_competitor_winner<>gp._fk_competitor
			WHERE g._fk_competitor_winner IS NOT NULL
        ) team_loss ON g._fk_tournament=team_loss._fk_tournament AND g._fk_tournamentRound >= team_loss._fk_tournamentRound AND bgp._fk_competitor=team_loss._fk_competitor
        WHERE b.completed IS TRUE AND b.bracketID=@bracketID
	    GROUP BY b.bracketID;
            ";

        internal static string UpsertBracketPerformance =
            @"
                INSERT IGNORE INTO bracket_result(_fk_bracket, bracketMax, pointsEarned)
                VALUES(@bracketID, @bracketMax, @pointsEarned)
                ON DUPLICATE KEY UPDATE bracketMax=@bracketMax AND pointsEarned=@pointsEarned;
            ";


        private static string KenPomColumns = @"kpm.offensiveEfficiency, kpm.defensiveEfficiency, kpm.overallEfficiency, kpm.tempo, IF(kpm.overallEfficiency IS NULL, FALSE, TRUE) AS 'hasKenPom'";
        private static string CompetitorColumns = @"c.competitorName, c.competitorID, c._fk_division, c._fk_seed, c._fk_tournament";

        internal static string KenPomDataForTournamentCompetitor()
        {
            return
            $@"SELECT {KenPomColumns}
            FROM ken_pom_data kpm WHERE kpm._fk_tournament=@tournamentID AND kpm._fk_competitor=@competitorID;";
        }

        internal static string KenPomDataForTournament()
        {
            return string.Format(@"SELECT {0}, {1}
            FROM competitor_tournament c
            LEFT OUTER JOIN ken_pom_data kpm ON c._fk_tournament=kpm._fk_tournament AND c.competitorID=kpm._fk_competitor
            WHERE c._fk_tournament=@tournamentID;", KenPomColumns, CompetitorColumns);
        }


        internal static string InsertKenPomDataCompetitor =
            @"
        DELETE FROM ken_pom_data 
        WHERE _fk_tournament=@tournamentID AND _fk_competitor=@competitorID;
        INSERT INTO ken_pom_data(_fk_competitor, _fk_tournament, offensiveefficiency, defensiveefficiency, overallefficiency, tempo)
        VALUES(@competitorID, @tournamentID, @offensiveEfficiency, @defensiveEfficiency, @overallEfficiency, @tempo);
        ";

        private static string TotalBracketBase =
            @"SELECT COUNT(bracketID) FROM user_bracket b WHERE b.completed IS TRUE AND _fk_tournament=@tournamentID {0};";

        internal static string BracketCountForUserTournament()
        {
            return string.Format(TotalBracketBase, " AND b._fk_user=@userID");
        }

        internal static string BracketCountTournament()
        {
            return string.Format(TotalBracketBase, string.Empty);
        }


        private static string ExposureSummaryBase = @"
        SELECT {0}, g._fk_tournamentRound, COUNT(bgp._fk_game) AS 'appearances' FROM tournament t
        JOIN competitor_tournament c ON c._fk_tournament=t.tournamentID
        LEFT OUTER JOIN bracket_game_prediction bgp ON bgp._fk_tournament=c._fk_tournament AND c.competitorID=bgp._fk_competitor
        LEFT OUTER JOIN user_bracket b ON b.bracketID=bgp._fk_bracket AND b.completed IS TRUE
        LEFT OUTER JOIN tournament_game g ON g.gameID=bgp._fk_game AND g._fk_tournament=c._fk_tournament
        WHERE t.tournamentID=@tournamentID {1}
        GROUP BY c.competitorID, g._fk_tournamentRound 
        ORDER BY competitorName, _fk_tournamentRound;";

        internal static string ExposureSummaryForUser()
        {
            return string.Format(ExposureSummaryBase, CompetitorColumns, "AND b._fk_user=@userID");
        }

        internal static string ExposureSummaryForTournament()
        {
            return string.Format(ExposureSummaryBase, CompetitorColumns, string.Empty);
        }

        internal static string GetFullTournament =
            @"
        SELECT tournamentID, name, t.createdTime, creator, t._fk_type AS tournamentType, tournamentFinalized,
	        g._fk_division AS gameDivision, _fk_tournamentRound, gameID, _fk_leftGame, _fk_rightGame, g._fk_competitor_Winner,
	        c._fk_seed, c.competitorName, c.competitorID, c._fk_division AS competitorDivision
        FROM tournament t
        JOIN tournament_game g ON g._fk_tournament=t.tournamentID
        LEFT OUTER JOIN game_participant p ON p._fk_game=g.gameID AND g._fk_leftGame IS NULL AND g._fk_rightGame IS NULL
        LEFT OUTER JOIN competitor_tournament c ON c.competitorID=p._fk_competitor AND t.tournamentID=c._fk_tournament
        WHERE t.tournamentID=@tournamentID
        ORDER BY g._fk_tournamentRound, gameDivision, g.gameID;";

        internal static string CompetitorsInGame()
        {
            return string.Format(@"
            SELECT {0} FROM game_participant gp
            JOIN tournament_game g ON g.gameID = gp._fk_game
            JOIN competitor_tournament c ON c.competitorID = gp._fk_competitor AND g._fk_tournament = c._fk_tournament
            WHERE gp._fk_game = @gameID; ", CompetitorColumns);
        }

        internal static string RoundOfGame = 
            @"SELECT _fk_tournamentRound FROM tournament_game WHERE gameID=@gameID;";


        

        // for now, only doing winner. if doing game participant, then use commented out SQL probably.
        internal static string SaveGameOutcome =
            @"
        DELETE gp FROM game_participant gp
        JOIN tournament_game g ON g.gameID=gp._fk_game AND g._fk_tournament= @tournamentID
        WHERE gp._fk_competitor= @otherCompetitorID AND g._fk_tournamentRound > @roundOfGame;
        UPDATE tournament_game SET _fk_competitor_winner=@competitorID 
        WHERE _fk_tournament=@tournamentID AND gameID=@gameID;
        INSERT INTO game_participant(_fk_competitor, _fk_game)
        SELECT @competitorID, gameID
        FROM tournament_game
        WHERE _fk_tournament = @tournamentID AND @gameID IN(_fk_leftGame, _fk_rightGame);
        UPDATE tournament_game SET _fk_competitor_winner=NULL 
        WHERE _fk_tournament=@tournamentID AND _fk_tournamentRound > @roundOfGame AND _fk_competitor_winner=@otherCompetitorID;
        ";

        internal static string AllCompletedBracketRecordsForTournament = 
            @"
        SELECT _fk_user, _fk_tournament, BracketID, champTotalPoints, completed, creationTime, completionTime
        FROM user_bracket b
        WHERE b.completed IS TRUE AND b._fk_tournament=@tournamentID
        ORDER BY completionTime DESC;
            ";

        internal static string MakePaged(int pageNum, int pageSize, string baseCommand)
        {
            int offset = pageNum * pageSize;
            baseCommand = baseCommand.Replace(';', ' ');
            return string.Format(
                @"
                {0}
                LIMIT {1}
                OFFSET {2};
            ", baseCommand, pageSize, offset);
        }
    }
}
