namespace bracket_server.Tournaments
{
    public class Game
    {
        public string ID { get; set; } = string.Empty;
        public Game? LeftGame { get; set; } = null;
        public Game? RightGame { get; set; } = null;

        public TournamentCompetitor? Competitor1 { get; set; } = null;
        public TournamentCompetitor? Competitor2 { get; set; } = null;
        public TournamentCompetitor? Winner { get; set; } = null;
        
        
        

        internal void SkeletonChildrenGames(int iterations)
        {
            if (iterations < 0) return;
            LeftGame = BasicGame();
            RightGame = BasicGame();
            LeftGame.SkeletonChildrenGames(iterations-1);
            RightGame.SkeletonChildrenGames(iterations-1);
        }

        internal static Game BasicGame()
        {
            return new Game()
            {
                ID = Guid.NewGuid().ToString()
            };
        }

        internal static Game MakeParentGame(Game game1, Game game2)
        {
            Game parent_game = new Game()
            {
                ID = Guid.NewGuid().ToString(),
                LeftGame = game1,
                RightGame = game2,
                Competitor1 = game1?.Winner,
                Competitor2 = game2?.Winner
            };
            return parent_game;
        }

        internal void RandomWinner()
        {
            if (Competitor1 == null || Competitor2 == null) throw new ArgumentException("null competitor");
            TournamentCompetitor winner = Random.Shared.NextDouble() > 0.5 ? Competitor1 : Competitor2;
            Winner = winner;
        }



    }
}
