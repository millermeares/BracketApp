namespace bracket_server.Brackets
{
    public class NewBracket
    {
        public string TournamentID { get; set; } 
        public UserID Owner { get; set; }
        public string ID { get; set; }
        public NewBracket(string tournamentID, UserID owner)
        {
            ID = Guid.NewGuid().ToString();
            TournamentID = tournamentID;
            Owner = owner;
        }
        
    }
}
