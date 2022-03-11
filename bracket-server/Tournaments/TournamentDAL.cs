using MillerAPI.DataAccess;
using System.Data.Common;

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
            List<Division> divisions = NCAADivisions(tournament.ID);

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

        public bool InsertDivisions(List<Division> divisions, DbTransaction trans, DbConnection conn)
        {
            try
            {
                foreach(Division division in divisions)
                {
                    InsertDivision(division, trans, conn);
                }
                return true;
            }catch(Exception ex)
            {
                RecordError(ex);
                return false;
            }
        }

        public void InsertDivision(Division division, DbTransaction trans, DbConnection conn)
        {

            using DbCommand cmd = GetCommand(TournamentDBString.InsertDivision, conn);
            division.DivisionParameters(cmd);
            int rows = cmd.ExecuteNonQuery();
            if (rows <= 0)
            {
                throw new Exception($"Unexpected number of rows {rows} when inserting tournament division");
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

        private Tournament TournamentTopLevelInfoFromReader(DbDataReader reader)
        {
            string name = GetString(reader, "name");
            string id = GetString(reader, "tournamentID");
            return new Tournament()
            {
                ID = id,
                Name = name
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

        private List<Division> NCAADivisions(string tournament)
        {
            return new List<Division>()
            {
                new Division("South", tournament), 
                new Division("East", tournament), 
                new Division("West", tournament), 
                new Division("Midwest", tournament)
            };
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
    }
}
