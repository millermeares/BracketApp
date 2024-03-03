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
        private Dictionary<string, int> _teamRoundLoss = new Dictionary<string, int>();

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
            // if left game or right game is not null, set setcompetitor from the winner of those games.
            SetCompetitorsFromChildWinners(game); // not sure this is still a thing. probably isn't. leaving it here bc it works fine.
            NoteLoserIfAlreadyHappened(game);
        }

        private void NoteLoserIfAlreadyHappened(Game game)
        {
            if (!game.HasWinner()) return;
            TournamentCompetitor loser = game.GetLoser();
            _teamRoundLoss.Add(loser.ID, game.Round);
        }

        private void SetCompetitorsFromChildWinners(Game game)
        {
            
            if (!game.HasChildGames())
            {
                game.PredictedCompetitor1 = game.Competitor1;
                game.PredictedCompetitor2 = game.Competitor2;
            }
            if (game.HasCompetitors()) return;
            Game left_game = _gameDictionary[game.LeftGame.ID];
            if(!game.HasCompetitor1() && left_game.HasWinner())
            {
                game.Competitor1 = left_game.Winner;
            }
            Game right_game = _gameDictionary[game.RightGame.ID];
            if(!game.HasCompetitor2() && right_game.HasWinner())
            {
                game.Competitor2 = right_game.Winner;
            }
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

        private bool TeamAlreadyLost(TournamentCompetitor competitor, int gameRound)
        {
            if(!_teamRoundLoss.ContainsKey(competitor.ID)) return false;
            return gameRound > _teamRoundLoss[competitor.ID];
        } 
        private void ApplyPick(BracketPick p)
        {
            if(p.GameID == ChampionshipGame.ID)
            {
                ChampionshipGame.PredictedWinner = _competitorDictionary[p.CompetitorID];
                return;
            }
            Game? parent_game = ChampionshipGame.FindParentGame(p.GameID);
            if(parent_game == null) throw new Exception($"could not find parent game for {p.GameID}.");
            Game? game = parent_game.GetImmediateChildGame(p.GameID);
            if (game == null) throw new Exception("could not find game in immediate children but it should really be there.");
            SetGamePredictedWinnerFromDict(game, p);
            if(parent_game.IsLeft(p.GameID))
            {
                parent_game.PredictedCompetitor1 = _competitorDictionary[p.CompetitorID];
                parent_game.Competitor1PredictionWrong = TeamAlreadyLost(_competitorDictionary[p.CompetitorID], parent_game.Round);
            } else
            {
                // right
                parent_game.PredictedCompetitor2= _competitorDictionary[p.CompetitorID];
                parent_game.Competitor2PredictionWrong = TeamAlreadyLost(_competitorDictionary[p.CompetitorID], parent_game.Round);
            }
        }

        private void SetGamePredictedWinnerFromDict(Game game, BracketPick p)
        {
            game.PredictedWinner = _competitorDictionary[p.CompetitorID];
        }

        public void ApplyPicks(List<BracketPick> picks)
        {
            picks.ForEach(pick => ApplyPick(pick));
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
            if (ChampionshipGame.PredictedWinner == null) return false;
            return ChampionshipGame.FullyPopulated();
        }

        private List<BracketPickChange> PickChangesChampGame(BracketPick p)
        {
            List<BracketPickChange> changes = new List<BracketPickChange>();
            if(ChampionshipGame.PredictedWinner != null && ChampionshipGame.PredictedWinner.ID == p.CompetitorID)
            {
                return changes; // winner already this.
            }
            changes.Add(new BracketPickChange(p.BracketID, p.TournamentID, p.GameID, p.CompetitorID, true, false));
            if(ChampionshipGame.PredictedWinner != null && ChampionshipGame.PredictedWinner.ID != p.CompetitorID)
            //technically found a bug fix that could have been making things not break.
            {
                changes.Add(new BracketPickChange(p.BracketID, p.TournamentID, p.GameID, ChampionshipGame.PredictedWinner.ID, false, false));
            }
            return changes;
        }

        public List<BracketPickChange> GetPickChanges(BracketPick p)
        {
            if(p.GameID == ChampionshipGame.ID)
            {
                return PickChangesChampGame(p);
            }
            List<BracketPickChange> pick_changes = new List<BracketPickChange>();
            Game? parent_game = ChampionshipGame.FindParentGame(p.GameID);
            if (parent_game == null) throw new ArgumentException("could not find parent game when getting pick changes.");

            Game? outcome_game = parent_game.GetImmediateChildGame(p.GameID);
            if(outcome_game == null) throw new ArgumentException();
            TournamentCompetitor winner = _competitorDictionary[p.CompetitorID];
            if(parent_game.IsLeft(p.GameID))
            {
                bool had_different_winner = parent_game.PredictedCompetitor1 != null && parent_game.PredictedCompetitor1 != winner;
                if(had_different_winner)
                {
                    pick_changes.AddRange(RemovePredictionsFromTree(outcome_game, parent_game.PredictedCompetitor1, p.BracketID));
                }
            } else
            {
                bool had_different_winner = parent_game.PredictedCompetitor2 != null && parent_game.PredictedCompetitor2 != winner;
                if (had_different_winner)
                {
                    pick_changes.AddRange(RemovePredictionsFromTree(outcome_game, parent_game.PredictedCompetitor2, p.BracketID));
                }
            }
            bool change_necessary = !parent_game.HasPredictedCompetitor(p.CompetitorID);
            if (change_necessary)
            {
                pick_changes.Add(new BracketPickChange(p.BracketID, p.TournamentID, p.GameID, p.CompetitorID, true, false));
            }

            return pick_changes;
        }

        // we want to remove the old winner though outcome game. 
        // if a competitor is present in the parent, that means we need to add a removal pick change for the child game. 
        private List<BracketPickChange> RemovePredictionsFromTree(Game outcomeGame, TournamentCompetitor competitor, string bracketID)
        {
            if (outcomeGame == ChampionshipGame)
            {
                if (ChampionshipGame.PredictedWinner != competitor)
                {
                    return new List<BracketPickChange>();
                }
                ChampionshipGame.PredictedWinner = null;
                BracketPickChange champ_change = new BracketPickChange(bracketID, ID, ChampionshipGame.ID, competitor.ID, false, false);
                return new List<BracketPickChange> { champ_change };
            }

            // traveling up tree. 
            // starting at the outcome game that changed. 
            Game? parent_game = ChampionshipGame.FindParentGame(outcomeGame.ID);
            if (parent_game == null) throw new ArgumentException();
            if(parent_game.HasPredictedCompetitor(competitor.ID))
            {
                List<BracketPickChange> changes = new List<BracketPickChange>() { new BracketPickChange(bracketID, ID, outcomeGame.ID, competitor.ID, false, false) };
                changes.AddRange(RemovePredictionsFromTree(parent_game, competitor, bracketID));
                return changes;
            }
            return new List<BracketPickChange>();
        }

        public List<Pick> Smartfill(SmartFillArgs args) // this could bring in the info like SeedData dictionary, etc.
        {
            List<Pick> changes = new List<Pick>();
            ChampionshipGame.PredictedWinner = SmartFillGamesToGetPredictedWinner(ChampionshipGame, args, ref changes);
            ResetUnderdogCounters();
            return changes;
        }

        protected TournamentCompetitor? SmartFillGamesToGetPredictedWinner(Game game, SmartFillArgs args, ref List<Pick> pick_changes)
        {
            if (game.HasPredictedWinner()) return game.PredictedWinner;
            if(!game.HasPredictedCompetitors())
            {
                bool do_comp_1_first = Random.Shared.NextDouble() > 0.5; // do this to make the order that things are fill in random.
                // this is.. fine but we probably won't have many cinderella teams on opposite sides of the bracket. 
                // killen says that's a good team ^^ oh
                if(do_comp_1_first && !game.HasPredictedCompetitor1())
                {
                    game.PredictedCompetitor1 = SmartFillGamesToGetPredictedWinner(game.LeftGame, args, ref pick_changes);
                }
                if(!game.HasCompetitor2())
                {
                    game.PredictedCompetitor2 = SmartFillGamesToGetPredictedWinner(game.RightGame, args, ref pick_changes);
                }
                if(!do_comp_1_first && !game.HasCompetitor1())
                {
                    game.PredictedCompetitor1 = SmartFillGamesToGetPredictedWinner(game.LeftGame, args, ref pick_changes);
                }
            }
            pick_changes.Add(SmartPredictWinner(game, args));
            return game.PredictedWinner;
        }

        protected Pick SmartPredictWinner(Game game, SmartFillArgs args)
        {
            // if one of the events has never happened before, return team 1, etc. 
            Func<Game, SmartFillArgs, double> func = GetSmartFillFunction(game, args);
            double team_1_win_percentage = func(game, args);
            game.PredictedWinner = WinnerFromTeamOneWinChance(team_1_win_percentage, game.PredictedCompetitor1, game.PredictedCompetitor2, args);
            return args.MakePick(game, ID);
        }

        private bool GameHasCompetitorIsSpecificTeamAndRound(Game game)
        {
            return CompetitorIsSpecificTeamAndRound(game.PredictedCompetitor1, game.Round) || CompetitorIsSpecificTeamAndRound(game.PredictedCompetitor2, game.Round);
        }

        private bool CompetitorIsSpecificTeamAndRound(TournamentCompetitor comp, int round)
        {
            return comp.Name == "Colgate" && round > 3;
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
            if(args.OneOfTeamsNotAllowedToWin(game))
            {
                return ForceWinnerForTeamAllowedToWin;
            }
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

        protected double ForceWinnerForTeamAllowedToWin(Game game, SmartFillArgs args)
        {
            bool team_1_allowed_to_win = args.TeamAllowedToWin(game.PredictedCompetitor1, game.Round);
            bool team_2_allowed_to_win = args.TeamAllowedToWin(game.PredictedCompetitor2, game.Round);
            if(!team_1_allowed_to_win && !team_2_allowed_to_win)
            {
                // if both not allowed to win, let the team with the higher seed win.
                if(game.PredictedCompetitor1.Seed == game.PredictedCompetitor2.Seed) // in this extremely rare case, just randomize.
                {
                    double winner = Random.Shared.NextDouble();
                    team_1_allowed_to_win = winner > 0.5;
                    team_2_allowed_to_win = winner <= 0.5;
                }
                else
                {
                    team_1_allowed_to_win = game.PredictedCompetitor1.Seed < game.PredictedCompetitor2.Seed;
                    team_2_allowed_to_win = game.PredictedCompetitor2.Seed < game.PredictedCompetitor1.Seed;
                }
            }

            if (team_1_allowed_to_win == team_2_allowed_to_win)
            {
                throw new Exception("at this point, at least one team needs to be allowed to win.");
            }
            return team_1_allowed_to_win ? 1 : 0;
        }
        //issue to solve: this could reduce the amount of underdog teams. maybe that's fine. maybe not.

        protected double SmartPickWinnerForUnderdogWinTeam(Game game, SmartFillArgs args)
        {
            if(TeamIsUnderdogWithFreeWin(game.PredictedCompetitor1) && TeamIsUnderdogWithFreeWin(game.PredictedCompetitor2))
            {
                var actual_func = GetSmartFillFunction(game, args, false);
                return actual_func(game, args);
            }

            if (TeamIsUnderdogWithFreeWin(game.PredictedCompetitor1)) return 1.00;
            if (TeamIsUnderdogWithFreeWin(game.PredictedCompetitor2)) return 0.00;

            if(TeamIsUnderdogWithBetterOdds(game.PredictedCompetitor1) && TeamIsUnderdogWithBetterOdds(game.PredictedCompetitor2))
            {
                var actual_func = GetSmartFillFunction(game, args, false);
                return actual_func(game, args);
            }

            if(TeamIsUnderdogWithBetterOdds(game.PredictedCompetitor1))
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
            if(TeamIsUnderdogWithBetterOdds(game.PredictedCompetitor2))
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
            DecrementUnderdogGameCount(game.PredictedCompetitor1);
            DecrementUnderdogGameCount(game.PredictedCompetitor2);
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
            return TeamHasUnderdogWin(game.PredictedCompetitor1) || TeamHasUnderdogWin(game.PredictedCompetitor2);
        }

        private bool TeamHasUnderdogWin(TournamentCompetitor comp)
        {
            return _smartFillUnderdogFreeWins.ContainsKey(comp.ID);
        }

        protected bool OneOfTheTeamsIsUnderdogWithFreeWin(Game game)
        {
            return TeamIsUnderdogWithFreeWin(game.PredictedCompetitor1) || TeamIsUnderdogWithFreeWin(game.PredictedCompetitor2);
        }

        private bool TeamIsUnderdogWithFreeWin(TournamentCompetitor comp)
        {
            return _smartFillUnderdogFreeWins.ContainsKey(comp.ID) && _smartFillUnderdogFreeWins[comp.ID] > 0;
        }

        protected bool OneOfTheTeamsIsUnderdogWithBetterOdds(Game game)
        {
            return TeamIsUnderdogWithBetterOdds(game.PredictedCompetitor1) && TeamIsUnderdogWithBetterOdds(game.PredictedCompetitor2);
        }

        private bool TeamIsUnderdogWithBetterOdds(TournamentCompetitor comp)
        {
            return _smartFillUnderdogFreeWins.ContainsKey(comp.ID) && _smartFillUnderdogFreeWins[comp.ID] == 0;
        }
        

        protected double SmartPickWinnerFromKenPom(Game game, SmartFillArgs args)
        {
            double spread = args.KenPom.GetKenPomSpreadDiff(game.PredictedCompetitor1, game.PredictedCompetitor2);
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
            if (game.PredictedCompetitor1 == null || game.PredictedCompetitor2 == null) throw new ArgumentException("predicted competitors null when smart picking from seed data");
            SeedData seed1 = args.SeedData.GetSeedData(game.PredictedCompetitor1.Seed);
            SeedData seed2 = args.SeedData.GetSeedData(game.PredictedCompetitor2.Seed);
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

        private void ResetUnderdogCounters()
        {
            _smartFillUnderdogFreeWins.Clear();
            UnderdogRunTeams = 0;
        }

        internal void ResetForNewSimulation()
        {
            ChampionshipGame.ClearPredictionsFromNonBaseGames();
            ResetUnderdogCounters(); // might be double resetting some things which is fine.
        }

        internal List<TournamentCompetitor> GetCompetitors()
        {
            return _competitorDictionary.Values.ToList();
        }
    }
}
