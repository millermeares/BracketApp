namespace bracket_server.Tournaments
{
    public class Round
    {
        public int OrderWithinTournament { get; set; }
        public string Name { get; set; }
        public Round(int order_with_tournament, string name)
        {
            OrderWithinTournament = order_with_tournament;
            Name = name;
        }
    }
}
