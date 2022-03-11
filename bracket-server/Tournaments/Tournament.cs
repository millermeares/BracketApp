namespace bracket_server.Tournaments
{
    public class Tournament
    {
        public string ID { get; set; } = string.Empty;
        public bool Finalized { get; set; }
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


    }
}
