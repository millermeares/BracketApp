using bracket_server.Brackets;
using bracket_server.Tournaments.Exposure;
using bracket_server.Tournaments.KenPom;
namespace bracket_server.Tournaments
{
    public interface ITournamentDAL
    {
        public Tournament CreateTournament(UserID user_id, string tournament_name); // eventually this could include more info like start/end, etc.
        public List<Tournament> AllTournaments();
        public List<TournamentCompetitor> TournamentCompetitorsForTournament(string tournamentID);
        public MillerAPI.DataAccess.IDataAccess GetDataAccess();
        public TournamentCompetitor CreateTournamentCompetitor(string tournamentID, NewTournamentCompetitor competitor);
        public bool DeleteCompetitor(TournamentCompetitor competitor);
        public bool DeleteTournament(string tournamentID);
        public List<SeedData> GetSeedDataForTournamentType(string tournamentType);
        public bool SaveSeedData(SeedData data);

        public bool FinalizeTournament(Tournament tournament);
        public Tournament GetTournamentTopLevelByID(string tournamentID);

        public string GetActiveBracketingTournamentID();
        public bool InsertActiveBracketingTournamentID(string tournamentID);

        public GenericID GetUnfinishedLatestBracketIDForTournamentForUser(UserID user_id, string tournamentID);

        public Bracket GetBracket(string bracketID);

        public Bracket GetLatestBracketForUser(UserID userID);

        public bool InsertBracket(NewBracket bracket);

        public bool FinishBracket(Bracket bracket);

        public bool SavePickChanges(List<BracketPickChange> pick_changes);
        public List<BracketSummary> BracketSummariesForUser(UserID userID);
        public bool SaveKenpomData(KenPomDataReference data);
        public KenPomData KenPomDataForCompetitor(string tournamentID, string competitor);
        public List<CompetitorKenPomData> AllCompetitorKenPomDataForTournament(string tournamentID);
        public Dictionary<string, KenPomData> KenPomDataForTournament(string tournamentID);
        public ExposureReport ExposureReportForUser(UserID userID, string tournamentID);
        public ExposureReport ExposureReportTournament(string tournamentID);
        public List<Round> GetRoundsForTournament(string tournamentID);
        public Tournament FullActiveTournament();
        public TournamentCompetitor? OtherCompetitorInGame(Pick p);
        public List<TournamentCompetitor> CompetitorsInGame(string gameID);
        public bool SaveOutcome(Pick p);
    }
}
