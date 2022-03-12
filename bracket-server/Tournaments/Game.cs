namespace bracket_server.Tournaments
{
    public class Game
    {
        public string ID { get; set; } = string.Empty;
        public string? Division { get; set; }
        public int Round { get; set; } = 1;
        public Game? LeftGame { get; set; } = null;
        public Game? RightGame { get; set; } = null;

        private TournamentCompetitor? _competitor1 = null; 

        public TournamentCompetitor? Competitor1
        {
            get => _competitor1;
            set
            {
                _competitor1 = value;
                SwapCompetitorsToMakeBetterSeedNumber1();
            }
        }
        private TournamentCompetitor? _competitor2 = null;
        public TournamentCompetitor? Competitor2
        {
            get => _competitor2;
            set
            {
                _competitor2 = value;
                SwapCompetitorsToMakeBetterSeedNumber1();
            }
        }

        private void SwapCompetitorsToMakeBetterSeedNumber1()
        {
            if (_competitor1 == null || _competitor2 == null) return;
            if (_competitor2.Seed > _competitor1.Seed) return;
            // swap.
            var temp = _competitor2;
            _competitor2 = _competitor1;
            _competitor1 = temp;
        }
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
            if (game1 == null || game2 == null) throw new ArgumentException("null games when making parent games");
            string? division = game1.Division == game2.Division ? game1.Division : null;
            Game parent_game = new Game()
            {
                ID = Guid.NewGuid().ToString(),
                LeftGame = game1,
                RightGame = game2,
                Competitor1 = game1?.Winner,
                Competitor2 = game2?.Winner,
                Division= division,
                Round = game1.Round + 1 // this can't be null shut up
            };
            return parent_game;
        }

        internal void RandomWinner()
        {
            if (Competitor1 == null || Competitor2 == null) throw new ArgumentException("null competitor");
            TournamentCompetitor winner = Random.Shared.NextDouble() > 0.5 ? Competitor1 : Competitor2;
            Winner = winner;
        }

        internal bool HasCompetitors()
        {
            return Competitor1 != null && Competitor2 != null;
        }

        internal bool HasChildGames()
        {
            return LeftGame != null && RightGame != null;
        }

        public List<Game> Flatten()
        {
            List<Game> games = new List<Game>();
            if(HasChildGames())
            {
                if (LeftGame == null || RightGame == null) throw new ArgumentException("null games");
                games.AddRange(LeftGame.Flatten());
                games.AddRange(RightGame.Flatten());
            }
            games.Add(this);
            return games;
        }

        internal Game? GetImmediateChildGame(string gameID)
        {
            if(LeftGame != null && LeftGame.ID == gameID)
            {
                return LeftGame;
            } 
            if(RightGame != null && RightGame.ID == gameID)
            {
                return RightGame;
            }
            return null;
        } 

        internal Game? FindParentGame(string gameID)
        {
            Game? possible_child = GetImmediateChildGame(gameID);
            if(possible_child != null)
            {
                return this;
            }
            if(LeftGame != null)
            {
                possible_child = LeftGame.FindParentGame(gameID);
                if(possible_child != null)
                {
                    return possible_child;
                }
            }
            if(RightGame != null)
            {
                possible_child = RightGame.FindParentGame(gameID);
                if(possible_child != null)
                {
                    return possible_child;
                }
            }
            return null;
        }

        public bool IsLeft(string gameID)
        {
            return LeftGame != null && LeftGame.ID == gameID;
        }



    }
}
