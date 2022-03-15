using bracket_server.Brackets;
using bracket_server.Tournaments.Exposure;
namespace bracket_server.Tournaments.SmartFill
{
    public class SmartFillEvaluation
    {

        private Dictionary<string, TeamExposureSummary> _teamExposureSummaries = new Dictionary<string, TeamExposureSummary>();
        private Tournament _tournament;
        public SmartFillEvaluation(Tournament tournament)
        {
            _tournament = tournament;
        }

        private SmartFillArgs GetSmartFillArgs(ITournamentDAL dal)
        {
            SmartFillArgs args = new SmartFillArgs(dal);
            args.FillAllDbArgs(_tournament);
            return args;
        }

        public ExposureReport DoSimulations(int count, ITournamentDAL dal)
        {
            SmartFillArgs args = GetSmartFillArgs(dal);
            InitializeExposureSummaries();
            DoSimulations(count, args);
            ExposureReport report = new ExposureReport(count, _tournament.ID);
            _teamExposureSummaries.Values.ToList().ForEach(s => report.AddExposureSummary(s));
            return report;
        }

        public void DoSimulations(int count, SmartFillArgs args) // return value tbd. probably an exposure summary. 
        {
            while(count > 0)
            {
                count--;
                DoSimulation(args);
            }
        }

        private void DoSimulation(SmartFillArgs args)
        {
            List<Pick> picks = _tournament.Smartfill(args);
            picks.ForEach(p => IncrementTeamExposureSummary(p));
            _tournament.ResetForNewSimulation();
        }

        private void IncrementTeamExposureSummary(Pick p)
        {
            _teamExposureSummaries[p.CompetitorID].IncrementAppearanceInRound(((SmartEvalPick)p).Round);
        }

        private void InitializeExposureSummaries()
        {
            List<TournamentCompetitor> competitors = _tournament.GetCompetitors();
            competitors.ForEach(c => _teamExposureSummaries.Add(c.ID, new TeamExposureSummary(c)));
        }

        
        
    }
}
