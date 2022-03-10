namespace bracket_server.Tournaments
{
    public class Division
    {
        public string Tournament { get; set; }
        public string Name { get; set; } 
        public Division(string name, string tournament)
        {
            Name = name;
            Tournament = tournament;
        }
    }
}
