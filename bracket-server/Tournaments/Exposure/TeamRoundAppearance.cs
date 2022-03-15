namespace bracket_server.Tournaments.Exposure
{
    public class TeamRoundAppearance : IComparable<TeamRoundAppearance>
    {
        public int TournamentRound { get; set; }
        public string CompetitorName { get; set; }
        internal double PercentAppearances { get; set; }
        public string PercentString
        {
            get
            {
                return (PercentAppearances * 100).ToString("##.#");
            }
        }
        public TeamRoundAppearance(int round, string name, double percent_appearances)
        {
            TournamentRound = round;
            CompetitorName = name;
            PercentAppearances = percent_appearances;
        }

        public int CompareTo(TeamRoundAppearance? other)
        {
            if (other == null) throw new NullReferenceException("can't compare null");
            int first_comp = PercentAppearances.CompareTo(other.PercentAppearances);
            if(first_comp != 0) return -1 * first_comp; // sort top down
            return CompetitorName.CompareTo(other.CompetitorName);
        }
    }
}
