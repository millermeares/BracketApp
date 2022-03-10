namespace bracket_server.Tournaments
{
    public interface ITournamentDAL
    {
        public Tournament CreateTournament(UserID user_id, string tournament_name); // eventually this could include more info like start/end, etc.
        public List<Tournament> AllTournaments();
        public List<Competitor> CompetitorsForTournament(string tournamentID);
        public MillerAPI.DataAccess.IDataAccess GetDataAccess();

        public bool DeleteTournament(string tournamentID); 
    }
}
