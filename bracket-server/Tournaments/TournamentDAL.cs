using bracket_server.Brackets;
using bracket_server.Tournaments.Exposure;
using bracket_server.Tournaments.KenPom;
using MillerAPI.DataAccess;
using System.Data.Common;
using static UserManagement.UserDataAccess.DBHelpers;
namespace bracket_server.Tournaments
{
    public class TournamentDAL : BaseDAL, ITournamentDAL
    {
        
        public TournamentDAL(IDataAccess access) : base(access)
        {

        }

        // for now, this is going to just create the ncaa tournament. it can be changed in the future if needed. but i'm hardcoding the rounds for now..

        public Tournament CreateTournament(UserID user_id, string tournament_name)
        {
            List<Round> rounds = NCAARounds();
            Tournament tournament = Tournament.New(tournament_name);

            return _dataAccess.DoTransaction((conn, trans) =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.CreateTournament, conn);
                tournament.TournamentParameters(cmd);
                cmd.AddParameter("@creator", user_id.ID);
                int rows = cmd.ExecuteNonQuery();
                if (rows <= 0) return Tournament.MakeEmpty();
                return tournament;
            });
        }

        public DBResult InsertRounds(List<Round> rounds, Tournament tournament, DbTransaction trans, DbConnection conn)
        {
            try
            {
                foreach (Round round in rounds)
                {
                    InsertRound(round, tournament, trans, conn);
                }
                return DBResult.Success;
            }
            catch (Exception ex)
            {
                _dataAccess.RecordError(ex);
                return DBResult.Fail;
            }

        }

        public void InsertRound(Round round, Tournament tournament, DbTransaction trans, DbConnection conn)
        {
            
            using DbCommand cmd = GetCommand(TournamentDBString.InsertRound, conn);
            round.RoundParameters(tournament.ID, cmd);
            int rows = cmd.ExecuteNonQuery();
            if(rows <= 0)
            {
                throw new Exception($"Unexpected number of rows {rows} when inserting tournament round");
            }
        }


        public List<Tournament> AllTournaments()
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.AllTournaments, conn);
                using DbDataReader reader = cmd.ExecuteReader();
                return ListFromReader(reader, TournamentTopLevelInfoFromReader);
            });
        }

        public Tournament GetTournamentTopLevelByID(string tournamentID)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.GetTournamentTopLevelByID, conn);
                cmd.AddParameter("@tournamentID", tournamentID);
                using DbDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return TournamentTopLevelInfoFromReader(reader);
                }
                return Tournament.MakeEmpty();
            });
        }

        private Tournament TournamentTopLevelInfoFromReader(DbDataReader reader)
        {
            string name = GetString(reader, "name");
            string id = GetString(reader, "tournamentID");
            return new Tournament()
            {
                ID = id,
                Name = name,
                Finalized = GetBool(reader, "tournamentFinalized"),
                CreatedTime = GetDatetime(reader, "createdTime"),
                TournamentType = GetString(reader, "tournamentType")
            };
        }

        public bool DeleteTournament(string tournamentID)
        {
            return _dataAccess.DoTransaction((conn, trans)=>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.DeleteTournament, conn, trans);
                cmd.AddParameter("@tournamentID", tournamentID);
                return cmd.ExecuteNonQuery() > 0;
            });
        }

        public List<TournamentCompetitor> TournamentCompetitorsForTournament(string tournamentID)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.GetCompetitorsForTournament, conn);
                cmd.AddParameter("@tournamentID", tournamentID);
                using DbDataReader reader = cmd.ExecuteReader();
                return ListFromReader(reader, TournamentCompetitorFromReader);
            });
        }

        private TournamentCompetitor TournamentCompetitorFromReader(DbDataReader reader)
        {
            string name = GetString(reader, "competitorName");
            string id = GetString(reader, "competitorID");
            string division = GetString(reader, "_fk_division");
            int seed= GetInt(reader, "_fk_seed");
            string tournament = GetString(reader, "_fk_tournament");
            return new TournamentCompetitor() 
            { 
                ID=id,
                Name=name, 
                Division =division,
                Seed=seed,
                TournamentID=tournament
            };
        }

        public List<SeedData> GetSeedDataForTournamentType(string tournamentType)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.GetSeedDataForTournament, conn);
                cmd.AddParameter("@tournamentType", tournamentType);
                using DbDataReader reader = cmd.ExecuteReader();
                return ListFromReader(reader, SeedDataFromReader);
            });
        }

        private SeedData SeedDataFromReader(DbDataReader reader)
        {
            return new SeedData()
            {
                SeedID = GetInt(reader, "seedID"),
                FinalFourOdds = GetDouble(reader, "finalFourOdds"),
                EliteEightOdds = GetDouble(reader,"eliteEightOdds"),
                TournamentType = GetString(reader, "_fk_tournamentType")
            };
        }



        public override string GetExceptionCategory()
        {
            return "tournament";
        }

        

        private List<Round> NCAARounds()
        {
            return new List<Round>()
            { 
                new Round(1, "Seed Round"), 
                new Round(2, "Round of 32"), 
                new Round(3, "Sweet 16"),
                new Round(4, "Elite 8"), 
                new Round(5, "Final 4"),
                new Round(6, "Championship")
            };
        }

        public TournamentCompetitor CreateTournamentCompetitor(string tournamentID, NewTournamentCompetitor competitor)
        {
            TournamentCompetitor tournament_competitor = Tournaments.TournamentCompetitor.MakeNew(competitor, tournamentID);
            return _dataAccess.DoTransaction((conn, trans) =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.InsertTournamentCompetitor, conn);
                tournament_competitor.TournamentCompetitorParams(cmd);
                int rows = cmd.ExecuteNonQuery();
                return tournament_competitor;
            });
        }

        public bool DeleteCompetitor(TournamentCompetitor competitor)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.DeleteTournamentCompetitor, conn);
                competitor.TournamentCompetitorParams(cmd);
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            });
        }

        public bool SaveSeedData(SeedData data)
        {
            return _dataAccess.DoTransaction((conn, trans) =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.UpdateSeedData, conn, trans);
                data.SeedDataParams(cmd);
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            });
        }

        public bool FinalizeTournament(Tournament tournament)
        {
            return _dataAccess.DoTransaction((conn, trans) =>
            {
                using DbCommand cmd = GetCommand("", conn);
                cmd.CommandText = TournamentDBString.FinalizeTournament(tournament, cmd);
                tournament.TournamentParameters(cmd);
                int rows = cmd.ExecuteNonQuery();
                // total games + total competitors in games + one row to finalize the tournament.
                int expected_rows = 63 + 64 + 1;
                if(rows != expected_rows)
                {
                    throw new Exception($"Unexpected number of affected rows when finalizing tournament {expected_rows}");
                }
                return true;
            });
        }

        public string GetStringParamValue(string key)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.GetStringParam, conn);
                cmd.AddParameter("@key", key);
                return ScalarStringFromCommand(cmd);
            });
        }

        public string GetActiveBracketingTournamentID()
        {
            return GetStringParamValue("activetournament");
        }

        public bool InsertStringParamValue(string key, string value)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.InsertStringParam, conn);
                cmd.AddParameter("@key", key);
                cmd.AddParameter("@value", value);
                return cmd.ExecuteNonQuery() > 0;
            });
        }

        public bool InsertActiveBracketingTournamentID(string tournamentID)
        {
            return InsertStringParamValue("activetournament", tournamentID);
        }

        public bool InsertBracket(NewBracket bracket)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.InsertNewBracket, conn);
                bracket.NewBracketParameters(cmd);
                return cmd.ExecuteNonQuery() > 0;
            });
        }

        public Bracket GetBracket(string bracketID)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.GetBracketByID(), conn);
                cmd.AddParameter("@bracketID", bracketID);
                using DbDataReader reader = cmd.ExecuteReader();
                return SingleBracketFromReader(reader);
            });
        }

        // call DbDataReader.read before this.
        private Bracket BracketHighLevelInfoFromReader(DbDataReader reader)
        {
            Bracket bracket = Bracket.MakeEmpty();
            bracket.ID= GetString(reader, "bracketID");
            bracket.ChampTotalPoints = GetInt(reader, "champTotalPoints");
            bracket.Completed = GetBool(reader, "completed");
            bracket.CreationTime = GetDatetime(reader, "creationTime");
            bracket.Owner = new UserID(GetString(reader, "userID"));
            return bracket;
        }

        private Game GameFromReader(DbDataReader reader, ref Dictionary<string, Game> gameDict)
        {
            //id, division, round, leftgame, rightgame, competitor1, competitor2, winner
            string gameID = GetString(reader, "gameID");
            string leftGameID = GetString(reader, "_fk_leftGame");
            string rightGameID = GetString(reader, "_fk_rightGame");
            string gameDivision = GetString(reader, "gameDivision");
            TournamentCompetitor? competitor_1 = GameCompetitorFromReader(reader);
            int round = GetInt(reader, "_fk_tournamentRound");
            
            Func<TournamentCompetitor?> getTournamentCompetitor2 = () =>
            {
                if (competitor_1 == null) return null;
                reader.Read(); // i can do this because this is the only part of this query that reads multiple lines.
                return GameCompetitorFromReader(reader);
            };
            string winner_id = GetString(reader, "_fk_competitor_Winner");
            
            Game? left_game = string.IsNullOrEmpty(leftGameID) ? null : gameDict[leftGameID];
            Game? right_game = string.IsNullOrEmpty(rightGameID) ? null : gameDict[rightGameID];
            TournamentCompetitor? competitor_2 = getTournamentCompetitor2();
            competitor_1 = GetCompetitorFromGameWinnerIfNull(competitor_1, left_game);
            competitor_2 = GetCompetitorFromGameWinnerIfNull(competitor_2, right_game);
            Game game = new Game()
            {
                ID = gameID,
                
                LeftGame = left_game,
                RightGame = right_game,
                Competitor1 = competitor_1,
                Competitor2 = competitor_2,
                Division = gameDivision,
                Round = round,
                Winner = GetWinner(winner_id, competitor_1, competitor_2)
            };
            gameDict.Add(gameID, game);
            return game;
        }

        private TournamentCompetitor? GetCompetitorFromGameWinnerIfNull(TournamentCompetitor? competitor, Game? origin_game)
        {
            if (competitor != null) return competitor;
            if (origin_game == null) return null; // i don't think this should happen.
            return origin_game.Winner;
        }

        private static TournamentCompetitor? GetWinner(string winner_id, TournamentCompetitor? comp1, TournamentCompetitor? comp2)
        {
            if (string.IsNullOrEmpty(winner_id)) return null;
            if(comp1 == null && comp2 == null)
            {
                throw new ArgumentException("both null when winner is not");
            }
            return comp1 != null && comp1.ID == winner_id ? comp1 : comp2;
        }

        private TournamentCompetitor? GameCompetitorFromReader(DbDataReader reader)
        {
            string competitorID = GetString(reader, "competitorID");
            if (string.IsNullOrEmpty(competitorID)) return null;

            int seed = GetInt(reader, "_fk_seed");
            string competitorName = GetString(reader, "competitorName");
            string division = GetString(reader, "competitorDivision");
            return new TournamentCompetitor()
            {
                Seed=seed,
                Division=division,
                ID=competitorID,
                Name=competitorName
            };
        }

        private BracketPick? PickFromReader(DbDataReader reader)
        {
            string competitorID = GetString(reader, "winnerPick");
            if (string.IsNullOrEmpty(competitorID))
            {
                return null;
            }
            string tournamentID = GetString(reader, "tournamentID");
            string bracketID = GetString(reader, "bracketID");
            string gameID = GetString(reader, "gamePick");
            return new BracketPick(bracketID, tournamentID, gameID, competitorID);
        }

        private Bracket SingleBracketFromReader(DbDataReader reader)
        {
            List<BracketPick> picks = new List<BracketPick>();
            Bracket bracket = Bracket.MakeEmpty();
            Dictionary<string, Game> gameDict = new Dictionary<string, Game>();
            while(reader.Read())
            {
                if (bracket.IsEmpty()) 
                {
                    bracket = BracketHighLevelInfoFromReader(reader);
                    bracket.Tournament = TournamentTopLevelInfoFromReader(reader);
                }
                bracket.Tournament.SetChampionshipGame(GameFromReader(reader, ref gameDict));
                BracketPick? p = PickFromReader(reader);
                if(p != null)
                {
                    picks.Add(p);
                }
            }
            bracket.Tournament.ApplyPicks(picks);
            return bracket;
        }

        public GenericID GetUnfinishedLatestBracketIDForTournamentForUser(UserID user_id, string tournamentID)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.GetLatestUnfinishedBracketForTournamentForUser, conn);
                cmd.AddParameter("@userID", user_id.ID);
                cmd.AddParameter("@tournamentID", tournamentID);
                string bracket_id = ScalarStringFromCommand(cmd);
                return string.IsNullOrEmpty(bracket_id) ? GenericID.MakeEmpty() : new GenericID(bracket_id);
            });
        }

        public Tournament FullActiveTournament()
        {
            string active_tournament = GetActiveBracketingTournamentID();
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.GetFullTournament, conn);
                cmd.AddParameter("@tournamentID", active_tournament);
                using DbDataReader reader = cmd.ExecuteReader();
                return FullTournamentFromReader(reader);
            });
        }

        private Tournament FullTournamentFromReader(DbDataReader reader)
        {
            Dictionary<string, Game> gameDict = new Dictionary<string, Game>();
            Tournament tournament = Tournament.MakeEmpty();
            while(reader.Read())
            {
                if(tournament.IsEmpty())
                {
                    tournament = TournamentTopLevelInfoFromReader(reader);
                }
                tournament.SetChampionshipGame(GameFromReader(reader, ref gameDict));
            }
            if(tournament.IsEmpty())
            {
                throw new Exception("tournament should not be empty here. probably a query issue.");
            }
            return tournament;
        }

        public Bracket GetLatestBracketForUser(UserID userID)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.LatestBracketForUser(), conn);
                cmd.AddParameter("@userID", userID.ID);
                using DbDataReader reader = cmd.ExecuteReader();
                return SingleBracketFromReader(reader);
            });
        }

        public bool FinishBracket(Bracket bracket)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.CompleteBracket, conn);
                bracket.BracketParameters(cmd);
                return cmd.ExecuteNonQuery() > 0;
            });
        }

        public bool SavePickChanges(List<BracketPickChange> pick_changes)
        {
            return _dataAccess.DoTransaction((conn, trans) =>
            {
                using DbCommand cmd = GetCommand("", conn, trans);
                cmd.CommandText = TournamentDBString.SavePickChanges(pick_changes, cmd);
                int rows = cmd.ExecuteNonQuery();
                if(rows != pick_changes.Count)
                {
                    throw new Exception($"Unexpected amount of affected rows {rows} when saving picks");
                }
                return true;
            });
        }

        // This query is currently very high latency. Probably need to pre-calculate the score information for each bracket in order to efficiently show them.
        // Or... only retrieve the scores for a specific small subset of brackets? Possibly? Thnking like 50.
        public List<BracketPerformanceSummary> BracketPerformanceSummariesForUser(UserID userID)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.GetBracketPerformanceSummaryForUser, conn);
                cmd.AddParameter("@userID", userID.ID);
                using DbDataReader reader = cmd.ExecuteReader();
                return ListFromReader(reader, BracketPerformanceSummaryFromReader);
            });
        }

        public List<BracketSummary> BracketSummariesForUser(UserID userID)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.GetBracketSummaryForUser, conn);
                cmd.AddParameter("@userID", userID.ID);
                using DbDataReader reader = cmd.ExecuteReader();
                return ListFromReader(reader, BracketSummaryFromReader);
            });
        }

        private BracketPerformanceSummary BracketPerformanceSummaryFromReader(DbDataReader reader)
        {
            string bracketID = GetString(reader, "bracketID");
            DateTime creationTime = GetDatetime(reader, "creationTime");
            DateTime completionTime = GetDatetime(reader, "completionTime");
            string tournamentName = GetString(reader, "tournamentName");
            string competitorName = GetString(reader, "champName");
            int currentPoints = GetInt(reader, "pointsEarned");
            int maxPoints = GetInt(reader, "bracketMax");
            string tournamentID = GetString(reader, "tournamentID");
            return new BracketPerformanceSummary(bracketID, tournamentName, tournamentID, completionTime, creationTime, competitorName, maxPoints, currentPoints);
        }

        private BracketSummary BracketSummaryFromReader(DbDataReader reader)
        {
            string bracketID = GetString(reader, "bracketID");
            DateTime creationTime = GetDatetime(reader, "creationTime");
            DateTime completionTime = GetDatetime(reader, "completionTime");
            string tournamentName = GetString(reader, "tournamentName");
            string competitorName = GetString(reader, "champName");
            string tournamentID = GetString(reader, "tournamentID");
            return new BracketSummary(bracketID, tournamentName, tournamentID, completionTime, creationTime, competitorName);
        }

        public List<CompetitorKenPomData> AllCompetitorKenPomDataForTournament(string tournamentID)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.KenPomDataForTournament(), conn);
                cmd.AddParameter("@tournamentID", tournamentID);
                using DbDataReader reader = cmd.ExecuteReader();
                return ListFromReader(reader, CompetitorKenPomDataFromReader);
            }); 
        }

        public Dictionary<string, KenPomData> KenPomDataForTournament(string tournamentID)
        {
            List<CompetitorKenPomData> data = AllCompetitorKenPomDataForTournament(tournamentID);
            return data.ToDictionary(d => d.Competitor.ID, d => d.Data);
        }

        public KenPomData KenPomDataForCompetitor(string tournamentID, string competitor)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.KenPomDataForTournamentCompetitor(), conn);
                cmd.AddParameter("@competitorID", competitor);
                cmd.AddParameter("@tournamentID", tournamentID);
                using DbDataReader reader = cmd.ExecuteReader();
                return KenPomDataFromReader(reader);
            });
        }

        public bool SaveKenpomData(KenPomDataReference data)
        {
            return _dataAccess.DoTransaction((conn, trans) =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.InsertKenPomDataCompetitor, conn, trans);
                data.KenPomParameter(cmd);
                return cmd.ExecuteNonQuery() > 0;
            });
        }

        private CompetitorKenPomData CompetitorKenPomDataFromReader(DbDataReader reader)
        {
            TournamentCompetitor competitor = TournamentCompetitorFromReader(reader);
            KenPomData data = KenPomDataFromReader(reader);
            return new CompetitorKenPomData(competitor, data);
        }

        private KenPomData KenPomDataFromReader(DbDataReader reader)
        {
            bool hasKenPom = GetBool(reader, "hasKenPom");
            if(!hasKenPom)
            {
                return new KenPomData(0, 0, 0, 0);
            }
            double efficiency = GetDouble(reader, "overallEfficiency", double.MinValue);
            double offensive_efficiency = GetDouble(reader, "offensiveEfficiency", double.MinValue);
            double defensive_efficiency = GetDouble(reader, "defensiveEfficiency", double.MinValue);
            double tempo = GetDouble(reader, "tempo", double.MinValue);
            return new KenPomData(offensive_efficiency, defensive_efficiency, tempo, efficiency);
        }

        public ExposureReport ExposureReportForUser(UserID userID, string tournamentID)
        {
            return _dataAccess.DoQuery(conn =>
            {
                ExposureReport? report = null;
                using(DbCommand cmd = GetCommand(TournamentDBString.BracketCountForUserTournament(), conn))
                {
                    cmd.AddParameter("@tournamentID", tournamentID);
                    cmd.AddParameter("@userID", userID.ID);
                    int count = NonNullScalarIntFromCommand(cmd);
                    report = new ExposureReport(count, tournamentID);
                }
                if (report == null) throw new NullReferenceException("report should not be null here");
                using (DbCommand exposure_cmd = GetCommand(TournamentDBString.ExposureSummaryForUser(), conn))
                {
                    exposure_cmd.AddParameter("@tournamentID", tournamentID);
                    exposure_cmd.AddParameter("@userID", userID.ID);
                    using DbDataReader reader = exposure_cmd.ExecuteReader();
                    FillExposureReportFromReader(reader, report);
                    return report;
                }
            });
        }

        public ExposureReport ExposureReportTournament(string tournamentID)
        {
            return _dataAccess.DoQuery(conn =>
            {
                ExposureReport? report = null;
                using (DbCommand count_cmd = GetCommand(TournamentDBString.BracketCountTournament(), conn))
                {
                    count_cmd.AddParameter("@tournamentID", tournamentID);
                    int count = NonNullScalarIntFromCommand(count_cmd);
                    report = new ExposureReport(count, tournamentID);
                }
                if (report == null) throw new NullReferenceException("report should not be null here");

                using (DbCommand cmd = GetCommand(TournamentDBString.ExposureSummaryForTournament(), conn))
                {
                    cmd.AddParameter("@tournamentID", tournamentID);
                    using DbDataReader reader = cmd.ExecuteReader();
                    FillExposureReportFromReader(reader, report);
                    return report;
                }
            });
        }

        private void FillExposureReportFromReader(DbDataReader reader, ExposureReport report)
        {
            TeamExposureSummary summary = TeamExposureSummary.MakeEmpty();
            while (reader.Read())
            {
                string competitor_id = GetString(reader, "competitorID");
                if(summary.IsEmpty() || summary.Competitor.ID != competitor_id)
                {
                    TournamentCompetitor competitor = TournamentCompetitorFromReader(reader);
                    summary = new TeamExposureSummary(competitor);
                    report.AddExposureSummary(summary);
                }
                // now read each line in. 
                int appearances = GetInt(reader, "appearances", 0);
                int round = GetInt(reader, "_fk_tournamentRound");
                summary.SetAppearancesInRound(round, appearances);
            }
        }
        public List<Round> GetRoundsForTournament(string tournamentID)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.RoundsForTournament, conn);
                cmd.AddParameter("@tournamentID", tournamentID);
                using DbDataReader reader = cmd.ExecuteReader();
                return ListFromReader(reader, RoundFromReader);
            });
        }

        private Round RoundFromReader(DbDataReader reader)
        {
            int round = GetInt(reader, "round");
            string name = GetString(reader, "name");
            return new Round(round, name);
        }

        

        public TournamentCompetitor? OtherCompetitorInGame(Pick p)
        {
            List<TournamentCompetitor> competitors = CompetitorsInGame(p.GameID);
            if(!competitors.Any(c => c.ID != p.CompetitorID))
            {
                return null;
            }
            return competitors.Find(c => c.ID != p.CompetitorID);
        }

        public List<TournamentCompetitor> CompetitorsInGame(string gameID)
        {
            List<TournamentCompetitor> competitors = new List<TournamentCompetitor>();
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.CompetitorsInGame(), conn);
                cmd.AddParameter("@gameID", gameID);
                using DbDataReader reader = cmd.ExecuteReader();
                return ListFromReader(reader, TournamentCompetitorFromReader);
            });
        }

        public bool SaveOutcome(Pick p)
        {
            TournamentCompetitor? other_competitor = OtherCompetitorInGame(p);
            int round_of_game = RoundOfGame(p.GameID);
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.SaveGameOutcome, conn);
                p.PickParameters(cmd);
                cmd.AddParameter("@otherCompetitorID", other_competitor == null ? null : other_competitor.ID);
                cmd.AddParameter("@roundOfGame", round_of_game);
                cmd.ExecuteNonQuery();
                return true;
            });
        }

        public int RoundOfGame(string gameID)
        {
            return _dataAccess.DoQuery(conn =>
            {
                using DbCommand cmd = GetCommand(TournamentDBString.RoundOfGame, conn);
                cmd.AddParameter("@gameID", gameID);
                return ScalarIntFromCommand(cmd);
            });
        } 

    }
}
