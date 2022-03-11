namespace bracket_server.Tournaments
{
    public static class FakeTournamentData
    {
        private static List<string> FakeUniversities = new List<string>() { "UNC", "Kentucky", "Dook",
            "NC State", "Georgetown", "Georgia Tech", "Seattle University", "Baylor", "Boston University", "Boston College",
            "FSU", "LSU", "UConn", "Maine University", "Washington State", "UCLA", "Kansas"};
        public static Tournament MakeFakeNCAATournament()
        {
            List<Game> base_games = GetFakeBaseGames(8);
            Game championship_game = ChampionshipGameFromBase(base_games);
            championship_game.RandomWinner();
            return MakeFakeTournament("NCAA FAKE", championship_game);
        }

        public static Tournament MakeFakeNCAATournamentSkeleton()
        {
            List<Game> base_games = GetFakeBaseGames(8);
            Game championship_game = Tournament.ChampionshipSkeletonFromBase(base_games);
            return MakeFakeTournament("NCAA FAKE", championship_game);
        }

        public static Game ChampionshipGameFromBase(List<Game> base_games)
        {
            if (base_games.Count == 1) return base_games.First();
            if (base_games.Count % 2 != 0) throw new ArgumentException("must have even number of games");

            List<Game> next_round_games = new List<Game>();
            int first_game_index = 0;
            while(first_game_index < base_games.Count)
            {
                Game game1 = base_games[first_game_index];
                Game game2 = base_games[first_game_index + 1];
                next_round_games.Add(AssignWinnerMakeParent(game1, game2));
                first_game_index += 2;
            }
            return ChampionshipGameFromBase(next_round_games);
        }

        private static Game AssignWinnerMakeParent(Game game1, Game game2)
        {
            game1.RandomWinner();
            game2.RandomWinner();
            return Game.MakeParentGame(game1, game2);
        }

        public static List<Game> GetFakeBaseGames(int games_per_quad)
        {
            List<Game> games = new List<Game>();
            for(int quad = 0; quad < 4; quad++)
            {
                games.AddRange(GetFakeQuadGames(quad+1, games_per_quad));
            }
            return games;
        }

        public static List<Game> GetFakeQuadGames(int quad, int game_count)
        {
            List<Game> games = new List<Game>();
            for(int game_num = 0; game_num < game_count; game_num++)
            {
                int lower_seed = game_num + 1;
                int higher_seed = (game_count * 2) + 1 - lower_seed;
                TournamentCompetitor comp_1 = FakeCompetitor(lower_seed, quad);
                TournamentCompetitor comp_2 = FakeCompetitor(higher_seed, quad);
                games.Add(new Game()
                {
                    ID = Guid.NewGuid().ToString(),
                    Competitor1 = comp_1,
                    Competitor2 = comp_2
                });
            }
            games.Sort((game1, game2) => CustomSort(game1, game2));
            return games;
        }

        private static TournamentCompetitor FakeCompetitor(int seed, int quad)
        {
            return new TournamentCompetitor()
            {
                Name = MakeName(seed, quad),
                ID = Guid.NewGuid().ToString(), 
                Seed = seed
            };
        }

        private static Tournament MakeFakeTournament(string name, Game game)
        {
            return new Tournament()
            {
                ID = Guid.NewGuid().ToString(),
                ChampionshipGame = game,
                Name = name,
                EventEnd = DateTime.Now,
                EventStart = DateTime.Now
            };
        }

        private static int CustomSort(Game game1, Game game2)
        {
            if (game1.Competitor1 == null || game2.Competitor1 == null) throw new ArgumentException("can't have null competitors in sort");
            List<int> ordered_seeds = new List<int>() { 1, 8, 5, 4, 6, 3, 7, 2 };
            for(int i = 0; i < ordered_seeds.Count; i++)
            {
                if (game1.Competitor1.Seed == ordered_seeds[i]) return -1;
                if (game2.Competitor1.Seed == ordered_seeds[i]) return 1;
            }
            return 0;
        }

        private static string MakeName(int seed, int quad)
        {
            return $"{FakeUniversities[seed]} {quad}";
        }
        

    }
}
