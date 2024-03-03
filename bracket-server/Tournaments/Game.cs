using bracket_server.Brackets;

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
        public bool Competitor1PredictionWrong { get; set; } = false;
        public bool Competitor2PredictionWrong { get; set; } = false;

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
            if (LeftGame != null || RightGame != null) return; // make this only happen for base games.
            if (_competitor2.Seed > _competitor1.Seed) return;

            // swap.
            var temp = _competitor2;
            _competitor2 = _competitor1;
            _competitor1 = temp;
        }
        public TournamentCompetitor? Winner { get; set; } = null;
        public TournamentCompetitor? PredictedWinner { get; set; } = null;
        public TournamentCompetitor? PredictedCompetitor1 { get; set; } = null;
        public TournamentCompetitor? PredictedCompetitor2 { get; set; } = null;
        

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


        internal bool HasCompetitor1()
        {
            return Competitor1 != null;
        }

        internal bool HasPredictedCompetitor1()
        {
            return PredictedCompetitor1 != null;
        }
        internal bool HasCompetitor2()
        {
            return Competitor2 != null;
        }
        internal bool HasPredictedCompetitor2()
        {
            return PredictedCompetitor2 != null;
        }
        internal bool HasCompetitors()
        {
            return Competitor1 != null && Competitor2 != null;
        }

        internal bool HasPredictedCompetitors()
        {
            return HasPredictedCompetitor1() && HasPredictedCompetitor2();
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

        // todo: there's a bug in this - maybe? 
        // if someone manually picks games up to champ game, some there can be empty games but this still return strue.
        public bool FullyPopulated()
        {
            // base game. 
            if(LeftGame == null && RightGame == null)
            {
                return Competitor1 != null && Competitor2 != null;
            }
            if(PredictedCompetitor1 == null || PredictedCompetitor2 == null)
            {
                return false;
            }
            bool left_game_populated = LeftGame == null ? true : LeftGame.FullyPopulated();
            bool right_game_populated = RightGame == null ? true : RightGame.FullyPopulated();
            return right_game_populated && left_game_populated;
        }

        public bool HasWinner()
        {
            return Winner != null;
        }

        public void SetWinnerFromCompetitors(string competitorID)
        {
            if (Competitor1 == null && Competitor2 == null) throw new ArgumentException("null competitors when settings winners");
            if(Competitor1 != null && competitorID == Competitor1.ID)
            {
                Winner = Competitor1;
            } else if(Competitor2 != null && competitorID == Competitor2.ID)
            {
                Winner = Competitor2;
            }
        }

        public bool HasCompetitor(string competitorID)
        {
            if (Competitor1 != null && Competitor1.ID == competitorID) return true;
            if (Competitor2 != null && Competitor2.ID == competitorID) return true;
            return false;
        }

        public bool HasPredictedCompetitor(string competitorID)
        {
            if (PredictedCompetitor1 != null && PredictedCompetitor1.ID == competitorID) return true;
            if (PredictedCompetitor2 != null && PredictedCompetitor2.ID == competitorID) return true;
            return false;
        }

        internal void ClearPredictionsFromNonBaseGames()
        {
            PredictedWinner = null;
            if(LeftGame == null || RightGame == null)
            {
                return;
            }
            PredictedCompetitor1 = null;
            PredictedCompetitor2 = null;
            LeftGame.ClearPredictionsFromNonBaseGames();
            RightGame.ClearPredictionsFromNonBaseGames();
        }

        internal bool HasPredictedWinner()
        {
            return PredictedWinner != null;
        }

        internal TournamentCompetitor GetLoser()
        {
            // this is only accessed when HasWinner is already checked. This will break if the results are not entered round-by-round.
            if (!HasWinner() || Competitor1 == null || Competitor2 == null) {

                throw new ArgumentException($"Asking for loser if no winner and competitors in game {ID} in {Division} division and {Round} round");
            }
                
            return Winner.ID == Competitor1.ID ? Competitor2 : Competitor1;
        }

        


    }
}
