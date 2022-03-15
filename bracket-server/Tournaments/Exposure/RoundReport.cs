namespace bracket_server.Tournaments.Exposure
{
    public class RoundReport : IComparable<RoundReport>
    {
        public Round Round { get; set; }
        public RoundReport(Round round)
        {
            Round = round;
        }

        internal SortedList<TeamRoundAppearance, TeamRoundAppearance> Appearances { get; set; } = new SortedList<TeamRoundAppearance, TeamRoundAppearance>();

        public List<TeamRoundAppearance> TeamAppearances => Appearances.Values.ToList();

        public int CompareTo(RoundReport? other)
        {
            if (other == null) throw new NullReferenceException("round report can't be null when comparing");
            return Round.CompareTo(other.Round);
        }
    }
}
