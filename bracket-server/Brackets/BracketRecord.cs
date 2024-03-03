using bracket_server.Tournaments;
using MySqlX.XDevAPI.CRUD;

namespace bracket_server.Brackets
{
    public class BracketRecord
    {
        public string TournamentID { get; set; }
        public string ID { get; set; }
        internal UserID Owner { get; set; }
        public bool Completed { get; set; } = false;
        public int ChampTotalPoints { get; set; }
        public DateTime CompletionTime { get; set; }
        public DateTime CreationTime { get; set; }
        public BracketRecord(string tournament, string bracketID, UserID owner, bool completed, int champTotalPoints, DateTime creationTime, DateTime completionTime)
        {
            TournamentID = tournament;
            ID = bracketID;
            Owner = owner;
            Completed = completed;
            ChampTotalPoints = champTotalPoints;
            CreationTime = creationTime;
            CompletionTime = completionTime;
        }
    }
}
