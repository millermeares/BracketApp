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
        public DateTime CreatedTime { get; set; } = DateTime.MinValue;
        public string TournamentType { get; set; } = string.Empty;
        private Dictionary<string, Game> _gameDictionary = new Dictionary<string, Game>();
        private Dictionary<string, TournamentCompetitor> _competitorDictionary = new Dictionary<string,TournamentCompetitor>();

        private Dictionary<string, int> _smartFillUnderdogFreeWins = new Dictionary<string, int>();


        private int UnderdogRunTeams = 0;
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
            changes.Add(new PickChange(p.BracketID, p.TournamentID, p.GameID, p.CompetitorID, true, false));
            if(ChampionshipGame.Winner != null && ChampionshipGame.Winner.ID != p.GameID)
            {
                changes.Add(new PickChange(p.BracketID, p.TournamentID, p.GameID, ChampionshipGame.Winner.ID, false, false));
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
                pick_changes.Add(new PickChange(p.BracketID, p.TournamentID, p.GameID, p.CompetitorID, true, false));
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
                PickChange champ_change = new PickChange(bracketID, ID, ChampionshipGame.ID, competitor.ID, false, false);
                return new List<PickChange> { champ_change };
            }

            // traveling up tree. 
            // starting at the outcome game that changed. 
            Game? parent_game = ChampionshipGame.FindParentGame(outcomeGame.ID);
            if (parent_game == null) throw new ArgumentException();
            if(parent_game.HasCompetitor(competitor.ID))
            {
                List<PickChange> changes = new List<PickChange>() { new PickChange(bracketID, ID, outcomeGame.ID, competitor.ID, false, false) };
                changes.AddRange(RemoveFromTree(parent_game, competitor, bracketID));
                return changes;
            }
            return new List<PickChange>();
        }

        // for now, completely random. 
        public void Autofill(SmartFillArgs args) // this could bring in the info like SeedData dictionary, etc.
        {
            List<PickChange> changes = new List<PickChange>();
            ChampionshipGame.Winner = SmartFillGamesToGetWinner(ChampionshipGame, args, ref changes);
            args.SavePickChanges(changes);
        }

        protected TournamentCompetitor? SmartFillGamesToGetWinner(Game game, SmartFillArgs args, ref List<PickChange> pick_changes)
        {
            if (game.HasWinner()) return game.Winner;
            if(!game.HasCompetitors())
            {
                bool do_comp_1_first = Random.Shared.NextDouble() > 0.5; // do this to make the order that things are fill in random.
                // this is.. fine but we probably won't have many cinderella teams on opposite sides of the bracket.
                if(do_comp_1_first && !game.HasCompetitor1())
                {
                    game.Competitor1 = SmartFillGamesToGetWinner(game.LeftGame, args, ref pick_changes);
                }
                if(!game.HasCompetitor2())
                {
                    game.Competitor2 = SmartFillGamesToGetWinner(game.RightGame, args, ref pick_changes);
                }
                if(!do_comp_1_first && !game.HasCompetitor1())
                {
                    game.Competitor1 = SmartFillGamesToGetWinner(game.LeftGame, args, ref pick_changes);
                }
            }
            pick_changes.Add(SmartPickWinner(game, args));
            return game.Winner;
        }

        protected PickChange SmartPickWinner(Game game, SmartFillArgs args)
        {
            Func<Game, SmartFillArgs, double> func = GetSmartFillFunction(game, args);
            double team_1_win_percentage = func(game, args);
            
            game.Winner = WinnerFromTeamOneWinChance(team_1_win_percentage, game.Competitor1, game.Competitor2, args);
            return args.MakePickChange(game, ID);
        }


        protected TournamentCompetitor WinnerFromTeamOneWinChance(double team_1_win_percentage, TournamentCompetitor comp1, TournamentCompetitor comp2, SmartFillArgs args)
        {
            double random_double = Random.Shared.NextDouble();
            bool team_one_winner = random_double < team_1_win_percentage;
            TournamentCompetitor winner = team_one_winner ? comp1 : comp2;
            if(OutcomeIsNewUnderdogWin(comp1, comp2, team_one_winner, args))
            {
                Increment(winner);
            } else
            {
                DecrementUnderdogGameCount(winner);
            }
            return winner;
        }

        private bool OutcomeIsNewUnderdogWin(TournamentCompetitor comp1, TournamentCompetitor comp2, bool team_one_win, SmartFillArgs args)
        {
            TournamentCompetitor winner = team_one_win ? comp1 : comp2;
            if (_smartFillUnderdogFreeWins.ContainsKey(winner.ID)) return false; // an old win.
            if (!args.KenPom.BothPartiesHaveKenPom(comp1, comp2)) return false; // both parties don't have ken pom. not sure we'll get here.
            double spread = args.KenPom.GetKenPomSpreadDiff(comp1, comp2);
            return (team_one_win && spread < (-1 * args.BigUnderdogThreshold)) || (!team_one_win && spread > args.BigUnderdogThreshold);
        }

        private void Increment(TournamentCompetitor winner)
        {
            _smartFillUnderdogFreeWins.Add(winner.ID, 2); // two free wins.
            UnderdogRunTeams++;
        }

        protected Func<Game, SmartFillArgs, double> GetSmartFillFunction(Game game, SmartFillArgs args, bool consider_underdog = true)
        {
            if (consider_underdog && OneOfTeamsHasUnderdogWin(game) && game.Round < 5)
            {
                return SmartPickWinnerForUnderdogWinTeam;
            }
            if (args.KenPom.BothPartiesHaveKenPom(game))
            {
                return SmartPickWinnerFromKenPom;
            }
            return SmartPickWinnerFromSeedData;
        }

        protected double SmartPickWinnerForUnderdogWinTeam(Game game, SmartFillArgs args)
        {
            if(TeamIsUnderdogWithFreeWin(game.Competitor1) && TeamIsUnderdogWithFreeWin(game.Competitor2))
            {
                var actual_func = GetSmartFillFunction(game, args, false);
                return actual_func(game, args);
            }

            if (TeamIsUnderdogWithFreeWin(game.Competitor1)) return 1.00;
            if (TeamIsUnderdogWithFreeWin(game.Competitor2)) return 0.00;

            if(TeamIsUnderdogWithBetterOdds(game.Competitor1) && TeamIsUnderdogWithBetterOdds(game.Competitor2))
            {
                var actual_func = GetSmartFillFunction(game, args, false);
                return actual_func(game, args);
            }

            if(TeamIsUnderdogWithBetterOdds(game.Competitor1))
            {
                var actual_func = GetSmartFillFunction(game, args, false);
                double team_1_win_chance = actual_func(game, args);
                if (team_1_win_chance < 0.5)
                {
                    double diff = (0.5 - team_1_win_chance) / 2;
                    team_1_win_chance += diff;
                }
                return team_1_win_chance;
            }
            if(TeamIsUnderdogWithBetterOdds(game.Competitor2))
            {
                var actual_func = GetSmartFillFunction(game, args, false);
                // if favored, do nothing. this is rare.
                double team_1_win_chance = actual_func(game, args);
                if(team_1_win_chance > 0.5)
                {
                    double diff = (team_1_win_chance - 0.5) / 2;
                    team_1_win_chance -= diff;
                }
                return team_1_win_chance;
            }
            var func = GetSmartFillFunction(game, args, false);
            return func(game, args); // this shouldn't happen - consider throwing.

        }

        private void DecrementUnderdogGameCount(Game game)
        {
            DecrementUnderdogGameCount(game.Competitor1);
            DecrementUnderdogGameCount(game.Competitor2);
        }
        private void DecrementUnderdogGameCount(TournamentCompetitor competitor)
        {
            if (!_smartFillUnderdogFreeWins.ContainsKey(competitor.ID)) return;
            _smartFillUnderdogFreeWins[competitor.ID] = _smartFillUnderdogFreeWins[competitor.ID] - 1;
            if(_smartFillUnderdogFreeWins[competitor.ID] < 0)
            {
                _smartFillUnderdogFreeWins.Remove(competitor.ID);
            }
        }

        protected bool OneOfTeamsHasUnderdogWin(Game game)
        {
            return TeamHasUnderdogWin(game.Competitor1) || TeamHasUnderdogWin(game.Competitor2);
        }

        private bool TeamHasUnderdogWin(TournamentCompetitor comp)
        {
            return _smartFillUnderdogFreeWins.ContainsKey(comp.ID);
        }

        protected bool OneOfTheTeamsIsUnderdogWithFreeWin(Game game)
        {
            return TeamIsUnderdogWithFreeWin(game.Competitor1) || TeamIsUnderdogWithFreeWin(game.Competitor2);
        }

        private bool TeamIsUnderdogWithFreeWin(TournamentCompetitor comp)
        {
            return _smartFillUnderdogFreeWins.ContainsKey(comp.ID) && _smartFillUnderdogFreeWins[comp.ID] > 0;
        }

        protected bool OneOfTheTeamsIsUnderdogWithBetterOdds(Game game)
        {
            return TeamIsUnderdogWithBetterOdds(game.Competitor1) && TeamIsUnderdogWithBetterOdds(game.Competitor2);
        }

        private bool TeamIsUnderdogWithBetterOdds(TournamentCompetitor comp)
        {
            return _smartFillUnderdogFreeWins.ContainsKey(comp.ID) && _smartFillUnderdogFreeWins[comp.ID] == 0;
        }
        

        protected double SmartPickWinnerFromKenPom(Game game, SmartFillArgs args)
        {
            double spread = args.KenPom.GetKenPomSpreadDiff(game.Competitor1, game.Competitor2);
            if (spread > args.AutoWinSpread) return 1; // too large of a spread for competitor 1 to lose
            if (spread < -1 * args.AutoWinSpread) return 0; // same but for comp 2

            // if spread is between autowinspread and big upset threshold and there are no more underdog wins allowed, then auto-win to the favorite.
            if(Math.Abs(spread) < args.AutoWinSpread && Math.Abs(spread) > args.BigUnderdogThreshold && !UnderdogRunAllowed(args))
            {
                // auto-win to the favorite.
                return spread > 0 ? 1 : 0;
            }

            double team_1_win_chance = args.KenPom.WinPercentageFromKenPomSpreadDiff(spread);
            //string msg = $"Spread: {spread.ToString("0.##")}. Team 1 win chance: {team_1_win_chance.ToString("0.##")}.";
            return team_1_win_chance;
        }


        protected double SmartPickWinnerFromSeedData(Game game, SmartFillArgs args)
        {
            if (game.Competitor1 == null || game.Competitor2 == null) throw new ArgumentException("competitors null");
            SeedData seed1 = args.SeedData.GetSeedData(game.Competitor1.Seed);
            SeedData seed2 = args.SeedData.GetSeedData(game.Competitor2.Seed);
            double total_final_four_odds = seed1.FinalFourOdds + seed2.FinalFourOdds;
            double outcome_double = GetRandomDouble(total_final_four_odds);
            return outcome_double;
        }

        protected double GetRandomDouble(double multiplier)
        {
            return Random.Shared.NextDouble() * multiplier;
        }

        private bool UnderdogRunAllowed(SmartFillArgs args)
        {
            return UnderdogRunTeams < args.MaxUnderdogRuns;
        }

        

        

    }
}
