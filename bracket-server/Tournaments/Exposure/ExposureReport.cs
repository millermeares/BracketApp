namespace bracket_server.Tournaments.Exposure
{
    public class ExposureReport
    {
        public int TotalTournaments { get; protected set; }
        public string TournamentID { get; set; }
        public List<TeamExposureSummary> ExposureSummaries { get; protected set;} = new List<TeamExposureSummary>();
        public ExposureReport(int total_tournaments, string tournamentID)
        {
            TotalTournaments = total_tournaments;
            TournamentID = tournamentID;
        }

       
        public void AddExposureSummary(TeamExposureSummary summary)
        {
            ExposureSummaries.Add(summary);
        }

        public bool IsEmpty()
        {
            return TotalTournaments > 0;
        }

        public static ExposureReport MakeEmpty()
        {
            return new ExposureReport(0, string.Empty);
        }

        public List<RoundReport> ReportByRound(List<Round> rounds)
        {
            List<RoundReport> rounds_report = new List<RoundReport>();
            foreach(Round round in rounds)
            {
                rounds_report.Add(ReportForRound(round));
            }
            rounds_report.Sort();
            return rounds_report;
        }

        public List<RoundReport> ReportByRound(ITournamentDAL dal)
        {
            List<Round> rounds = dal.GetRoundsForTournament(TournamentID);
            return ReportByRound(rounds);
        }

        public RoundReport ReportForRound(Round round)
        {
            RoundReport report = new RoundReport(round);
            foreach(TeamExposureSummary summary in ExposureSummaries)
            {
                TeamRoundAppearance appearance = summary.AppearancesInRound(round.OrderWithinTournament, TotalTournaments);
                report.Appearances.Add(appearance, appearance);
            }
            return report;
        }

    }
}
