namespace bracket_server.Tournaments
{
    public class Tournament
    {
        public string ID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime EventStart { get; set; } = DateTime.MinValue;
        public DateTime EventEnd { get; set; } = DateTime.MaxValue;
        public Game ChampionshipGame { get; set; } = new Game();

        public static Tournament New(string name)
        {
            return new Tournament()
            {
                ID = Guid.NewGuid().ToString(),
                Name = name
            };
        }

        public static Tournament MakeEmpty()
        {
            return new Tournament();
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(ID);
        }

    }
}
