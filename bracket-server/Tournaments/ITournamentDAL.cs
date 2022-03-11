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
    }
}
