using bracket_server.Brackets;

namespace bracket_server.Tournaments
{
    public class Tournament
    {
        public string ID { get; set; } = string.Empty;
        public bool Finalized { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime EventStart { get; set; } = DateTime.MinValue;
        public DateTime EventEnd { get; set; } = DateTime.MaxValue;
        public Game ChampionshipGame { get; protected set; } = new Game();

        private Dictionary<string, Game> _gameDictionary = new Dictionary<string, Game>();
        private Dictionary<string, TournamentCompetitor> _competitorDictionary = new Dictionary<string,TournamentCompetitor>();
        public static Tournament New(string name)
        {
            return new Tournament()
            {
                ID = Guid.NewGuid().ToString(),
                Name = name
            };
        }

        public void SetChampionshipGame(Game game)
        {
            ChampionshipGame = game;
            _gameDictionary.Add(game.ID, game);
            AddCompetitorsToDictIfThere(game);

        }

        private void AddCompetitorsToDictIfThere(Game game)
        {
            if(game.Competitor1 != null && !_competitorDictionary.ContainsKey(game.Competitor1.ID))
            {
                _competitorDictionary.Add(game.Competitor1.ID, game.Competitor1);
            }
            if(game.Competitor2 != null && !_competitorDictionary.ContainsKey(game.Competitor2.ID))
            {
                _competitorDictionary.Add(game.Competitor2.ID, game.Competitor2);
            }
        }

        public static Tournament MakeEmpty()
        {
            return new Tournament();
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(ID);
        }


        private void ApplyPick(Pick p)
        {
            if(p.GameID == ChampionshipGame.ID)
            {
                ChampionshipGame.Winner = _competitorDictionary[p.CompetitorID];
                return;
            }
            Game? parent_game = ChampionshipGame.FindParentGame(p.GameID);
            if(parent_game == null) throw new Exception($"could not find parent game for {p.GameID}.");
            Game? game = parent_game.GetImmediateChildGame(p.GameID);
            if (game == null) throw new Exception("could not find game in immediate children but it should really be there.");
            SetGameWinnerFromDict(game, p);
            if(parent_game.IsLeft(p.GameID))
            {
                parent_game.Competitor1 = _competitorDictionary[p.CompetitorID];
            } else
            {
                // right
                parent_game.Competitor2 = _competitorDictionary[p.CompetitorID];
            }
        }

        private void SetGameWinnerFromDict(Game game, Pick p)
        {
            game.Winner = _competitorDictionary[p.CompetitorID];
        }

        public void ApplyPicks(List<Pick> picks)
        {
            foreach(Pick p in picks)
            {
                ApplyPick(p);
            }
        }

        internal static Game ChampionshipSkeletonFromBase(List<Game> base_games)
        {
            if (base_games.Count == 1) return base_games.First();
            if (base_games.Count % 2 != 0) throw new ArgumentException("must have even number of games");
            List<Game> next_round_games = new List<Game>();
            int first_game_index = 0;
            while (first_game_index < base_games.Count)
            {
                Game game1 = base_games[first_game_index];
                Game game2 = base_games[first_game_index + 1];
                next_round_games.Add(Game.MakeParentGame(game1, game2));
                first_game_index += 2;
            }
            return ChampionshipSkeletonFromBase(next_round_games);
        }

        internal void FillOutTournament(List<TournamentCompetitor> competitors)
        {
            List<Game> base_games = MakeBaseGames(competitors);
            Game championship_game = Tournament.ChampionshipSkeletonFromBase(base_games);
            ChampionshipGame = championship_game;
            Finalized = true;
        }


        private static List<Game> MakeBaseGames(List<TournamentCompetitor> competitors)
        {
            List<Game> games = new List<Game>();
            List<string> divisions = competitors.Select(c => c.Division).Distinct().ToList();
            foreach (string division in divisions)
            {
                List<Game> division_games = MakeDivisionBaseGames(division, competitors.Where(c => c.Division == division));
                games.AddRange(division_games);
            }
            games.Sort(CustomSort); // this matters probably.
            return games;
        }

        private static List<Game> MakeDivisionBaseGames(string division, IEnumerable<TournamentCompetitor> division_competitors)
        {
            List<Game> games = new List<Game>();
            for (int i = 1; i <= 8; i++) // hardcoded for ncaa - could derive this somehow.
            {
                TournamentCompetitor competitor_1 = division_competitors.First(c => c.Seed == i);
                TournamentCompetitor competitor_2 = division_competitors.First(c => c.Seed == 17 - i);
                games.Add(new Game()
                {
                    ID = Guid.NewGuid().ToString(),
                    Competitor1 = competitor_1,
                    Competitor2 = competitor_2,
                    Division = division,
                    Round = 1 // seed round
                });
            }
            return games;
        }

        private static int CustomSort(Game game1, Game game2)
        {
            if (game1.Division != game2.Division && game1.Division != null) // not sure if this is smart
            {
                return game1.Division.CompareTo(game2.Division);
            }
            if (game1.Competitor1 == null || game2.Competitor1 == null) throw new ArgumentException("can't have null competitors in sort");
            List<int> ordered_seeds = new List<int>() { 1, 8, 5, 4, 6, 3, 7, 2 };
            for (int i = 0; i < ordered_seeds.Count; i++)
            {
                if (game1.Competitor1.Seed == ordered_seeds[i]) return -1;
                if (game2.Competitor1.Seed == ordered_seeds[i]) return 1;
            }
            return 0;
        }

        public bool FullyPopulated()
        {
            if (ChampionshipGame.Winner == null) return false;
            return ChampionshipGame.FullyPopulated();
        }

        private List<PickChange> PickChangesChampGame(Pick p)
        {
            List<PickChange> changes = new List<PickChange>();
            if(ChampionshipGame.Winner != null && ChampionshipGame.Winner.ID == p.CompetitorID)
            {
                return changes; // winner already this.
            }
            changes.Add(new PickChange(p.BracketID, p.TournamentID, p.GameID, p.CompetitorID, true));
            if(ChampionshipGame.Winner != null && ChampionshipGame.Winner.ID != p.GameID)
            {
                changes.Add(new PickChange(p.BracketID, p.TournamentID, p.GameID, ChampionshipGame.Winner.ID, false));
            }
            return changes;
        }

        public List<PickChange> GetPickChanges(Pick p)
        {
            if(p.GameID == ChampionshipGame.ID)
            {
                return PickChangesChampGame(p);
            }
            List<PickChange> pick_changes = new List<PickChange>();
            Game? parent_game = ChampionshipGame.FindParentGame(p.GameID);
            if (parent_game == null) throw new ArgumentException("could not find parent game when getting pick changes.");

            Game? outcome_game = parent_game.GetImmediateChildGame(p.GameID);
            if(outcome_game == null) throw new ArgumentException();
            TournamentCompetitor winner = _competitorDictionary[p.CompetitorID];
            if(parent_game.IsLeft(p.GameID))
            {
                bool had_different_winner = parent_game.Competitor1 != null && parent_game.Competitor1 != winner;
                if(had_different_winner)
                {
                    pick_changes.AddRange(RemoveFromTree(outcome_game, parent_game.Competitor1, p.BracketID));
                }
            } else
            {
                bool had_different_winner = parent_game.Competitor2 != null && parent_game.Competitor2 != winner;
                if (had_different_winner)
                {
                    pick_changes.AddRange(RemoveFromTree(outcome_game, parent_game.Competitor2, p.BracketID));
                }
            }
            bool change_necessary = !parent_game.HasCompetitor(p.CompetitorID);
            if (change_necessary)
            {
                pick_changes.Add(new PickChange(p.BracketID, p.TournamentID, p.GameID, p.CompetitorID, true));
            }

            return pick_changes;
        }

        // we want to remove the old winner though outcome game. 
        // if a competitor is present in the parent, that means we need to add a removal pick change for the child game. 
        private List<PickChange> RemoveFromTree(Game outcomeGame, TournamentCompetitor competitor, string bracketID)
        {
            if (outcomeGame == ChampionshipGame)
            {
                // ok.. what here? 
                if (ChampionshipGame.Winner != competitor)
                {
                    return new List<PickChange>();
                }
                ChampionshipGame.Winner = null;
                PickChange champ_change = new PickChange(bracketID, ID, ChampionshipGame.ID, competitor.ID, false);
                return new List<PickChange> { champ_change };
            }

            // traveling up tree. 
            // starting at the outcome game that changed. 
            Game? parent_game = ChampionshipGame.FindParentGame(outcomeGame.ID);
            if (parent_game == null) throw new ArgumentException();
            if(parent_game.HasCompetitor(competitor.ID))
            {
                List<PickChange> changes = new List<PickChange>() { new PickChange(bracketID, ID, outcomeGame.ID, competitor.ID, false) };
                changes.AddRange(RemoveFromTree(parent_game, competitor, bracketID));
                return changes;
            }
            return new List<PickChange>();
        }

        

    }
}
