namespace bracket_server.Tournaments
{
    public class Tournament
    {
        public string ID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime EventStart { get; set; } = DateTime.MinValue;
        public DateTime EventEnd { get; set; } = DateTime.MaxValue;
        public Game ChampionshipGame { get; set; } = new Game();

    }
}
