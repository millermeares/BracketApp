namespace bracket_server.Tournaments
{
    public class Round : IComparable<Round>
    {
        public int OrderWithinTournament { get; set; }
        public string Name { get; set; }
        public Round(int order_with_tournament, string name)
        {
            OrderWithinTournament = order_with_tournament;
            Name = name;
        }

        public int CompareTo(Round? other)
        {
            if (other == null) throw new NullReferenceException("round can't be null when comparing");
            return OrderWithinTournament.CompareTo(other.OrderWithinTournament);
        }
    }
}
