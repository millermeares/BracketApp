namespace bracket_server.Tournaments.Exposure
{
    public class TeamExposureSummary
    {
        public TournamentCompetitor Competitor { get; set; }
        private Dictionary<int, int> _appearancesPerRound { get; set; } = new Dictionary<int, int>();
        public TeamExposureSummary(TournamentCompetitor competitor)
        {
            Competitor = competitor;
        }

        public void AddAppearancesInRound(int round, int appearances)
        { 
            //todo: no presence in round = 0. make sure that's implemented.
            if(_appearancesPerRound.ContainsKey(round))
            {
                int x = 0;
            }
            _appearancesPerRound.Add(round, appearances);
        }

        public static TeamExposureSummary MakeEmpty()
        {
            return new TeamExposureSummary(TournamentCompetitor.MakeEmpty());
        }
        public bool IsEmpty()
        {
            return Competitor.IsEmpty();
        }

        internal int GetAppearancesInRound(int round)
        {
            if(_appearancesPerRound.ContainsKey(round)) return _appearancesPerRound[round];
            return 0;
        }

        internal TeamRoundAppearance AppearancesInRound(int round, int total_tournaments)
        {
            int appearances = GetAppearancesInRound(round);
            double appearance_pct = appearances == 0 ? 0 : (double)appearances / total_tournaments;
            return new TeamRoundAppearance(round, Competitor.Name, appearance_pct);
        }

        public List<TeamRoundAppearance> GetAppearancesByRound(int minRound, int maxRound, int total_tournaments)
        {
            List<TeamRoundAppearance> appearances = new List<TeamRoundAppearance>();
            if (minRound > maxRound) throw new ArgumentException("these arguments make no sense");
            for(int i = minRound; i <= maxRound; i++)
            {
                appearances.Add(AppearancesInRound(i, total_tournaments));
            }
            return appearances;
        }
    }
}
